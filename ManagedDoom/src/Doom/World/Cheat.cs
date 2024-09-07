//
// Copyright (C) 1993-1996 Id Software, Inc.
// Copyright (C) 2019-2020 Nobuaki Tanaka
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//



using System;
using System.Linq;

namespace ManagedDoom
{
    public sealed class Cheat
    {
        private static readonly CheatInfo[] _list =
        [
            new CheatInfo("idfa", (cheat, typed) => cheat.FullAmmo(), false),
            new CheatInfo("idkfa", (cheat, typed) => cheat.FullAmmoAndKeys(), false),
            new CheatInfo("iddqd", (cheat, typed) => cheat.GodMode(), false),
            new CheatInfo("idclip", (cheat, typed) => cheat.NoClip(), false),
            new CheatInfo("idspispopd", (cheat, typed) => cheat.NoClip(), false),
            new CheatInfo("iddt", (cheat, typed) => cheat.FullMap(), true),
            new CheatInfo("idbehold", (cheat, typed) => cheat.ShowPowerUpList(), false),
            new CheatInfo("idbehold?", (cheat, typed) => cheat.DoPowerUp(typed), false),
            new CheatInfo("idchoppers", (cheat, typed) => cheat.GiveChainsaw(), false),
            new CheatInfo("tntem", (cheat, typed) => cheat.KillMonsters(), false),
            new CheatInfo("killem", (cheat, typed) => cheat.KillMonsters(), false),
            new CheatInfo("fhhall", (cheat, typed) => cheat.KillMonsters(), false),
            new CheatInfo("idclev??", (cheat, typed) => cheat.ChangeLevel(typed), true),
            new CheatInfo("idmus??", (cheat, typed) => cheat.ChangeMusic(typed), false)
        ];

        private static readonly int _maxCodeLength = _list.Max(info => info.Code.Length);

        private readonly World _world;

        private readonly char[] _buffer;
        private int _p;

        public Cheat(World world)
        {
            _world = world;

            _buffer = new char[_maxCodeLength];
            _p = 0;
        }

        public bool DoEvent(DoomEvent e)
        {
            if (e.Type == EventType.KeyDown)
            {
                _buffer[_p] = e.Key.GetChar();

                _p = (_p + 1) % _buffer.Length;

                CheckBuffer();
            }

            return true;
        }

        private void CheckBuffer()
        {
            for (int i = 0; i < _list.Length; i++)
            {
                string code = _list[i].Code;
                int q = _p;
                int j;
                for (j = 0; j < code.Length; j++)
                {
                    q--;
                    if (q == -1)
                    {
                        q = _buffer.Length - 1;
                    }
                    char ch = code[code.Length - j - 1];
                    if (_buffer[q] != ch && ch != '?')
                    {
                        break;
                    }
                }

                if (j == code.Length)
                {
                    char[] typed = new char[code.Length];
                    int k = code.Length;
                    q = _p;
                    for (j = 0; j < code.Length; j++)
                    {
                        k--;
                        q--;
                        if (q == -1)
                        {
                            q = _buffer.Length - 1;
                        }
                        typed[k] = _buffer[q];
                    }

                    if (_world.Options.Skill != GameSkill.Nightmare || _list[i].AvailableOnNightmare)
                    {
                        _list[i].Action(this, new string(typed));
                    }
                }
            }
        }

        private void GiveWeapons()
        {
            Player player = _world.ConsolePlayer;
            if (_world.Options.GameMode == GameMode.Commercial)
            {
                for (int i = 0; i < (int)WeaponType.Count; i++)
                {
                    player.WeaponOwned[i] = true;
                }
            }
            else
            {
                for (int i = 0; i <= (int)WeaponType.Missile; i++)
                {
                    player.WeaponOwned[i] = true;
                }
                player.WeaponOwned[(int)WeaponType.Chainsaw] = true;
                if (_world.Options.GameMode != GameMode.Shareware)
                {
                    player.WeaponOwned[(int)WeaponType.Plasma] = true;
                    player.WeaponOwned[(int)WeaponType.Bfg] = true;
                }
            }

            player.Backpack = true;
            for (int i = 0; i < (int)AmmoType.Count; i++)
            {
                player.MaxAmmo[i] = 2 * DoomInfo.AmmoInfos.Max[i];
                player.Ammo[i] = 2 * DoomInfo.AmmoInfos.Max[i];
            }
        }

        private void FullAmmo()
        {
            GiveWeapons();
            Player player = _world.ConsolePlayer;
            player.ArmorType = DoomInfo.DeHackEdConst.IdfaArmorClass;
            player.ArmorPoints = DoomInfo.DeHackEdConst.IdfaArmor;
            player.SendMessage(DoomInfo.Strings.STSTR_FAADDED);
        }

        private void FullAmmoAndKeys()
        {
            GiveWeapons();
            Player player = _world.ConsolePlayer;
            player.ArmorType = DoomInfo.DeHackEdConst.IdkfaArmorClass;
            player.ArmorPoints = DoomInfo.DeHackEdConst.IdkfaArmor;
            for (int i = 0; i < (int)CardType.Count; i++)
            {
                player.Cards[i] = true;
            }
            player.SendMessage(DoomInfo.Strings.STSTR_KFAADDED);
        }

        private void GodMode()
        {
            Player player = _world.ConsolePlayer;
            if ((player.Cheats & CheatFlags.GodMode) != 0)
            {
                player.Cheats &= ~CheatFlags.GodMode;
                player.SendMessage(DoomInfo.Strings.STSTR_DQDOFF);
            }
            else
            {
                player.Cheats |= CheatFlags.GodMode;
                player.Health = Math.Max(DoomInfo.DeHackEdConst.GodModeHealth, player.Health);
                player.Mobj.Health = player.Health;
                player.SendMessage(DoomInfo.Strings.STSTR_DQDON);
            }
        }

        private void NoClip()
        {
            Player player = _world.ConsolePlayer;
            if ((player.Cheats & CheatFlags.NoClip) != 0)
            {
                player.Cheats &= ~CheatFlags.NoClip;
                player.SendMessage(DoomInfo.Strings.STSTR_NCOFF);
            }
            else
            {
                player.Cheats |= CheatFlags.NoClip;
                player.SendMessage(DoomInfo.Strings.STSTR_NCON);
            }
        }

        private void FullMap()
        {
            _world.AutoMap.ToggleCheat();
        }

        private void ShowPowerUpList()
        {
            Player player = _world.ConsolePlayer;
            player.SendMessage(DoomInfo.Strings.STSTR_BEHOLD);
        }

        private void DoPowerUp(string typed)
        {
            switch (typed.Last())
            {
                case 'v':
                    ToggleInvulnerability();
                    break;
                case 's':
                    ToggleStrength();
                    break;
                case 'i':
                    ToggleInvisibility();
                    break;
                case 'r':
                    ToggleIronFeet();
                    break;
                case 'a':
                    ToggleAllMap();
                    break;
                case 'l':
                    ToggleInfrared();
                    break;
            }
        }

        private void ToggleInvulnerability()
        {
            Player player = _world.ConsolePlayer;
            if (player.Powers[(int)PowerType.Invulnerability] > 0)
            {
                player.Powers[(int)PowerType.Invulnerability] = 0;
            }
            else
            {
                player.Powers[(int)PowerType.Invulnerability] = DoomInfo.PowerDuration.Invulnerability;
            }
            player.SendMessage(DoomInfo.Strings.STSTR_BEHOLDX);
        }

        private void ToggleStrength()
        {
            Player player = _world.ConsolePlayer;
            if (player.Powers[(int)PowerType.Strength] != 0)
            {
                player.Powers[(int)PowerType.Strength] = 0;
            }
            else
            {
                player.Powers[(int)PowerType.Strength] = 1;
            }
            player.SendMessage(DoomInfo.Strings.STSTR_BEHOLDX);
        }

        private void ToggleInvisibility()
        {
            Player player = _world.ConsolePlayer;
            if (player.Powers[(int)PowerType.Invisibility] > 0)
            {
                player.Powers[(int)PowerType.Invisibility] = 0;
                player.Mobj.Flags &= ~MobjFlags.Shadow;
            }
            else
            {
                player.Powers[(int)PowerType.Invisibility] = DoomInfo.PowerDuration.Invisibility;
                player.Mobj.Flags |= MobjFlags.Shadow;
            }
            player.SendMessage(DoomInfo.Strings.STSTR_BEHOLDX);
        }

        private void ToggleIronFeet()
        {
            Player player = _world.ConsolePlayer;
            if (player.Powers[(int)PowerType.IronFeet] > 0)
            {
                player.Powers[(int)PowerType.IronFeet] = 0;
            }
            else
            {
                player.Powers[(int)PowerType.IronFeet] = DoomInfo.PowerDuration.IronFeet;
            }
            player.SendMessage(DoomInfo.Strings.STSTR_BEHOLDX);
        }

        private void ToggleAllMap()
        {
            Player player = _world.ConsolePlayer;
            if (player.Powers[(int)PowerType.AllMap] != 0)
            {
                player.Powers[(int)PowerType.AllMap] = 0;
            }
            else
            {
                player.Powers[(int)PowerType.AllMap] = 1;
            }
            player.SendMessage(DoomInfo.Strings.STSTR_BEHOLDX);
        }

        private void ToggleInfrared()
        {
            Player player = _world.ConsolePlayer;
            if (player.Powers[(int)PowerType.Infrared] > 0)
            {
                player.Powers[(int)PowerType.Infrared] = 0;
            }
            else
            {
                player.Powers[(int)PowerType.Infrared] = DoomInfo.PowerDuration.Infrared;
            }
            player.SendMessage(DoomInfo.Strings.STSTR_BEHOLDX);
        }

        private void GiveChainsaw()
        {
            Player player = _world.ConsolePlayer;
            player.WeaponOwned[(int)WeaponType.Chainsaw] = true;
            player.SendMessage(DoomInfo.Strings.STSTR_CHOPPERS);
        }

        private void KillMonsters()
        {
            Player player = _world.ConsolePlayer;
            int count = 0;
            foreach (Thinker thinker in _world.Thinkers)
            {
                var mobj = thinker as Mobj;
                if (mobj != null &&
                    mobj.Player == null &&
                    ((mobj.Flags & MobjFlags.CountKill) != 0 || mobj.Type == MobjType.Skull) &&
                    mobj.Health > 0)
                {
                    _world.ThingInteraction.DamageMobj(mobj, null, player.Mobj, 10000);
                    count++;
                }
            }
            player.SendMessage(count + " monsters killed");
        }

        private void ChangeLevel(string typed)
        {
            if (_world.Options.GameMode == GameMode.Commercial)
            {
                if (!int.TryParse(typed.AsSpan(typed.Length - 2, 2), out int map))
                {
                    return;
                }
                GameSkill skill = _world.Options.Skill;
                _world.Game.DeferedInitNew(skill, 1, map);
            }
            else
            {
                if (!int.TryParse(typed.AsSpan(typed.Length - 2, 1), out int episode))
                {
                    return;
                }
                if (!int.TryParse(typed.AsSpan(typed.Length - 1, 1), out int map))
                {
                    return;
                }
                GameSkill skill = _world.Options.Skill;
                _world.Game.DeferedInitNew(skill, episode, map);
            }
        }

        private void ChangeMusic(string typed)
        {
            var options = new GameOptions
            {
                GameMode = _world.Options.GameMode
            };
            if (_world.Options.GameMode == GameMode.Commercial)
            {
                if (!int.TryParse(typed.AsSpan(typed.Length - 2, 2), out int map))
                {
                    return;
                }
                options.Map = map;
            }
            else
            {
                if (!int.TryParse(typed.AsSpan(typed.Length - 2, 1), out int episode))
                {
                    return;
                }
                if (!int.TryParse(typed.AsSpan(typed.Length - 1, 1), out int map))
                {
                    return;
                }
                options.Episode = episode;
                options.Map = map;
            }
            _world.Options.Music.StartMusic(Map.GetMapBgm(options), true);
            _world.ConsolePlayer.SendMessage(DoomInfo.Strings.STSTR_MUS);
        }



        private class CheatInfo
        {
            public readonly string Code;
            public readonly Action<Cheat, string> Action;
            public readonly bool AvailableOnNightmare;

            public CheatInfo(string code, Action<Cheat, string> action, bool availableOnNightmare)
            {
                Code = code;
                Action = action;
                AvailableOnNightmare = availableOnNightmare;
            }
        }
    }
}

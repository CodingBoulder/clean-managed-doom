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

namespace ManagedDoom
{
    public sealed class Finale
    {
        public static readonly int TextSpeed = 3;
        public static readonly int TextWait = 250;

        private readonly GameOptions _options;

        // Stage of animation:
        // 0 = text, 1 = art screen, 2 = character cast.
        private int _stage;
        private int _count;

        private readonly string _flat;
        private readonly string _text;

        // For bunny scroll.
        private int _scrolled;
        private bool _showTheEnd;
        private int _theEndIndex;

        private UpdateResult _updateResult;

        public Finale(GameOptions options)
        {
            _options = options;

            string c1Text;
            string c2Text;
            string c3Text;
            string c4Text;
            string c5Text;
            string c6Text;
            switch (options.MissionPack)
            {
                case MissionPack.Plutonia:
                    c1Text = DoomInfo.Strings.P1TEXT;
                    c2Text = DoomInfo.Strings.P2TEXT;
                    c3Text = DoomInfo.Strings.P3TEXT;
                    c4Text = DoomInfo.Strings.P4TEXT;
                    c5Text = DoomInfo.Strings.P5TEXT;
                    c6Text = DoomInfo.Strings.P6TEXT;
                    break;

                case MissionPack.Tnt:
                    c1Text = DoomInfo.Strings.T1TEXT;
                    c2Text = DoomInfo.Strings.T2TEXT;
                    c3Text = DoomInfo.Strings.T3TEXT;
                    c4Text = DoomInfo.Strings.T4TEXT;
                    c5Text = DoomInfo.Strings.T5TEXT;
                    c6Text = DoomInfo.Strings.T6TEXT;
                    break;

                default:
                    c1Text = DoomInfo.Strings.C1TEXT;
                    c2Text = DoomInfo.Strings.C2TEXT;
                    c3Text = DoomInfo.Strings.C3TEXT;
                    c4Text = DoomInfo.Strings.C4TEXT;
                    c5Text = DoomInfo.Strings.C5TEXT;
                    c6Text = DoomInfo.Strings.C6TEXT;
                    break;
            }

            switch (options.GameMode)
            {
                case GameMode.Shareware:
                case GameMode.Registered:
                case GameMode.Retail:
                    options.Music.StartMusic(Bgm.VICTOR, true);
                    switch (options.Episode)
                    {
                        case 1:
                            _flat = "FLOOR4_8";
                            _text = DoomInfo.Strings.E1TEXT;
                            break;

                        case 2:
                            _flat = "SFLR6_1";
                            _text = DoomInfo.Strings.E2TEXT;
                            break;

                        case 3:
                            _flat = "MFLR8_4";
                            _text = DoomInfo.Strings.E3TEXT;
                            break;

                        case 4:
                            _flat = "MFLR8_3";
                            _text = DoomInfo.Strings.E4TEXT;
                            break;

                        default:
                            break;
                    }
                    break;

                case GameMode.Commercial:
                    options.Music.StartMusic(Bgm.READ_M, true);
                    switch (options.Map)
                    {
                        case 6:
                            _flat = "SLIME16";
                            _text = c1Text;
                            break;

                        case 11:
                            _flat = "RROCK14";
                            _text = c2Text;
                            break;

                        case 20:
                            _flat = "RROCK07";
                            _text = c3Text;
                            break;

                        case 30:
                            _flat = "RROCK17";
                            _text = c4Text;
                            break;

                        case 15:
                            _flat = "RROCK13";
                            _text = c5Text;
                            break;

                        case 31:
                            _flat = "RROCK19";
                            _text = c6Text;
                            break;

                        default:
                            break;
                    }
                    break;

                default:
                    options.Music.StartMusic(Bgm.READ_M, true);
                    _flat = "F_SKY1";
                    _text = DoomInfo.Strings.C1TEXT;
                    break;
            }

            _stage = 0;
            _count = 0;

            _scrolled = 0;
            _showTheEnd = false;
            _theEndIndex = 0;
        }

        public UpdateResult Update()
        {
            _updateResult = UpdateResult.None;

            // Check for skipping.
            if (_options.GameMode == GameMode.Commercial && _count > 50)
            {
                int i;

                // Go on to the next level.
                for (i = 0; i < Player.MaxPlayerCount; i++)
                {
                    if (_options.Players[i].Cmd.Buttons != 0)
                    {
                        break;
                    }
                }

                if (i < Player.MaxPlayerCount && _stage != 2)
                {
                    if (_options.Map == 30)
                    {
                        StartCast();
                    }
                    else
                    {
                        return UpdateResult.Completed;
                    }
                }
            }

            // Advance animation.
            _count++;

            if (_stage == 2)
            {
                UpdateCast();
                return _updateResult;
            }

            if (_options.GameMode == GameMode.Commercial)
            {
                return _updateResult;
            }

            if (_stage == 0 && _count > _text.Length * TextSpeed + TextWait)
            {
                _count = 0;
                _stage = 1;
                _updateResult = UpdateResult.NeedWipe;
                if (_options.Episode == 3)
                {
                    _options.Music.StartMusic(Bgm.BUNNY, true);
                }
            }

            if (_stage == 1 && _options.Episode == 3)
            {
                BunnyScroll();
            }

            return _updateResult;
        }

        private void BunnyScroll()
        {
            _scrolled = 320 - (_count - 230) / 2;
            if (_scrolled > 320)
            {
                _scrolled = 320;
            }
            if (_scrolled < 0)
            {
                _scrolled = 0;
            }

            if (_count < 1130)
            {
                return;
            }

            _showTheEnd = true;

            if (_count < 1180)
            {
                _theEndIndex = 0;
                return;
            }

            int stage = (_count - 1180) / 5;
            if (stage > 6)
            {
                stage = 6;
            }
            if (stage > _theEndIndex)
            {
                StartSound(Sfx.PISTOL);
                _theEndIndex = stage;
            }
        }



        private static readonly CastInfo[] _castorder =
        [
            new CastInfo(DoomInfo.Strings.CC_ZOMBIE, MobjType.Possessed),
            new CastInfo(DoomInfo.Strings.CC_SHOTGUN, MobjType.Shotguy),
            new CastInfo(DoomInfo.Strings.CC_HEAVY, MobjType.Chainguy),
            new CastInfo(DoomInfo.Strings.CC_IMP, MobjType.Troop),
            new CastInfo(DoomInfo.Strings.CC_DEMON, MobjType.Sergeant),
            new CastInfo(DoomInfo.Strings.CC_LOST, MobjType.Skull),
            new CastInfo(DoomInfo.Strings.CC_CACO, MobjType.Head),
            new CastInfo(DoomInfo.Strings.CC_HELL, MobjType.Knight),
            new CastInfo(DoomInfo.Strings.CC_BARON, MobjType.Bruiser),
            new CastInfo(DoomInfo.Strings.CC_ARACH, MobjType.Baby),
            new CastInfo(DoomInfo.Strings.CC_PAIN, MobjType.Pain),
            new CastInfo(DoomInfo.Strings.CC_REVEN, MobjType.Undead),
            new CastInfo(DoomInfo.Strings.CC_MANCU, MobjType.Fatso),
            new CastInfo(DoomInfo.Strings.CC_ARCH, MobjType.Vile),
            new CastInfo(DoomInfo.Strings.CC_SPIDER, MobjType.Spider),
            new CastInfo(DoomInfo.Strings.CC_CYBER, MobjType.Cyborg),
            new CastInfo(DoomInfo.Strings.CC_HERO, MobjType.Player)
        ];

        private int _castNumber;
        private MobjStateDef _castState;
        private int _castTics;
        private int _castFrames;
        private bool _castDeath;
        private bool _castOnMelee;
        private bool _castAttacking;

        private void StartCast()
        {
            _stage = 2;

            _castNumber = 0;
            _castState = DoomInfo.States[(int)DoomInfo.MobjInfos[(int)_castorder[_castNumber].Type].SeeState];
            _castTics = _castState.Tics;
            _castFrames = 0;
            _castDeath = false;
            _castOnMelee = false;
            _castAttacking = false;

            _updateResult = UpdateResult.NeedWipe;

            _options.Music.StartMusic(Bgm.EVIL, true);
        }

        private void UpdateCast()
        {
            if (--_castTics > 0)
            {
                // Not time to change state yet.
                return;
            }

            if (_castState.Tics == -1 || _castState.Next == MobjState.Null)
            {
                // Switch from deathstate to next monster.
                _castNumber++;
                _castDeath = false;
                if (_castNumber == _castorder.Length)
                {
                    _castNumber = 0;
                }
                if (DoomInfo.MobjInfos[(int)_castorder[_castNumber].Type].SeeSound != 0)
                {
                    StartSound(DoomInfo.MobjInfos[(int)_castorder[_castNumber].Type].SeeSound);
                }
                _castState = DoomInfo.States[(int)DoomInfo.MobjInfos[(int)_castorder[_castNumber].Type].SeeState];
                _castFrames = 0;
            }
            else
            {
                // Just advance to next state in animation.
                if (_castState == DoomInfo.States[(int)MobjState.PlayAtk1])
                {
                    // Oh, gross hack!
                    _castAttacking = false;
                    _castState = DoomInfo.States[(int)DoomInfo.MobjInfos[(int)_castorder[_castNumber].Type].SeeState];
                    _castFrames = 0;
                    goto stopAttack;
                }
                MobjState st = _castState.Next;
                _castState = DoomInfo.States[(int)st];
                _castFrames++;

                // Sound hacks....
                Sfx sfx;
                switch (st)
                {
                    case MobjState.PlayAtk1:
                        sfx = Sfx.DSHTGN;
                        break;

                    case MobjState.PossAtk2:
                        sfx = Sfx.PISTOL;
                        break;

                    case MobjState.SposAtk2:
                        sfx = Sfx.SHOTGN;
                        break;

                    case MobjState.VileAtk2:
                        sfx = Sfx.VILATK;
                        break;

                    case MobjState.SkelFist2:
                        sfx = Sfx.SKESWG;
                        break;

                    case MobjState.SkelFist4:
                        sfx = Sfx.SKEPCH;
                        break;

                    case MobjState.SkelMiss2:
                        sfx = Sfx.SKEATK;
                        break;

                    case MobjState.FattAtk8:
                    case MobjState.FattAtk5:
                    case MobjState.FattAtk2:
                        sfx = Sfx.FIRSHT;
                        break;

                    case MobjState.CposAtk2:
                    case MobjState.CposAtk3:
                    case MobjState.CposAtk4:
                        sfx = Sfx.SHOTGN;
                        break;

                    case MobjState.TrooAtk3:
                        sfx = Sfx.CLAW;
                        break;

                    case MobjState.SargAtk2:
                        sfx = Sfx.SGTATK;
                        break;

                    case MobjState.BossAtk2:
                    case MobjState.Bos2Atk2:
                    case MobjState.HeadAtk2:
                        sfx = Sfx.FIRSHT;
                        break;

                    case MobjState.SkullAtk2:
                        sfx = Sfx.SKLATK;
                        break;

                    case MobjState.SpidAtk2:
                    case MobjState.SpidAtk3:
                        sfx = Sfx.SHOTGN;
                        break;

                    case MobjState.BspiAtk2:
                        sfx = Sfx.PLASMA;
                        break;

                    case MobjState.CyberAtk2:
                    case MobjState.CyberAtk4:
                    case MobjState.CyberAtk6:
                        sfx = Sfx.RLAUNC;
                        break;

                    case MobjState.PainAtk3:
                        sfx = Sfx.SKLATK;
                        break;

                    default:
                        sfx = 0;
                        break;
                }

                if (sfx != 0)
                {
                    StartSound(sfx);
                }
            }

            if (_castFrames == 12)
            {
                // Go into attack frame.
                _castAttacking = true;
                if (_castOnMelee)
                {
                    _castState = DoomInfo.States[(int)DoomInfo.MobjInfos[(int)_castorder[_castNumber].Type].MeleeState];
                }
                else
                {
                    _castState = DoomInfo.States[(int)DoomInfo.MobjInfos[(int)_castorder[_castNumber].Type].MissileState];
                }

                _castOnMelee = !_castOnMelee;
                if (_castState == DoomInfo.States[(int)MobjState.Null])
                {
                    if (_castOnMelee)
                    {
                        _castState = DoomInfo.States[(int)DoomInfo.MobjInfos[(int)_castorder[_castNumber].Type].MeleeState];
                    }
                    else
                    {
                        _castState = DoomInfo.States[(int)DoomInfo.MobjInfos[(int)_castorder[_castNumber].Type].MissileState];
                    }
                }
            }

            if (_castAttacking)
            {
                if (_castFrames == 24 ||
                    _castState == DoomInfo.States[(int)DoomInfo.MobjInfos[(int)_castorder[_castNumber].Type].SeeState])
                {
                    _castAttacking = false;
                    _castState = DoomInfo.States[(int)DoomInfo.MobjInfos[(int)_castorder[_castNumber].Type].SeeState];
                    _castFrames = 0;
                }
            }

        stopAttack:

            _castTics = _castState.Tics;
            if (_castTics == -1)
            {
                _castTics = 15;
            }
        }

        public bool DoEvent(DoomEvent e)
        {
            if (_stage != 2)
            {
                return false;
            }

            if (e.Type == EventType.KeyDown)
            {
                if (_castDeath)
                {
                    // Already in dying frames.
                    return true;
                }

                // Go into death frame.
                _castDeath = true;
                _castState = DoomInfo.States[(int)DoomInfo.MobjInfos[(int)_castorder[_castNumber].Type].DeathState];
                _castTics = _castState.Tics;
                _castFrames = 0;
                _castAttacking = false;
                if (DoomInfo.MobjInfos[(int)_castorder[_castNumber].Type].DeathSound != 0)
                {
                    StartSound(DoomInfo.MobjInfos[(int)_castorder[_castNumber].Type].DeathSound);
                }

                return true;
            }

            return false;
        }

        private void StartSound(Sfx sfx)
        {
            _options.Sound.StartSound(sfx);
        }



        public GameOptions Options => _options;
        public string Flat => _flat;
        public string Text => _text;
        public int Count => _count;
        public int Stage => _stage;

        // For cast.
        public string CastName => _castorder[_castNumber].Name;
        public MobjStateDef CastState => _castState;

        // For bunny scroll.
        public int Scrolled => _scrolled;
        public int TheEndIndex => _theEndIndex;
        public bool ShowTheEnd => _showTheEnd;



        private class CastInfo
        {
            public string Name;
            public MobjType Type;

            public CastInfo(string name, MobjType type)
            {
                Name = name;
                Type = type;
            }
        }
    }
}

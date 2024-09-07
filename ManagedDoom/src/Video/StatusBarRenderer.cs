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

namespace ManagedDoom.Video
{
    public sealed class StatusBarRenderer
    {
        public static readonly int Height = 32;

        // Ammo number pos.
        private static readonly int _ammoWidth = 3;
        private static readonly int _ammoX = 44;
        private static readonly int _ammoY = 171;

        // Health number pos.
        private static readonly int _healthX = 90;
        private static readonly int _healthY = 171;

        // Weapon pos.
        private static readonly int _armsX = 111;
        private static readonly int _armsY = 172;
        private static readonly int _armsBackgroundX = 104;
        private static readonly int _armsBackgroundY = 168;
        private static readonly int _armsSpaceX = 12;
        private static readonly int _armsSpaceY = 10;

        // Frags pos.
        private static readonly int _fragsWidth = 2;
        private static readonly int _fragsX = 138;
        private static readonly int _fragsY = 171;

        // Armor number pos.
        private static readonly int _armorX = 221;
        private static readonly int _armorY = 171;

        // Key icon positions.
        private static readonly int _key0Width = 8;
        private static readonly int _key0X = 239;
        private static readonly int _key0Y = 171;
#pragma warning disable IDE0052 // Remove unread private members
        private static readonly int _key1Width = _key0Width;
#pragma warning restore IDE0052 // Remove unread private members
        private static readonly int _key1X = 239;
        private static readonly int _key1Y = 181;
#pragma warning disable IDE0052 // Remove unread private members
        private static readonly int _key2Width = _key0Width;
#pragma warning restore IDE0052 // Remove unread private members
        private static readonly int _key2X = 239;
        private static readonly int _key2Y = 191;

        // Ammunition counter.
        private static readonly int _ammo0Width = 3;
        private static readonly int _ammo0X = 288;
        private static readonly int _ammo0Y = 173;
        private static readonly int _ammo1Width = _ammo0Width;
        private static readonly int _ammo1X = 288;
        private static readonly int _ammo1Y = 179;
        private static readonly int _ammo2Width = _ammo0Width;
        private static readonly int _ammo2X = 288;
        private static readonly int _ammo2Y = 191;
        private static readonly int _ammo3Wdth = _ammo0Width;
        private static readonly int _ammo3X = 288;
        private static readonly int _ammo3Y = 185;

        // Indicate maximum ammunition.
        // Only needed because backpack exists.
        private static readonly int _maxAmmo0Width = 3;
        private static readonly int _maxAmmo0X = 314;
        private static readonly int _maxAmmo0Y = 173;
        private static readonly int _maxAmmo1Width = _maxAmmo0Width;
        private static readonly int _maxAmmo1X = 314;
        private static readonly int _maxAmmo1Y = 179;
        private static readonly int _maxAmmo2Width = _maxAmmo0Width;
        private static readonly int _maxAmmo2X = 314;
        private static readonly int _maxAmmo2Y = 191;
        private static readonly int _maxAmmo3Width = _maxAmmo0Width;
        private static readonly int _maxAmmo3X = 314;
        private static readonly int _maxAmmo3Y = 185;

        private static readonly int _faceX = 143;
        private static readonly int _faceY = 168;
        private static readonly int _faceBackgroundX = 143;
        private static readonly int _faceBackgroundY = 169;

        private readonly DrawScreen _screen;

        private readonly Patches _patches;

        private readonly int _scale;

        private readonly NumberWidget _ready;
        private readonly PercentWidget _health;
        private readonly PercentWidget _armor;

        private readonly NumberWidget[] _ammo;
        private readonly NumberWidget[] _maxAmmo;

        private readonly MultIconWidget[] _weapons;

        private readonly NumberWidget _frags;

        private readonly MultIconWidget[] _keys;

        public StatusBarRenderer(Wad wad, DrawScreen screen)
        {
            _screen = screen;

            _patches = new Patches(wad);

            _scale = screen.Width / 320;

            _ready = new NumberWidget
            {
                Patches = _patches.TallNumbers,
                Width = _ammoWidth,
                X = _ammoX,
                Y = _ammoY
            };

            _health = new PercentWidget();
            _health.NumberWidget.Patches = _patches.TallNumbers;
            _health.NumberWidget.Width = 3;
            _health.NumberWidget.X = _healthX;
            _health.NumberWidget.Y = _healthY;
            _health.Patch = _patches.TallPercent;

            _armor = new PercentWidget();
            _armor.NumberWidget.Patches = _patches.TallNumbers;
            _armor.NumberWidget.Width = 3;
            _armor.NumberWidget.X = _armorX;
            _armor.NumberWidget.Y = _armorY;
            _armor.Patch = _patches.TallPercent;

            _ammo = new NumberWidget[(int)AmmoType.Count];
            _ammo[0] = new NumberWidget
            {
                Patches = _patches.ShortNumbers,
                Width = _ammo0Width,
                X = _ammo0X,
                Y = _ammo0Y
            };
            _ammo[1] = new NumberWidget
            {
                Patches = _patches.ShortNumbers,
                Width = _ammo1Width,
                X = _ammo1X,
                Y = _ammo1Y
            };
            _ammo[2] = new NumberWidget
            {
                Patches = _patches.ShortNumbers,
                Width = _ammo2Width,
                X = _ammo2X,
                Y = _ammo2Y
            };
            _ammo[3] = new NumberWidget
            {
                Patches = _patches.ShortNumbers,
                Width = _ammo3Wdth,
                X = _ammo3X,
                Y = _ammo3Y
            };

            _maxAmmo = new NumberWidget[(int)AmmoType.Count];
            _maxAmmo[0] = new NumberWidget
            {
                Patches = _patches.ShortNumbers,
                Width = _maxAmmo0Width,
                X = _maxAmmo0X,
                Y = _maxAmmo0Y
            };
            _maxAmmo[1] = new NumberWidget
            {
                Patches = _patches.ShortNumbers,
                Width = _maxAmmo1Width,
                X = _maxAmmo1X,
                Y = _maxAmmo1Y
            };
            _maxAmmo[2] = new NumberWidget
            {
                Patches = _patches.ShortNumbers,
                Width = _maxAmmo2Width,
                X = _maxAmmo2X,
                Y = _maxAmmo2Y
            };
            _maxAmmo[3] = new NumberWidget
            {
                Patches = _patches.ShortNumbers,
                Width = _maxAmmo3Width,
                X = _maxAmmo3X,
                Y = _maxAmmo3Y
            };

            _weapons = new MultIconWidget[6];
            for (int i = 0; i < _weapons.Length; i++)
            {
                _weapons[i] = new MultIconWidget
                {
                    X = _armsX + (i % 3) * _armsSpaceX,
                    Y = _armsY + (i / 3) * _armsSpaceY,
                    Patches = _patches.Arms[i]
                };
            }

            _frags = new NumberWidget
            {
                Patches = _patches.TallNumbers,
                Width = _fragsWidth,
                X = _fragsX,
                Y = _fragsY
            };

            _keys = new MultIconWidget[3];
            _keys[0] = new MultIconWidget
            {
                X = _key0X,
                Y = _key0Y,
                Patches = _patches.Keys
            };
            _keys[1] = new MultIconWidget
            {
                X = _key1X,
                Y = _key1Y,
                Patches = _patches.Keys
            };
            _keys[2] = new MultIconWidget
            {
                X = _key2X,
                Y = _key2Y,
                Patches = _patches.Keys
            };
        }

        public void Render(Player player, bool drawBackground)
        {
            if (drawBackground)
            {
                _screen.DrawPatch(
                    _patches.Background,
                    0,
                    _scale * (200 - Height),
                    _scale);
            }

            if (DoomInfo.WeaponInfos[(int)player.ReadyWeapon].Ammo != AmmoType.NoAmmo)
            {
                int num = player.Ammo[(int)DoomInfo.WeaponInfos[(int)player.ReadyWeapon].Ammo];
                DrawNumber(_ready, num);
            }

            DrawPercent(_health, player.Health);
            DrawPercent(_armor, player.ArmorPoints);

            for (int i = 0; i < (int)AmmoType.Count; i++)
            {
                DrawNumber(_ammo[i], player.Ammo[i]);
                DrawNumber(_maxAmmo[i], player.MaxAmmo[i]);
            }

            if (player.Mobj.World.Options.Deathmatch == 0)
            {
                if (drawBackground)
                {
                    _screen.DrawPatch(
                        _patches.ArmsBackground,
                        _scale * _armsBackgroundX,
                        _scale * _armsBackgroundY,
                        _scale);
                }

                for (int i = 0; i < _weapons.Length; i++)
                {
                    DrawMultIcon(_weapons[i], player.WeaponOwned[i + 1] ? 1 : 0);
                }
            }
            else
            {
                int sum = 0;
                for (int i = 0; i < player.Frags.Length; i++)
                {
                    sum += player.Frags[i];
                }
                DrawNumber(_frags, sum);
            }

            if (drawBackground)
            {
                if (player.Mobj.World.Options.NetGame)
                {
                    _screen.DrawPatch(
                        _patches.FaceBackground[player.Number],
                        _scale * _faceBackgroundX,
                        _scale * _faceBackgroundY,
                        _scale);
                }

                _screen.DrawPatch(
                    _patches.Faces[player.Mobj.World.StatusBar.FaceIndex],
                    _scale * _faceX,
                    _scale * _faceY,
                    _scale);
            }

            for (int i = 0; i < 3; i++)
            {
                if (player.Cards[i + 3])
                {
                    DrawMultIcon(_keys[i], i + 3);
                }
                else if (player.Cards[i])
                {
                    DrawMultIcon(_keys[i], i);
                }
            }
        }

        private void DrawNumber(NumberWidget widget, int num)
        {
            int digits = widget.Width;

            int w = widget.Patches[0].Width;
            int h = widget.Patches[0].Height;
            int x = widget.X;

            bool neg = num < 0;

            if (neg)
            {
                if (digits == 2 && num < -9)
                {
                    num = -9;
                }
                else if (digits == 3 && num < -99)
                {
                    num = -99;
                }

                num = -num;
            }

            x = widget.X - digits * w;

            if (num == 1994)
            {
                return;
            }

            x = widget.X;

            // In the special case of 0, you draw 0.
            if (num == 0)
            {
                _screen.DrawPatch(
                    widget.Patches[0],
                    _scale * (x - w),
                    _scale * widget.Y,
                    _scale);
            }

            // Draw the new number.
            while (num != 0 && digits-- != 0)
            {
                x -= w;

                _screen.DrawPatch(
                    widget.Patches[num % 10],
                    _scale * x,
                    _scale * widget.Y,
                    _scale);

                num /= 10;
            }

            // Draw a minus sign if necessary.
            if (neg)
            {
                _screen.DrawPatch(
                    _patches.TallMinus,
                    _scale * (x - 8),
                    _scale * widget.Y,
                    _scale);
            }
        }

        private void DrawPercent(PercentWidget per, int value)
        {
            _screen.DrawPatch(
                per.Patch,
                _scale * per.NumberWidget.X,
                _scale * per.NumberWidget.Y,
                _scale);

            DrawNumber(per.NumberWidget, value);
        }

        private void DrawMultIcon(MultIconWidget mi, int value)
        {
            _screen.DrawPatch(
                mi.Patches[value],
                _scale * mi.X,
                _scale * mi.Y,
                _scale);
        }



        private class NumberWidget
        {
            public int X;
            public int Y;
            public int Width;
            public Patch[] Patches;
        }

        private class PercentWidget
        {
            public NumberWidget NumberWidget = new();
            public Patch Patch;
        }

        private class MultIconWidget
        {
            public int X;
            public int Y;
            public Patch[] Patches;
        }

        private class Patches
        {
            public Patch Background;
            public Patch[] TallNumbers;
            public Patch[] ShortNumbers;
            public Patch TallMinus;
            public Patch TallPercent;
            public Patch[] Keys;
            public Patch ArmsBackground;
            public Patch[][] Arms;
            public Patch[] FaceBackground;
            public Patch[] Faces;

            public Patches(Wad wad)
            {
                Background = Patch.FromWad(wad, "STBAR");

                TallNumbers = new Patch[10];
                ShortNumbers = new Patch[10];
                for (int i = 0; i < 10; i++)
                {
                    TallNumbers[i] = Patch.FromWad(wad, "STTNUM" + i);
                    ShortNumbers[i] = Patch.FromWad(wad, "STYSNUM" + i);
                }
                TallMinus = Patch.FromWad(wad, "STTMINUS");
                TallPercent = Patch.FromWad(wad, "STTPRCNT");

                Keys = new Patch[(int)CardType.Count];
                for (int i = 0; i < Keys.Length; i++)
                {
                    Keys[i] = Patch.FromWad(wad, "STKEYS" + i);
                }

                ArmsBackground = Patch.FromWad(wad, "STARMS");
                Arms = new Patch[6][];
                for (int i = 0; i < 6; i++)
                {
                    int num = i + 2;
                    Arms[i] = new Patch[2];
                    Arms[i][0] = Patch.FromWad(wad, "STGNUM" + num);
                    Arms[i][1] = ShortNumbers[num];
                }

                FaceBackground = new Patch[Player.MaxPlayerCount];
                for (int i = 0; i < FaceBackground.Length; i++)
                {
                    FaceBackground[i] = Patch.FromWad(wad, "STFB" + i);
                }
                Faces = new Patch[StatusBar.Face.FaceCount];
                int faceCount = 0;
                for (int i = 0; i < StatusBar.Face.PainFaceCount; i++)
                {
                    for (int j = 0; j < StatusBar.Face.StraightFaceCount; j++)
                    {
                        Faces[faceCount++] = Patch.FromWad(wad, "STFST" + i + j);
                    }
                    Faces[faceCount++] = Patch.FromWad(wad, "STFTR" + i + "0");
                    Faces[faceCount++] = Patch.FromWad(wad, "STFTL" + i + "0");
                    Faces[faceCount++] = Patch.FromWad(wad, "STFOUCH" + i);
                    Faces[faceCount++] = Patch.FromWad(wad, "STFEVL" + i);
                    Faces[faceCount++] = Patch.FromWad(wad, "STFKILL" + i);
                }
                Faces[faceCount++] = Patch.FromWad(wad, "STFGOD0");
                Faces[faceCount++] = Patch.FromWad(wad, "STFDEAD0");
            }
        }
    }
}

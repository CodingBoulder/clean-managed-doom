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
    public sealed class AutoMapRenderer
    {
        private static readonly float _pr = 8 * DoomInfo.MobjInfos[(int)MobjType.Player].Radius.ToFloat() / 7;

        // The vector graphics for the automap.
        // A line drawing of the player pointing right, starting from the middle.
        private static readonly float[] _playerArrow =
        [
            -_pr + _pr / 8, 0, _pr, 0, // -----
            _pr, 0, _pr - _pr / 2, _pr / 4, // ----->
            _pr, 0, _pr - _pr / 2, -_pr / 4,
            -_pr + _pr / 8, 0, -_pr - _pr / 8, _pr / 4, // >---->
            -_pr + _pr / 8, 0, -_pr - _pr / 8, -_pr / 4,
            -_pr + 3 * _pr / 8, 0, -_pr + _pr / 8, _pr / 4, // >>--->
            -_pr + 3 * _pr / 8, 0, -_pr + _pr / 8, -_pr / 4
        ];

        private static readonly float _tr = 16;

        private static readonly float[] _thingTriangle =
        [
            -0.5F * _tr, -0.7F * _tr, _tr, 0F,
            _tr, 0F, -0.5F * _tr, 0.7F * _tr,
            -0.5F * _tr, 0.7F * _tr, -0.5F * _tr, -0.7F * _tr
        ];

        // For use if I do walls with outsides / insides.
        private static readonly int _reds = (256 - 5 * 16);
        private static readonly int _redRange = 16;
        private static readonly int _greens = (7 * 16);
        private static readonly int _greenRange = 16;
        private static readonly int _grays = (6 * 16);
        private static readonly int _grayRange = 16;
        private static readonly int _browns = (4 * 16);
        private static readonly int _brownRange = 16;
        private static readonly int _yellows = (256 - 32 + 7);
        private static readonly int _yellowRange = 1;
        private static readonly int _black = 0;
        private static readonly int _white = (256 - 47);

        // Automap colors.
        private static readonly int _background = _black;
        private static readonly int _wallColors = _reds;
        private static readonly int _wallRange = _redRange;
        private static readonly int _tsWallColors = _grays;
#pragma warning disable IDE0052 // Remove unread private members
        private static readonly int _tsWallRange = _grayRange;
        private static readonly int _fdWallColors = _browns;
        private static readonly int _fdWallRange = _brownRange;
        private static readonly int _cdWallColors = _yellows;
        private static readonly int _cdWallRange = _yellowRange;
        private static readonly int _thingColors = _greens;
        private static readonly int _thingRange = _greenRange;
        private static readonly int _secretWallColors = _wallColors;
        private static readonly int _secretWallRange = _wallRange;
#pragma warning restore IDE0052 // Remove unread private members

        private static readonly int[] _playerColors =
        [
            _greens,
            _grays,
            _browns,
            _reds
        ];

        private readonly DrawScreen _screen;

        private readonly int _scale;
        private readonly int _amWidth;
        private readonly int _amHeight;
        private readonly float _ppu;

        private float _minX;
        private float _maxX;
        private float _minY;
        private float _maxY;

#pragma warning disable IDE0052 // Remove unread private members
        private float _width;
        private float _height;
#pragma warning restore IDE0052 // Remove unread private members

        private float _actualViewX;
        private float _actualViewY;
        private float _zoom;

        private float _renderViewX;
        private float _renderViewY;

        private readonly Patch[] _markNumbers;

        public AutoMapRenderer(Wad wad, DrawScreen screen)
        {
            _screen = screen;

            _scale = screen.Width / 320;
            _amWidth = screen.Width;
            _amHeight = screen.Height - _scale * StatusBarRenderer.Height;
            _ppu = (float)_scale / 16;

            _markNumbers = new Patch[10];
            for (int i = 0; i < _markNumbers.Length; i++)
            {
                _markNumbers[i] = Patch.FromWad(wad, "AMMNUM" + i);
            }
        }

        public void Render(Player player)
        {
            _screen.FillRect(0, 0, _amWidth, _amHeight, _background);

            World world = player.Mobj.World;
            AutoMap am = world.AutoMap;

            _minX = am.MinX.ToFloat();
            _maxX = am.MaxX.ToFloat();
            _width = _maxX - _minX;
            _minY = am.MinY.ToFloat();
            _maxY = am.MaxY.ToFloat();
            _height = _maxY - _minY;

            _actualViewX = am.ViewX.ToFloat();
            _actualViewY = am.ViewY.ToFloat();
            _zoom = am.Zoom.ToFloat();

            // This hack aligns the view point to an integer coordinate
            // so that line shake is reduced when the view point moves.
            _renderViewX = MathF.Round(_zoom * _ppu * _actualViewX) / (_zoom * _ppu);
            _renderViewY = MathF.Round(_zoom * _ppu * _actualViewY) / (_zoom * _ppu);

            foreach (LineDef line in world.Map.Lines)
            {
                DrawPos v1 = ToScreenPos(line.Vertex1);
                DrawPos v2 = ToScreenPos(line.Vertex2);

                bool cheating = am.State != AutoMapState.None;

                if (cheating || (line.Flags & LineFlags.Mapped) != 0)
                {
                    if ((line.Flags & LineFlags.DontDraw) != 0 && !cheating)
                    {
                        continue;
                    }

                    if (line.BackSector == null)
                    {
                        _screen.DrawLine(v1.X, v1.Y, v2.X, v2.Y, _wallColors);
                    }
                    else
                    {
                        if (line.Special == (LineSpecial)39)
                        {
                            // Teleporters.
                            _screen.DrawLine(v1.X, v1.Y, v2.X, v2.Y, _wallColors + _wallRange / 2);
                        }
                        else if ((line.Flags & LineFlags.Secret) != 0)
                        {
                            // Secret door.
                            if (cheating)
                            {
                                _screen.DrawLine(v1.X, v1.Y, v2.X, v2.Y, _secretWallColors);
                            }
                            else
                            {
                                _screen.DrawLine(v1.X, v1.Y, v2.X, v2.Y, _wallColors);
                            }
                        }
                        else if (line.BackSector.FloorHeight != line.FrontSector.FloorHeight)
                        {
                            // Floor level change.
                            _screen.DrawLine(v1.X, v1.Y, v2.X, v2.Y, _fdWallColors);
                        }
                        else if (line.BackSector.CeilingHeight != line.FrontSector.CeilingHeight)
                        {
                            // Ceiling level change.
                            _screen.DrawLine(v1.X, v1.Y, v2.X, v2.Y, _cdWallColors);
                        }
                        else if (cheating)
                        {
                            _screen.DrawLine(v1.X, v1.Y, v2.X, v2.Y, _tsWallColors);
                        }
                    }
                }
                else if (player.Powers[(int)PowerType.AllMap] > 0)
                {
                    if ((line.Flags & LineFlags.DontDraw) == 0)
                    {
                        _screen.DrawLine(v1.X, v1.Y, v2.X, v2.Y, _grays + 3);
                    }
                }
            }

            for (int i = 0; i < am.Marks.Count; i++)
            {
                DrawPos pos = ToScreenPos(am.Marks[i]);
                _screen.DrawPatch(
                    _markNumbers[i],
                    (int)MathF.Round(pos.X),
                    (int)MathF.Round(pos.Y),
                    _scale);
            }

            if (am.State == AutoMapState.AllThings)
            {
                DrawThings(world);
            }

            DrawPlayers(world);

            if (!am.Follow)
            {
                _screen.DrawLine(
                    _amWidth / 2 - 2 * _scale, _amHeight / 2,
                    _amWidth / 2 + 2 * _scale, _amHeight / 2,
                    _grays);

                _screen.DrawLine(
                    _amWidth / 2, _amHeight / 2 - 2 * _scale,
                    _amWidth / 2, _amHeight / 2 + 2 * _scale,
                    _grays);
            }

            _screen.DrawText(
                world.Map.Title,
                0,
                _amHeight - _scale,
                _scale);
        }

        private void DrawPlayers(World world)
        {
            GameOptions options = world.Options;
            Player[] players = options.Players;
            Player consolePlayer = world.ConsolePlayer;
            AutoMap am = world.AutoMap;

            if (!options.NetGame)
            {
                DrawCharacter(consolePlayer.Mobj, _playerArrow, _white);
                return;
            }

            for (int i = 0; i < Player.MaxPlayerCount; i++)
            {
                Player player = players[i];
                if (options.Deathmatch != 0 && !options.DemoPlayback && player != consolePlayer)
                {
                    continue;
                }

                if (!player.InGame)
                {
                    continue;
                }

                int color;
                if (player.Powers[(int)PowerType.Invisibility] > 0)
                {
                    // Close to black.
                    color = 246;
                }
                else
                {
                    color = _playerColors[i];
                }

                DrawCharacter(player.Mobj, _playerArrow, color);
            }
        }

        private void DrawThings(World world)
        {
            foreach (Thinker thinker in world.Thinkers)
            {
                var mobj = thinker as Mobj;
                if (mobj != null)
                {
                    DrawCharacter(mobj, _thingTriangle, _greens);
                }
            }
        }

        private void DrawCharacter(Mobj mobj, float[] data, int color)
        {
            DrawPos pos = ToScreenPos(mobj.X, mobj.Y);
            float sin = (float)Math.Sin(mobj.Angle.ToRadian());
            float cos = (float)Math.Cos(mobj.Angle.ToRadian());
            for (int i = 0; i < data.Length; i += 4)
            {
                float x1 = pos.X + _zoom * _ppu * (cos * data[i + 0] - sin * data[i + 1]);
                float y1 = pos.Y - _zoom * _ppu * (sin * data[i + 0] + cos * data[i + 1]);
                float x2 = pos.X + _zoom * _ppu * (cos * data[i + 2] - sin * data[i + 3]);
                float y2 = pos.Y - _zoom * _ppu * (sin * data[i + 2] + cos * data[i + 3]);
                _screen.DrawLine(x1, y1, x2, y2, color);
            }
        }

        private DrawPos ToScreenPos(Fixed x, Fixed y)
        {
            float posX = _zoom * _ppu * (x.ToFloat() - _renderViewX) + _amWidth / 2;
            float posY = -_zoom * _ppu * (y.ToFloat() - _renderViewY) + _amHeight / 2;
            return new DrawPos(posX, posY);
        }

        private DrawPos ToScreenPos(Vertex v)
        {
            float posX = _zoom * _ppu * (v.X.ToFloat() - _renderViewX) + _amWidth / 2;
            float posY = -_zoom * _ppu * (v.Y.ToFloat() - _renderViewY) + _amHeight / 2;
            return new DrawPos(posX, posY);
        }



        private struct DrawPos
        {
            public float X;
            public float Y;

            public DrawPos(float x, float y)
            {
                X = x;
                Y = y;
            }
        }
    }
}

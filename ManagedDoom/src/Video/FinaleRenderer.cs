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
    public sealed class FinaleRenderer
    {
        private readonly Wad? _wad;
        private readonly IFlatLookup _flats;
        private readonly ISpriteLookup _sprites;

        private readonly DrawScreen _screen;
        private readonly int _scale;

        private readonly PatchCache _cache;

        public FinaleRenderer(GameContent content, DrawScreen screen)
        {
            _wad = content.Wad;
            _flats = content.Flats;
            _sprites = content.Sprites;

            _screen = screen;
            _scale = screen.Width / 320;

            _cache = new PatchCache(_wad);
        }

        public void Render(Finale finale)
        {
            if (finale.Stage == 2)
            {
                RenderCast(finale);
                return;
            }

            if (finale.Stage == 0)
            {
                RenderTextScreen(finale);
            }
            else
            {
                switch (finale.Options.Episode)
                {
                    case 1:
                        DrawPatch("CREDIT", 0, 0);
                        break;

                    case 2:
                        DrawPatch("VICTORY2", 0, 0);
                        break;

                    case 3:
                        BunnyScroll(finale);
                        break;

                    case 4:
                        DrawPatch("ENDPIC", 0, 0);
                        break;
                }
            }
        }

        private void RenderTextScreen(Finale finale)
        {
            FillFlat(_flats[finale.Flat]);

            // Draw some of the text onto the screen.
            int cx = 10 * _scale;
            int cy = 17 * _scale;
            int ch = 0;

            int count = (finale.Count - 10) / Finale.TextSpeed;
            if (count < 0)
            {
                count = 0;
            }

            for (; count > 0; count--)
            {
                if (ch == finale.Text.Length)
                {
                    break;
                }

                char c = finale.Text[ch++];

                if (c == '\n')
                {
                    cx = 10 * _scale;
                    cy += 11 * _scale;
                    continue;
                }

                _screen.DrawChar(c, cx, cy, _scale);

                cx += _screen.MeasureChar(c, _scale);
            }
        }

        private void BunnyScroll(Finale finale)
        {
            int scroll = 320 - finale.Scrolled;
            DrawPatch("PFUB2", scroll - 320, 0);
            DrawPatch("PFUB1", scroll, 0);

            if (finale.ShowTheEnd)
            {
                string patch = "END0";
                switch (finale.TheEndIndex)
                {
                    case 1:
                        patch = "END1";
                        break;
                    case 2:
                        patch = "END2";
                        break;
                    case 3:
                        patch = "END3";
                        break;
                    case 4:
                        patch = "END4";
                        break;
                    case 5:
                        patch = "END5";
                        break;
                    case 6:
                        patch = "END6";
                        break;
                }

                DrawPatch(
                    patch,
                    (320 - 13 * 8) / 2,
                    (240 - 8 * 8) / 2);
            }
        }

        private void FillFlat(Flat flat)
        {
            byte[] src = flat.Data;
            byte[] dst = _screen.Data;
            int scale = _screen.Width / 320;
            Fixed xFrac = Fixed.One / scale - Fixed.Epsilon;
            Fixed step = Fixed.One / scale;
            for (int x = 0; x < _screen.Width; x++)
            {
                Fixed yFrac = Fixed.One / scale - Fixed.Epsilon;
                int p = _screen.Height * x;
                for (int y = 0; y < _screen.Height; y++)
                {
                    int spotX = xFrac.ToIntFloor() & 0x3F;
                    int spotY = yFrac.ToIntFloor() & 0x3F;
                    dst[p] = src[(spotY << 6) + spotX];
                    yFrac += step;
                    p++;
                }
                xFrac += step;
            }
        }

        private void DrawPatch(string name, int x, int y)
        {
            int scale = _screen.Width / 320;
            _screen.DrawPatch(_cache[name], scale * x, scale * y, scale);
        }

        private void RenderCast(Finale finale)
        {
            DrawPatch("BOSSBACK", 0, 0);

            int frame = finale.CastState.Frame & 0x7fff;
            Patch patch = _sprites[finale.CastState.Sprite].Frames[frame].Patches[0];
            if (_sprites[finale.CastState.Sprite].Frames[frame].Flip[0])
            {
                _screen.DrawPatchFlip(
                    patch,
                    _screen.Width / 2,
                    _screen.Height - _scale * 30,
                    _scale);
            }
            else
            {
                _screen.DrawPatch(
                    patch,
                    _screen.Width / 2,
                    _screen.Height - _scale * 30,
                    _scale);
            }

            int width = _screen.MeasureText(finale.CastName, _scale);
            _screen.DrawText(
                finale.CastName,
                (_screen.Width - width) / 2,
                _screen.Height - _scale * 13,
                _scale);
        }
    }
}

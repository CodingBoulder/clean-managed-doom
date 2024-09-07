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
using System.Collections.Generic;

namespace ManagedDoom.Video
{
    public sealed class DrawScreen
    {
        private readonly int width;
        private readonly int height;
        private readonly byte[] data;

        private readonly Patch[] chars;

        public DrawScreen(Wad wad, int width, int height)
        {
            this.width = width;
            this.height = height;
            data = new byte[width * height];

            chars = new Patch[128];
            for (int i = 0; i < chars.Length; i++)
            {
                string name = "STCFN" + i.ToString("000");
                int lump = wad.GetLumpNumber(name);
                if (lump != -1)
                {
                    chars[i] = Patch.FromData(name, wad.ReadLump(lump));
                }
            }
        }

        public void DrawPatch(Patch patch, int x, int y, int scale)
        {
            int drawX = x - scale * patch.LeftOffset;
            int drawY = y - scale * patch.TopOffset;
            int drawWidth = scale * patch.Width;

            int i = 0;
            Fixed frac = Fixed.One / scale - Fixed.Epsilon;
            Fixed step = Fixed.One / scale;

            if (drawX < 0)
            {
                int exceed = -drawX;
                frac += exceed * step;
                i += exceed;
            }

            if (drawX + drawWidth > width)
            {
                int exceed = drawX + drawWidth - width;
                drawWidth -= exceed;
            }

            for (; i < drawWidth; i++)
            {
                DrawColumn(patch.Columns[frac.ToIntFloor()], drawX + i, drawY, scale);
                frac += step;
            }
        }

        public void DrawPatchFlip(Patch patch, int x, int y, int scale)
        {
            int drawX = x - scale * patch.LeftOffset;
            int drawY = y - scale * patch.TopOffset;
            int drawWidth = scale * patch.Width;

            int i = 0;
            Fixed frac = Fixed.One / scale - Fixed.Epsilon;
            Fixed step = Fixed.One / scale;

            if (drawX < 0)
            {
                int exceed = -drawX;
                frac += exceed * step;
                i += exceed;
            }

            if (drawX + drawWidth > width)
            {
                int exceed = drawX + drawWidth - width;
                drawWidth -= exceed;
            }

            for (; i < drawWidth; i++)
            {
                int col = patch.Width - frac.ToIntFloor() - 1;
                DrawColumn(patch.Columns[col], drawX + i, drawY, scale);
                frac += step;
            }
        }

        private void DrawColumn(Column[] source, int x, int y, int scale)
        {
            Fixed step = Fixed.One / scale;

            foreach (Column column in source)
            {
                int exTopDelta = scale * column.TopDelta;
                int exLength = scale * column.Length;

                int sourceIndex = column.Offset;
                int drawY = y + exTopDelta;
                int drawLength = exLength;

                int i = 0;
                int p = height * x + drawY;
                Fixed frac = Fixed.One / scale - Fixed.Epsilon;

                if (drawY < 0)
                {
                    int exceed = -drawY;
                    p += exceed;
                    frac += exceed * step;
                    i += exceed;
                }

                if (drawY + drawLength > height)
                {
                    int exceed = drawY + drawLength - height;
                    drawLength -= exceed;
                }

                for (; i < drawLength; i++)
                {
                    data[p] = column.Data[sourceIndex + frac.ToIntFloor()];
                    p++;
                    frac += step;
                }
            }
        }

        public void DrawText(IReadOnlyList<char> text, int x, int y, int scale)
        {
            int drawX = x;
            int drawY = y - 7 * scale;
            foreach (char ch in text)
            {
                if (ch >= chars.Length)
                {
                    continue;
                }

                if (ch == 32)
                {
                    drawX += 4 * scale;
                    continue;
                }

                int index = (int)ch;
                if ('a' <= index && index <= 'z')
                {
                    index = index - 'a' + 'A';
                }

                Patch patch = chars[index];
                if (patch == null)
                {
                    continue;
                }

                DrawPatch(patch, drawX, drawY, scale);

                drawX += scale * patch.Width;
            }
        }

        public void DrawChar(char ch, int x, int y, int scale)
        {
            int drawX = x;
            int drawY = y - 7 * scale;

            if (ch >= chars.Length)
            {
                return;
            }

            if (ch == 32)
            {
                return;
            }

            int index = (int)ch;
            if ('a' <= index && index <= 'z')
            {
                index = index - 'a' + 'A';
            }

            Patch patch = chars[index];
            if (patch == null)
            {
                return;
            }

            DrawPatch(patch, drawX, drawY, scale);
        }

        public void DrawText(string text, int x, int y, int scale)
        {
            int drawX = x;
            int drawY = y - 7 * scale;
            foreach (char ch in text)
            {
                if (ch >= chars.Length)
                {
                    continue;
                }

                if (ch == 32)
                {
                    drawX += 4 * scale;
                    continue;
                }

                int index = (int)ch;
                if ('a' <= index && index <= 'z')
                {
                    index = index - 'a' + 'A';
                }

                Patch patch = chars[index];
                if (patch == null)
                {
                    continue;
                }

                DrawPatch(patch, drawX, drawY, scale);

                drawX += scale * patch.Width;
            }
        }

        public int MeasureChar(char ch, int scale)
        {
            if (ch >= chars.Length)
            {
                return 0;
            }

            if (ch == 32)
            {
                return 4 * scale;
            }

            int index = (int)ch;
            if ('a' <= index && index <= 'z')
            {
                index = index - 'a' + 'A';
            }

            Patch patch = chars[index];
            if (patch == null)
            {
                return 0;
            }

            return scale * patch.Width;
        }

        public int MeasureText(IReadOnlyList<char> text, int scale)
        {
            int width = 0;

            foreach (char ch in text)
            {
                if (ch >= chars.Length)
                {
                    continue;
                }

                if (ch == 32)
                {
                    width += 4 * scale;
                    continue;
                }

                int index = (int)ch;
                if ('a' <= index && index <= 'z')
                {
                    index = index - 'a' + 'A';
                }

                Patch patch = chars[index];
                if (patch == null)
                {
                    continue;
                }

                width += scale * patch.Width;
            }

            return width;
        }

        public int MeasureText(string text, int scale)
        {
            int width = 0;

            foreach (char ch in text)
            {
                if (ch >= chars.Length)
                {
                    continue;
                }

                if (ch == 32)
                {
                    width += 4 * scale;
                    continue;
                }

                int index = (int)ch;
                if ('a' <= index && index <= 'z')
                {
                    index = index - 'a' + 'A';
                }

                Patch patch = chars[index];
                if (patch == null)
                {
                    continue;
                }

                width += scale * patch.Width;
            }

            return width;
        }

        public void FillRect(int x, int y, int w, int h, int color)
        {
            int x1 = x;
            int x2 = x + w;
            for (int drawX = x1; drawX < x2; drawX++)
            {
                int pos = height * drawX + y;
                for (int i = 0; i < h; i++)
                {
                    data[pos] = (byte)color;
                    pos++;
                }
            }
        }



        [Flags]
        private enum OutCode
        {
            Inside = 0,
            Left = 1,
            Right = 2,
            Bottom = 4,
            Top = 8
        }

        private OutCode ComputeOutCode(float x, float y)
        {
            OutCode code = OutCode.Inside;

            if (x < 0)
            {
                code |= OutCode.Left;
            }
            else if (x > width)
            {
                code |= OutCode.Right;
            }

            if (y < 0)
            {
                code |= OutCode.Bottom;
            }
            else if (y > height)
            {
                code |= OutCode.Top;
            }

            return code;
        }

        public void DrawLine(float x1, float y1, float x2, float y2, int color)
        {
            OutCode outCode1 = ComputeOutCode(x1, y1);
            OutCode outCode2 = ComputeOutCode(x2, y2);

            bool accept = false;

            while (true)
            {
                if ((outCode1 | outCode2) == 0)
                {
                    accept = true;
                    break;
                }
                else if ((outCode1 & outCode2) != 0)
                {
                    break;
                }
                else
                {
                    float x = 0.0F;
                    float y = 0.0F;

                    OutCode outcodeOut = outCode2 > outCode1 ? outCode2 : outCode1;

                    if ((outcodeOut & OutCode.Top) != 0)
                    {
                        x = x1 + (x2 - x1) * (height - y1) / (y2 - y1);
                        y = height;
                    }
                    else if ((outcodeOut & OutCode.Bottom) != 0)
                    {
                        x = x1 + (x2 - x1) * (0 - y1) / (y2 - y1);
                        y = 0;
                    }
                    else if ((outcodeOut & OutCode.Right) != 0)
                    {
                        y = y1 + (y2 - y1) * (width - x1) / (x2 - x1);
                        x = width;
                    }
                    else if ((outcodeOut & OutCode.Left) != 0)
                    {
                        y = y1 + (y2 - y1) * (0 - x1) / (x2 - x1);
                        x = 0;
                    }

                    if (outcodeOut == outCode1)
                    {
                        x1 = x;
                        y1 = y;
                        outCode1 = ComputeOutCode(x1, y1);
                    }
                    else
                    {
                        x2 = x;
                        y2 = y;
                        outCode2 = ComputeOutCode(x2, y2);
                    }
                }
            }

            if (accept)
            {
                int bx1 = Math.Clamp((int)x1, 0, width - 1);
                int by1 = Math.Clamp((int)y1, 0, height - 1);
                int bx2 = Math.Clamp((int)x2, 0, width - 1);
                int by2 = Math.Clamp((int)y2, 0, height - 1);
                Bresenham(bx1, by1, bx2, by2, color);
            }
        }

        private void Bresenham(int x1, int y1, int x2, int y2, int color)
        {
            int dx = x2 - x1;
            int ax = 2 * (dx < 0 ? -dx : dx);
            int sx = dx < 0 ? -1 : 1;

            int dy = y2 - y1;
            int ay = 2 * (dy < 0 ? -dy : dy);
            int sy = dy < 0 ? -1 : 1;

            int x = x1;
            int y = y1;

            if (ax > ay)
            {
                int d = ay - ax / 2;

                while (true)
                {
                    data[height * x + y] = (byte)color;

                    if (x == x2)
                    {
                        return;
                    }

                    if (d >= 0)
                    {
                        y += sy;
                        d -= ax;
                    }

                    x += sx;
                    d += ay;
                }
            }
            else
            {
                int d = ax - ay / 2;
                while (true)
                {
                    data[height * x + y] = (byte)color;

                    if (y == y2)
                    {
                        return;
                    }

                    if (d >= 0)
                    {
                        x += sx;
                        d -= ay;
                    }

                    y += sy;
                    d += ax;
                }
            }
        }

        public int Width => width;
        public int Height => height;
        public byte[] Data => data;
    }
}

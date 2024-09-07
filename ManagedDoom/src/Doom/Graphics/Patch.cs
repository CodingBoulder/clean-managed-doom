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

namespace ManagedDoom
{
    public sealed class Patch
    {
        private readonly string _name;
        private readonly int _width;
        private readonly int _height;
        private readonly int _leftOffset;
        private readonly int _topOffset;
        private readonly Column[][] _columns;

        public Patch(
            string name,
            int width,
            int height,
            int leftOffset,
            int topOffset,
            Column[][] columns)
        {
            _name = name;
            _width = width;
            _height = height;
            _leftOffset = leftOffset;
            _topOffset = topOffset;
            _columns = columns;
        }

        public static Patch FromData(string name, byte[] data)
        {
            short width = BitConverter.ToInt16(data, 0);
            short height = BitConverter.ToInt16(data, 2);
            short leftOffset = BitConverter.ToInt16(data, 4);
            short topOffset = BitConverter.ToInt16(data, 6);

            PadData(ref data, width);

            var columns = new Column[width][];
            for (int x = 0; x < width; x++)
            {
                var cs = new List<Column>();
                int p = BitConverter.ToInt32(data, 8 + 4 * x);
                while (true)
                {
                    byte topDelta = data[p];
                    if (topDelta == Column.Last)
                    {
                        break;
                    }
                    byte length = data[p + 1];
                    int offset = p + 3;
                    cs.Add(new Column(topDelta, data, offset, length));
                    p += length + 4;
                }
                columns[x] = cs.ToArray();
            }

            return new Patch(
                name,
                width,
                height,
                leftOffset,
                topOffset,
                columns);
        }

        public static Patch FromWad(Wad wad, string name)
        {
            return FromData(name, wad.ReadLump(name));
        }

        private static void PadData(ref byte[] data, int width)
        {
            int need = 0;
            for (int x = 0; x < width; x++)
            {
                int p = BitConverter.ToInt32(data, 8 + 4 * x);
                while (true)
                {
                    byte topDelta = data[p];
                    if (topDelta == Column.Last)
                    {
                        break;
                    }
                    byte length = data[p + 1];
                    int offset = p + 3;
                    need = Math.Max(offset + 128, need);
                    p += length + 4;
                }
            }

            if (data.Length < need)
            {
                Array.Resize(ref data, need);
            }
        }

        public override string ToString()
        {
            return _name;
        }

        public string Name => _name;
        public int Width => _width;
        public int Height => _height;
        public int LeftOffset => _leftOffset;
        public int TopOffset => _topOffset;
        public Column[][] Columns => _columns;
    }
}

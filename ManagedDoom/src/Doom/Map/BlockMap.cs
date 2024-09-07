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
    public sealed class BlockMap
    {
        public static readonly int IntBlockSize = 128;
        public static readonly Fixed BlockSize = Fixed.FromInt(IntBlockSize);
        public static readonly int BlockMask = BlockSize.Data - 1;
        public static readonly int FracToBlockShift = Fixed.FracBits + 7;
        public static readonly int BlockToFracShift = FracToBlockShift - Fixed.FracBits;

        private readonly Fixed _originX;
        private readonly Fixed _originY;

        private readonly int _width;
        private readonly int _height;

        private readonly short[] _table;

        private readonly LineDef[] _lines;

        private readonly Mobj[] _thingLists;

        private BlockMap(
            Fixed originX,
            Fixed originY,
            int width,
            int height,
            short[] table,
            LineDef[] lines)
        {
            _originX = originX;
            _originY = originY;
            _width = width;
            _height = height;
            _table = table;
            _lines = lines;

            _thingLists = new Mobj[width * height];
        }

        public static BlockMap FromWad(Wad wad, int lump, LineDef[] lines)
        {
            byte[] data = wad.ReadLump(lump);

            short[] table = new short[data.Length / 2];
            for (int i = 0; i < table.Length; i++)
            {
                int offset = 2 * i;
                table[i] = BitConverter.ToInt16(data, offset);
            }

            var originX = Fixed.FromInt(table[0]);
            var originY = Fixed.FromInt(table[1]);
            short width = table[2];
            short height = table[3];

            return new BlockMap(
                originX,
                originY,
                width,
                height,
                table,
                lines);
        }

        public int GetBlockX(Fixed x)
        {
            return (x - _originX).Data >> FracToBlockShift;
        }

        public int GetBlockY(Fixed y)
        {
            return (y - _originY).Data >> FracToBlockShift;
        }

        public int GetIndex(int blockX, int blockY)
        {
            if (0 <= blockX && blockX < _width && 0 <= blockY && blockY < _height)
            {
                return _width * blockY + blockX;
            }
            else
            {
                return -1;
            }
        }

        public int GetIndex(Fixed x, Fixed y)
        {
            int blockX = GetBlockX(x);
            int blockY = GetBlockY(y);
            return GetIndex(blockX, blockY);
        }

        public bool IterateLines(int blockX, int blockY, Func<LineDef, bool> func, int validCount)
        {
            int index = GetIndex(blockX, blockY);

            if (index == -1)
            {
                return true;
            }

            for (short offset = _table[4 + index]; _table[offset] != -1; offset++)
            {
                LineDef line = _lines[_table[offset]];

                if (line.ValidCount == validCount)
                {
                    continue;
                }

                line.ValidCount = validCount;

                if (!func(line))
                {
                    return false;
                }
            }

            return true;
        }

        public bool IterateThings(int blockX, int blockY, Func<Mobj, bool> func)
        {
            int index = GetIndex(blockX, blockY);

            if (index == -1)
            {
                return true;
            }

            for (Mobj? mobj = _thingLists[index]; mobj != null; mobj = mobj.BlockNext)
            {
                if (!func(mobj))
                {
                    return false;
                }
            }

            return true;
        }

        public Fixed OriginX => _originX;
        public Fixed OriginY => _originY;
        public int Width => _width;
        public int Height => _height;
        public Mobj?[] ThingLists => _thingLists;
    }
}

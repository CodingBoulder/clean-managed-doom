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
    public sealed class Subsector
    {
        private static readonly int _dataSize = 4;

        private readonly Sector _sector;
        private readonly int _segCount;
        private readonly int _firstSeg;

        public Subsector(Sector sector, int segCount, int firstSeg)
        {
            _sector = sector;
            _segCount = segCount;
            _firstSeg = firstSeg;
        }

        public static Subsector FromData(byte[] data, int offset, Seg[] segs)
        {
            short segCount = BitConverter.ToInt16(data, offset);
            short firstSegNumber = BitConverter.ToInt16(data, offset + 2);

            return new Subsector(
                segs[firstSegNumber].SideDef.Sector,
                segCount,
                firstSegNumber);
        }

        public static Subsector[] FromWad(Wad wad, int lump, Seg[] segs)
        {
            int length = wad.GetLumpSize(lump);
            if (length % Subsector._dataSize != 0)
            {
                throw new Exception();
            }

            byte[] data = wad.ReadLump(lump);
            int count = length / Subsector._dataSize;
            var subsectors = new Subsector[count];

            for (int i = 0; i < count; i++)
            {
                int offset = Subsector._dataSize * i;
                subsectors[i] = Subsector.FromData(data, offset, segs);
            }

            return subsectors;
        }

        public Sector Sector => _sector;
        public int SegCount => _segCount;
        public int FirstSeg => _firstSeg;
    }
}

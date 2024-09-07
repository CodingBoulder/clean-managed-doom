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
    public sealed class Reject
    {
        private readonly byte[] _data;
        private readonly int _sectorCount;

        private Reject(byte[] data, int sectorCount)
        {
            // If the reject table is too small, expand it to avoid crash.
            // https://doomwiki.org/wiki/Reject#Reject_Overflow
            int expectedLength = (sectorCount * sectorCount + 7) / 8;
            if (data.Length < expectedLength)
            {
                Array.Resize(ref data, expectedLength);
            }

            _data = data;
            _sectorCount = sectorCount;
        }

        public static Reject FromWad(Wad wad, int lump, Sector[] sectors)
        {
            return new Reject(wad.ReadLump(lump), sectors.Length);
        }

        public bool Check(Sector sector1, Sector sector2)
        {
            int s1 = sector1.Number;
            int s2 = sector2.Number;

            int p = s1 * _sectorCount + s2;
            int byteIndex = p >> 3;
            int bitIndex = 1 << (p & 7);

            return (_data[byteIndex] & bitIndex) != 0;
        }
    }
}

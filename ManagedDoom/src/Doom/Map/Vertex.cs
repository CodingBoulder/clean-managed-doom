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
    public sealed class Vertex
    {
        private static readonly int _dataSize = 4;

        private readonly Fixed _x;
        private readonly Fixed _y;

        public Vertex(Fixed x, Fixed y)
        {
            _x = x;
            _y = y;
        }

        public static Vertex FromData(byte[] data, int offset)
        {
            short x = BitConverter.ToInt16(data, offset);
            short y = BitConverter.ToInt16(data, offset + 2);

            return new Vertex(Fixed.FromInt(x), Fixed.FromInt(y));
        }

        public static Vertex[] FromWad(Wad wad, int lump)
        {
            int length = wad.GetLumpSize(lump);
            if (length % _dataSize != 0)
            {
                throw new Exception();
            }

            byte[] data = wad.ReadLump(lump);
            int count = length / _dataSize;
            var vertices = new Vertex[count]; ;

            for (int i = 0; i < count; i++)
            {
                int offset = _dataSize * i;
                vertices[i] = FromData(data, offset);
            }

            return vertices;
        }

        public Fixed X => _x;
        public Fixed Y => _y;
    }
}

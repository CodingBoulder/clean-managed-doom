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
    public sealed class Seg
    {
        private static readonly int _dataSize = 12;

        private readonly Vertex _vertex1;
        private readonly Vertex _vertex2;
        private readonly Fixed _offset;
        private readonly Angle _angle;
        private readonly SideDef? _sideDef;
        private readonly LineDef? _lineDef;
        private readonly Sector? _frontSector;
        private readonly Sector? _backSector;

        public Seg(
            Vertex vertex1,
            Vertex vertex2,
            Fixed offset,
            Angle angle,
            SideDef? sideDef,
            LineDef? lineDef,
            Sector? frontSector,
            Sector? backSector)
        {
            _vertex1 = vertex1;
            _vertex2 = vertex2;
            _offset = offset;
            _angle = angle;
            _sideDef = sideDef;
            _lineDef = lineDef;
            _frontSector = frontSector;
            _backSector = backSector;
        }

        public static Seg FromData(byte[] data, int offset, Vertex[] vertices, LineDef[] lines)
        {
            short vertex1Number = BitConverter.ToInt16(data, offset);
            short vertex2Number = BitConverter.ToInt16(data, offset + 2);
            short angle = BitConverter.ToInt16(data, offset + 4);
            short lineNumber = BitConverter.ToInt16(data, offset + 6);
            short side = BitConverter.ToInt16(data, offset + 8);
            short segOffset = BitConverter.ToInt16(data, offset + 10);

            LineDef lineDef = lines[lineNumber];
            SideDef? frontSide = side == 0 ? lineDef.FrontSide : lineDef.BackSide;
            SideDef? backSide = side == 0 ? lineDef.BackSide : lineDef.FrontSide;

            return new Seg(
                vertices[vertex1Number],
                vertices[vertex2Number],
                Fixed.FromInt(segOffset),
                new Angle((uint)angle << 16),
                frontSide,
                lineDef,
                frontSide.Sector,
                (lineDef.Flags & LineFlags.TwoSided) != 0 ? backSide?.Sector : null);
        }

        public static Seg[] FromWad(Wad wad, int lump, Vertex[] vertices, LineDef[] lines)
        {
            int length = wad.GetLumpSize(lump);
            if (length % Seg._dataSize != 0)
            {
                throw new Exception();
            }

            byte[] data = wad.ReadLump(lump);
            int count = length / Seg._dataSize;
            var segs = new Seg[count]; ;

            for (int i = 0; i < count; i++)
            {
                int offset = Seg._dataSize * i;
                segs[i] = Seg.FromData(data, offset, vertices, lines);
            }

            return segs;
        }

        public Vertex Vertex1 => _vertex1;
        public Vertex Vertex2 => _vertex2;
        public Fixed Offset => _offset;
        public Angle Angle => _angle;
        public SideDef? SideDef => _sideDef;
        public LineDef? LineDef => _lineDef;
        public Sector? FrontSector => _frontSector;
        public Sector? BackSector => _backSector;
    }
}

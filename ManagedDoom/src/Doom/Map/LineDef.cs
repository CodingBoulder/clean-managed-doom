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
    public sealed class LineDef
    {
        private static readonly int _dataSize = 14;

        private readonly Vertex _vertex1;
        private readonly Vertex _vertex2;

        private readonly Fixed _dx;
        private readonly Fixed _dy;

        private LineFlags _flags;
        private LineSpecial _special;
        private short _tag;

        private readonly SideDef? _frontSide;
        private readonly SideDef? _backSide;

        private readonly Fixed[] _boundingBox;

        private readonly SlopeType _slopeType;

        private readonly Sector? _frontSector;
        private readonly Sector? _backSector;

        private int _validCount;

        private Thinker _specialData;

        private Mobj _soundOrigin;

        public LineDef(
            Vertex vertex1,
            Vertex vertex2,
            LineFlags flags,
            LineSpecial special,
            short tag,
            SideDef? frontSide,
            SideDef? backSide)
        {
            _vertex1 = vertex1;
            _vertex2 = vertex2;
            _flags = flags;
            _special = special;
            _tag = tag;
            _frontSide = frontSide;
            _backSide = backSide;

            _dx = vertex2.X - vertex1.X;
            _dy = vertex2.Y - vertex1.Y;

            if (_dx == Fixed.Zero)
            {
                _slopeType = SlopeType.Vertical;
            }
            else if (_dy == Fixed.Zero)
            {
                _slopeType = SlopeType.Horizontal;
            }
            else
            {
                if (_dy / _dx > Fixed.Zero)
                {
                    _slopeType = SlopeType.Positive;
                }
                else
                {
                    _slopeType = SlopeType.Negative;
                }
            }

            _boundingBox = new Fixed[4];
            _boundingBox[Box.Top] = Fixed.Max(vertex1.Y, vertex2.Y);
            _boundingBox[Box.Bottom] = Fixed.Min(vertex1.Y, vertex2.Y);
            _boundingBox[Box.Left] = Fixed.Min(vertex1.X, vertex2.X);
            _boundingBox[Box.Right] = Fixed.Max(vertex1.X, vertex2.X);

            _frontSector = frontSide?.Sector;
            _backSector = backSide?.Sector;
        }

        public static LineDef FromData(byte[] data, int offset, Vertex[] vertices, SideDef[] sides)
        {
            short vertex1Number = BitConverter.ToInt16(data, offset);
            short vertex2Number = BitConverter.ToInt16(data, offset + 2);
            short flags = BitConverter.ToInt16(data, offset + 4);
            short special = BitConverter.ToInt16(data, offset + 6);
            short tag = BitConverter.ToInt16(data, offset + 8);
            short side0Number = BitConverter.ToInt16(data, offset + 10);
            short side1Number = BitConverter.ToInt16(data, offset + 12);

            return new LineDef(
                vertices[vertex1Number],
                vertices[vertex2Number],
                (LineFlags)flags,
                (LineSpecial)special,
                tag,
                sides[side0Number],
                side1Number != -1 ? sides[side1Number] : null);
        }

        public static LineDef[] FromWad(Wad wad, int lump, Vertex[] vertices, SideDef[] sides)
        {
            int length = wad.GetLumpSize(lump);
            if (length % _dataSize != 0)
            {
                throw new Exception();
            }

            byte[] data = wad.ReadLump(lump);
            int count = length / _dataSize;
            var lines = new LineDef[count]; ;

            for (int i = 0; i < count; i++)
            {
                int offset = 14 * i;
                lines[i] = FromData(data, offset, vertices, sides);
            }

            return lines;
        }

        public Vertex Vertex1 => _vertex1;
        public Vertex Vertex2 => _vertex2;

        public Fixed Dx => _dx;
        public Fixed Dy => _dy;

        public LineFlags Flags
        {
            get => _flags;
            set => _flags = value;
        }

        public LineSpecial Special
        {
            get => _special;
            set => _special = value;
        }

        public short Tag
        {
            get => _tag;
            set => _tag = value;
        }

        public SideDef? FrontSide => _frontSide;
        public SideDef? BackSide => _backSide;

        public Fixed[] BoundingBox => _boundingBox;

        public SlopeType SlopeType => _slopeType;

        public Sector? FrontSector => _frontSector;
        public Sector? BackSector => _backSector;

        public int ValidCount
        {
            get => _validCount;
            set => _validCount = value;
        }

        public Thinker SpecialData
        {
            get => _specialData;
            set => _specialData = value;
        }

        public Mobj SoundOrigin
        {
            get => _soundOrigin;
            set => _soundOrigin = value;
        }
    }
}

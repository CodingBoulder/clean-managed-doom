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
    public sealed class Column
    {
        public const int Last = 0xFF;

        private readonly int _topDelta;
        private readonly byte[] _data;
        private readonly int _offset;
        private readonly int _length;

        public Column(int topDelta, byte[] data, int offset, int length)
        {
            _topDelta = topDelta;
            _data = data;
            _offset = offset;
            _length = length;
        }

        public int TopDelta => _topDelta;
        public byte[] Data => _data;
        public int Offset => _offset;
        public int Length => _length;
    }
}

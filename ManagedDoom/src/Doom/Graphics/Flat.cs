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
    public sealed class Flat
    {
        private readonly string _name;
        private readonly byte[] _data;

        public Flat(string name, byte[] data)
        {
            _name = name;
            _data = data;
        }

        public override string ToString()
        {
            return _name;
        }

        public string Name => _name;
        public byte[] Data => _data;
    }
}

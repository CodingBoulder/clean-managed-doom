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
    public sealed class DoomString
    {
        private static readonly Dictionary<string, DoomString> _valueTable = [];
        private static readonly Dictionary<string, DoomString> _nameTable = [];

        private string _replaced;

        public DoomString(string original)
        {
            _replaced = original;

            if (!_valueTable.ContainsKey(original))
            {
                _valueTable.Add(original, this);
            }
        }

        public DoomString(string name, string original) : this(original)
        {
            _nameTable.Add(name, this);
        }

        public override string ToString()
        {
            return _replaced;
        }

        public char this[int index]
        {
            get
            {
                return _replaced[index];
            }
        }

        public static implicit operator string(DoomString ds)
        {
            return ds._replaced;
        }

        public static void ReplaceByValue(string original, string replaced)
        {
            if (_valueTable.TryGetValue(original, out DoomString? ds))
            {
                ds._replaced = replaced;
            }
        }

        public static void ReplaceByName(string name, string value)
        {
            if (_nameTable.TryGetValue(name, out DoomString? ds))
            {
                ds._replaced = value;
            }
        }
    }
}

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
using System.Collections;
using System.Collections.Generic;

namespace ManagedDoom
{
    public sealed class DummyFlatLookup : IFlatLookup
    {
        private readonly DummyData _dummyData = new();
        private readonly Flat[] _flats;

        private readonly Dictionary<string, Flat> _nameToFlat;
        private readonly Dictionary<string, int> _nameToNumber;

        private readonly int _skyFlatNumber;
        private readonly Flat _skyFlat;

        public DummyFlatLookup(Wad wad)
        {
            int firstFlat = wad.GetLumpNumber("F_START") + 1;
            int lastFlat = wad.GetLumpNumber("F_END") - 1;
            int count = lastFlat - firstFlat + 1;

            _flats = new Flat[count];

            _nameToFlat = [];
            _nameToNumber = [];

            for (int lump = firstFlat; lump <= lastFlat; lump++)
            {
                if (wad.GetLumpSize(lump) != 4096)
                {
                    continue;
                }

                int number = lump - firstFlat;
                string name = wad.LumpInfos[lump].Name;
                Flat flat = name != "F_SKY1" ? _dummyData.GetFlat() : _dummyData.GetSkyFlat();

                _flats[number] = flat;
                _nameToFlat[name] = flat;
                _nameToNumber[name] = number;
            }

            _skyFlatNumber = _nameToNumber["F_SKY1"];
            _skyFlat = _nameToFlat["F_SKY1"];
        }

        public int GetNumber(string name)
        {
            if (_nameToNumber.TryGetValue(name, out int number))
            {
                return number;
            }
            else
            {
                return -1;
            }
        }

        public IEnumerator<Flat> GetEnumerator()
        {
            return ((IEnumerable<Flat>)_flats).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _flats.GetEnumerator();
        }

        public int Count => _flats.Length;
        public Flat this[int num] => _flats[num];
        public Flat this[string name] => _nameToFlat[name];
        public int SkyFlatNumber => _skyFlatNumber;
        public Flat SkyFlat => _skyFlat;
    }
}

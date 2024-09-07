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
using System.Runtime.ExceptionServices;

namespace ManagedDoom
{
    public sealed class FlatLookup : IFlatLookup
    {
        private Flat[] _flats;

        private Dictionary<string, Flat> _nameToFlat;
        private Dictionary<string, int> _nameToNumber;

        private int _skyFlatNumber;
        private Flat _skyFlat;

        public FlatLookup(Wad wad)
        {
            int fStartCount = CountLump(wad, "F_START");
            int fEndCount = CountLump(wad, "F_END");
            int ffStartCount = CountLump(wad, "FF_START");
            int ffEndCount = CountLump(wad, "FF_END");

            // Usual case.
            bool standard =
                fStartCount == 1 &&
                fEndCount == 1 &&
                ffStartCount == 0 &&
                ffEndCount == 0;

            // A trick to add custom flats is used.
            // https://www.doomworld.com/tutorials/fx2.php
            bool customFlatTrick =
                fStartCount == 1 &&
                fEndCount >= 2;

            // Need deutex to add flats.
            bool deutexMerge =
                fStartCount + ffStartCount >= 2 &&
                fEndCount + ffEndCount >= 2;

            if (standard || customFlatTrick)
            {
                InitStandard(wad);
            }
            else if (deutexMerge)
            {
                InitDeuTexMerge(wad);
            }
            else
            {
                throw new Exception("Failed to read flats.");
            }
        }

        private void InitStandard(Wad wad)
        {
            try
            {
                Console.Write("Load flats: ");

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
                    var flat = new Flat(name, wad.ReadLump(lump));

                    _flats[number] = flat;
                    _nameToFlat[name] = flat;
                    _nameToNumber[name] = number;
                }

                _skyFlatNumber = _nameToNumber["F_SKY1"];
                _skyFlat = _nameToFlat["F_SKY1"];

                Console.WriteLine("OK (" + _nameToFlat.Count + " flats)");
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed");
                ExceptionDispatchInfo.Throw(e);
            }
        }

        private void InitDeuTexMerge(Wad wad)
        {
            try
            {
                Console.Write("Load flats: ");

                var allFlats = new List<int>();
                bool flatZone = false;
                for (int lump = 0; lump < wad.LumpInfos.Count; lump++)
                {
                    string name = wad.LumpInfos[lump].Name;
                    if (flatZone)
                    {
                        if (name == "F_END" || name == "FF_END")
                        {
                            flatZone = false;
                        }
                        else
                        {
                            allFlats.Add(lump);
                        }
                    }
                    else
                    {
                        if (name == "F_START" || name == "FF_START")
                        {
                            flatZone = true;
                        }
                    }
                }
                allFlats.Reverse();

                var dupCheck = new HashSet<string>();
                var distinctFlats = new List<int>();
                foreach (int lump in allFlats)
                {
                    if (!dupCheck.Contains(wad.LumpInfos[lump].Name))
                    {
                        distinctFlats.Add(lump);
                        dupCheck.Add(wad.LumpInfos[lump].Name);
                    }
                }
                distinctFlats.Reverse();

                _flats = new Flat[distinctFlats.Count];

                _nameToFlat = [];
                _nameToNumber = [];

                for (int number = 0; number < _flats.Length; number++)
                {
                    int lump = distinctFlats[number];

                    if (wad.GetLumpSize(lump) != 4096)
                    {
                        continue;
                    }

                    string name = wad.LumpInfos[lump].Name;
                    var flat = new Flat(name, wad.ReadLump(lump));

                    _flats[number] = flat;
                    _nameToFlat[name] = flat;
                    _nameToNumber[name] = number;
                }

                _skyFlatNumber = _nameToNumber["F_SKY1"];
                _skyFlat = _nameToFlat["F_SKY1"];

                Console.WriteLine("OK (" + _nameToFlat.Count + " flats)");
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed");
                ExceptionDispatchInfo.Throw(e);
            }
        }

        public int GetNumber(string name)
            => _nameToNumber.TryGetValue(name, out int number)
                ? number
                : -1;

        public IEnumerator<Flat> GetEnumerator()
        {
            return ((IEnumerable<Flat>)_flats).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _flats.GetEnumerator();
        }

        private static int CountLump(Wad wad, string name)
        {
            int count = 0;
            foreach (LumpInfo lump in wad.LumpInfos)
            {
                if (lump.Name == name)
                {
                    count++;
                }
            }
            return count;
        }

        public int Count => _flats.Length;
        public Flat this[int num] => _flats[num];
        public Flat this[string name] => _nameToFlat[name];
        public int SkyFlatNumber => _skyFlatNumber;
        public Flat SkyFlat => _skyFlat;
    }
}

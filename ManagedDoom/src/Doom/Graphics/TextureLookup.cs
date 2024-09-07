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
    public sealed class TextureLookup : ITextureLookup
    {
        private List<Texture> _textures;
        private Dictionary<string, Texture> _nameToTexture;
        private Dictionary<string, int> _nameToNumber;

        private int[] _switchList;

        public void Initialize(Wad wad)
        {
            try
            {
                Console.Write("Load textures: ");

                InitLookup(wad);
                InitSwitchList();

                Console.WriteLine("OK (" + _textures.Count + " textures)");
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed");
                ExceptionDispatchInfo.Throw(e);
            }
        }

        private void InitLookup(Wad wad)
        {
            _textures = [];
            _nameToTexture = [];
            _nameToNumber = [];

            Patch[] patches = LoadPatches(wad);

            for (int n = 1; n <= 2; n++)
            {
                int lumpNumber = wad.GetLumpNumber("TEXTURE" + n);
                if (lumpNumber == -1)
                {
                    break;
                }

                byte[] data = wad.ReadLump(lumpNumber);
                int count = BitConverter.ToInt32(data, 0);
                for (int i = 0; i < count; i++)
                {
                    int offset = BitConverter.ToInt32(data, 4 + 4 * i);
                    var texture = Texture.FromData(data, offset, patches);
                    _nameToNumber.TryAdd(texture.Name, _textures.Count);
                    _textures.Add(texture);
                    _nameToTexture.TryAdd(texture.Name, texture);
                }
            }
        }

        private void InitSwitchList()
        {
            var list = new List<int>();
            foreach (Tuple<DoomString, DoomString> tuple in DoomInfo.SwitchNames)
            {
                int texNum1 = GetNumber(tuple.Item1);
                int texNum2 = GetNumber(tuple.Item2);
                if (texNum1 != -1 && texNum2 != -1)
                {
                    list.Add(texNum1);
                    list.Add(texNum2);
                }
            }
            _switchList = list.ToArray();
        }

        public int GetNumber(string name)
        {
            if (name[0] == '-')
            {
                return 0;
            }

            if (_nameToNumber.TryGetValue(name, out int number))
            {
                return number;
            }
            else
            {
                return -1;
            }
        }

        private static Patch[] LoadPatches(Wad wad)
        {
            string[] patchNames = LoadPatchNames(wad);
            var patches = new Patch[patchNames.Length];
            for (int i = 0; i < patches.Length; i++)
            {
                string name = patchNames[i];

                // This check is necessary to avoid crash in DOOM1.WAD.
                if (wad.GetLumpNumber(name) == -1)
                {
                    continue;
                }

                byte[] data = wad.ReadLump(name);
                patches[i] = Patch.FromData(name, data);
            }
            return patches;
        }

        private static string[] LoadPatchNames(Wad wad)
        {
            byte[] data = wad.ReadLump("PNAMES");
            int count = BitConverter.ToInt32(data, 0);
            string[] names = new string[count];
            for (int i = 0; i < names.Length; i++)
            {
                names[i] = DoomInterop.ToString(data, 4 + 8 * i, 8);
            }
            return names;
        }

        public IEnumerator<Texture> GetEnumerator()
        {
            return _textures.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _textures.GetEnumerator();
        }

        public int Count => _textures.Count;
        public Texture this[int num] => _textures[num];
        public Texture this[string name] => _nameToTexture[name];
        public int[] SwitchList => _switchList;
    }
}

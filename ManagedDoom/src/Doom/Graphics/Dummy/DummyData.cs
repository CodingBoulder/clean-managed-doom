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



using System.Collections.Generic;

namespace ManagedDoom
{
    public class DummyData
    {
        private Patch _dummyPatch;

        public Patch GetPatch()
        {
            if (_dummyPatch != null)
            {
                return _dummyPatch;
            }
            else
            {
                int width = 64;
                int height = 128;

                byte[] data = new byte[height + 32];
                for (int y = 0; y < data.Length; y++)
                {
                    data[y] = y / 32 % 2 == 0 ? (byte)80 : (byte)96;
                }

                var columns = new Column[width][];
                var c1 = new Column[] { new(0, data, 0, height) };
                var c2 = new Column[] { new(0, data, 32, height) };
                for (int x = 0; x < width; x++)
                {
                    columns[x] = x / 32 % 2 == 0 ? c1 : c2;
                }

                _dummyPatch = new Patch("DUMMY", width, height, 32, 128, columns);

                return _dummyPatch;
            }
        }

        private readonly Dictionary<int, Texture> _dummyTextures = [];

        public Texture GetTexture(int height)
        {
            if (_dummyTextures.TryGetValue(height, out Texture? texture))
            {
                return texture;
            }
            else
            {
                var patch = new TexturePatch[] { new(0, 0, GetPatch()) };

                texture = new Texture("DUMMY", false, 64, height, patch);

                _dummyTextures.Add(height, texture);

                return texture;
            }
        }

        private Flat _dummyFlat;

        public Flat GetFlat()
        {
            if (_dummyFlat != null)
            {
                return _dummyFlat;
            }
            else
            {
                byte[] data = new byte[64 * 64];
                int spot = 0;
                for (int y = 0; y < 64; y++)
                {
                    for (int x = 0; x < 64; x++)
                    {
                        data[spot] = ((x / 32) ^ (y / 32)) == 0 ? (byte)80 : (byte)96;
                        spot++;
                    }
                }

                _dummyFlat = new Flat("DUMMY", data);

                return _dummyFlat;
            }
        }



        private Flat _dummySkyFlat;

        public Flat GetSkyFlat()
        {
            if (_dummySkyFlat != null)
            {
                return _dummySkyFlat;
            }
            else
            {
                _dummySkyFlat = new Flat("DUMMY", GetFlat().Data);

                return _dummySkyFlat;
            }
        }
    }
}

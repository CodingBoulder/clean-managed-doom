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
    public sealed class TexturePatch
    {
        public const int DataSize = 10;

        private readonly int _originX;
        private readonly int _originY;
        private readonly Patch _patch;

        public TexturePatch(
            int originX,
            int originY,
            Patch patch)
        {
            _originX = originX;
            _originY = originY;
            _patch = patch;
        }

        public static TexturePatch FromData(byte[] data, int offset, Patch[] patches)
        {
            short originX = BitConverter.ToInt16(data, offset);
            short originY = BitConverter.ToInt16(data, offset + 2);
            short patchNum = BitConverter.ToInt16(data, offset + 4);

            return new TexturePatch(
                originX,
                originY,
                patches[patchNum]);
        }

        public string Name => _patch.Name;
        public int OriginX => _originX;
        public int OriginY => _originY;
        public int Width => _patch.Width;
        public int Height => _patch.Height;
        public Column[][] Columns => _patch.Columns;
    }
}

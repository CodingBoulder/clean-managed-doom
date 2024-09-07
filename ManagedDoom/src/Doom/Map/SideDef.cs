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
    public sealed class SideDef
    {
        private static readonly int _dataSize = 30;

        private Fixed _textureOffset;
        private Fixed _rowOffset;
        private int _topTexture;
        private int _bottomTexture;
        private int _middleTexture;
        private readonly Sector _sector;

        public SideDef(
            Fixed textureOffset,
            Fixed rowOffset,
            int topTexture,
            int bottomTexture,
            int middleTexture,
            Sector sector)
        {
            _textureOffset = textureOffset;
            _rowOffset = rowOffset;
            _topTexture = topTexture;
            _bottomTexture = bottomTexture;
            _middleTexture = middleTexture;
            _sector = sector;
        }

        public static SideDef FromData(byte[] data, int offset, ITextureLookup textures, Sector[] sectors)
        {
            short textureOffset = BitConverter.ToInt16(data, offset);
            short rowOffset = BitConverter.ToInt16(data, offset + 2);
            string topTextureName = DoomInterop.ToString(data, offset + 4, 8);
            string bottomTextureName = DoomInterop.ToString(data, offset + 12, 8);
            string middleTextureName = DoomInterop.ToString(data, offset + 20, 8);
            short sectorNum = BitConverter.ToInt16(data, offset + 28);

            return new SideDef(
                Fixed.FromInt(textureOffset),
                Fixed.FromInt(rowOffset),
                textures.GetNumber(topTextureName),
                textures.GetNumber(bottomTextureName),
                textures.GetNumber(middleTextureName),
                sectorNum != -1 ? sectors[sectorNum] : null);
        }

        public static SideDef[] FromWad(Wad wad, int lump, ITextureLookup textures, Sector[] sectors)
        {
            int length = wad.GetLumpSize(lump);
            if (length % _dataSize != 0)
            {
                throw new Exception();
            }

            byte[] data = wad.ReadLump(lump);
            int count = length / _dataSize;
            var sides = new SideDef[count]; ;

            for (int i = 0; i < count; i++)
            {
                int offset = _dataSize * i;
                sides[i] = FromData(data, offset, textures, sectors);
            }

            return sides;
        }

        public Fixed TextureOffset
        {
            get => _textureOffset;
            set => _textureOffset = value;
        }

        public Fixed RowOffset
        {
            get => _rowOffset;
            set => _rowOffset = value;
        }

        public int TopTexture
        {
            get => _topTexture;
            set => _topTexture = value;
        }

        public int BottomTexture
        {
            get => _bottomTexture;
            set => _bottomTexture = value;
        }

        public int MiddleTexture
        {
            get => _middleTexture;
            set => _middleTexture = value;
        }

        public Sector Sector => _sector;
    }
}

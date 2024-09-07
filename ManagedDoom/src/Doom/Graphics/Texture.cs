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
    public sealed class Texture
    {
        private readonly string _name;
        private readonly bool _masked;
        private readonly int _width;
        private readonly int _height;
        private readonly TexturePatch[] _patches;
        private readonly Patch _composite;

        public Texture(
            string name,
            bool masked,
            int width,
            int height,
            TexturePatch[] patches)
        {
            _name = name;
            _masked = masked;
            _width = width;
            _height = height;
            _patches = patches;
            _composite = GenerateComposite(name, width, height, patches);
        }

        public static Texture FromData(byte[] data, int offset, Patch[] patchLookup)
        {
            string name = DoomInterop.ToString(data, offset, 8);
            int masked = BitConverter.ToInt32(data, offset + 8);
            short width = BitConverter.ToInt16(data, offset + 12);
            short height = BitConverter.ToInt16(data, offset + 14);
            short patchCount = BitConverter.ToInt16(data, offset + 20);
            var patches = new TexturePatch[patchCount];
            for (int i = 0; i < patchCount; i++)
            {
                int patchOffset = offset + 22 + TexturePatch.DataSize * i;
                patches[i] = TexturePatch.FromData(data, patchOffset, patchLookup);
            }

            return new Texture(
                name,
                masked != 0,
                width,
                height,
                patches);
        }

        public static string GetName(byte[] data, int offset)
        {
            return DoomInterop.ToString(data, offset, 8);
        }

        public static int GetHeight(byte[] data, int offset)
        {
            return BitConverter.ToInt16(data, offset + 14);
        }

        private static Patch GenerateComposite(string name, int width, int height, TexturePatch[] patches)
        {
            int[] patchCount = new int[width];
            var columns = new Column[width][];
            int compositeColumnCount = 0;

            foreach (TexturePatch patch in patches)
            {
                int left = patch.OriginX;
                int right = left + patch.Width;

                int start = Math.Max(left, 0);
                int end = Math.Min(right, width);

                for (int x = start; x < end; x++)
                {
                    patchCount[x]++;
                    if (patchCount[x] == 2)
                    {
                        compositeColumnCount++;
                    }
                    columns[x] = patch.Columns[x - patch.OriginX];
                }
            }

            int padding = Math.Max(128 - height, 0);
            byte[] data = new byte[height * compositeColumnCount + padding];
            int i = 0;
            for (int x = 0; x < width; x++)
            {
                if (patchCount[x] == 0)
                {
                    columns[x] = [];
                }

                if (patchCount[x] >= 2)
                {
                    var column = new Column(0, data, height * i, height);

                    foreach (TexturePatch patch in patches)
                    {
                        int px = x - patch.OriginX;
                        if (px < 0 || px >= patch.Width)
                        {
                            continue;
                        }
                        Column[] patchColumn = patch.Columns[px];
                        DrawColumnInCache(
                            patchColumn,
                            column.Data,
                            column.Offset,
                            patch.OriginY,
                            height);
                    }

                    columns[x] = [column];

                    i++;
                }
            }

            return new Patch(name, width, height, 0, 0, columns);
        }

        private static void DrawColumnInCache(
            Column[] source,
            byte[] destination,
            int destinationOffset,
            int destinationY,
            int destinationHeight)
        {
            foreach (Column column in source)
            {
                int sourceIndex = column.Offset;
                int destinationIndex = destinationOffset + destinationY + column.TopDelta;
                int length = column.Length;

                int topExceedance = -(destinationY + column.TopDelta);
                if (topExceedance > 0)
                {
                    sourceIndex += topExceedance;
                    destinationIndex += topExceedance;
                    length -= topExceedance;
                }

                int bottomExceedance = destinationY + column.TopDelta + column.Length - destinationHeight;
                if (bottomExceedance > 0)
                {
                    length -= bottomExceedance;
                }

                if (length > 0)
                {
                    Array.Copy(
                        column.Data,
                        sourceIndex,
                        destination,
                        destinationIndex,
                        length);
                }
            }
        }

        public override string ToString()
        {
            return _name;
        }

        public string Name => _name;
        public bool Masked => _masked;
        public int Width => _width;
        public int Height => _height;
        public IReadOnlyList<TexturePatch> Patches => _patches;
        public Patch Composite => _composite;
    }
}

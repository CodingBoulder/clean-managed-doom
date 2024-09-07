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
using System.Runtime.ExceptionServices;

namespace ManagedDoom
{
    public sealed class Palette
    {
        public static readonly int DamageStart = 1;
        public static readonly int DamageCount = 8;

        public static readonly int BonusStart = 9;
        public static readonly int BonusCount = 4;

        public static readonly int IronFeet = 13;

        private readonly byte[] _data;

        private readonly uint[][] _palettes;

        public Palette(Wad wad)
        {
            try
            {
                Console.Write("Load palette: ");

                _data = wad.ReadLump("PLAYPAL");

                int count = _data.Length / (3 * 256);
                _palettes = new uint[count][];
                for (int i = 0; i < _palettes.Length; i++)
                {
                    _palettes[i] = new uint[256];
                }

                Console.WriteLine("OK");
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed");
                ExceptionDispatchInfo.Throw(e);
            }
        }

        public void ResetColors(double p)
        {
            for (int i = 0; i < _palettes.Length; i++)
            {
                int paletteOffset = (3 * 256) * i;
                for (int j = 0; j < 256; j++)
                {
                    int colorOffset = paletteOffset + 3 * j;

                    byte r = _data[colorOffset];
                    byte g = _data[colorOffset + 1];
                    byte b = _data[colorOffset + 2];

                    r = (byte)Math.Round(255 * CorrectionCurve(r / 255.0, p));
                    g = (byte)Math.Round(255 * CorrectionCurve(g / 255.0, p));
                    b = (byte)Math.Round(255 * CorrectionCurve(b / 255.0, p));

                    _palettes[i][j] = (uint)((r << 0) | (g << 8) | (b << 16) | (255 << 24));
                }
            }
        }

        private static double CorrectionCurve(double x, double p)
        {
            return Math.Pow(x, p);
        }

        public uint[] this[int paletteNumber]
        {
            get
            {
                return _palettes[paletteNumber];
            }
        }
    }
}

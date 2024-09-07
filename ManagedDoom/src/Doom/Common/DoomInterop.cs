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
    public static class DoomInterop
    {
        public static string ToString(byte[] data, int offset, int maxLength)
        {
            int length = 0;
            for (int i = 0; i < maxLength; i++)
            {
                if (data[offset + i] == 0)
                {
                    break;
                }
                length++;
            }
            char[] chars = new char[length];
            for (int i = 0; i < chars.Length; i++)
            {
                byte c = data[offset + i];
                if ('a' <= c && c <= 'z')
                {
                    c -= 0x20;
                }
                chars[i] = (char)c;
            }
            return new string(chars);
        }
    }
}

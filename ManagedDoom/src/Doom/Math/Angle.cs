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
using System.Runtime.CompilerServices;

namespace ManagedDoom
{
    public struct Angle
    {
        private static readonly double _twoXPI = 2 * Math.PI;
        public static readonly Angle Ang0 = new(0x00000000);
        public static readonly Angle Ang45 = new(0x20000000);
        public static readonly Angle Ang90 = new(0x40000000);
        public static readonly Angle Ang180 = new(0x80000000);
        public static readonly Angle Ang270 = new(0xC0000000);

        private readonly uint _data;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Angle(uint data)
        {
            _data = data;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Angle(int data)
        {
            _data = (uint)data;
        }

        public static Angle FromRadian(double radian)
        {
            double data = Math.Round(0x100000000 * (radian / (_twoXPI)));
            return new Angle((uint)data);
        }

        public static Angle FromDegree(double degree)
        {
            double data = Math.Round(0x100000000 * (degree / 360));
            return new Angle((uint)data);
        }

        public double ToRadian()
        {
            return _twoXPI * ((double)_data / 0x100000000);
        }

        public double ToDegree()
        {
            return 360 * ((double)_data / 0x100000000);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Angle Abs(Angle angle)
        {
            int data = (int)angle._data;
            if (data < 0)
            {
                return new Angle((uint)-data);
            }
            else
            {
                return angle;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Angle operator +(Angle a)
        {
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Angle operator -(Angle a)
        {
            return new Angle((uint)-(int)a._data);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Angle operator +(Angle a, Angle b)
        {
            return new Angle(a._data + b._data);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Angle operator -(Angle a, Angle b)
        {
            return new Angle(a._data - b._data);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Angle operator *(uint a, Angle b)
        {
            return new Angle(a * b._data);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Angle operator *(Angle a, uint b)
        {
            return new Angle(a._data * b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Angle operator /(Angle a, uint b)
        {
            return new Angle(a._data / b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Angle a, Angle b)
        {
            return a._data == b._data;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Angle a, Angle b)
        {
            return a._data != b._data;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(Angle a, Angle b)
        {
            return a._data < b._data;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(Angle a, Angle b)
        {
            return a._data > b._data;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(Angle a, Angle b)
        {
            return a._data <= b._data;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(Angle a, Angle b)
        {
            return a._data >= b._data;
        }

        public override bool Equals(object? obj)
        {
            throw new NotSupportedException();
        }

        public override int GetHashCode()
        {
            return _data.GetHashCode();
        }

        public override string ToString()
        {
            return ToDegree().ToString();
        }

        public uint Data
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _data;
        }
    }
}

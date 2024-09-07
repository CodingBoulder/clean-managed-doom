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
    public struct Fixed
    {
        public const int FracBits = 16;
        public const int FracUnit = 1 << FracBits;

        public static readonly Fixed Zero = new(0);
        public static readonly Fixed One = new(FracUnit);

        public static readonly Fixed MaxValue = new(int.MaxValue);
        public static readonly Fixed MinValue = new(int.MinValue);

        public static readonly Fixed Epsilon = new(1);
        public static readonly Fixed OnePlusEpsilon = new(FracUnit + 1);
        public static readonly Fixed OneMinusEpsilon = new(FracUnit - 1);

        private readonly int _data;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Fixed(int data)
        {
            _data = data;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fixed FromInt(int value)
        {
            return new Fixed(value << FracBits);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fixed FromFloat(float value)
        {
            return new Fixed((int)(FracUnit * value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fixed FromDouble(double value)
        {
            return new Fixed((int)(FracUnit * value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float ToFloat()
        {
            return (float)_data / FracUnit;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double ToDouble()
        {
            return (double)_data / FracUnit;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fixed Abs(Fixed a)
        {
            if (a._data < 0)
            {
                return new Fixed(-a._data);
            }
            else
            {
                return a;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fixed operator +(Fixed a)
        {
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fixed operator -(Fixed a)
        {
            return new Fixed(-a._data);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fixed operator +(Fixed a, Fixed b)
        {
            return new Fixed(a._data + b._data);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fixed operator -(Fixed a, Fixed b)
        {
            return new Fixed(a._data - b._data);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fixed operator *(Fixed a, Fixed b)
        {
            return new Fixed((int)(((long)a._data * (long)b._data) >> FracBits));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fixed operator *(int a, Fixed b)
        {
            return new Fixed(a * b._data);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fixed operator *(Fixed a, int b)
        {
            return new Fixed(a._data * b);
        }

        public static Fixed operator /(Fixed a, Fixed b)
        {
            if ((CIntAbs(a._data) >> 14) >= CIntAbs(b._data))
            {
                return new Fixed((a._data ^ b._data) < 0 ? int.MinValue : int.MaxValue);
            }

            return FixedDiv2(a, b);
        }

        // The Math.Abs method throws an exception if the input value is -2147483648.
        // Due to this behavior, the visibility check can crash in some maps.
        // So I implemented another Abs method, which is identical to C's one.
        private static int CIntAbs(int n)
        {
            return n < 0 ? -n : n;
        }

        private static Fixed FixedDiv2(Fixed a, Fixed b)
        {
            double c = ((double)a._data) / ((double)b._data) * FracUnit;

            if (c >= 2147483648.0 || c < -2147483648.0)
            {
                throw new DivideByZeroException();
            }

            return new Fixed((int)c);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fixed operator /(int a, Fixed b)
        {
            return Fixed.FromInt(a) / b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fixed operator /(Fixed a, int b)
        {
            return new Fixed(a._data / b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fixed operator <<(Fixed a, int b)
        {
            return new Fixed(a._data << b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fixed operator >>(Fixed a, int b)
        {
            return new Fixed(a._data >> b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Fixed a, Fixed b)
        {
            return a._data == b._data;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Fixed a, Fixed b)
        {
            return a._data != b._data;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(Fixed a, Fixed b)
        {
            return a._data < b._data;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(Fixed a, Fixed b)
        {
            return a._data > b._data;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(Fixed a, Fixed b)
        {
            return a._data <= b._data;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(Fixed a, Fixed b)
        {
            return a._data >= b._data;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fixed Min(Fixed a, Fixed b)
        {
            if (a < b)
            {
                return a;
            }
            else
            {
                return b;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fixed Max(Fixed a, Fixed b)
        {
            if (a < b)
            {
                return b;
            }
            else
            {
                return a;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ToIntFloor()
        {
            return _data >> FracBits;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ToIntCeiling()
        {
            return (_data + FracUnit - 1) >> FracBits;
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
            return ((double)_data / FracUnit).ToString();
        }

        public int Data
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _data;
        }
    }
}

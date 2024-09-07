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
    public sealed class MapThing
    {
        private static readonly int _dataSize = 10;

        public static MapThing Empty = new(
            Fixed.Zero,
            Fixed.Zero,
            Angle.Ang0,
            0,
            0);

        private readonly Fixed _x;
        private readonly Fixed _y;
        private readonly Angle _angle;
        private int _type;
        private readonly ThingFlags _flags;

        public MapThing(
            Fixed x,
            Fixed y,
            Angle angle,
            int type,
            ThingFlags flags)
        {
            _x = x;
            _y = y;
            _angle = angle;
            _type = type;
            _flags = flags;
        }

        public static MapThing FromData(byte[] data, int offset)
        {
            short x = BitConverter.ToInt16(data, offset);
            short y = BitConverter.ToInt16(data, offset + 2);
            short angle = BitConverter.ToInt16(data, offset + 4);
            short type = BitConverter.ToInt16(data, offset + 6);
            short flags = BitConverter.ToInt16(data, offset + 8);

            return new MapThing(
                Fixed.FromInt(x),
                Fixed.FromInt(y),
                new Angle(ManagedDoom.Angle.Ang45.Data * (uint)(angle / 45)),
                type,
                (ThingFlags)flags);
        }

        public static MapThing[] FromWad(Wad wad, int lump)
        {
            int length = wad.GetLumpSize(lump);
            if (length % _dataSize != 0)
            {
                throw new Exception();
            }

            byte[] data = wad.ReadLump(lump);
            int count = length / _dataSize;
            var things = new MapThing[count];

            for (int i = 0; i < count; i++)
            {
                int offset = _dataSize * i;
                things[i] = FromData(data, offset);
            }

            return things;
        }

        public Fixed X => _x;
        public Fixed Y => _y;
        public Angle Angle => _angle;

        public int Type
        {
            get => _type;
            set => _type = value;
        }

        public ThingFlags Flags => _flags;
    }
}

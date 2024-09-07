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
    public sealed class Node
    {
        private static readonly int _dataSize = 28;

        private readonly Fixed _x;
        private readonly Fixed _y;
        private readonly Fixed _dx;
        private readonly Fixed _dy;

        private readonly Fixed[][] _boundingBox;

        private readonly int[] _children;

        public Node(
            Fixed x,
            Fixed y,
            Fixed dx,
            Fixed dy,
            Fixed frontBoundingBoxTop,
            Fixed frontBoundingBoxBottom,
            Fixed frontBoundingBoxLeft,
            Fixed frontBoundingBoxRight,
            Fixed backBoundingBoxTop,
            Fixed backBoundingBoxBottom,
            Fixed backBoundingBoxLeft,
            Fixed backBoundingBoxRight,
            int frontChild,
            int backChild)
        {
            _x = x;
            _y = y;
            _dx = dx;
            _dy = dy;

            var frontBoundingBox = new Fixed[4]
            {
                frontBoundingBoxTop,
                frontBoundingBoxBottom,
                frontBoundingBoxLeft,
                frontBoundingBoxRight
            };

            var backBoundingBox = new Fixed[4]
            {
                backBoundingBoxTop,
                backBoundingBoxBottom,
                backBoundingBoxLeft,
                backBoundingBoxRight
            };

            _boundingBox =
            [
                frontBoundingBox,
                backBoundingBox
            ];

            _children =
            [
                frontChild,
                backChild
            ];
        }

        public static Node FromData(byte[] data, int offset)
        {
            short x = BitConverter.ToInt16(data, offset);
            short y = BitConverter.ToInt16(data, offset + 2);
            short dx = BitConverter.ToInt16(data, offset + 4);
            short dy = BitConverter.ToInt16(data, offset + 6);
            short frontBoundingBoxTop = BitConverter.ToInt16(data, offset + 8);
            short frontBoundingBoxBottom = BitConverter.ToInt16(data, offset + 10);
            short frontBoundingBoxLeft = BitConverter.ToInt16(data, offset + 12);
            short frontBoundingBoxRight = BitConverter.ToInt16(data, offset + 14);
            short backBoundingBoxTop = BitConverter.ToInt16(data, offset + 16);
            short backBoundingBoxBottom = BitConverter.ToInt16(data, offset + 18);
            short backBoundingBoxLeft = BitConverter.ToInt16(data, offset + 20);
            short backBoundingBoxRight = BitConverter.ToInt16(data, offset + 22);
            short frontChild = BitConverter.ToInt16(data, offset + 24);
            short backChild = BitConverter.ToInt16(data, offset + 26);

            return new Node(
                Fixed.FromInt(x),
                Fixed.FromInt(y),
                Fixed.FromInt(dx),
                Fixed.FromInt(dy),
                Fixed.FromInt(frontBoundingBoxTop),
                Fixed.FromInt(frontBoundingBoxBottom),
                Fixed.FromInt(frontBoundingBoxLeft),
                Fixed.FromInt(frontBoundingBoxRight),
                Fixed.FromInt(backBoundingBoxTop),
                Fixed.FromInt(backBoundingBoxBottom),
                Fixed.FromInt(backBoundingBoxLeft),
                Fixed.FromInt(backBoundingBoxRight),
                frontChild,
                backChild);
        }

        public static Node[] FromWad(Wad wad, int lump)
        {
            int length = wad.GetLumpSize(lump);
            if (length % Node._dataSize != 0)
            {
                throw new Exception();
            }

            byte[] data = wad.ReadLump(lump);
            int count = length / Node._dataSize;
            var nodes = new Node[count];

            for (int i = 0; i < count; i++)
            {
                int offset = Node._dataSize * i;
                nodes[i] = Node.FromData(data, offset);
            }

            return nodes;
        }

        public static bool IsSubsector(int node)
        {
            return (node & unchecked((int)0xFFFF8000)) != 0;
        }

        public static int GetSubsector(int node)
        {
            return node ^ unchecked((int)0xFFFF8000);
        }

        public Fixed X => _x;
        public Fixed Y => _y;
        public Fixed Dx => _dx;
        public Fixed Dy => _dy;
        public Fixed[][] BoundingBox => _boundingBox;
        public int[] Children => _children;
    }
}

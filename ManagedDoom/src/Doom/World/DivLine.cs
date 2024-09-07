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
    public sealed class DivLine
    {
        private Fixed _x;
        private Fixed _y;
        private Fixed _dx;
        private Fixed _dy;

        public void MakeFrom(LineDef line)
        {
            _x = line.Vertex1.X;
            _y = line.Vertex1.Y;
            _dx = line.Dx;
            _dy = line.Dy;
        }

        public Fixed X
        {
            get => _x;
            set => _x = value;
        }

        public Fixed Y
        {
            get => _y;
            set => _y = value;
        }

        public Fixed Dx
        {
            get => _dx;
            set => _dx = value;
        }

        public Fixed Dy
        {
            get => _dy;
            set => _dy = value;
        }
    }
}

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
    public sealed class PlayerSpriteDef
    {
        private MobjStateDef? _state;
        private int _tics;
        private Fixed _sx;
        private Fixed _sy;

        public void Clear()
        {
            _state = null;
            _tics = 0;
            _sx = Fixed.Zero;
            _sy = Fixed.Zero;
        }

        public MobjStateDef? State
        {
            get => _state;
            set => _state = value;
        }

        public int Tics
        {
            get => _tics;
            set => _tics = value;
        }

        public Fixed Sx
        {
            get => _sx;
            set => _sx = value;
        }

        public Fixed Sy
        {
            get => _sy;
            set => _sy = value;
        }
    }
}

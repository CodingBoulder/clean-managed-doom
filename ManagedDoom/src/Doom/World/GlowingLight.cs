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
    public sealed class GlowingLight : Thinker
    {
        private static readonly int _glowSpeed = 8;

        private Sector _sector;
        private int _minLight;
        private int _maxLight;
        private int _direction;

        public override void Run()
        {
            switch (_direction)
            {
                case -1:
                    // Down.
                    _sector.LightLevel -= _glowSpeed;
                    if (_sector.LightLevel <= _minLight)
                    {
                        _sector.LightLevel += _glowSpeed;
                        _direction = 1;
                    }
                    break;

                case 1:
                    // Up.
                    _sector.LightLevel += _glowSpeed;
                    if (_sector.LightLevel >= _maxLight)
                    {
                        _sector.LightLevel -= _glowSpeed;
                        _direction = -1;
                    }
                    break;
            }
        }

        public Sector Sector
        {
            get => _sector;
            set => _sector = value;
        }

        public int MinLight
        {
            get => _minLight;
            set => _minLight = value;
        }

        public int MaxLight
        {
            get => _maxLight;
            set => _maxLight = value;
        }

        public int Direction
        {
            get => _direction;
            set => _direction = value;
        }
    }
}

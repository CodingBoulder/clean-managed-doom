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
    public sealed class FireFlicker : Thinker
    {
        private readonly World _world;

        private Sector _sector;
        private int _count;
        private int _maxLight;
        private int _minLight;

        public FireFlicker(World world)
        {
            _world = world;
        }

        public override void Run()
        {
            if (--_count > 0)
            {
                return;
            }

            int amount = (_world.Random.Next() & 3) * 16;

            if (_sector.LightLevel - amount < _minLight)
            {
                _sector.LightLevel = _minLight;
            }
            else
            {
                _sector.LightLevel = _maxLight - amount;
            }

            _count = 4;
        }

        public Sector Sector
        {
            get => _sector;
            set => _sector = value;
        }

        public int Count
        {
            get => _count;
            set => _count = value;
        }

        public int MaxLight
        {
            get => _maxLight;
            set => _maxLight = value;
        }

        public int MinLight
        {
            get => _minLight;
            set => _minLight = value;
        }
    }
}

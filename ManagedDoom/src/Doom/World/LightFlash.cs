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
    public sealed class LightFlash : Thinker
    {
        private readonly World _world;

        private Sector _sector;
        private int _count;
        private int _maxLight;
        private int _minLight;
        private int _maxTime;
        private int _minTime;

        public LightFlash(World world)
        {
            _world = world;
        }

        public override void Run()
        {
            if (--_count > 0)
            {
                return;
            }

            if (_sector.LightLevel == _maxLight)
            {
                _sector.LightLevel = _minLight;
                _count = (_world.Random.Next() & _minTime) + 1;
            }
            else
            {
                _sector.LightLevel = _maxLight;
                _count = (_world.Random.Next() & _maxTime) + 1;
            }
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

        public int MaxTime
        {
            get => _maxTime;
            set => _maxTime = value;
        }

        public int MinTime
        {
            get => _minTime;
            set => _minTime = value;
        }
    }
}

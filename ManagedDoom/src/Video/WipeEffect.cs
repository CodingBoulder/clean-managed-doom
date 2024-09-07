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

namespace ManagedDoom.Video
{
    public sealed class WipeEffect
    {
        private readonly short[] _y;
        private readonly int _height;
        private readonly DoomRandom _random;

        public WipeEffect(int width, int height)
        {
            _y = new short[width];
            _height = height;
            _random = new DoomRandom(DateTime.Now.Millisecond);
        }

        public void Start()
        {
            _y[0] = (short)(-(_random.Next() % 16));
            for (int i = 1; i < _y.Length; i++)
            {
                int r = (_random.Next() % 3) - 1;
                _y[i] = (short)(_y[i - 1] + r);
                if (_y[i] > 0)
                {
                    _y[i] = 0;
                }
                else if (_y[i] == -16)
                {
                    _y[i] = -15;
                }
            }
        }

        public UpdateResult Update()
        {
            bool done = true;

            for (int i = 0; i < _y.Length; i++)
            {
                if (_y[i] < 0)
                {
                    _y[i]++;
                    done = false;
                }
                else if (_y[i] < _height)
                {
                    int dy = (_y[i] < 16) ? _y[i] + 1 : 8;
                    if (_y[i] + dy >= _height)
                    {
                        dy = _height - _y[i];
                    }
                    _y[i] += (short)dy;
                    done = false;
                }
            }

            if (done)
            {
                return UpdateResult.Completed;
            }
            else
            {
                return UpdateResult.None;
            }
        }

        public short[] Y => _y;
    }
}

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
using System.Collections.Generic;
using System.IO;

namespace ManagedDoom
{
    public sealed class Animation
    {
        private readonly Intermission _im;
        private readonly int _number;

        private readonly AnimationType _type;
        private readonly int _period;
        private readonly int _frameCount;
        private readonly int _locationX;
        private readonly int _locationY;
        private readonly int _data;
        private readonly string[] _patches;
        private int _patchNumber;
        private int _nextTic;

        public Animation(Intermission intermission, AnimationInfo info, int number)
        {
            _im = intermission;
            _number = number;

            _type = info.Type;
            _period = info.Period;
            _frameCount = info.Count;
            _locationX = info.X;
            _locationY = info.Y;
            _data = info.Data;

            _patches = new string[_frameCount];
            for (int i = 0; i < _frameCount; i++)
            {
                // MONDO HACK!
                if (_im.Info.Episode != 1 || number != 8)
                {
                    _patches[i] = "WIA" + _im.Info.Episode + number.ToString("00") + i.ToString("00");
                }
                else
                {
                    // HACK ALERT!
                    _patches[i] = "WIA104" + i.ToString("00");
                }
            }
        }

        public void Reset(int bgCount)
        {
            _patchNumber = -1;

            // Specify the next time to draw it.
            if (_type == AnimationType.Always)
            {
                _nextTic = bgCount + 1 + (_im.Random.Next() % _period);
            }
            else if (_type == AnimationType.Random)
            {
                _nextTic = bgCount + 1 + (_im.Random.Next() % _data);
            }
            else if (_type == AnimationType.Level)
            {
                _nextTic = bgCount + 1;
            }
        }

        public void Update(int bgCount)
        {
            if (bgCount == _nextTic)
            {
                switch (_type)
                {
                    case AnimationType.Always:
                        if (++_patchNumber >= _frameCount)
                        {
                            _patchNumber = 0;
                        }
                        _nextTic = bgCount + _period;
                        break;

                    case AnimationType.Random:
                        _patchNumber++;
                        if (_patchNumber == _frameCount)
                        {
                            _patchNumber = -1;
                            _nextTic = bgCount + (_im.Random.Next() % _data);
                        }
                        else
                        {
                            _nextTic = bgCount + _period;
                        }
                        break;

                    case AnimationType.Level:
                        // Gawd-awful hack for level anims.
                        if (!(_im.State == IntermissionState.StatCount && _number == 7) && _im.Info.NextLevel == Data)
                        {
                            _patchNumber++;
                            if (_patchNumber == _frameCount)
                            {
                                _patchNumber--;
                            }
                            _nextTic = bgCount + _period;
                        }
                        break;
                }
            }
        }

        public int LocationX => _locationX;
        public int LocationY => _locationY;
        public int Data => _data;
        public IReadOnlyList<string> Patches => _patches;
        public int PatchNumber => _patchNumber;
    }
}

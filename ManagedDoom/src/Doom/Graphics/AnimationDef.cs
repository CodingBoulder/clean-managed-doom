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
    public sealed class AnimationDef
    {
        private readonly bool _isTexture;
        private readonly string _endName;
        private readonly string _startName;
        private readonly int _speed;

        public AnimationDef(bool isTexture, string endName, string startName, int speed)
        {
            _isTexture = isTexture;
            _endName = endName;
            _startName = startName;
            _speed = speed;
        }

        public bool IsTexture => _isTexture;
        public string EndName => _endName;
        public string StartName => _startName;
        public int Speed => _speed;
    }
}

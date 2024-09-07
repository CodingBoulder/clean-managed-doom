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
    public sealed class Button
    {
        private LineDef? _line;
        private ButtonPosition _position;
        private int _texture;
        private int _timer;
        private Mobj? _soundOrigin;

        public void Clear()
        {
            _line = null;
            _position = 0;
            _texture = 0;
            _timer = 0;
            _soundOrigin = null;
        }

        public LineDef? Line
        {
            get => _line;
            set => _line = value;
        }

        public ButtonPosition Position
        {
            get => _position;
            set => _position = value;
        }

        public int Texture
        {
            get => _texture;
            set => _texture = value;
        }

        public int Timer
        {
            get => _timer;
            set => _timer = value;
        }

        public Mobj? SoundOrigin
        {
            get => _soundOrigin;
            set => _soundOrigin = value;
        }
    }
}

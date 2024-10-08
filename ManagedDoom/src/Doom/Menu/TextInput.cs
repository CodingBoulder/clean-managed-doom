﻿//
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
using System.Linq;

namespace ManagedDoom
{
    public sealed class TextInput
    {
        private readonly List<char> _text;
        private readonly Action<IReadOnlyList<char>> _typed;
        private readonly Action<IReadOnlyList<char>> _finished;
        private readonly Action _canceled;

        private TextInputState _state;

        public TextInput(
            IReadOnlyList<char> initialText,
            Action<IReadOnlyList<char>> typed,
            Action<IReadOnlyList<char>> finished,
            Action canceled)
        {
            _text = initialText.ToList();
            _typed = typed;
            _finished = finished;
            _canceled = canceled;

            _state = TextInputState.Typing;
        }

        public bool DoEvent(DoomEvent e)
        {
            char ch = e.Key.GetChar();
            if (ch != 0)
            {
                _text.Add(ch);
                _typed(_text);
                return true;
            }

            if (e.Key == DoomKey.Backspace && e.Type == EventType.KeyDown)
            {
                if (_text.Count > 0)
                {
                    _text.RemoveAt(_text.Count - 1);
                }
                _typed(_text);
                return true;
            }

            if (e.Key == DoomKey.Enter && e.Type == EventType.KeyDown)
            {
                _finished(_text);
                _state = TextInputState.Finished;
                return true;
            }

            if (e.Key == DoomKey.Escape && e.Type == EventType.KeyDown)
            {
                _canceled();
                _state = TextInputState.Canceled;
                return true;
            }

            return true;
        }

        public IReadOnlyList<char> Text => _text;
        public TextInputState State => _state;
    }
}

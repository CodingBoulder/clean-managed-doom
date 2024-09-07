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

namespace ManagedDoom
{
    public class TextBoxMenuItem : MenuItem
    {
        private readonly int _itemX;
        private readonly int _itemY;

        private IReadOnlyList<char> _text;
        private TextInput? _edit;

        public TextBoxMenuItem(int skullX, int skullY, int itemX, int itemY)
            : base(skullX, skullY, null)
        {
            _itemX = itemX;
            _itemY = itemY;
        }

        public TextInput Edit(Action finished)
        {
            _edit = new TextInput(
                _text ?? [],
                cs => { },
                cs => { _text = cs; _edit = null; finished(); },
                () => { _edit = null; });

            return _edit;
        }

        public void SetText(string text)
        {
            if (text != null)
            {
                _text = text.ToCharArray();
            }
        }

        public IReadOnlyList<char> Text
        {
            get
            {
                if (_edit == null)
                {
                    return _text;
                }
                else
                {
                    return _edit.Text;
                }
            }
        }

        public int ItemX => _itemX;
        public int ItemY => _itemY;
        public bool Editing => _edit != null;
    }
}

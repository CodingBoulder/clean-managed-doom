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
using System.Linq;

namespace ManagedDoom
{
    public sealed class KeyBinding
    {
        public static readonly KeyBinding Empty = new();

        private readonly DoomKey[] keys;
        private readonly DoomMouseButton[] mouseButtons;

        private KeyBinding()
        {
            keys = [];
            mouseButtons = [];
        }

        public KeyBinding(IReadOnlyList<DoomKey> keys)
        {
            this.keys = keys.ToArray();
            mouseButtons = [];
        }

        public KeyBinding(IReadOnlyList<DoomKey> keys, IReadOnlyList<DoomMouseButton> mouseButtons)
        {
            this.keys = keys.ToArray();
            this.mouseButtons = mouseButtons.ToArray();
        }

        public override string ToString()
        {
            IEnumerable<string> keyValues = keys.Select(key => DoomKeyEx.ToString(key));
            IEnumerable<string> mouseValues = mouseButtons.Select(button => DoomMouseButtonEx.ToString(button));
            string[] values = keyValues.Concat(mouseValues).ToArray();
            if (values.Length > 0)
            {
                return string.Join(", ", values);
            }
            else
            {
                return "none";
            }
        }

        public static KeyBinding Parse(string value)
        {
            if (value == "none")
            {
                return Empty;
            }

            var keys = new List<DoomKey>();
            var mouseButtons = new List<DoomMouseButton>();

            IEnumerable<string> split = value.Split(',').Select(x => x.Trim());
            foreach (string? s in split)
            {
                DoomKey key = DoomKeyEx.Parse(s);
                if (key != DoomKey.Unknown)
                {
                    keys.Add(key);
                    continue;
                }

                DoomMouseButton mouseButton = DoomMouseButtonEx.Parse(s);
                if (mouseButton != DoomMouseButton.Unknown)
                {
                    mouseButtons.Add(mouseButton);
                }
            }

            return new KeyBinding(keys, mouseButtons);
        }

        public IReadOnlyList<DoomKey> Keys => keys;
        public IReadOnlyList<DoomMouseButton> MouseButtons => mouseButtons;
    }
}

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
using System.IO;

namespace ManagedDoom
{
    public sealed class SaveSlots
    {
        private static readonly int _slotCount = 6;
        private static readonly int _descriptionSize = 24;

        private string[] _slots;

        private void ReadSlots()
        {
            _slots = new string[_slotCount];

            string? directory = ConfigUtilities.GetExeDirectory();
            byte[] buffer = new byte[_descriptionSize];
            for (int i = 0; i < _slots.Length; i++)
            {
                string path = Path.Combine(directory, "doomsav" + i + ".dsg");
                if (File.Exists(path))
                {
                    using var reader = new FileStream(path, FileMode.Open, FileAccess.Read);
                    reader.Read(buffer, 0, buffer.Length);
                    _slots[i] = DoomInterop.ToString(buffer, 0, buffer.Length);
                }
            }
        }

        public string this[int number]
        {
            get
            {
                if (_slots == null)
                {
                    ReadSlots();
                }

                return _slots[number];
            }

            set
            {
                _slots[number] = value;
            }
        }

        public int Count => _slots.Length;
    }
}

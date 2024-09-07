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
using System.Linq;

namespace ManagedDoom
{
    public sealed class SaveMenu : MenuDef
    {
        private readonly string[] _name;
        private readonly int[] _titleX;
        private readonly int[] _titleY;
        private readonly TextBoxMenuItem[] _items;

        private int _index;
        private TextBoxMenuItem _choice;

        private TextInput? _textInput;

        private int _lastSaveSlot;

        public SaveMenu(
            DoomMenu menu,
            string name, int titleX, int titleY,
            int firstChoice,
            params TextBoxMenuItem[] items) : base(menu)
        {
            _name = [name];
            _titleX = [titleX];
            _titleY = [titleY];
            _items = items;

            _index = firstChoice;
            _choice = items[_index];

            _lastSaveSlot = -1;
        }

        public override void Open()
        {
            if (Menu.Doom.State != DoomState.Game ||
                Menu.Doom.Game.State != GameState.Level)
            {
                Menu.NotifySaveFailed();
                return;
            }

            for (int i = 0; i < _items.Length; i++)
            {
                _items[i].SetText(Menu.SaveSlots[i]);
            }
        }

        private void Up()
        {
            _index--;
            if (_index < 0)
            {
                _index = _items.Length - 1;
            }

            _choice = _items[_index];
        }

        private void Down()
        {
            _index++;
            if (_index >= _items.Length)
            {
                _index = 0;
            }

            _choice = _items[_index];
        }

        public override bool DoEvent(DoomEvent e)
        {
            if (e.Type != EventType.KeyDown)
            {
                return true;
            }

            if (_textInput != null)
            {
                bool result = _textInput.DoEvent(e);

                if (_textInput.State == TextInputState.Canceled)
                {
                    _textInput = null;
                }
                else if (_textInput.State == TextInputState.Finished)
                {
                    _textInput = null;
                }

                if (result)
                {
                    return true;
                }
            }

            if (e.Key == DoomKey.Up)
            {
                Up();
                Menu.StartSound(Sfx.PSTOP);
            }

            if (e.Key == DoomKey.Down)
            {
                Down();
                Menu.StartSound(Sfx.PSTOP);
            }

            if (e.Key == DoomKey.Enter)
            {
                _textInput = _choice.Edit(() => DoSave(_index));
                Menu.StartSound(Sfx.PISTOL);
            }

            if (e.Key == DoomKey.Escape)
            {
                Menu.Close();
                Menu.StartSound(Sfx.SWTCHX);
            }

            return true;
        }

        public void DoSave(int slotNumber)
        {
            Menu.SaveSlots[slotNumber] = new string(_items[slotNumber].Text.ToArray());
            if (Menu.Doom.SaveGame(slotNumber, Menu.SaveSlots[slotNumber]))
            {
                Menu.Close();
                _lastSaveSlot = slotNumber;
            }
            else
            {
                Menu.NotifySaveFailed();
            }
            Menu.StartSound(Sfx.PISTOL);
        }

        public IReadOnlyList<string> Name => _name;
        public IReadOnlyList<int> TitleX => _titleX;
        public IReadOnlyList<int> TitleY => _titleY;
        public IReadOnlyList<MenuItem> Items => _items;
        public MenuItem Choice => _choice;
        public int LastSaveSlot => _lastSaveSlot;
    }
}

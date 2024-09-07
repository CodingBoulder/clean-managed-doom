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
    public class SliderMenuItem : MenuItem
    {
        private readonly string _name;
        private readonly int _itemX;
        private readonly int _itemY;

        private readonly int _sliderLength;
        private int _sliderPosition;

        private readonly Func<int> _reset;
        private readonly Action<int> _action;

        public SliderMenuItem(
            string name,
            int skullX, int skullY,
            int itemX, int itemY,
            int sliderLength,
            Func<int> reset,
            Action<int> action)
            : base(skullX, skullY, null)
        {
            _name = name;
            _itemX = itemX;
            _itemY = itemY;

            _sliderLength = sliderLength;
            _sliderPosition = 0;

            _action = action;
            _reset = reset;
        }

        public void Reset()
        {
            if (_reset != null)
            {
                _sliderPosition = _reset();
            }
        }

        public void Up()
        {
            if (_sliderPosition < SliderLength - 1)
            {
                _sliderPosition++;
            }

            _action?.Invoke(_sliderPosition);
        }

        public void Down()
        {
            if (_sliderPosition > 0)
            {
                _sliderPosition--;
            }

            _action?.Invoke(_sliderPosition);
        }

        public string Name => _name;
        public int ItemX => _itemX;
        public int ItemY => _itemY;

        public int SliderX => _itemX;
        public int SliderY => _itemY + 16;
        public int SliderLength => _sliderLength;
        public int SliderPosition => _sliderPosition;
    }
}

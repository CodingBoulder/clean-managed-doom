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
    public sealed class TicCmd
    {
        private sbyte _forwardMove;
        private sbyte _sideMove;
        private short _angleTurn;
        private byte _buttons;

        public void Clear()
        {
            _forwardMove = 0;
            _sideMove = 0;
            _angleTurn = 0;
            _buttons = 0;
        }

        public void CopyFrom(TicCmd cmd)
        {
            _forwardMove = cmd._forwardMove;
            _sideMove = cmd._sideMove;
            _angleTurn = cmd._angleTurn;
            _buttons = cmd._buttons;
        }

        public sbyte ForwardMove
        {
            get => _forwardMove;
            set => _forwardMove = value;
        }

        public sbyte SideMove
        {
            get => _sideMove;
            set => _sideMove = value;
        }

        public short AngleTurn
        {
            get => _angleTurn;
            set => _angleTurn = value;
        }

        public byte Buttons
        {
            get => _buttons;
            set => _buttons = value;
        }
    }
}

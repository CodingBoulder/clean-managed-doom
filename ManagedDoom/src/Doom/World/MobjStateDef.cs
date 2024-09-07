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
    public class MobjStateDef
    {
        private int _number;
        private Sprite _sprite;
        private int _frame;
        private int _tics;
        private Action<World, Player, PlayerSpriteDef>? _playerAction;
        private Action<World, Mobj>? _mobjAction;
        private MobjState _next;
        private int _misc1;
        private int _misc2;

        public MobjStateDef(
            int number,
            Sprite sprite,
            int frame,
            int tics,
            Action<World, Player, PlayerSpriteDef>? playerAction,
            Action<World, Mobj>? mobjAction,
            MobjState next,
            int misc1,
            int misc2)
        {
            _number = number;
            _sprite = sprite;
            _frame = frame;
            _tics = tics;
            _playerAction = playerAction;
            _mobjAction = mobjAction;
            _next = next;
            _misc1 = misc1;
            _misc2 = misc2;
        }

        public int Number
        {
            get => _number;
            set => _number = value;
        }

        public Sprite Sprite
        {
            get => _sprite;
            set => _sprite = value;
        }

        public int Frame
        {
            get => _frame;
            set => _frame = value;
        }

        public int Tics
        {
            get => _tics;
            set => _tics = value;
        }

        public Action<World, Player, PlayerSpriteDef>? PlayerAction
        {
            get => _playerAction;
            set => _playerAction = value;
        }

        public Action<World, Mobj>? MobjAction
        {
            get => _mobjAction;
            set => _mobjAction = value;
        }

        public MobjState Next
        {
            get => _next;
            set => _next = value;
        }

        public int Misc1
        {
            get => _misc1;
            set => _misc1 = value;
        }

        public int Misc2
        {
            get => _misc2;
            set => _misc2 = value;
        }
    }
}

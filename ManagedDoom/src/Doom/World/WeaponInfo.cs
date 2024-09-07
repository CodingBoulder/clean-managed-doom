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
    public sealed class WeaponInfo
    {
        private AmmoType _ammo;
        private MobjState _upState;
        private MobjState _downState;
        private MobjState _readyState;
        private MobjState _attackState;
        private MobjState _flashState;

        public WeaponInfo(
            AmmoType ammo,
            MobjState upState,
            MobjState downState,
            MobjState readyState,
            MobjState attackState,
            MobjState flashState)
        {
            _ammo = ammo;
            _upState = upState;
            _downState = downState;
            _readyState = readyState;
            _attackState = attackState;
            _flashState = flashState;
        }

        public AmmoType Ammo
        {
            get => _ammo;
            set => _ammo = value;
        }

        public MobjState UpState
        {
            get => _upState;
            set => _upState = value;
        }

        public MobjState DownState
        {
            get => _downState;
            set => _downState = value;
        }

        public MobjState ReadyState
        {
            get => _readyState;
            set => _readyState = value;
        }

        public MobjState AttackState
        {
            get => _attackState;
            set => _attackState = value;
        }

        public MobjState FlashState
        {
            get => _flashState;
            set => _flashState = value;
        }
    }
}

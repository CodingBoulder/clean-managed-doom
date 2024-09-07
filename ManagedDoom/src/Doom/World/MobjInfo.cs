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
    public sealed class MobjInfo
    {
        private int _doomEdNum;
        private MobjState _spawnState;
        private int _spawnHealth;
        private MobjState _seeState;
        private Sfx _seeSound;
        private int _reactionTime;
        private Sfx _attackSound;
        private MobjState _painState;
        private int _painChance;
        private Sfx _painSound;
        private MobjState _meleeState;
        private MobjState _missileState;
        private MobjState _deathState;
        private MobjState _xdeathState;
        private Sfx _deathSound;
        private int _speed;
        private Fixed _radius;
        private Fixed _height;
        private int _mass;
        private int _damage;
        private Sfx _activeSound;
        private MobjFlags _flags;
        private MobjState _raiseState;

        public MobjInfo(
            int doomEdNum,
            MobjState spawnState,
            int spawnHealth,
            MobjState seeState,
            Sfx seeSound,
            int reactionTime,
            Sfx attackSound,
            MobjState painState,
            int painChance,
            Sfx painSound,
            MobjState meleeState,
            MobjState missileState,
            MobjState deathState,
            MobjState xdeathState,
            Sfx deathSound,
            int speed,
            Fixed radius,
            Fixed height,
            int mass,
            int damage,
            Sfx activeSound,
            MobjFlags flags,
            MobjState raiseState)
        {
            _doomEdNum = doomEdNum;
            _spawnState = spawnState;
            _spawnHealth = spawnHealth;
            _seeState = seeState;
            _seeSound = seeSound;
            _reactionTime = reactionTime;
            _attackSound = attackSound;
            _painState = painState;
            _painChance = painChance;
            _painSound = painSound;
            _meleeState = meleeState;
            _missileState = missileState;
            _deathState = deathState;
            _xdeathState = xdeathState;
            _deathSound = deathSound;
            _speed = speed;
            _radius = radius;
            _height = height;
            _mass = mass;
            _damage = damage;
            _activeSound = activeSound;
            _flags = flags;
            _raiseState = raiseState;
        }

        public int DoomEdNum
        {
            get => _doomEdNum;
            set => _doomEdNum = value;
        }

        public MobjState SpawnState
        {
            get => _spawnState;
            set => _spawnState = value;
        }

        public int SpawnHealth
        {
            get => _spawnHealth;
            set => _spawnHealth = value;
        }

        public MobjState SeeState
        {
            get => _seeState;
            set => _seeState = value;
        }

        public Sfx SeeSound
        {
            get => _seeSound;
            set => _seeSound = value;
        }

        public int ReactionTime
        {
            get => _reactionTime;
            set => _reactionTime = value;
        }

        public Sfx AttackSound
        {
            get => _attackSound;
            set => _attackSound = value;
        }

        public MobjState PainState
        {
            get => _painState;
            set => _painState = value;
        }

        public int PainChance
        {
            get => _painChance;
            set => _painChance = value;
        }

        public Sfx PainSound
        {
            get => _painSound;
            set => _painSound = value;
        }

        public MobjState MeleeState
        {
            get => _meleeState;
            set => _meleeState = value;
        }

        public MobjState MissileState
        {
            get => _missileState;
            set => _missileState = value;
        }

        public MobjState DeathState
        {
            get => _deathState;
            set => _deathState = value;
        }

        public MobjState XdeathState
        {
            get => _xdeathState;
            set => _xdeathState = value;
        }

        public Sfx DeathSound
        {
            get => _deathSound;
            set => _deathSound = value;
        }

        public int Speed
        {
            get => _speed;
            set => _speed = value;
        }

        public Fixed Radius
        {
            get => _radius;
            set => _radius = value;
        }

        public Fixed Height
        {
            get => _height;
            set => _height = value;
        }

        public int Mass
        {
            get => _mass;
            set => _mass = value;
        }

        public int Damage
        {
            get => _damage;
            set => _damage = value;
        }

        public Sfx ActiveSound
        {
            get => _activeSound;
            set => _activeSound = value;
        }

        public MobjFlags Flags
        {
            get => _flags;
            set => _flags = value;
        }

        public MobjState Raisestate
        {
            get => _raiseState;
            set => _raiseState = value;
        }
    }
}

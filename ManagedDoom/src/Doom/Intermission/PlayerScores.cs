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
    public class PlayerScores
    {
        // Whether the player is in game.
        private bool _inGame;

        // Player stats, kills, collected items etc.
        private int _killCount;
        private int _itemCount;
        private int _secretCount;
        private int _time;
        private readonly int[] _frags;

        public PlayerScores()
        {
            _frags = new int[Player.MaxPlayerCount];
        }

        public bool InGame
        {
            get => _inGame;
            set => _inGame = value;
        }

        public int KillCount
        {
            get => _killCount;
            set => _killCount = value;
        }

        public int ItemCount
        {
            get => _itemCount;
            set => _itemCount = value;
        }

        public int SecretCount
        {
            get => _secretCount;
            set => _secretCount = value;
        }

        public int Time
        {
            get => _time;
            set => _time = value;
        }

        public int[] Frags
        {
            get => _frags;
        }
    }
}

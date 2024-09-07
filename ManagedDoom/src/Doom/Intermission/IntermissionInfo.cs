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
    public class IntermissionInfo
    {
        // Episode number (0-2).
        private int _episode;

        // If true, splash the secret level.
        private bool _didSecret;

        // Previous and next levels, origin 0.
        private int _lastLevel;
        private int _nextLevel;

        private int _maxKillCount;
        private int _maxItemCount;
        private int _maxSecretCount;
        private int _totalFrags;

        // The par time.
        private int _parTime;

        private readonly PlayerScores[] _players;

        public IntermissionInfo()
        {
            _players = new PlayerScores[Player.MaxPlayerCount];
            for (int i = 0; i < Player.MaxPlayerCount; i++)
            {
                _players[i] = new PlayerScores();
            }
        }

        public int Episode
        {
            get => _episode;
            set => _episode = value;
        }

        public bool DidSecret
        {
            get => _didSecret;
            set => _didSecret = value;
        }

        public int LastLevel
        {
            get => _lastLevel;
            set => _lastLevel = value;
        }

        public int NextLevel
        {
            get => _nextLevel;
            set => _nextLevel = value;
        }

        public int MaxKillCount
        {
            get => Math.Max(_maxKillCount, 1);
            set => _maxKillCount = value;
        }

        public int MaxItemCount
        {
            get => Math.Max(_maxItemCount, 1);
            set => _maxItemCount = value;
        }

        public int MaxSecretCount
        {
            get => Math.Max(_maxSecretCount, 1);
            set => _maxSecretCount = value;
        }

        public int TotalFrags
        {
            get => Math.Max(_totalFrags, 1);
            set => _totalFrags = value;
        }

        public int ParTime
        {
            get => _parTime;
            set => _parTime = value;
        }

        public PlayerScores[] Players
        {
            get => _players;
        }
    }
}

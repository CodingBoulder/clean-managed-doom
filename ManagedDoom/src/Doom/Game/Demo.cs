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
    public sealed class Demo
    {
        private int _p;
        private readonly byte[] _data;

        private readonly GameOptions _options;

        private readonly int _playerCount;

        public Demo(byte[] data)
        {
            _p = 0;

            if (data[_p++] != 109)
            {
                throw new Exception("Demo is from a different game version!");
            }

            _data = data;

            _options = new GameOptions
            {
                Skill = (GameSkill)data[_p++],
                Episode = data[_p++],
                Map = data[_p++],
                Deathmatch = data[_p++],
                RespawnMonsters = data[_p++] != 0,
                FastMonsters = data[_p++] != 0,
                NoMonsters = data[_p++] != 0,
                ConsolePlayer = data[_p++]
            };

            _options.Players[0].InGame = data[_p++] != 0;
            _options.Players[1].InGame = data[_p++] != 0;
            _options.Players[2].InGame = data[_p++] != 0;
            _options.Players[3].InGame = data[_p++] != 0;

            _options.DemoPlayback = true;

            _playerCount = 0;
            for (int i = 0; i < Player.MaxPlayerCount; i++)
            {
                if (_options.Players[i].InGame)
                {
                    _playerCount++;
                }
            }
            if (_playerCount >= 2)
            {
                _options.NetGame = true;
            }
        }

        public Demo(string fileName) : this(File.ReadAllBytes(fileName))
        {
        }

        public bool ReadCmd(TicCmd[] cmds)
        {
            if (_p == _data.Length)
            {
                return false;
            }

            if (_data[_p] == 0x80)
            {
                return false;
            }

            if (_p + 4 * _playerCount > _data.Length)
            {
                return false;
            }

            Player[] players = _options.Players;
            for (int i = 0; i < Player.MaxPlayerCount; i++)
            {
                if (players[i].InGame)
                {
                    TicCmd cmd = cmds[i];
                    cmd.ForwardMove = (sbyte)_data[_p++];
                    cmd.SideMove = (sbyte)_data[_p++];
                    cmd.AngleTurn = (short)(_data[_p++] << 8);
                    cmd.Buttons = _data[_p++];
                }
            }

            return true;
        }

        public GameOptions Options => _options;
    }
}

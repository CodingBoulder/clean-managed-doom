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



using ManagedDoom.Audio;
using ManagedDoom.UserInput;
using ManagedDoom.Video;

namespace ManagedDoom
{
    public sealed class GameOptions
    {
        private GameVersion _gameVersion;
        private GameMode _gameMode;
        private MissionPack _missionPack;

        private readonly Player[] _players;
        private int _consolePlayer;

        private int _episode;
        private int _map;
        private GameSkill _skill;

        private bool _demoPlayback;
        private bool _netGame;

        private int _deathmatch;
        private bool _fastMonsters;
        private bool _respawnMonsters;
        private bool _noMonsters;

        private readonly IntermissionInfo _intermissionInfo;

        private readonly DoomRandom _random;

        private IVideo _video;
        private ISound _sound;
        private IMusic _music;
        private IUserInput _userInput;

        public GameOptions()
        {
            _gameVersion = GameVersion.Version109;
            _gameMode = GameMode.Commercial;
            _missionPack = MissionPack.Doom2;

            _players = new Player[Player.MaxPlayerCount];
            for (int i = 0; i < Player.MaxPlayerCount; i++)
            {
                _players[i] = new Player(i);
            }
            _players[0].InGame = true;
            _consolePlayer = 0;

            _episode = 1;
            _map = 1;
            _skill = GameSkill.Medium;

            _demoPlayback = false;
            _netGame = false;

            _deathmatch = 0;
            _fastMonsters = false;
            _respawnMonsters = false;
            _noMonsters = false;

            _intermissionInfo = new IntermissionInfo();

            _random = new DoomRandom();

            _video = NullVideo.GetInstance();
            _sound = NullSound.GetInstance();
            _music = NullMusic.GetInstance();
            _userInput = NullUserInput.GetInstance();
        }

        public GameOptions(CommandLineArgs args, GameContent content) : this()
        {
            if (args.solonet.Present)
            {
                _netGame = true;
            }

            _gameVersion = content.Wad.GameVersion;
            _gameMode = content.Wad.GameMode;
            _missionPack = content.Wad.MissionPack;
        }

        public GameVersion GameVersion
        {
            get => _gameVersion;
            set => _gameVersion = value;
        }

        public GameMode GameMode
        {
            get => _gameMode;
            set => _gameMode = value;
        }

        public MissionPack MissionPack
        {
            get => _missionPack;
            set => _missionPack = value;
        }

        public Player[] Players
        {
            get => _players;
        }

        public int ConsolePlayer
        {
            get => _consolePlayer;
            set => _consolePlayer = value;
        }

        public int Episode
        {
            get => _episode;
            set => _episode = value;
        }

        public int Map
        {
            get => _map;
            set => _map = value;
        }

        public GameSkill Skill
        {
            get => _skill;
            set => _skill = value;
        }

        public bool DemoPlayback
        {
            get => _demoPlayback;
            set => _demoPlayback = value;
        }

        public bool NetGame
        {
            get => _netGame;
            set => _netGame = value;
        }

        public int Deathmatch
        {
            get => _deathmatch;
            set => _deathmatch = value;
        }

        public bool FastMonsters
        {
            get => _fastMonsters;
            set => _fastMonsters = value;
        }

        public bool RespawnMonsters
        {
            get => _respawnMonsters;
            set => _respawnMonsters = value;
        }

        public bool NoMonsters
        {
            get => _noMonsters;
            set => _noMonsters = value;
        }

        public IntermissionInfo IntermissionInfo
        {
            get => _intermissionInfo;
        }

        public DoomRandom Random
        {
            get => _random;
        }

        public IVideo Video
        {
            get => _video;
            set => _video = value;
        }

        public ISound Sound
        {
            get => _sound;
            set => _sound = value;
        }

        public IMusic Music
        {
            get => _music;
            set => _music = value;
        }

        public IUserInput UserInput
        {
            get => _userInput;
            set => _userInput = value;
        }
    }
}

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
    public sealed class OpeningSequence
    {
        private readonly GameContent _content;
        private readonly GameOptions _options;

        private OpeningSequenceState _state;

        private int _currentStage;
        private int _nextStage;

        private int _count;
        private int _timer;

        private readonly TicCmd[] _cmds;
        private Demo? _demo;
        private DoomGame? _game;

        private bool _reset;

        public OpeningSequence(GameContent content, GameOptions options)
        {
            _content = content;
            _options = options;

            _cmds = new TicCmd[Player.MaxPlayerCount];
            for (int i = 0; i < Player.MaxPlayerCount; i++)
            {
                _cmds[i] = new TicCmd();
            }

            _currentStage = 0;
            _nextStage = 0;

            _reset = false;

            StartTitleScreen();
        }

        public void Reset()
        {
            _currentStage = 0;
            _nextStage = 0;

            _demo = null;
            _game = null;

            _reset = true;

            StartTitleScreen();
        }

        public UpdateResult Update()
        {
            UpdateResult updateResult = UpdateResult.None;

            if (_nextStage != _currentStage)
            {
                switch (_nextStage)
                {
                    case 0:
                        StartTitleScreen();
                        break;
                    case 1:
                        StartDemo("DEMO1");
                        break;
                    case 2:
                        StartCreditScreen();
                        break;
                    case 3:
                        StartDemo("DEMO2");
                        break;
                    case 4:
                        StartTitleScreen();
                        break;
                    case 5:
                        StartDemo("DEMO3");
                        break;
                    case 6:
                        StartCreditScreen();
                        break;
                    case 7:
                        StartDemo("DEMO4");
                        break;
                }

                _currentStage = _nextStage;
                updateResult = UpdateResult.NeedWipe;
            }

            switch (_currentStage)
            {
                case 0:
                    _count++;
                    if (_count == _timer)
                    {
                        _nextStage = 1;
                    }
                    break;

                case 1:
                    if (!_demo.ReadCmd(_cmds))
                    {
                        _nextStage = 2;
                    }
                    else
                    {
                        _game.Update(_cmds);
                    }
                    break;

                case 2:
                    _count++;
                    if (_count == _timer)
                    {
                        _nextStage = 3;
                    }
                    break;

                case 3:
                    if (!_demo.ReadCmd(_cmds))
                    {
                        _nextStage = 4;
                    }
                    else
                    {
                        _game.Update(_cmds);
                    }
                    break;

                case 4:
                    _count++;
                    if (_count == _timer)
                    {
                        _nextStage = 5;
                    }
                    break;

                case 5:
                    if (!_demo.ReadCmd(_cmds))
                    {
                        if (_content.Wad.GetLumpNumber("DEMO4") == -1)
                        {
                            _nextStage = 0;
                        }
                        else
                        {
                            _nextStage = 6;
                        }
                    }
                    else
                    {
                        _game.Update(_cmds);
                    }
                    break;

                case 6:
                    _count++;
                    if (_count == _timer)
                    {
                        _nextStage = 7;
                    }
                    break;

                case 7:
                    if (!_demo.ReadCmd(_cmds))
                    {
                        _nextStage = 0;
                    }
                    else
                    {
                        _game.Update(_cmds);
                    }
                    break;
            }

            if (_state == OpeningSequenceState.Title && _count == 1)
            {
                if (_options.GameMode == GameMode.Commercial)
                {
                    _options.Music.StartMusic(Bgm.DM2TTL, false);
                }
                else
                {
                    _options.Music.StartMusic(Bgm.INTRO, false);
                }
            }

            if (_reset)
            {
                _reset = false;
                return UpdateResult.NeedWipe;
            }
            else
            {
                return updateResult;
            }
        }

        private void StartTitleScreen()
        {
            _state = OpeningSequenceState.Title;

            _count = 0;
            if (_options.GameMode == GameMode.Commercial)
            {
                _timer = 35 * 11;
            }
            else
            {
                _timer = 170;
            }
        }

        private void StartCreditScreen()
        {
            _state = OpeningSequenceState.Credit;

            _count = 0;
            _timer = 200;
        }

        private void StartDemo(string lump)
        {
            _state = OpeningSequenceState.Demo;

            _demo = new Demo(_content.Wad.ReadLump(lump));
            _demo.Options.GameVersion = _options.GameVersion;
            _demo.Options.GameMode = _options.GameMode;
            _demo.Options.MissionPack = _options.MissionPack;
            _demo.Options.Video = _options.Video;
            _demo.Options.Sound = _options.Sound;
            _demo.Options.Music = _options.Music;

            _game = new DoomGame(_content, _demo.Options);
            _game.DeferedInitNew();
        }

        public OpeningSequenceState State => _state;
        public DoomGame? DemoGame => _game;
    }
}

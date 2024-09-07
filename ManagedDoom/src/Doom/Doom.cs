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
using ManagedDoom.Audio;
using ManagedDoom.Video;
using ManagedDoom.UserInput;

namespace ManagedDoom
{
    public class Doom
    {
        private readonly CommandLineArgs _args;
        private readonly Config _config;
        private readonly GameContent _content;
        private readonly IVideo _video;
        private readonly ISound _sound;
#pragma warning disable IDE0052 // Remove unread private members
        private readonly IMusic _music;
#pragma warning restore IDE0052 // Remove unread private members
        private readonly IUserInput _userInput;

        private readonly List<DoomEvent> _events = [];

        private readonly GameOptions _options;

        private readonly DoomMenu _menu;

        private readonly OpeningSequence _opening;

        private DemoPlayback? _demoPlayback;

        private readonly TicCmd[] _cmds;
        private readonly DoomGame _game;

        private readonly WipeEffect _wipeEffect;
        private bool _wiping;

        private DoomState _currentState;
        private DoomState _nextState;
        private bool _needWipe;

        private bool _sendPause;

        private bool _quit;
        private string _quitMessage = string.Empty;

        private bool _mouseGrabbed;

        public Doom(CommandLineArgs args, Config config, GameContent content, IVideo video, ISound sound, IMusic music, IUserInput userInput)
        {
            video ??= NullVideo.GetInstance();
            sound ??= NullSound.GetInstance();
            music ??= NullMusic.GetInstance();
            userInput ??= NullUserInput.GetInstance();

            _args = args;
            _config = config;
            _content = content;
            _video = video;
            _sound = sound;
            _music = music;
            _userInput = userInput;

            _options = new GameOptions(args, content)
            {
                Video = video,
                Sound = sound,
                Music = music,
                UserInput = userInput
            };

            _menu = new DoomMenu(this);

            _opening = new OpeningSequence(content, _options);

            _cmds = new TicCmd[Player.MaxPlayerCount];
            for (int i = 0; i < Player.MaxPlayerCount; i++)
            {
                _cmds[i] = new TicCmd();
            }
            _game = new DoomGame(content, _options);

            _wipeEffect = new WipeEffect(video.WipeBandCount, video.WipeHeight);
            _wiping = false;

            _currentState = DoomState.None;
            _nextState = DoomState.Opening;
            _needWipe = false;

            _sendPause = false;

            _quit = false;
            
            _mouseGrabbed = false;

            CheckGameArgs();
        }

        private void CheckGameArgs()
        {
            if (_args.warp.Present)
            {
                _nextState = DoomState.Game;
                _options.Episode = _args.warp.Value.Item1;
                _options.Map = _args.warp.Value.Item2;
                _game.DeferedInitNew();
            }
            else if (_args.episode.Present)
            {
                _nextState = DoomState.Game;
                _options.Episode = _args.episode.Value;
                _options.Map = 1;
                _game.DeferedInitNew();
            }

            if (_args.skill.Present)
            {
                _options.Skill = (GameSkill)(_args.skill.Value - 1);
            }

            if (_args.deathmatch.Present)
            {
                _options.Deathmatch = 1;
            }

            if (_args.altdeath.Present)
            {
                _options.Deathmatch = 2;
            }

            if (_args.fast.Present)
            {
                _options.FastMonsters = true;
            }

            if (_args.respawn.Present)
            {
                _options.RespawnMonsters = true;
            }

            if (_args.nomonsters.Present)
            {
                _options.NoMonsters = true;
            }

            if (_args.loadgame.Present)
            {
                _nextState = DoomState.Game;
                _game.LoadGame(_args.loadgame.Value);
            }

            if (_args.playdemo.Present)
            {
                _nextState = DoomState.DemoPlayback;
                _demoPlayback = new DemoPlayback(_args, _content, _options, _args.playdemo.Value);
            }

            if (_args.timedemo.Present)
            {
                _nextState = DoomState.DemoPlayback;
                _demoPlayback = new DemoPlayback(_args, _content, _options, _args.timedemo.Value);
            }
        }

        public void NewGame(GameSkill skill, int episode, int map)
        {
            _game.DeferedInitNew(skill, episode, map);
            _nextState = DoomState.Game;
        }

        public void EndGame()
        {
            _nextState = DoomState.Opening;
        }

        private void DoEvents()
        {
            if (_wiping)
            {
                return;
            }

            foreach (DoomEvent e in _events)
            {
                if (_menu.DoEvent(e))
                {
                    continue;
                }

                if (e.Type == EventType.KeyDown)
                {
                    if (CheckFunctionKey(e.Key))
                    {
                        continue;
                    }
                }

                if (_currentState == DoomState.Game)
                {
                    if (e.Key == DoomKey.Pause && e.Type == EventType.KeyDown)
                    {
                        _sendPause = true;
                        continue;
                    }

                    if (_game.DoEvent(e))
                    {
                        continue;
                    }
                }
                else if (_currentState == DoomState.DemoPlayback)
                {
                    _demoPlayback.DoEvent(e);
                }
            }

            _events.Clear();
        }

        private bool CheckFunctionKey(DoomKey key)
        {
            switch (key)
            {
                case DoomKey.F1:
                    _menu.ShowHelpScreen();
                    return true;

                case DoomKey.F2:
                    _menu.ShowSaveScreen();
                    return true;

                case DoomKey.F3:
                    _menu.ShowLoadScreen();
                    return true;

                case DoomKey.F4:
                    _menu.ShowVolumeControl();
                    return true;

                case DoomKey.F6:
                    _menu.QuickSave();
                    return true;

                case DoomKey.F7:
                    if (_currentState == DoomState.Game)
                    {
                        _menu.EndGame();
                    }
                    else
                    {
                        _options.Sound.StartSound(Sfx.OOF);
                    }
                    return true;

                case DoomKey.F8:
                    _video.DisplayMessage = !_video.DisplayMessage;
                    if (_currentState == DoomState.Game && _game.State == GameState.Level)
                    {
                        string msg;
                        if (_video.DisplayMessage)
                        {
                            msg = DoomInfo.Strings.MSGON;
                        }
                        else
                        {
                            msg = DoomInfo.Strings.MSGOFF;
                        }
                        _game.World.ConsolePlayer.SendMessage(msg);
                    }
                    _menu.StartSound(Sfx.SWTCHN);
                    return true;

                case DoomKey.F9:
                    _menu.QuickLoad();
                    return true;

                case DoomKey.F10:
                    _menu.Quit();
                    return true;

                case DoomKey.F11:
                    int gcl = _video.GammaCorrectionLevel;
                    gcl++;
                    if (gcl > _video.MaxGammaCorrectionLevel)
                    {
                        gcl = 0;
                    }
                    _video.GammaCorrectionLevel = gcl;
                    if (_currentState == DoomState.Game && _game.State == GameState.Level)
                    {
                        string msg;
                        if (gcl == 0)
                        {
                            msg = DoomInfo.Strings.GAMMALVL0;
                        }
                        else
                        {
                            msg = "Gamma correction level " + gcl;
                        }
                        _game.World.ConsolePlayer.SendMessage(msg);
                    }
                    return true;

                case DoomKey.Add:
                case DoomKey.Quote:
                case DoomKey.Equal:
                    if (_currentState == DoomState.Game &&
                        _game.State == GameState.Level &&
                        _game.World.AutoMap.Visible)
                    {
                        return false;
                    }
                    else
                    {
                        _video.WindowSize = Math.Min(_video.WindowSize + 1, _video.MaxWindowSize);
                        _sound.StartSound(Sfx.STNMOV);
                        return true;
                    }

                case DoomKey.Subtract:
                case DoomKey.Hyphen:
                case DoomKey.Semicolon:
                    if (_currentState == DoomState.Game &&
                        _game.State == GameState.Level &&
                        _game.World.AutoMap.Visible)
                    {
                        return false;
                    }
                    else
                    {
                        _video.WindowSize = Math.Max(_video.WindowSize - 1, 0);
                        _sound.StartSound(Sfx.STNMOV);
                        return true;
                    }

                default:
                    return false;
            }
        }

        public UpdateResult Update()
        {
            DoEvents();

            if (!_wiping)
            {
                _menu.Update();

                if (_nextState != _currentState)
                {
                    if (_nextState != DoomState.Opening)
                    {
                        _opening.Reset();
                    }

                    if (_nextState != DoomState.DemoPlayback)
                    {
                        _demoPlayback = null;
                    }

                    _currentState = _nextState;
                }

                if (_quit)
                {
                    return UpdateResult.Completed;
                }

                if (_needWipe)
                {
                    _needWipe = false;
                    StartWipe();
                }
            }

            if (!_wiping)
            {
                switch (_currentState)
                {
                    case DoomState.Opening:
                        if (_opening.Update() == UpdateResult.NeedWipe)
                        {
                            StartWipe();
                        }
                        break;

                    case DoomState.DemoPlayback:
                        UpdateResult result = _demoPlayback.Update();
                        if (result == UpdateResult.NeedWipe)
                        {
                            StartWipe();
                        }
                        else if (result == UpdateResult.Completed)
                        {
                            Quit("FPS: " + _demoPlayback.Fps.ToString("0.0"));
                        }
                        break;

                    case DoomState.Game:
                        _userInput.BuildTicCmd(_cmds[_options.ConsolePlayer]);
                        if (_sendPause)
                        {
                            _sendPause = false;
                            _cmds[_options.ConsolePlayer].Buttons |= (byte)(TicCmdButtons.Special | TicCmdButtons.Pause);
                        }
                        if (_game.Update(_cmds) == UpdateResult.NeedWipe)
                        {
                            StartWipe();
                        }
                        break;

                    default:
                        throw new Exception("Invalid application state!");
                }
            }

            if (_wiping)
            {
                if (_wipeEffect.Update() == UpdateResult.Completed)
                {
                    _wiping = false;
                }
            }

            _sound.Update();

            CheckMouseState();

            return UpdateResult.None;
        }

        private void CheckMouseState()
        {
            bool mouseShouldBeGrabbed;
            if (!_video.HasFocus())
            {
                mouseShouldBeGrabbed = false;
            }
            else if (_config.video_fullscreen)
            {
                mouseShouldBeGrabbed = true;
            }
            else
            {
                mouseShouldBeGrabbed = _currentState == DoomState.Game && !_menu.Active;
            }

            if (_mouseGrabbed)
            {
                if (!mouseShouldBeGrabbed)
                {
                    _userInput.ReleaseMouse();
                    _mouseGrabbed = false;
                }
            }
            else
            {
                if (mouseShouldBeGrabbed)
                {
                    _userInput.GrabMouse();
                    _mouseGrabbed = true;
                }
            }
        }

        private void StartWipe()
        {
            _wipeEffect.Start();
            _video.InitializeWipe();
            _wiping = true;
        }

        public void PauseGame()
        {
            if (_currentState == DoomState.Game &&
                _game.State == GameState.Level &&
                !_game.Paused && !_sendPause)
            {
                _sendPause = true;
            }
        }

        public void ResumeGame()
        {
            if (_currentState == DoomState.Game &&
                _game.State == GameState.Level &&
                _game.Paused && !_sendPause)
            {
                _sendPause = true;
            }
        }

        public bool SaveGame(int slotNumber, string description)
        {
            if (_currentState == DoomState.Game && _game.State == GameState.Level)
            {
                _game.SaveGame(slotNumber, description);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void LoadGame(int slotNumber)
        {
            _game.LoadGame(slotNumber);
            _nextState = DoomState.Game;
        }

        public void Quit()
        {
            _quit = true;
        }

        public void Quit(string message)
        {
            _quit = true;
            _quitMessage = message;
        }

        public void PostEvent(DoomEvent e)
        {
            if (_events.Count < 64)
            {
                _events.Add(e);
            }
        }

        public DoomState State => _currentState;
        public OpeningSequence Opening => _opening;
        public DemoPlayback? DemoPlayback => _demoPlayback;
        public GameOptions Options => _options;
        public DoomGame Game => _game;
        public DoomMenu Menu => _menu;
        public WipeEffect WipeEffect => _wipeEffect;
        public bool Wiping => _wiping;
        public string QuitMessage => _quitMessage;
    }
}

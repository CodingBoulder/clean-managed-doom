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
    public sealed class DoomGame
    {
        private readonly GameContent _content;
        private readonly GameOptions _options;

        private GameAction _gameAction;
        private GameState _gameState;

        private int _gameTic;

        private World _world;
        private Intermission? _intermission;
        private Finale _finale;

        private bool _paused;

        private int _loadGameSlotNumber;
        private int _saveGameSlotNumber;
        private string _saveGameDescription;

        public DoomGame(GameContent content, GameOptions options)
        {
            _content = content;
            _options = options;

            _gameAction = GameAction.Nothing;

            _gameTic = 0;
        }


        ////////////////////////////////////////////////////////////
        // Public methods to control the game state
        ////////////////////////////////////////////////////////////

        /// <summary>
        /// Start a new game.
        /// Can be called by the startup code or the menu task.
        /// </summary>
        public void DeferedInitNew()
        {
            _gameAction = GameAction.NewGame;
        }

        /// <summary>
        /// Start a new game.
        /// Can be called by the startup code or the menu task.
        /// </summary>
        public void DeferedInitNew(GameSkill skill, int episode, int map)
        {
            _options.Skill = skill;
            _options.Episode = episode;
            _options.Map = map;
            _gameAction = GameAction.NewGame;
        }

        /// <summary>
        /// Load the saved game at the given slot number.
        /// Can be called by the startup code or the menu task.
        /// </summary>
        public void LoadGame(int slotNumber)
        {
            _loadGameSlotNumber = slotNumber;
            _gameAction = GameAction.LoadGame;
        }

        /// <summary>
        /// Save the game at the given slot number.
        /// Can be called by the startup code or the menu task.
        /// </summary>
        public void SaveGame(int slotNumber, string description)
        {
            _saveGameSlotNumber = slotNumber;
            _saveGameDescription = description;
            _gameAction = GameAction.SaveGame;
        }

        /// <summary>
        /// Advance the game one frame.
        /// </summary>
        public UpdateResult Update(TicCmd[] cmds)
        {
            // Do player reborns if needed.
            Player[] players = _options.Players;
            for (int i = 0; i < Player.MaxPlayerCount; i++)
            {
                if (players[i].InGame && players[i].PlayerState == PlayerState.Reborn)
                {
                    DoReborn(i);
                }
            }

            // Do things to change the game state.
            while (_gameAction != GameAction.Nothing)
            {
                switch (_gameAction)
                {
                    case GameAction.LoadLevel:
                        DoLoadLevel();
                        break;
                    case GameAction.NewGame:
                        DoNewGame();
                        break;
                    case GameAction.LoadGame:
                        DoLoadGame();
                        break;
                    case GameAction.SaveGame:
                        DoSaveGame();
                        break;
                    case GameAction.Completed:
                        DoCompleted();
                        break;
                    case GameAction.Victory:
                        DoFinale();
                        break;
                    case GameAction.WorldDone:
                        DoWorldDone();
                        break;
                    case GameAction.Nothing:
                        break;
                }
            }

            for (int i = 0; i < Player.MaxPlayerCount; i++)
            {
                if (players[i].InGame)
                {
                    TicCmd cmd = players[i].Cmd;
                    cmd.CopyFrom(cmds[i]);

                    /*
					if (demorecording)
					{
						G_WriteDemoTiccmd(cmd);
					}
					*/

                    // Check for turbo cheats.
                    if (cmd.ForwardMove > GameConst.TurboThreshold &&
                        (_world.LevelTime & 31) == 0 &&
                        ((_world.LevelTime >> 5) & 3) == i)
                    {
                        Player player = players[_options.ConsolePlayer];
                        player.SendMessage(players[i].Name + " is turbo!");
                    }
                }
            }

            // Check for special buttons.
            for (int i = 0; i < Player.MaxPlayerCount; i++)
            {
                if (players[i].InGame)
                {
                    if ((players[i].Cmd.Buttons & TicCmdButtons.Special) != 0)
                    {
                        if ((players[i].Cmd.Buttons & TicCmdButtons.SpecialMask) == TicCmdButtons.Pause)
                        {
                            _paused = !_paused;
                            if (_paused)
                            {
                                _options.Sound.Pause();
                            }
                            else
                            {
                                _options.Sound.Resume();
                            }
                        }
                    }
                }
            }

            // Do main actions.
            UpdateResult result = UpdateResult.None;
            switch (_gameState)
            {
                case GameState.Level:
                    if (!_paused || _world.FirstTicIsNotYetDone)
                    {
                        result = _world.Update();
                        if (result == UpdateResult.Completed)
                        {
                            _gameAction = GameAction.Completed;
                        }
                    }
                    break;

                case GameState.Intermission:
                    result = _intermission.Update();
                    if (result == UpdateResult.Completed)
                    {
                        _gameAction = GameAction.WorldDone;

                        if (_world.SecretExit)
                        {
                            players[_options.ConsolePlayer].DidSecret = true;
                        }

                        if (_options.GameMode == GameMode.Commercial)
                        {
                            switch (_options.Map)
                            {
                                case 6:
                                case 11:
                                case 20:
                                case 30:
                                    DoFinale();
                                    result = UpdateResult.NeedWipe;
                                    break;

                                case 15:
                                case 31:
                                    if (_world.SecretExit)
                                    {
                                        DoFinale();
                                        result = UpdateResult.NeedWipe;
                                    }
                                    break;
                            }
                        }
                    }
                    break;

                case GameState.Finale:
                    result = _finale.Update();
                    if (result == UpdateResult.Completed)
                    {
                        _gameAction = GameAction.WorldDone;
                    }
                    break;
            }

            _gameTic++;

            if (result == UpdateResult.NeedWipe)
            {
                return UpdateResult.NeedWipe;
            }
            else
            {
                return UpdateResult.None;
            }
        }


        ////////////////////////////////////////////////////////////
        // Actual game actions
        ////////////////////////////////////////////////////////////

        // It seems that these methods should not be called directly
        // from outside for some reason.
        // So if you want to start a new game or do load / save, use
        // the following public methods.
        //
        //     - DeferedInitNew
        //     - LoadGame
        //     - SaveGame

        private void DoLoadLevel()
        {
            _gameAction = GameAction.Nothing;

            _gameState = GameState.Level;

            Player[] players = _options.Players;
            for (int i = 0; i < Player.MaxPlayerCount; i++)
            {
                if (players[i].InGame && players[i].PlayerState == PlayerState.Dead)
                {
                    players[i].PlayerState = PlayerState.Reborn;
                }
                Array.Clear(players[i].Frags, 0, players[i].Frags.Length);
            }

            _intermission = null;

            _options.Sound.Reset();

            _world = new World(_content, _options, this);

            _options.UserInput.Reset();
        }

        private void DoNewGame()
        {
            _gameAction = GameAction.Nothing;

            InitNew(_options.Skill, _options.Episode, _options.Map);
        }

        private void DoLoadGame()
        {
            _gameAction = GameAction.Nothing;

            string? directory = ConfigUtilities.GetExeDirectory();
            string path = Path.Combine(directory, "doomsav" + _loadGameSlotNumber + ".dsg");
            SaveAndLoad.Load(this, path);
        }

        private void DoSaveGame()
        {
            _gameAction = GameAction.Nothing;

            string? directory = ConfigUtilities.GetExeDirectory();
            string path = Path.Combine(directory, "doomsav" + _saveGameSlotNumber + ".dsg");
            SaveAndLoad.Save(this, _saveGameDescription, path);
            _world.ConsolePlayer.SendMessage(DoomInfo.Strings.GGSAVED);
        }

        private void DoCompleted()
        {
            _gameAction = GameAction.Nothing;

            for (int i = 0; i < Player.MaxPlayerCount; i++)
            {
                if (_options.Players[i].InGame)
                {
                    // Take away cards and stuff.
                    _options.Players[i].FinishLevel();
                }
            }

            if (_options.GameMode != GameMode.Commercial)
            {
                switch (_options.Map)
                {
                    case 8:
                        _gameAction = GameAction.Victory;
                        return;
                    case 9:
                        for (int i = 0; i < Player.MaxPlayerCount; i++)
                        {
                            _options.Players[i].DidSecret = true;
                        }
                        break;
                }
            }

            if ((_options.Map == 8) && (_options.GameMode != GameMode.Commercial))
            {
                // Victory.
                _gameAction = GameAction.Victory;
                return;
            }

            if ((_options.Map == 9) && (_options.GameMode != GameMode.Commercial))
            {
                // Exit secret level.
                for (int i = 0; i < Player.MaxPlayerCount; i++)
                {
                    _options.Players[i].DidSecret = true;
                }
            }

            IntermissionInfo imInfo = _options.IntermissionInfo;

            imInfo.DidSecret = _options.Players[_options.ConsolePlayer].DidSecret;
            imInfo.Episode = _options.Episode - 1;
            imInfo.LastLevel = _options.Map - 1;

            // IntermissionInfo.Next is 0 biased, unlike GameOptions.Map.
            if (_options.GameMode == GameMode.Commercial)
            {
                if (_world.SecretExit)
                {
                    switch (_options.Map)
                    {
                        case 15:
                            imInfo.NextLevel = 30;
                            break;
                        case 31:
                            imInfo.NextLevel = 31;
                            break;
                    }
                }
                else
                {
                    switch (_options.Map)
                    {
                        case 31:
                        case 32:
                            imInfo.NextLevel = 15;
                            break;
                        default:
                            imInfo.NextLevel = _options.Map;
                            break;
                    }
                }
            }
            else
            {
                if (_world.SecretExit)
                {
                    // Go to secret level.
                    imInfo.NextLevel = 8;
                }
                else if (_options.Map == 9)
                {
                    // Returning from secret level.
                    switch (_options.Episode)
                    {
                        case 1:
                            imInfo.NextLevel = 3;
                            break;
                        case 2:
                            imInfo.NextLevel = 5;
                            break;
                        case 3:
                            imInfo.NextLevel = 6;
                            break;
                        case 4:
                            imInfo.NextLevel = 2;
                            break;
                    }
                }
                else
                {
                    // Go to next level.
                    imInfo.NextLevel = _options.Map;
                }
            }

            imInfo.MaxKillCount = _world.TotalKills;
            imInfo.MaxItemCount = _world.TotalItems;
            imInfo.MaxSecretCount = _world.TotalSecrets;
            imInfo.TotalFrags = 0;
            if (_options.GameMode == GameMode.Commercial)
            {
                imInfo.ParTime = 35 * DoomInfo.ParTimes.Doom2[_options.Map - 1];
            }
            else
            {
                imInfo.ParTime = 35 * DoomInfo.ParTimes.Doom1[_options.Episode - 1][_options.Map - 1];
            }

            Player[] players = _options.Players;
            for (int i = 0; i < Player.MaxPlayerCount; i++)
            {
                imInfo.Players[i].InGame = players[i].InGame;
                imInfo.Players[i].KillCount = players[i].KillCount;
                imInfo.Players[i].ItemCount = players[i].ItemCount;
                imInfo.Players[i].SecretCount = players[i].SecretCount;
                imInfo.Players[i].Time = _world.LevelTime;
                Array.Copy(players[i].Frags, imInfo.Players[i].Frags, Player.MaxPlayerCount);
            }

            _gameState = GameState.Intermission;
            _intermission = new Intermission(_options, imInfo);
        }

        private void DoWorldDone()
        {
            _gameAction = GameAction.Nothing;

            _gameState = GameState.Level;
            _options.Map = _options.IntermissionInfo.NextLevel + 1;
            DoLoadLevel();
        }

        private void DoFinale()
        {
            _gameAction = GameAction.Nothing;

            _gameState = GameState.Finale;
            _finale = new Finale(_options);
        }


        ////////////////////////////////////////////////////////////
        // Miscellaneous things
        ////////////////////////////////////////////////////////////

        public void InitNew(GameSkill skill, int episode, int map)
        {
            _options.Skill = (GameSkill)Math.Clamp((int)skill, (int)GameSkill.Baby, (int)GameSkill.Nightmare);

            if (_options.GameMode == GameMode.Retail)
            {
                _options.Episode = Math.Clamp(episode, 1, 4);
            }
            else if (_options.GameMode == GameMode.Shareware)
            {
                _options.Episode = 1;
            }
            else
            {
                _options.Episode = Math.Clamp(episode, 1, 4);
            }

            if (_options.GameMode == GameMode.Commercial)
            {
                _options.Map = Math.Clamp(map, 1, 32);
            }
            else
            {
                _options.Map = Math.Clamp(map, 1, 9);
            }

            _options.Random.Clear();

            // Force players to be initialized upon first level load.
            for (int i = 0; i < Player.MaxPlayerCount; i++)
            {
                _options.Players[i].PlayerState = PlayerState.Reborn;
            }

            DoLoadLevel();
        }

        public bool DoEvent(DoomEvent e)
        {
            if (_gameState == GameState.Level)
            {
                return _world.DoEvent(e);
            }
            else if (_gameState == GameState.Finale)
            {
                return _finale.DoEvent(e);
            }

            return false;
        }

        private void DoReborn(int playerNumber)
        {
            if (!_options.NetGame)
            {
                // Reload the level from scratch.
                _gameAction = GameAction.LoadLevel;
            }
            else
            {
                // Respawn at the start.

                // First dissasociate the corpse.
                _options.Players[playerNumber].Mobj.Player = null;

                ThingAllocation ta = _world.ThingAllocation;

                // Spawn at random spot if in death match.
                if (_options.Deathmatch != 0)
                {
                    ta.DeathMatchSpawnPlayer(playerNumber);
                    return;
                }

                if (ta.CheckSpot(playerNumber, ta.PlayerStarts[playerNumber]))
                {
                    ta.SpawnPlayer(ta.PlayerStarts[playerNumber]);
                    return;
                }

                // Try to spawn at one of the other players spots.
                for (int i = 0; i < Player.MaxPlayerCount; i++)
                {
                    if (ta.CheckSpot(playerNumber, ta.PlayerStarts[i]))
                    {
                        // Fake as other player.
                        ta.PlayerStarts[i].Type = playerNumber + 1;

                        _world.ThingAllocation.SpawnPlayer(ta.PlayerStarts[i]);

                        // Restore.
                        ta.PlayerStarts[i].Type = i + 1;

                        return;
                    }
                }

                // He's going to be inside something.
                // Too bad.
                _world.ThingAllocation.SpawnPlayer(ta.PlayerStarts[playerNumber]);
            }
        }


        public GameOptions Options => _options;
        public GameState State => _gameState;
        public int GameTic => _gameTic;
        public World World => _world;
        public Intermission? Intermission => _intermission;
        public Finale Finale => _finale;
        public bool Paused => _paused;



        private enum GameAction
        {
            Nothing,
            LoadLevel,
            NewGame,
            LoadGame,
            SaveGame,
            Completed,
            Victory,
            WorldDone
        }
    }
}

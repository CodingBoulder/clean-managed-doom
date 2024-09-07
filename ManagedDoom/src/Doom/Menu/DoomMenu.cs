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
    public sealed class DoomMenu
    {
        private readonly Doom _doom;

        private readonly SelectableMenu _main;
        private readonly SelectableMenu _episodeMenu;
        private readonly SelectableMenu _skillMenu;
        private readonly SelectableMenu _optionMenu;
        private readonly SelectableMenu _volume;
        private readonly LoadMenu _load;
        private readonly SaveMenu _save;
        private readonly HelpScreen _help;

        private readonly PressAnyKey _thisIsShareware;
        private readonly PressAnyKey _saveFailed;
        private readonly YesNoConfirm _nightmareConfirm;
        private readonly YesNoConfirm _endGameConfirm;
        private readonly QuitConfirm _quitConfirm;

        private MenuDef _current;

        private bool _active;

        private int _tics;

        private int _selectedEpisode;

        private readonly SaveSlots _saveSlots;

        public DoomMenu(Doom doom)
        {
            _doom = doom;

            _thisIsShareware = new PressAnyKey(
                this,
                DoomInfo.Strings.SWSTRING,
                null);

            _saveFailed = new PressAnyKey(
                this,
                DoomInfo.Strings.SAVEDEAD,
                null);

            _nightmareConfirm = new YesNoConfirm(
                this,
                DoomInfo.Strings.NIGHTMARE,
                () => doom.NewGame(GameSkill.Nightmare, _selectedEpisode, 1));

            _endGameConfirm = new YesNoConfirm(
                this,
                DoomInfo.Strings.ENDGAME,
                () => doom.EndGame());

            _quitConfirm = new QuitConfirm(
                this,
                doom);

            _skillMenu = new SelectableMenu(
                this,
                "M_NEWG", 96, 14,
                "M_SKILL", 54, 38,
                2,

                new SimpleMenuItem(
                    "M_JKILL", 16, 58, 48, 63,
                    () => doom.NewGame(GameSkill.Baby, _selectedEpisode, 1),
                    null),

                new SimpleMenuItem(
                    "M_ROUGH", 16, 74, 48, 79,
                    () => doom.NewGame(GameSkill.Easy, _selectedEpisode, 1),
                    null),

                new SimpleMenuItem(
                    "M_HURT", 16, 90, 48, 95,
                    () => doom.NewGame(GameSkill.Medium, _selectedEpisode, 1),
                    null),

                new SimpleMenuItem(
                    "M_ULTRA", 16, 106, 48, 111,
                    () => doom.NewGame(GameSkill.Hard, _selectedEpisode, 1),
                    null),

                new SimpleMenuItem(
                    "M_NMARE", 16, 122, 48, 127,
                    null,
                    _nightmareConfirm));

            if (doom.Options.GameMode == GameMode.Retail)
            {
                _episodeMenu = new SelectableMenu(
                    this,
                    "M_EPISOD", 54, 38,
                    0,

                    new SimpleMenuItem(
                        "M_EPI1", 16, 58, 48, 63,
                        () => _selectedEpisode = 1,
                        _skillMenu),

                    new SimpleMenuItem(
                        "M_EPI2", 16, 74, 48, 79,
                        () => _selectedEpisode = 2,
                        _skillMenu),

                    new SimpleMenuItem(
                        "M_EPI3", 16, 90, 48, 95,
                        () => _selectedEpisode = 3,
                        _skillMenu),

                    new SimpleMenuItem(
                        "M_EPI4", 16, 106, 48, 111,
                        () => _selectedEpisode = 4,
                        _skillMenu));
            }
            else
            {
                if (doom.Options.GameMode == GameMode.Shareware)
                {
                    _episodeMenu = new SelectableMenu(
                        this,
                        "M_EPISOD", 54, 38,
                        0,

                        new SimpleMenuItem(
                            "M_EPI1", 16, 58, 48, 63,
                            () => _selectedEpisode = 1,
                            _skillMenu),

                        new SimpleMenuItem(
                            "M_EPI2", 16, 74, 48, 79,
                            null,
                            _thisIsShareware),

                        new SimpleMenuItem(
                            "M_EPI3", 16, 90, 48, 95,
                            null,
                            _thisIsShareware));
                }
                else
                {
                    _episodeMenu = new SelectableMenu(
                        this,
                        "M_EPISOD", 54, 38,
                        0,

                        new SimpleMenuItem(
                            "M_EPI1", 16, 58, 48, 63,
                            () => _selectedEpisode = 1,
                            _skillMenu),
                        new SimpleMenuItem(
                            "M_EPI2", 16, 74, 48, 79,
                            () => _selectedEpisode = 2,
                            _skillMenu),
                        new SimpleMenuItem(
                            "M_EPI3", 16, 90, 48, 95,
                            () => _selectedEpisode = 3,
                            _skillMenu));
                }
            }

            Audio.ISound sound = doom.Options.Sound;
            Audio.IMusic music = doom.Options.Music;
            _volume = new SelectableMenu(
                this,
                "M_SVOL", 60, 38,
                0,

                new SliderMenuItem(
                    "M_SFXVOL", 48, 59, 80, 64,
                    sound.MaxVolume + 1,
                    () => sound.Volume,
                    vol => sound.Volume = vol),

                new SliderMenuItem("M_MUSVOL", 48, 91, 80, 96,
                    music.MaxVolume + 1,
                    () => music.Volume,
                    vol => music.Volume = vol));

            Video.IVideo video = doom.Options.Video;
            UserInput.IUserInput userInput = doom.Options.UserInput;
            _optionMenu = new SelectableMenu(
                this,
                "M_OPTTTL", 108, 15,
                0,

                new SimpleMenuItem(
                    "M_ENDGAM", 28, 32, 60, 37,
                    null,
                    _endGameConfirm,
                    () => doom.State == DoomState.Game),

                new ToggleMenuItem(
                    "M_MESSG", 28, 48, 60, 53, "M_MSGON", "M_MSGOFF", 180,
                    () => video.DisplayMessage ? 0 : 1,
                    value => video.DisplayMessage = value == 0),

                new SliderMenuItem(
                    "M_SCRNSZ", 28, 80 - 16, 60, 85 - 16,
                    video.MaxWindowSize + 1,
                    () => video.WindowSize,
                    size => video.WindowSize = size),

                new SliderMenuItem(
                    "M_MSENS", 28, 112 - 16, 60, 117 - 16,
                    userInput.MaxMouseSensitivity + 1,
                    () => userInput.MouseSensitivity,
                    ms => userInput.MouseSensitivity = ms),

                new SimpleMenuItem(
                    "M_SVOL", 28, 144 - 16, 60, 149 - 16,
                    null,
                    _volume));

            _load = new LoadMenu(
                this,
                "M_LOADG", 72, 28,
                0,
                new TextBoxMenuItem(48, 49, 72, 61),
                new TextBoxMenuItem(48, 65, 72, 77),
                new TextBoxMenuItem(48, 81, 72, 93),
                new TextBoxMenuItem(48, 97, 72, 109),
                new TextBoxMenuItem(48, 113, 72, 125),
                new TextBoxMenuItem(48, 129, 72, 141));

            _save = new SaveMenu(
                this,
                "M_SAVEG", 72, 28,
                0,
                new TextBoxMenuItem(48, 49, 72, 61),
                new TextBoxMenuItem(48, 65, 72, 77),
                new TextBoxMenuItem(48, 81, 72, 93),
                new TextBoxMenuItem(48, 97, 72, 109),
                new TextBoxMenuItem(48, 113, 72, 125),
                new TextBoxMenuItem(48, 129, 72, 141));

            _help = new HelpScreen(this);

            if (doom.Options.GameMode == GameMode.Commercial)
            {
                _main = new SelectableMenu(
                this,
                "M_DOOM", 94, 2,
                0,
                new SimpleMenuItem("M_NGAME", 65, 67, 97, 72, null, _skillMenu),
                new SimpleMenuItem("M_OPTION", 65, 83, 97, 88, null, _optionMenu),
                new SimpleMenuItem("M_LOADG", 65, 99, 97, 104, null, _load),
                new SimpleMenuItem("M_SAVEG", 65, 115, 97, 120, null, _save,
                    () => !(doom.State == DoomState.Game &&
                        doom.Game.State != GameState.Level)),
                new SimpleMenuItem("M_QUITG", 65, 131, 97, 136, null, _quitConfirm));
            }
            else
            {
                _main = new SelectableMenu(
                this,
                "M_DOOM", 94, 2,
                0,
                new SimpleMenuItem("M_NGAME", 65, 59, 97, 64, null, _episodeMenu),
                new SimpleMenuItem("M_OPTION", 65, 75, 97, 80, null, _optionMenu),
                new SimpleMenuItem("M_LOADG", 65, 91, 97, 96, null, _load),
                new SimpleMenuItem("M_SAVEG", 65, 107, 97, 112, null, _save,
                    () => !(doom.State == DoomState.Game &&
                        doom.Game.State != GameState.Level)),
                new SimpleMenuItem("M_RDTHIS", 65, 123, 97, 128, null, _help),
                new SimpleMenuItem("M_QUITG", 65, 139, 97, 144, null, _quitConfirm));
            }

            _current = _main;
            _active = false;

            _tics = 0;

            _selectedEpisode = 1;

            _saveSlots = new SaveSlots();
        }

        public bool DoEvent(DoomEvent e)
        {
            if (_active)
            {
                if (_current.DoEvent(e))
                {
                    return true;
                }

                if (e.Key == DoomKey.Escape && e.Type == EventType.KeyDown)
                {
                    Close();
                }

                return true;
            }
            else
            {
                if (e.Key == DoomKey.Escape && e.Type == EventType.KeyDown)
                {
                    SetCurrent(_main);
                    Open();
                    StartSound(Sfx.SWTCHN);
                    return true;
                }

                if (e.Type == EventType.KeyDown && _doom.State == DoomState.Opening)
                {
                    if (e.Key == DoomKey.Enter ||
                        e.Key == DoomKey.Space ||
                        e.Key == DoomKey.LControl ||
                        e.Key == DoomKey.RControl ||
                        e.Key == DoomKey.Escape)
                    {
                        SetCurrent(_main);
                        Open();
                        StartSound(Sfx.SWTCHN);
                        return true;
                    }
                }

                return false;
            }
        }

        public void Update()
        {
            _tics++;

            _current?.Update();

            if (_active && !_doom.Options.NetGame)
            {
                _doom.PauseGame();
            }
        }

        public void SetCurrent(MenuDef next)
        {
            _current = next;
            _current.Open();
        }

        public void Open()
        {
            _active = true;
        }

        public void Close()
        {
            _active = false;

            if (!_doom.Options.NetGame)
            {
                _doom.ResumeGame();
            }
        }

        public void StartSound(Sfx sfx)
        {
            _doom.Options.Sound.StartSound(sfx);
        }

        public void NotifySaveFailed()
        {
            SetCurrent(_saveFailed);
        }

        public void ShowHelpScreen()
        {
            SetCurrent(_help);
            Open();
            StartSound(Sfx.SWTCHN);
        }

        public void ShowSaveScreen()
        {
            SetCurrent(_save);
            Open();
            StartSound(Sfx.SWTCHN);
        }

        public void ShowLoadScreen()
        {
            SetCurrent(_load);
            Open();
            StartSound(Sfx.SWTCHN);
        }

        public void ShowVolumeControl()
        {
            SetCurrent(_volume);
            Open();
            StartSound(Sfx.SWTCHN);
        }

        public void QuickSave()
        {
            if (_save.LastSaveSlot == -1)
            {
                ShowSaveScreen();
            }
            else
            {
                string desc = _saveSlots[_save.LastSaveSlot];
                var confirm = new YesNoConfirm(
                    this,
                    ((string)DoomInfo.Strings.QSPROMPT).Replace("%s", desc),
                    () => _save.DoSave(_save.LastSaveSlot));
                SetCurrent(confirm);
                Open();
                StartSound(Sfx.SWTCHN);
            }
        }

        public void QuickLoad()
        {
            if (_save.LastSaveSlot == -1)
            {
                var pak = new PressAnyKey(
                    this,
                    DoomInfo.Strings.QSAVESPOT,
                    null);
                SetCurrent(pak);
                Open();
                StartSound(Sfx.SWTCHN);
            }
            else
            {
                string desc = _saveSlots[_save.LastSaveSlot];
                var confirm = new YesNoConfirm(
                    this,
                    ((string)DoomInfo.Strings.QLPROMPT).Replace("%s", desc),
                    () => _load.DoLoad(_save.LastSaveSlot));
                SetCurrent(confirm);
                Open();
                StartSound(Sfx.SWTCHN);
            }
        }

        public void EndGame()
        {
            SetCurrent(_endGameConfirm);
            Open();
            StartSound(Sfx.SWTCHN);
        }

        public void Quit()
        {
            SetCurrent(_quitConfirm);
            Open();
            StartSound(Sfx.SWTCHN);
        }

        public Doom Doom => _doom;
        public GameOptions Options => _doom.Options;
        public MenuDef Current => _current;
        public bool Active => _active;
        public int Tics => _tics;
        public SaveSlots SaveSlots => _saveSlots;
    }
}

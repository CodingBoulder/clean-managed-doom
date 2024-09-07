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
using System.Diagnostics;
using System.IO;

namespace ManagedDoom
{
    public sealed class DemoPlayback
    {
        private readonly Demo _demo;
        private readonly TicCmd[] _cmds;
        private readonly DoomGame _game;

        private readonly Stopwatch _stopwatch;
        private int _frameCount;

        public DemoPlayback(CommandLineArgs args, GameContent content, GameOptions options, string demoName)
        {
            if (File.Exists(demoName))
            {
                _demo = new Demo(demoName);
            }
            else if (File.Exists(demoName + ".lmp"))
            {
                _demo = new Demo(demoName + ".lmp");
            }
            else
            {
                string lumpName = demoName.ToUpper();
                if (content.Wad.GetLumpNumber(lumpName) == -1)
                {
                    throw new Exception("Demo '" + demoName + "' was not found!");
                }
                _demo = new Demo(content.Wad.ReadLump(lumpName));
            }

            _demo.Options.GameVersion = options.GameVersion;
            _demo.Options.GameMode = options.GameMode;
            _demo.Options.MissionPack = options.MissionPack;
            _demo.Options.Video = options.Video;
            _demo.Options.Sound = options.Sound;
            _demo.Options.Music = options.Music;

            if (args.solonet.Present)
            {
                _demo.Options.NetGame = true;
            }

            _cmds = new TicCmd[Player.MaxPlayerCount];
            for (int i = 0; i < Player.MaxPlayerCount; i++)
            {
                _cmds[i] = new TicCmd();
            }

            _game = new DoomGame(content, _demo.Options);
            _game.DeferedInitNew();

            _stopwatch = new Stopwatch();
        }

        public UpdateResult Update()
        {
            if (!_stopwatch.IsRunning)
            {
                _stopwatch.Start();
            }

            if (!_demo.ReadCmd(_cmds))
            {
                _stopwatch.Stop();
                return UpdateResult.Completed;
            }
            else
            {
                _frameCount++;
                return _game.Update(_cmds);
            }
        }

        public void DoEvent(DoomEvent e)
        {
            _game.DoEvent(e);
        }

        public DoomGame Game => _game;
        public double Fps => _frameCount / _stopwatch.Elapsed.TotalSeconds;
    }
}

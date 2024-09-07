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
using System.Runtime.InteropServices;

namespace ManagedDoom.Video
{
    public sealed class Renderer
    {
        private static readonly double[] _gammaCorrectionParameters =
        [
            1.00,
            0.95,
            0.90,
            0.85,
            0.80,
            0.75,
            0.70,
            0.65,
            0.60,
            0.55,
            0.50
        ];

        private readonly Config _config;

        private readonly Palette _palette;

        private readonly DrawScreen _screen;

        private readonly MenuRenderer _menu;
        private readonly ThreeDRenderer _threeD;
        private readonly StatusBarRenderer _statusBar;
        private readonly IntermissionRenderer _intermission;
        private readonly OpeningSequenceRenderer _openingSequence;
        private readonly AutoMapRenderer _autoMap;
        private readonly FinaleRenderer _finale;

        private readonly Patch _pause;

        private readonly int _wipeBandWidth;
        private readonly int _wipeBandCount;
        private readonly int _wipeHeight;
        private readonly byte[] _wipeBuffer;

        public Renderer(Config config, GameContent content)
        {
            this._config = config;

            _palette = content.Palette;

            if (config.video_highresolution)
            {
                _screen = new DrawScreen(content.Wad, 640, 400);
            }
            else
            {
                _screen = new DrawScreen(content.Wad, 320, 200);
            }

            config.video_gamescreensize = Math.Clamp(config.video_gamescreensize, 0, MaxWindowSize);
            config.video_gammacorrection = Math.Clamp(config.video_gammacorrection, 0, MaxGammaCorrectionLevel);

            _menu = new MenuRenderer(content.Wad, _screen);
            _threeD = new ThreeDRenderer(content, _screen, config.video_gamescreensize);
            _statusBar = new StatusBarRenderer(content.Wad, _screen);
            _intermission = new IntermissionRenderer(content.Wad, _screen);
            _openingSequence = new OpeningSequenceRenderer(content.Wad, _screen, this);
            _autoMap = new AutoMapRenderer(content.Wad, _screen);
            _finale = new FinaleRenderer(content, _screen);

            _pause = Patch.FromWad(content.Wad, "M_PAUSE");

            int scale = _screen.Width / 320;
            _wipeBandWidth = 2 * scale;
            _wipeBandCount = _screen.Width / _wipeBandWidth + 1;
            _wipeHeight = _screen.Height / scale;
            _wipeBuffer = new byte[_screen.Data.Length];

            _palette.ResetColors(_gammaCorrectionParameters[config.video_gammacorrection]);
        }

        public void RenderDoom(Doom doom, Fixed frameFrac)
        {
            if (doom.State == DoomState.Opening)
            {
                _openingSequence.Render(doom.Opening, frameFrac);
            }
            else if (doom.State == DoomState.DemoPlayback)
            {
                RenderGame(doom.DemoPlayback.Game, frameFrac);
            }
            else if (doom.State == DoomState.Game)
            {
                RenderGame(doom.Game, frameFrac);
            }

            if (!doom.Menu.Active)
            {
                if (doom.State == DoomState.Game &&
                    doom.Game.State == GameState.Level &&
                    doom.Game.Paused)
                {
                    int scale = _screen.Width / 320;
                    _screen.DrawPatch(
                        _pause,
                        (_screen.Width - scale * _pause.Width) / 2,
                        4 * scale,
                        scale);
                }
            }
        }

        public void RenderMenu(Doom doom)
        {
            if (doom.Menu.Active)
            {
                _menu.Render(doom.Menu);
            }
        }

        public void RenderGame(DoomGame game, Fixed frameFrac)
        {
            if (game.Paused)
            {
                frameFrac = Fixed.One;
            }

            if (game.State == GameState.Level)
            {
                Player consolePlayer = game.World.ConsolePlayer;
                Player displayPlayer = game.World.DisplayPlayer;

                if (game.World.AutoMap.Visible)
                {
                    _autoMap.Render(consolePlayer);
                    _statusBar.Render(consolePlayer, true);
                }
                else
                {
                    _threeD.Render(displayPlayer, frameFrac);
                    if (_threeD.WindowSize < 8)
                    {
                        _statusBar.Render(consolePlayer, true);
                    }
                    else if (_threeD.WindowSize == ThreeDRenderer.MaxScreenSize)
                    {
                        _statusBar.Render(consolePlayer, false);
                    }
                }

                if (_config.video_displaymessage || ReferenceEquals(consolePlayer.Message, (string)DoomInfo.Strings.MSGOFF))
                {
                    if (consolePlayer.MessageTime > 0)
                    {
                        int scale = _screen.Width / 320;
                        _screen.DrawText(consolePlayer.Message, 0, 7 * scale, scale);
                    }
                }
            }
            else if (game.State == GameState.Intermission)
            {
                _intermission.Render(game.Intermission);
            }
            else if (game.State == GameState.Finale)
            {
                _finale.Render(game.Finale);
            }
        }

        public void Render(Doom doom, byte[] destination, Fixed frameFrac)
        {
            if (doom.Wiping)
            {
                RenderWipe(doom, destination);
                return;
            }

            RenderDoom(doom, frameFrac);
            RenderMenu(doom);

            uint[] colors = _palette[0];
            if (doom.State == DoomState.Game &&
                doom.Game.State == GameState.Level)
            {
                colors = _palette[GetPaletteNumber(doom.Game.World.ConsolePlayer)];
            }
            else if (doom.State == DoomState.Opening &&
                doom.Opening.State == OpeningSequenceState.Demo &&
                doom.Opening.DemoGame.State == GameState.Level)
            {
                colors = _palette[GetPaletteNumber(doom.Opening.DemoGame.World.ConsolePlayer)];
            }
            else if (doom.State == DoomState.DemoPlayback &&
                doom.DemoPlayback.Game.State == GameState.Level)
            {
                colors = _palette[GetPaletteNumber(doom.DemoPlayback.Game.World.ConsolePlayer)];
            }

            WriteData(colors, destination);
        }

        private void RenderWipe(Doom doom, byte[] destination)
        {
            RenderDoom(doom, Fixed.One);

            WipeEffect wipe = doom.WipeEffect;
            int scale = _screen.Width / 320;
            for (int i = 0; i < _wipeBandCount - 1; i++)
            {
                int x1 = _wipeBandWidth * i;
                int x2 = x1 + _wipeBandWidth;
                int y1 = Math.Max(scale * wipe.Y[i], 0);
                int y2 = Math.Max(scale * wipe.Y[i + 1], 0);
                float dy = (float)(y2 - y1) / _wipeBandWidth;
                for (int x = x1; x < x2; x++)
                {
                    int y = (int)MathF.Round(y1 + dy * ((x - x1) / 2 * 2));
                    int copyLength = _screen.Height - y;
                    if (copyLength > 0)
                    {
                        int srcPos = _screen.Height * x;
                        int dstPos = _screen.Height * x + y;
                        Array.Copy(_wipeBuffer, srcPos, _screen.Data, dstPos, copyLength);
                    }
                }
            }

            RenderMenu(doom);

            WriteData(_palette[0], destination);
        }

        public void InitializeWipe()
        {
            Array.Copy(_screen.Data, _wipeBuffer, _screen.Data.Length);
        }

        private void WriteData(uint[] colors, byte[] destination)
        {
            byte[] screenData = _screen.Data;
            Span<uint> p = MemoryMarshal.Cast<byte, uint>(destination);
            for (int i = 0; i < p.Length; i++)
            {
                p[i] = colors[screenData[i]];
            }
        }

        private static int GetPaletteNumber(Player player)
        {
            int count = player.DamageCount;

            if (player.Powers[(int)PowerType.Strength] != 0)
            {
                // Slowly fade the berzerk out.
                int bzc = 12 - (player.Powers[(int)PowerType.Strength] >> 6);
                if (bzc > count)
                {
                    count = bzc;
                }
            }

            int palette;

            if (count != 0)
            {
                palette = (count + 7) >> 3;

                if (palette >= Palette.DamageCount)
                {
                    palette = Palette.DamageCount - 1;
                }

                palette += Palette.DamageStart;
            }
            else if (player.BonusCount != 0)
            {
                palette = (player.BonusCount + 7) >> 3;

                if (palette >= Palette.BonusCount)
                {
                    palette = Palette.BonusCount - 1;
                }

                palette += Palette.BonusStart;
            }
            else if (player.Powers[(int)PowerType.IronFeet] > 4 * 32 ||
                (player.Powers[(int)PowerType.IronFeet] & 8) != 0)
            {
                palette = Palette.IronFeet;
            }
            else
            {
                palette = 0;
            }

            return palette;
        }

        public int Width => _screen.Width;
        public int Height => _screen.Height;

        public int WipeBandCount => _wipeBandCount;
        public int WipeHeight => _wipeHeight;

        public int MaxWindowSize
        {
            get
            {
                return ThreeDRenderer.MaxScreenSize;
            }
        }

        public int WindowSize
        {
            get
            {
                return _threeD.WindowSize;
            }

            set
            {
                _config.video_gamescreensize = value;
                _threeD.WindowSize = value;
            }
        }

        public bool DisplayMessage
        {
            get
            {
                return _config.video_displaymessage;
            }

            set
            {
                _config.video_displaymessage = value;
            }
        }

        public int MaxGammaCorrectionLevel
        {
            get
            {
                return _gammaCorrectionParameters.Length - 1;
            }
        }

        public int GammaCorrectionLevel
        {
            get
            {
                return _config.video_gammacorrection;
            }

            set
            {
                _config.video_gammacorrection = value;
                _palette.ResetColors(_gammaCorrectionParameters[_config.video_gammacorrection]);
            }
        }
    }
}

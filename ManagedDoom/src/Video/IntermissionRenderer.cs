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

namespace ManagedDoom.Video
{
    public sealed class IntermissionRenderer
    {
        // GLOBAL LOCATIONS
        private static readonly int _titleY = 2;
        private static readonly int _spacingY = 33;

        // SINGPLE-PLAYER STUFF
        private static readonly int _spStatsX = 50;
        private static readonly int _spStatsY = 50;
        private static readonly int _spTimeX = 16;
        private static readonly int _spTimeY = 200 - 32;

        // NET GAME STUFF
        private static readonly int _ngStatsY = 50;
        private static readonly int _ngSpacingX = 64;

        // DEATHMATCH STUFF
        private static readonly int _dmMatrixX = 42;
        private static readonly int _dmMatrixY = 68;
        private static readonly int _dmSpacingX = 40;
        private static readonly int _dmTotalsX = 269;
        private static readonly int _dmKillersX = 10;
        private static readonly int _dmKillersY = 100;
        private static readonly int _dmVictimsX = 5;
        private static readonly int _dmVictimsY = 50;

        private static readonly string[] _mapPictures =
        [
            "WIMAP0",
            "WIMAP1",
            "WIMAP2"
        ];

        private static readonly string[] _playerBoxes =
        [
            "STPB0",
            "STPB1",
            "STPB2",
            "STPB3"
        ];

        private static readonly string[] _youAreHere =
        [
            "WIURH0",
            "WIURH1"
        ];

        private static readonly string[][] _doomLevels;
        private static readonly string[] _doom2Levels;

        static IntermissionRenderer()
        {
            _doomLevels = new string[4][];
            for (int e = 0; e < 4; e++)
            {
                _doomLevels[e] = new string[9];
                for (int m = 0; m < 9; m++)
                {
                    _doomLevels[e][m] = "WILV" + e + m;
                }
            }

            _doom2Levels = new string[32];
            for (int m = 0; m < 32; m++)
            {
                _doom2Levels[m] = "CWILV" + m.ToString("00");
            }
        }


        private readonly DrawScreen _screen;

        private readonly PatchCache _cache;

        private readonly Patch _minus;
        private readonly Patch[] _numbers;
        private readonly Patch _percent;
        private readonly Patch _colon;

        private readonly int _scale;

        public IntermissionRenderer(Wad wad, DrawScreen screen)
        {
            this._screen = screen;

            _cache = new PatchCache(wad);

            _minus = Patch.FromWad(wad, "WIMINUS");
            _numbers = new Patch[10];
            for (int i = 0; i < 10; i++)
            {
                _numbers[i] = Patch.FromWad(wad, "WINUM" + i);
            }
            _percent = Patch.FromWad(wad, "WIPCNT");
            _colon = Patch.FromWad(wad, "WICOLON");

            _scale = screen.Width / 320;
        }


        private void DrawPatch(Patch patch, int x, int y)
        {
            _screen.DrawPatch(patch, _scale * x, _scale * y, _scale);
        }

        private void DrawPatch(string name, int x, int y)
        {
            int scale = _screen.Width / 320;
            _screen.DrawPatch(_cache[name], scale * x, scale * y, scale);
        }

        private int GetWidth(string name)
        {
            return _cache.GetWidth(name);
        }

        private int GetHeight(string name)
        {
            return _cache.GetHeight(name);
        }


        public void Render(Intermission im)
        {
            switch (im.State)
            {
                case IntermissionState.StatCount:
                    if (im.Options.Deathmatch != 0)
                    {
                        DrawDeathmatchStats(im);
                    }
                    else if (im.Options.NetGame)
                    {
                        DrawNetGameStats(im);
                    }
                    else
                    {
                        DrawSinglePlayerStats(im);
                    }
                    break;

                case IntermissionState.ShowNextLoc:
                    DrawShowNextLoc(im);
                    break;

                case IntermissionState.NoState:
                    DrawNoState(im);
                    break;
            }
        }


        private void DrawBackground(Intermission im)
        {
            if (im.Options.GameMode == GameMode.Commercial)
            {
                DrawPatch("INTERPIC", 0, 0);
            }
            else
            {
                int e = im.Options.Episode - 1;
                if (e < _mapPictures.Length)
                {
                    DrawPatch(_mapPictures[e], 0, 0);
                }
                else
                {
                    DrawPatch("INTERPIC", 0, 0);
                }
            }
        }

        private void DrawSinglePlayerStats(Intermission im)
        {
            DrawBackground(im);

            // Draw animated background.
            DrawBackgroundAnimation(im);

            // Draw level name.
            DrawFinishedLevelName(im);

            // Line height.
            int lineHeight = (3 * _numbers[0].Height) / 2;

            DrawPatch(
                "WIOSTK", // KILLS
                _spStatsX,
                _spStatsY);

            DrawPercent(
                320 - _spStatsX,
                _spStatsY,
                im.KillCount[0]);

            DrawPatch(
                "WIOSTI", // ITEMS
                _spStatsX,
                _spStatsY + lineHeight);

            DrawPercent(
                320 - _spStatsX,
                _spStatsY + lineHeight,
                im.ItemCount[0]);

            DrawPatch(
                "WISCRT2", // SECRET
                _spStatsX,
                _spStatsY + 2 * lineHeight);

            DrawPercent(
                320 - _spStatsX,
                _spStatsY + 2 * lineHeight,
                im.SecretCount[0]);

            DrawPatch(
                "WITIME", // TIME
                _spTimeX,
                _spTimeY);

            DrawTime(
                320 / 2 - _spTimeX,
                _spTimeY,
                im.TimeCount);

            if (im.Info.Episode < 3)
            {

                DrawPatch(
                    "WIPAR", // PAR
                    320 / 2 + _spTimeX,
                    _spTimeY);

                DrawTime(
                    320 - _spTimeX,
                    _spTimeY,
                    im.ParCount);
            }
        }

        private void DrawNetGameStats(Intermission im)
        {
            DrawBackground(im);

            // Draw animated background.
            DrawBackgroundAnimation(im);

            // Draw level name.
            DrawFinishedLevelName(im);

            int ngStatsX = 32 + GetWidth("STFST01") / 2;
            if (!im.DoFrags)
            {
                ngStatsX += 32;
            }

            // Draw stat titles (top line).
            DrawPatch(
                "WIOSTK", // KILLS
                ngStatsX + _ngSpacingX - GetWidth("WIOSTK"),
                _ngStatsY);

            DrawPatch(
                "WIOSTI", // ITEMS
                ngStatsX + 2 * _ngSpacingX - GetWidth("WIOSTI"),
                _ngStatsY);

            DrawPatch(
                "WIOSTS", // SCRT
                ngStatsX + 3 * _ngSpacingX - GetWidth("WIOSTS"),
                _ngStatsY);

            if (im.DoFrags)
            {
                DrawPatch(
                    "WIFRGS", // FRAGS
                    ngStatsX + 4 * _ngSpacingX - GetWidth("WIFRGS"),
                    _ngStatsY);
            }

            // Draw stats.
            int y = _ngStatsY + GetHeight("WIOSTK");

            for (int i = 0; i < Player.MaxPlayerCount; i++)
            {
                if (!im.Options.Players[i].InGame)
                {
                    continue;
                }

                int x = ngStatsX;

                DrawPatch(
                    _playerBoxes[i],
                    x - GetWidth(_playerBoxes[i]),
                    y);

                if (i == im.Options.ConsolePlayer)
                {
                    DrawPatch(
                        "STFST01", // Player face
                        x - GetWidth(_playerBoxes[i]),
                        y);
                }

                x += _ngSpacingX;

                DrawPercent(x - _percent.Width, y + 10, im.KillCount[i]);
                x += _ngSpacingX;

                DrawPercent(x - _percent.Width, y + 10, im.ItemCount[i]);
                x += _ngSpacingX;

                DrawPercent(x - _percent.Width, y + 10, im.SecretCount[i]);
                x += _ngSpacingX;

                if (im.DoFrags)
                {
                    DrawNumber(x, y + 10, im.FragCount[i], -1);
                }

                y += _spacingY;
            }
        }

        private void DrawDeathmatchStats(Intermission im)
        {
            DrawBackground(im);

            // Draw animated background.
            DrawBackgroundAnimation(im);

            // Draw level name.
            DrawFinishedLevelName(im);

            // Draw stat titles (top line).
            DrawPatch(
                "WIMSTT", // TOTAL
                _dmTotalsX - GetWidth("WIMSTT") / 2,
                _dmMatrixY - _spacingY + 10);

            DrawPatch(
                "WIKILRS", // KILLERS
                _dmKillersX,
                _dmKillersY);

            DrawPatch(
                "WIVCTMS", // VICTIMS
                _dmVictimsX,
                _dmVictimsY);

            // Draw player boxes.
            int x = _dmMatrixX + _dmSpacingX;
            int y = _dmMatrixY;

            for (int i = 0; i < Player.MaxPlayerCount; i++)
            {
                if (im.Options.Players[i].InGame)
                {
                    DrawPatch(
                        _playerBoxes[i],
                        x - GetWidth(_playerBoxes[i]) / 2,
                        _dmMatrixY - _spacingY);

                    DrawPatch(
                        _playerBoxes[i],
                        _dmMatrixX - GetWidth(_playerBoxes[i]) / 2,
                        y);

                    if (i == im.Options.ConsolePlayer)
                    {
                        DrawPatch(
                            "STFDEAD0", // Player face (dead)
                            x - GetWidth(_playerBoxes[i]) / 2,
                            _dmMatrixY - _spacingY);

                        DrawPatch(
                            "STFST01", // Player face
                            _dmMatrixX - GetWidth(_playerBoxes[i]) / 2,
                            y);
                    }
                }
                else
                {
                    // V_DrawPatch(x-SHORT(bp[i]->width)/2,
                    //   DM_MATRIXY - WI_SPACINGY, FB, bp[i]);
                    // V_DrawPatch(DM_MATRIXX-SHORT(bp[i]->width)/2,
                    //   y, FB, bp[i]);
                }

                x += _dmSpacingX;
                y += _spacingY;
            }

            // Draw stats.
            y = _dmMatrixY + 10;
            int w = _numbers[0].Width;

            for (int i = 0; i < Player.MaxPlayerCount; i++)
            {
                x = _dmMatrixX + _dmSpacingX;

                if (im.Options.Players[i].InGame)
                {
                    for (int j = 0; j < Player.MaxPlayerCount; j++)
                    {
                        if (im.Options.Players[j].InGame)
                        {
                            DrawNumber(x + w, y, im.DeathmatchFrags[i][j], 2);
                        }

                        x += _dmSpacingX;
                    }

                    DrawNumber(_dmTotalsX + w, y, im.DeathmatchTotals[i], 2);
                }

                y += _spacingY;
            }
        }


        private void DrawNoState(Intermission im)
        {
            DrawShowNextLoc(im);
        }

        private void DrawShowNextLoc(Intermission im)
        {
            DrawBackground(im);

            // Draw animated background.
            DrawBackgroundAnimation(im);

            if (im.Options.GameMode != GameMode.Commercial)
            {
                if (im.Info.Episode > 2)
                {
                    DrawEnteringLevelName(im);
                    return;
                }

                int last = (im.Info.LastLevel == 8) ? im.Info.NextLevel - 1 : im.Info.LastLevel;

                // Draw a splat on taken cities.
                for (int i = 0; i <= last; i++)
                {
                    int x = WorldMap.Locations[im.Info.Episode][i].X;
                    int y = WorldMap.Locations[im.Info.Episode][i].Y;
                    DrawPatch("WISPLAT", x, y);
                }

                // Splat the secret level?
                if (im.Info.DidSecret)
                {
                    int x = WorldMap.Locations[im.Info.Episode][8].X;
                    int y = WorldMap.Locations[im.Info.Episode][8].Y;
                    DrawPatch("WISPLAT", x, y);
                }

                // Draw "you are here".
                if (im.ShowYouAreHere)
                {
                    int x = WorldMap.Locations[im.Info.Episode][im.Info.NextLevel].X;
                    int y = WorldMap.Locations[im.Info.Episode][im.Info.NextLevel].Y;
                    DrawSuitablePatch(_youAreHere, x, y);
                }
            }

            // Draw next level name.
            if ((im.Options.GameMode != GameMode.Commercial) || im.Info.NextLevel != 30)
            {
                DrawEnteringLevelName(im);
            }
        }

        private void DrawFinishedLevelName(Intermission intermission)
        {
            IntermissionInfo wbs = intermission.Info;
            int y = _titleY;

            string levelName;
            if (intermission.Options.GameMode != GameMode.Commercial)
            {
                int e = intermission.Options.Episode - 1;
                levelName = _doomLevels[e][wbs.LastLevel];
            }
            else
            {
                levelName = _doom2Levels[wbs.LastLevel];
            }

            // Draw level name. 
            DrawPatch(
                levelName,
                (320 - GetWidth(levelName)) / 2,
                y);

            // Draw "Finished!".
            y += (5 * GetHeight(levelName)) / 4;

            DrawPatch(
                "WIF",
                (320 - GetWidth("WIF")) / 2,
                y);
        }

        private void DrawEnteringLevelName(Intermission im)
        {
            IntermissionInfo wbs = im.Info;
            int y = _titleY;

            string levelName;
            if (im.Options.GameMode != GameMode.Commercial)
            {
                int e = im.Options.Episode - 1;
                levelName = _doomLevels[e][wbs.NextLevel];
            }
            else
            {
                levelName = _doom2Levels[wbs.NextLevel];
            }

            // Draw "Entering".
            DrawPatch(
                "WIENTER",
                (320 - GetWidth("WIENTER")) / 2,
                y);

            // Draw level name.
            y += (5 * GetHeight(levelName)) / 4;

            DrawPatch(
                levelName,
                (320 - GetWidth(levelName)) / 2,
                y);
        }


        private int DrawNumber(int x, int y, int n, int digits)
        {
            if (digits < 0)
            {
                if (n == 0)
                {
                    // Make variable-length zeros 1 digit long.
                    digits = 1;
                }
                else
                {
                    // Figure out number of digits.
                    digits = 0;
                    int temp = n;
                    while (temp != 0)
                    {
                        temp /= 10;
                        digits++;
                    }
                }
            }

            bool neg = n < 0;
            if (neg)
            {
                n = -n;
            }

            // If non-number, do not draw it.
            if (n == 1994)
            {
                return 0;
            }

            int fontWidth = _numbers[0].Width;

            // Draw the new number.
            while (digits-- != 0)
            {
                x -= fontWidth;
                DrawPatch(_numbers[n % 10], x, y);
                n /= 10;
            }

            // Draw a minus sign if necessary.
            if (neg)
            {
                DrawPatch(_minus, x -= 8, y);
            }

            return x;
        }

        private void DrawPercent(int x, int y, int p)
        {
            if (p < 0)
            {
                return;
            }

            DrawPatch(_percent, x, y);
            DrawNumber(x, y, p, -1);
        }

        private void DrawTime(int x, int y, int t)
        {
            if (t < 0)
            {
                return;
            }

            if (t <= 61 * 59)
            {
                int div = 1;

                do
                {
                    int n = (t / div) % 60;
                    x = DrawNumber(x, y, n, 2) - _colon.Width;
                    div *= 60;

                    // Draw.
                    if (div == 60 || t / div != 0)
                    {
                        DrawPatch(_colon, x, y);
                    }
                }
                while (t / div != 0);
            }
            else
            {
                DrawPatch(
                    "WISUCKS", // SUCKS
                    x - GetWidth("WISUCKS"),
                    y);
            }
        }

        private void DrawBackgroundAnimation(Intermission im)
        {
            if (im.Options.GameMode == GameMode.Commercial)
            {
                return;
            }

            if (im.Info.Episode > 2)
            {
                return;
            }

            for (int i = 0; i < im.Animations.Length; i++)
            {
                Animation a = im.Animations[i];
                if (a.PatchNumber >= 0)
                {
                    DrawPatch(a.Patches[a.PatchNumber], a.LocationX, a.LocationY);
                }
            }
        }

        private void DrawSuitablePatch(string[] candidates, int x, int y)
        {
            bool fits = false;
            int i = 0;

            do
            {
                Patch patch = _cache[candidates[i]];

                int left = x - patch.LeftOffset;
                int top = y - patch.TopOffset;
                int right = left + patch.Width;
                int bottom = top + patch.Height;

                if (left >= 0 && right < 320 && top >= 0 && bottom < 320)
                {
                    fits = true;
                }
                else
                {
                    i++;
                }
            }
            while (!fits && i != 2);

            if (fits && i < 2)
            {
                DrawPatch(candidates[i], x, y);
            }
            else
            {
                throw new Exception("Could not place patch!");
            }
        }
    }
}

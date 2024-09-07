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

namespace ManagedDoom
{
    public class Specials
    {
        private static readonly int _maxButtonCount = 32;
        private static readonly int _buttonTime = 35;

        private readonly World _world;

        private bool _levelTimer;
        private int _levelTimeCount;

        private readonly Button[] _buttonList;

        private readonly int[] _textureTranslation;
        private readonly int[] _flatTranslation;

        private LineDef[] _scrollLines;

        public Specials(World world)
        {
            _world = world;

            _levelTimer = false;

            _buttonList = new Button[_maxButtonCount];
            for (int i = 0; i < _buttonList.Length; i++)
            {
                _buttonList[i] = new Button();
            }

            _textureTranslation = new int[world.Map.Textures.Count];
            for (int i = 0; i < _textureTranslation.Length; i++)
            {
                _textureTranslation[i] = i;
            }

            _flatTranslation = new int[world.Map.Flats.Count];
            for (int i = 0; i < _flatTranslation.Length; i++)
            {
                _flatTranslation[i] = i;
            }
        }

        /// <summary>
        /// After the map has been loaded, scan for specials that spawn thinkers.
        /// </summary>
        public void SpawnSpecials(int levelTimeCount)
        {
            _levelTimer = true;
            _levelTimeCount = levelTimeCount;
            SpawnSpecials();
        }

        /// <summary>
        /// After the map has been loaded, scan for specials that spawn thinkers.
        /// </summary>
        public void SpawnSpecials()
        {
            // Init special sectors.
            LightingChange lc = _world.LightingChange;
            SectorAction sa = _world.SectorAction;
            foreach (Sector sector in _world.Map.Sectors)
            {
                if (sector.Special == 0)
                {
                    continue;
                }

                switch ((int)sector.Special)
                {
                    case 1:
                        // Flickering lights.
                        lc.SpawnLightFlash(sector);
                        break;

                    case 2:
                        // Strobe fast.
                        lc.SpawnStrobeFlash(sector, StrobeFlash.FastDark, false);
                        break;

                    case 3:
                        // Strobe slow.
                        lc.SpawnStrobeFlash(sector, StrobeFlash.SlowDark, false);
                        break;

                    case 4:
                        // Strobe fast / death slime.
                        lc.SpawnStrobeFlash(sector, StrobeFlash.FastDark, false);
                        sector.Special = (SectorSpecial)4;
                        break;

                    case 8:
                        // Glowing light.
                        lc.SpawnGlowingLight(sector);
                        break;
                    case 9:
                        // Secret sector.
                        _world.TotalSecrets++;
                        break;

                    case 10:
                        // Door close in 30 seconds.
                        sa.SpawnDoorCloseIn30(sector);
                        break;

                    case 12:
                        // Sync strobe slow.
                        lc.SpawnStrobeFlash(sector, StrobeFlash.SlowDark, true);
                        break;

                    case 13:
                        // Sync strobe fast.
                        lc.SpawnStrobeFlash(sector, StrobeFlash.FastDark, true);
                        break;

                    case 14:
                        // Door raise in 5 minutes.
                        sa.SpawnDoorRaiseIn5Mins(sector);
                        break;

                    case 17:
                        lc.SpawnFireFlicker(sector);
                        break;
                }
            }

            var scrollList = new List<LineDef>();
            foreach (LineDef line in _world.Map.Lines)
            {
                switch ((int)line.Special)
                {
                    case 48:
                        // Texture scroll.
                        scrollList.Add(line);
                        break;
                }
            }
            _scrollLines = scrollList.ToArray();
        }

        public void ChangeSwitchTexture(LineDef line, bool useAgain)
        {
            if (!useAgain)
            {
                line.Special = 0;
            }

            SideDef? frontSide = line.FrontSide;
            int topTexture = frontSide.TopTexture;
            int middleTexture = frontSide.MiddleTexture;
            int bottomTexture = frontSide.BottomTexture;

            Sfx sound = Sfx.SWTCHN;

            // Exit switch?
            if ((int)line.Special == 11)
            {
                sound = Sfx.SWTCHX;
            }

            int[] switchList = _world.Map.Textures.SwitchList;

            for (int i = 0; i < switchList.Length; i++)
            {
                if (switchList[i] == topTexture)
                {
                    _world.StartSound(line.SoundOrigin, sound, SfxType.Misc);
                    frontSide.TopTexture = switchList[i ^ 1];

                    if (useAgain)
                    {
                        StartButton(line, ButtonPosition.Top, switchList[i], _buttonTime);
                    }

                    return;
                }
                else
                {
                    if (switchList[i] == middleTexture)
                    {
                        _world.StartSound(line.SoundOrigin, sound, SfxType.Misc);
                        frontSide.MiddleTexture = switchList[i ^ 1];

                        if (useAgain)
                        {
                            StartButton(line, ButtonPosition.Middle, switchList[i], _buttonTime);
                        }

                        return;
                    }
                    else
                    {
                        if (switchList[i] == bottomTexture)
                        {
                            _world.StartSound(line.SoundOrigin, sound, SfxType.Misc);
                            frontSide.BottomTexture = switchList[i ^ 1];

                            if (useAgain)
                            {
                                StartButton(line, ButtonPosition.Bottom, switchList[i], _buttonTime);
                            }

                            return;
                        }
                    }
                }
            }
        }

        private void StartButton(LineDef line, ButtonPosition w, int texture, int time)
        {
            // See if button is already pressed.
            for (int i = 0; i < _maxButtonCount; i++)
            {
                if (_buttonList[i].Timer != 0 && _buttonList[i].Line == line)
                {
                    return;
                }
            }

            for (int i = 0; i < _maxButtonCount; i++)
            {
                if (_buttonList[i].Timer == 0)
                {
                    _buttonList[i].Line = line;
                    _buttonList[i].Position = w;
                    _buttonList[i].Texture = texture;
                    _buttonList[i].Timer = time;
                    _buttonList[i].SoundOrigin = line.SoundOrigin;
                    return;
                }
            }

            throw new Exception("No button slots left!");
        }

        /// <summary>
        /// Animate planes, scroll walls, etc.
        /// </summary>
        public void Update()
        {
            // Level timer.
            if (_levelTimer)
            {
                _levelTimeCount--;
                if (_levelTimeCount == 0)
                {
                    _world.ExitLevel();
                }
            }

            // Animate flats and textures globally.
            TextureAnimationInfo[] animations = _world.Map.Animation.Animations;
            for (int k = 0; k < animations.Length; k++)
            {
                TextureAnimationInfo anim = animations[k];
                for (int i = anim.BasePic; i < anim.BasePic + anim.NumPics; i++)
                {
                    int pic = anim.BasePic + ((_world.LevelTime / anim.Speed + i) % anim.NumPics);
                    if (anim.IsTexture)
                    {
                        _textureTranslation[i] = pic;
                    }
                    else
                    {
                        _flatTranslation[i] = pic;
                    }
                }
            }

            // Animate line specials.
            foreach (LineDef line in _scrollLines)
            {
                line.FrontSide.TextureOffset += Fixed.One;
            }

            // Do buttons.
            for (int i = 0; i < _maxButtonCount; i++)
            {
                if (_buttonList[i].Timer > 0)
                {
                    _buttonList[i].Timer--;

                    if (_buttonList[i].Timer == 0)
                    {
                        switch (_buttonList[i].Position)
                        {
                            case ButtonPosition.Top:
                                _buttonList[i].Line.FrontSide.TopTexture = _buttonList[i].Texture;
                                break;

                            case ButtonPosition.Middle:
                                _buttonList[i].Line.FrontSide.MiddleTexture = _buttonList[i].Texture;
                                break;

                            case ButtonPosition.Bottom:
                                _buttonList[i].Line.FrontSide.BottomTexture = _buttonList[i].Texture;
                                break;
                        }

                        _world.StartSound(_buttonList[i].SoundOrigin, Sfx.SWTCHN, SfxType.Misc, 50);
                        _buttonList[i].Clear();
                    }
                }
            }
        }

        public int[] TextureTranslation => _textureTranslation;
        public int[] FlatTranslation => _flatTranslation;
    }
}

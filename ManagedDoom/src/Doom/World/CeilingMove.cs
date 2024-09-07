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
    public sealed class CeilingMove : Thinker
    {
        private readonly World _world;

        private CeilingMoveType _type;
        private Sector _sector;
        private Fixed _bottomHeight;
        private Fixed _topHeight;
        private Fixed _speed;
        private bool _crush;

        // 1 = up, 0 = waiting, -1 = down.
        private int _direction;

        // Corresponding sector tag.
        private int _tag;

        private int _oldDirection;

        public CeilingMove(World world)
        {
            _world = world;
        }

        public override void Run()
        {
            SectorActionResult result;

            SectorAction sa = _world.SectorAction;

            switch (_direction)
            {
                case 0:
                    // In statis.
                    break;

                case 1:
                    // Up.
                    result = sa.MovePlane(
                        _sector,
                        _speed,
                        _topHeight,
                        false,
                        1,
                        _direction);

                    if (((_world.LevelTime + _sector.Number) & 7) == 0)
                    {
                        switch (_type)
                        {
                            case CeilingMoveType.SilentCrushAndRaise:
                                break;

                            default:
                                _world.StartSound(_sector.SoundOrigin, Sfx.STNMOV, SfxType.Misc);
                                break;
                        }
                    }

                    if (result == SectorActionResult.PastDestination)
                    {
                        switch (_type)
                        {
                            case CeilingMoveType.RaiseToHighest:
                                sa.RemoveActiveCeiling(this);
                                _sector.DisableFrameInterpolationForOneFrame();
                                break;

                            case CeilingMoveType.SilentCrushAndRaise:
                            case CeilingMoveType.FastCrushAndRaise:
                            case CeilingMoveType.CrushAndRaise:
                                if (_type == CeilingMoveType.SilentCrushAndRaise)
                                {
                                    _world.StartSound(_sector.SoundOrigin, Sfx.PSTOP, SfxType.Misc);
                                }
                                _direction = -1;
                                break;

                            default:
                                break;
                        }

                    }
                    break;

                case -1:
                    // Down.
                    result = sa.MovePlane(
                        _sector,
                        _speed,
                        _bottomHeight,
                        _crush,
                        1,
                        _direction);

                    if (((_world.LevelTime + _sector.Number) & 7) == 0)
                    {
                        switch (_type)
                        {
                            case CeilingMoveType.SilentCrushAndRaise:
                                break;

                            default:
                                _world.StartSound(_sector.SoundOrigin, Sfx.STNMOV, SfxType.Misc);
                                break;
                        }
                    }

                    if (result == SectorActionResult.PastDestination)
                    {
                        switch (_type)
                        {
                            case CeilingMoveType.SilentCrushAndRaise:
                            case CeilingMoveType.CrushAndRaise:
                            case CeilingMoveType.FastCrushAndRaise:
                                if (_type == CeilingMoveType.SilentCrushAndRaise)
                                {
                                    _world.StartSound(_sector.SoundOrigin, Sfx.PSTOP, SfxType.Misc);
                                    _speed = SectorAction.CeilingSpeed;
                                }
                                if (_type == CeilingMoveType.CrushAndRaise)
                                {
                                    _speed = SectorAction.CeilingSpeed;
                                }
                                _direction = 1;
                                break;

                            case CeilingMoveType.LowerAndCrush:
                            case CeilingMoveType.LowerToFloor:
                                sa.RemoveActiveCeiling(this);
                                _sector.DisableFrameInterpolationForOneFrame();
                                break;

                            default:
                                break;
                        }
                    }
                    else
                    {
                        if (result == SectorActionResult.Crushed)
                        {
                            switch (_type)
                            {
                                case CeilingMoveType.SilentCrushAndRaise:
                                case CeilingMoveType.CrushAndRaise:
                                case CeilingMoveType.LowerAndCrush:
                                    _speed = SectorAction.CeilingSpeed / 8;
                                    break;

                                default:
                                    break;
                            }
                        }
                    }
                    break;
            }
        }

        public CeilingMoveType Type
        {
            get => _type;
            set => _type = value;
        }

        public Sector Sector
        {
            get => _sector;
            set => _sector = value;
        }

        public Fixed BottomHeight
        {
            get => _bottomHeight;
            set => _bottomHeight = value;
        }

        public Fixed TopHeight
        {
            get => _topHeight;
            set => _topHeight = value;
        }

        public Fixed Speed
        {
            get => _speed;
            set => _speed = value;
        }

        public bool Crush
        {
            get => _crush;
            set => _crush = value;
        }

        public int Direction
        {
            get => _direction;
            set => _direction = value;
        }

        public int Tag
        {
            get => _tag;
            set => _tag = value;
        }

        public int OldDirection
        {
            get => _oldDirection;
            set => _oldDirection = value;
        }
    }
}

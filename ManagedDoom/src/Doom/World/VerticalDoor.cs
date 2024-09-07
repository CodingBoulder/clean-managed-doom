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
    public class VerticalDoor : Thinker
    {
        private readonly World _world;

        private VerticalDoorType _type;
        private Sector _sector;
        private Fixed _topHeight;
        private Fixed _speed;

        // 1 = up, 0 = waiting at top, -1 = down.
        private int _direction;

        // Tics to wait at the top.
        private int _topWait;

        // When it reaches 0, start going down
        // (keep in case a door going down is reset).
        private int _topCountDown;

        public VerticalDoor(World world)
        {
            _world = world;
        }

        public override void Run()
        {
            SectorAction sa = _world.SectorAction;

            SectorActionResult result;

            switch (_direction)
            {
                case 0:
                    // Waiting.
                    if (--_topCountDown == 0)
                    {
                        switch (_type)
                        {
                            case VerticalDoorType.BlazeRaise:
                                // Time to go back down.
                                _direction = -1;
                                _world.StartSound(_sector.SoundOrigin, Sfx.BDCLS, SfxType.Misc);
                                break;

                            case VerticalDoorType.Normal:
                                // Time to go back down.
                                _direction = -1;
                                _world.StartSound(_sector.SoundOrigin, Sfx.DORCLS, SfxType.Misc);
                                break;

                            case VerticalDoorType.Close30ThenOpen:
                                _direction = 1;
                                _world.StartSound(_sector.SoundOrigin, Sfx.DOROPN, SfxType.Misc);
                                break;

                            default:
                                break;
                        }
                    }
                    break;

                case 2:
                    // Initial wait.
                    if (--_topCountDown == 0)
                    {
                        switch (_type)
                        {
                            case VerticalDoorType.RaiseIn5Mins:
                                _direction = 1;
                                _type = VerticalDoorType.Normal;
                                _world.StartSound(_sector.SoundOrigin, Sfx.DOROPN, SfxType.Misc);
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
                        _sector.FloorHeight,
                        false, 1, _direction);
                    if (result == SectorActionResult.PastDestination)
                    {
                        switch (_type)
                        {
                            case VerticalDoorType.BlazeRaise:
                            case VerticalDoorType.BlazeClose:
                                _sector.SpecialData = null;
                                // Unlink and free.
                                _world.Thinkers.Remove(this);
                                _sector.DisableFrameInterpolationForOneFrame();
                                _world.StartSound(_sector.SoundOrigin, Sfx.BDCLS, SfxType.Misc);
                                break;

                            case VerticalDoorType.Normal:
                            case VerticalDoorType.Close:
                                _sector.SpecialData = null;
                                // Unlink and free.
                                _world.Thinkers.Remove(this);
                                _sector.DisableFrameInterpolationForOneFrame();
                                break;

                            case VerticalDoorType.Close30ThenOpen:
                                _direction = 0;
                                _topCountDown = 35 * 30;
                                break;

                            default:
                                break;
                        }
                    }
                    else if (result == SectorActionResult.Crushed)
                    {
                        switch (_type)
                        {
                            case VerticalDoorType.BlazeClose:
                            case VerticalDoorType.Close: // Do not go back up!
                                break;

                            default:
                                _direction = 1;
                                _world.StartSound(_sector.SoundOrigin, Sfx.DOROPN, SfxType.Misc);
                                break;
                        }
                    }
                    break;

                case 1:
                    // Up.
                    result = sa.MovePlane(
                        _sector,
                        _speed,
                        _topHeight,
                        false, 1, _direction);

                    if (result == SectorActionResult.PastDestination)
                    {
                        switch (_type)
                        {
                            case VerticalDoorType.BlazeRaise:
                            case VerticalDoorType.Normal:
                                // Wait at top.
                                _direction = 0;
                                _topCountDown = _topWait;
                                break;

                            case VerticalDoorType.Close30ThenOpen:
                            case VerticalDoorType.BlazeOpen:
                            case VerticalDoorType.Open:
                                _sector.SpecialData = null;
                                // Unlink and free.
                                _world.Thinkers.Remove(this);
                                _sector.DisableFrameInterpolationForOneFrame();
                                break;

                            default:
                                break;
                        }
                    }
                    break;
            }
        }

        public VerticalDoorType Type
        {
            get => _type;
            set => _type = value;
        }

        public Sector Sector
        {
            get => _sector;
            set => _sector = value;
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

        public int Direction
        {
            get => _direction;
            set => _direction = value;
        }

        public int TopWait
        {
            get => _topWait;
            set => _topWait = value;
        }

        public int TopCountDown
        {
            get => _topCountDown;
            set => _topCountDown = value;
        }
    }
}

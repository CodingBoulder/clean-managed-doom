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

namespace ManagedDoom
{
    public sealed class Platform : Thinker
    {
        private readonly World _world;

        private Sector _sector;
        private Fixed _speed;
        private Fixed _low;
        private Fixed _high;
        private int _wait;
        private int _count;
        private PlatformState _status;
        private PlatformState _oldStatus;
        private bool _crush;
        private int _tag;
        private PlatformType _type;

        public Platform(World world)
        {
            _world = world;
        }

        public override void Run()
        {
            SectorAction sa = _world.SectorAction;

            SectorActionResult result;

            switch (_status)
            {
                case PlatformState.Up:
                    result = sa.MovePlane(_sector, _speed, _high, _crush, 0, 1);

                    if (_type == PlatformType.RaiseAndChange ||
                        _type == PlatformType.RaiseToNearestAndChange)
                    {
                        if (((_world.LevelTime + _sector.Number) & 7) == 0)
                        {
                            _world.StartSound(_sector.SoundOrigin, Sfx.STNMOV, SfxType.Misc);
                        }
                    }

                    if (result == SectorActionResult.Crushed && !_crush)
                    {
                        _count = _wait;
                        _status = PlatformState.Down;
                        _world.StartSound(_sector.SoundOrigin, Sfx.PSTART, SfxType.Misc);
                    }
                    else
                    {
                        if (result == SectorActionResult.PastDestination)
                        {
                            _count = _wait;
                            _status = PlatformState.Waiting;
                            _world.StartSound(_sector.SoundOrigin, Sfx.PSTOP, SfxType.Misc);

                            switch (_type)
                            {
                                case PlatformType.BlazeDwus:
                                case PlatformType.DownWaitUpStay:
                                    sa.RemoveActivePlatform(this);
                                    _sector.DisableFrameInterpolationForOneFrame();
                                    break;

                                case PlatformType.RaiseAndChange:
                                case PlatformType.RaiseToNearestAndChange:
                                    sa.RemoveActivePlatform(this);
                                    _sector.DisableFrameInterpolationForOneFrame();
                                    break;

                                default:
                                    break;
                            }
                        }
                    }

                    break;

                case PlatformState.Down:
                    result = sa.MovePlane(_sector, _speed, _low, false, 0, -1);

                    if (result == SectorActionResult.PastDestination)
                    {
                        _count = _wait;
                        _status = PlatformState.Waiting;
                        _world.StartSound(_sector.SoundOrigin, Sfx.PSTOP, SfxType.Misc);
                    }

                    break;

                case PlatformState.Waiting:
                    if (--_count == 0)
                    {
                        if (_sector.FloorHeight == _low)
                        {
                            _status = PlatformState.Up;
                        }
                        else
                        {
                            _status = PlatformState.Down;
                        }
                        _world.StartSound(_sector.SoundOrigin, Sfx.PSTART, SfxType.Misc);
                    }

                    break;

                case PlatformState.InStasis:
                    break;
            }
        }

        public Sector Sector
        {
            get => _sector;
            set => _sector = value;
        }

        public Fixed Speed
        {
            get => _speed;
            set => _speed = value;
        }

        public Fixed Low
        {
            get => _low;
            set => _low = value;
        }

        public Fixed High
        {
            get => _high;
            set => _high = value;
        }

        public int Wait
        {
            get => _wait;
            set => _wait = value;
        }

        public int Count
        {
            get => _count;
            set => _count = value;
        }

        public PlatformState Status
        {
            get => _status;
            set => _status = value;
        }

        public PlatformState OldStatus
        {
            get => _oldStatus;
            set => _oldStatus = value;
        }

        public bool Crush
        {
            get => _crush;
            set => _crush = value;
        }

        public int Tag
        {
            get => _tag;
            set => _tag = value;
        }

        public PlatformType Type
        {
            get => _type;
            set => _type = value;
        }
    }
}

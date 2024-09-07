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
    public sealed class FloorMove : Thinker
    {
        private readonly World _world;

        private FloorMoveType _type;
        private bool _crush;
        private Sector _sector;
        private int _direction;
        private SectorSpecial _newSpecial;
        private int _texture;
        private Fixed _floorDestHeight;
        private Fixed _speed;

        public FloorMove(World world)
        {
            _world = world;
        }

        public override void Run()
        {
            SectorActionResult result;

            SectorAction sa = _world.SectorAction;

            result = sa.MovePlane(
                _sector,
                _speed,
                _floorDestHeight,
                _crush,
                0,
                _direction);

            if (((_world.LevelTime + _sector.Number) & 7) == 0)
            {
                _world.StartSound(_sector.SoundOrigin, Sfx.STNMOV, SfxType.Misc);
            }

            if (result == SectorActionResult.PastDestination)
            {
                _sector.SpecialData = null;

                if (_direction == 1)
                {
                    switch (_type)
                    {
                        case FloorMoveType.DonutRaise:
                            _sector.Special = _newSpecial;
                            _sector.FloorFlat = _texture;
                            break;
                    }
                }
                else if (_direction == -1)
                {
                    switch (_type)
                    {
                        case FloorMoveType.LowerAndChange:
                            _sector.Special = _newSpecial;
                            _sector.FloorFlat = _texture;
                            break;
                    }
                }

                _world.Thinkers.Remove(this);
                _sector.DisableFrameInterpolationForOneFrame();

                _world.StartSound(_sector.SoundOrigin, Sfx.PSTOP, SfxType.Misc);
            }
        }

        public FloorMoveType Type
        {
            get => _type;
            set => _type = value;
        }

        public bool Crush
        {
            get => _crush;
            set => _crush = value;
        }

        public Sector Sector
        {
            get => _sector;
            set => _sector = value;
        }

        public int Direction
        {
            get => _direction;
            set => _direction = value;
        }

        public SectorSpecial NewSpecial
        {
            get => _newSpecial;
            set => _newSpecial = value;
        }

        public int Texture
        {
            get => _texture;
            set => _texture = value;
        }

        public Fixed FloorDestHeight
        {
            get => _floorDestHeight;
            set => _floorDestHeight = value;
        }

        public Fixed Speed
        {
            get => _speed;
            set => _speed = value;
        }
    }
}

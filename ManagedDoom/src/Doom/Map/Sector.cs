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
using System.Collections;
using System.Collections.Generic;

namespace ManagedDoom
{
    public sealed class Sector
    {
        private static readonly int _dataSize = 26;

        private readonly int _number;
        private Fixed _floorHeight;
        private Fixed _ceilingHeight;
        private int _floorFlat;
        private int _ceilingFlat;
        private int _lightLevel;
        private SectorSpecial _special;
        private int _tag;

        // 0 = untraversed, 1, 2 = sndlines - 1.
        private int _soundTraversed;

        // Thing that made a sound (or null).
        private Mobj? _soundTarget;

        // Mapblock bounding box for height changes.
        private int[] _blockBox;

        // Origin for any sounds played by the sector.
        private Mobj _soundOrigin;

        // If == validcount, already checked.
        private int _validCount;

        // List of mobjs in sector.
        private Mobj? _thingList;

        // Thinker for reversable actions.
        private Thinker? _specialData;

        private LineDef[] _lines;

        // For frame interpolation.
        private Fixed _oldFloorHeight;
        private Fixed _oldCeilingHeight;

        public Sector(
            int number,
            Fixed floorHeight,
            Fixed ceilingHeight,
            int floorFlat,
            int ceilingFlat,
            int lightLevel,
            SectorSpecial special,
            int tag)
        {
            _number = number;
            _floorHeight = floorHeight;
            _ceilingHeight = ceilingHeight;
            _floorFlat = floorFlat;
            _ceilingFlat = ceilingFlat;
            _lightLevel = lightLevel;
            _special = special;
            _tag = tag;

            _oldFloorHeight = floorHeight;
            _oldCeilingHeight = ceilingHeight;
        }

        public static Sector FromData(byte[] data, int offset, int number, IFlatLookup flats)
        {
            short floorHeight = BitConverter.ToInt16(data, offset);
            short ceilingHeight = BitConverter.ToInt16(data, offset + 2);
            string floorFlatName = DoomInterop.ToString(data, offset + 4, 8);
            string ceilingFlatName = DoomInterop.ToString(data, offset + 12, 8);
            short lightLevel = BitConverter.ToInt16(data, offset + 20);
            short special = BitConverter.ToInt16(data, offset + 22);
            short tag = BitConverter.ToInt16(data, offset + 24);

            return new Sector(
                number,
                Fixed.FromInt(floorHeight),
                Fixed.FromInt(ceilingHeight),
                flats.GetNumber(floorFlatName),
                flats.GetNumber(ceilingFlatName),
                lightLevel,
                (SectorSpecial)special,
                tag);
        }

        public static Sector[] FromWad(Wad wad, int lump, IFlatLookup flats)
        {
            int length = wad.GetLumpSize(lump);
            if (length % _dataSize != 0)
            {
                throw new Exception();
            }

            byte[] data = wad.ReadLump(lump);
            int count = length / _dataSize;
            var sectors = new Sector[count]; ;

            for (int i = 0; i < count; i++)
            {
                int offset = _dataSize * i;
                sectors[i] = FromData(data, offset, i, flats);
            }

            return sectors;
        }

        public void UpdateFrameInterpolationInfo()
        {
            _oldFloorHeight = _floorHeight;
            _oldCeilingHeight = _ceilingHeight;
        }

        public Fixed GetInterpolatedFloorHeight(Fixed frameFrac)
        {
            return _oldFloorHeight + frameFrac * (_floorHeight - _oldFloorHeight);
        }

        public Fixed GetInterpolatedCeilingHeight(Fixed frameFrac)
        {
            return _oldCeilingHeight + frameFrac * (_ceilingHeight - _oldCeilingHeight);
        }

        public void DisableFrameInterpolationForOneFrame()
        {
            _oldFloorHeight = _floorHeight;
            _oldCeilingHeight = _ceilingHeight;
        }

        public ThingEnumerator GetEnumerator()
        {
            return new ThingEnumerator(this);
        }



        public struct ThingEnumerator : IEnumerator<Mobj>
        {
            private readonly Sector _sector;
            private Mobj? _thing;
            private Mobj? _current;

            public ThingEnumerator(Sector sector)
            {
                _sector = sector;
                _thing = sector._thingList;
                _current = null;
            }

            public bool MoveNext()
            {
                if (_thing != null)
                {
                    _current = _thing;
                    _thing = _thing.SectorNext;
                    return true;
                }
                else
                {
                    _current = null;
                    return false;
                }
            }

            public void Reset()
            {
                _thing = _sector._thingList;
                _current = null;
            }

            public void Dispose()
            {
            }

            public Mobj? Current => _current;

            object IEnumerator.Current => throw new NotImplementedException();
        }

        public int Number => _number;

        public Fixed FloorHeight
        {
            get => _floorHeight;
            set => _floorHeight = value;
        }

        public Fixed CeilingHeight
        {
            get => _ceilingHeight;
            set => _ceilingHeight = value;
        }

        public int FloorFlat
        {
            get => _floorFlat;
            set => _floorFlat = value;
        }

        public int CeilingFlat
        {
            get => _ceilingFlat;
            set => _ceilingFlat = value;
        }

        public int LightLevel
        {
            get => _lightLevel;
            set => _lightLevel = value;
        }

        public SectorSpecial Special
        {
            get => _special;
            set => _special = value;
        }

        public int Tag
        {
            get => _tag;
            set => _tag = value;
        }

        public int SoundTraversed
        {
            get => _soundTraversed;
            set => _soundTraversed = value;
        }

        public Mobj? SoundTarget
        {
            get => _soundTarget;
            set => _soundTarget = value;
        }

        public int[] BlockBox
        {
            get => _blockBox;
            set => _blockBox = value;
        }

        public Mobj SoundOrigin
        {
            get => _soundOrigin;
            set => _soundOrigin = value;
        }

        public int ValidCount
        {
            get => _validCount;
            set => _validCount = value;
        }

        public Mobj? ThingList
        {
            get => _thingList;
            set => _thingList = value;
        }

        public Thinker? SpecialData
        {
            get => _specialData;
            set => _specialData = value;
        }

        public LineDef[] Lines
        {
            get => _lines;
            set => _lines = value;
        }
    }
}

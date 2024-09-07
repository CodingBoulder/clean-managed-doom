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
    public sealed class MapCollision
    {
        private Fixed _openTop;
        private Fixed _openBottom;
        private Fixed _openRange;
        private Fixed _lowFloor;

        /// <summary>
        /// Sets opentop and openbottom to the window through a two sided line.
        /// </summary>
        public void LineOpening(LineDef line)
        {
            if (line.BackSide == null)
            {
                // If the line is single sided, nothing can pass through.
                _openRange = Fixed.Zero;
                return;
            }

            Sector? front = line.FrontSector;
            Sector? back = line.BackSector;

            if (front.CeilingHeight < back.CeilingHeight)
            {
                _openTop = front.CeilingHeight;
            }
            else
            {
                _openTop = back.CeilingHeight;
            }

            if (front.FloorHeight > back.FloorHeight)
            {
                _openBottom = front.FloorHeight;
                _lowFloor = back.FloorHeight;
            }
            else
            {
                _openBottom = back.FloorHeight;
                _lowFloor = front.FloorHeight;
            }

            _openRange = _openTop - _openBottom;
        }

        public Fixed OpenTop => _openTop;
        public Fixed OpenBottom => _openBottom;
        public Fixed OpenRange => _openRange;
        public Fixed LowFloor => _lowFloor;
    }
}

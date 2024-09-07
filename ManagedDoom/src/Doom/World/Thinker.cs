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
    public class Thinker
    {
        private Thinker _prev;
        private Thinker _next;
        private ThinkerState _thinkerState;

        public Thinker()
        {
        }

        public virtual void Run()
        {
        }

        public virtual void UpdateFrameInterpolationInfo()
        {
        }

        public Thinker Prev
        {
            get => _prev;
            set => _prev = value;
        }

        public Thinker Next
        {
            get => _next;
            set => _next = value;
        }

        public ThinkerState ThinkerState
        {
            get => _thinkerState;
            set => _thinkerState = value;
        }
    }
}

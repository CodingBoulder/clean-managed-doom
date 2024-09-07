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
    public sealed class Thinkers
    {
        public Thinkers()
        {
            InitThinkers();
        }

        private Thinker _cap;

        private void InitThinkers()
        {
            _cap = new Thinker();
            _cap.Prev = _cap.Next = _cap;
        }

        public void Add(Thinker thinker)
        {
            _cap.Prev.Next = thinker;
            thinker.Next = _cap;
            thinker.Prev = _cap.Prev;
            _cap.Prev = thinker;
        }

        public void Remove(Thinker thinker)
        {
            thinker.ThinkerState = ThinkerState.Removed;
        }

        public void Run()
        {
            Thinker current = _cap.Next;
            while (current != _cap)
            {
                if (current.ThinkerState == ThinkerState.Removed)
                {
                    // Time to remove it.
                    current.Next.Prev = current.Prev;
                    current.Prev.Next = current.Next;
                }
                else
                {
                    if (current.ThinkerState == ThinkerState.Active)
                    {
                        current.Run();
                    }
                }
                current = current.Next;
            }
        }

        public void UpdateFrameInterpolationInfo()
        {
            Thinker current = _cap.Next;
            while (current != _cap)
            {
                current.UpdateFrameInterpolationInfo();
                current = current.Next;
            }
        }

        public void Reset()
        {
            _cap.Prev = _cap.Next = _cap;
        }

        public ThinkerEnumerator GetEnumerator()
        {
            return new ThinkerEnumerator(this);
        }



        public struct ThinkerEnumerator : IEnumerator<Thinker>
        {
            private readonly Thinkers _thinkers;
            private Thinker _current;

            public ThinkerEnumerator(Thinkers thinkers)
            {
                _thinkers = thinkers;
                _current = thinkers._cap;
            }

            public bool MoveNext()
            {
                while (true)
                {
                    _current = _current.Next;
                    if (_current == _thinkers._cap)
                    {
                        return false;
                    }
                    else if (_current.ThinkerState != ThinkerState.Removed)
                    {
                        return true;
                    }
                }
            }

            public void Reset()
            {
                _current = _thinkers._cap;
            }

            public void Dispose()
            {
            }

            public Thinker Current => _current;

            object IEnumerator.Current => throw new NotImplementedException();
        }
    }
}

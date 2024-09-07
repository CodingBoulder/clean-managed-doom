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
    public sealed class AutoMap
    {
        private readonly World _world;

        private readonly Fixed _minX;
        private readonly Fixed _maxX;
        private readonly Fixed _minY;
        private readonly Fixed _maxY;

        private Fixed _viewX;
        private Fixed _viewY;

        private bool _visible;
        private AutoMapState _state;

        private Fixed _zoom;
        private bool _follow;

        private bool _zoomIn;
        private bool _zoomOut;

        private bool _left;
        private bool _right;
        private bool _up;
        private bool _down;

        private readonly List<Vertex> _marks;
        private int _nextMarkNumber;

        public AutoMap(World world)
        {
            _world = world;

            _minX = Fixed.MaxValue;
            _maxX = Fixed.MinValue;
            _minY = Fixed.MaxValue;
            _maxY = Fixed.MinValue;
            foreach (Vertex vertex in world.Map.Vertices)
            {
                if (vertex.X < _minX)
                {
                    _minX = vertex.X;
                }

                if (vertex.X > _maxX)
                {
                    _maxX = vertex.X;
                }

                if (vertex.Y < _minY)
                {
                    _minY = vertex.Y;
                }

                if (vertex.Y > _maxY)
                {
                    _maxY = vertex.Y;
                }
            }

            _viewX = _minX + (_maxX - _minX) / 2;
            _viewY = _minY + (_maxY - _minY) / 2;

            _visible = false;
            _state = AutoMapState.None;

            _zoom = Fixed.One;
            _follow = true;

            _zoomIn = false;
            _zoomOut = false;
            _left = false;
            _right = false;
            _up = false;
            _down = false;

            _marks = [];
            _nextMarkNumber = 0;
        }

        public void Update()
        {
            if (_zoomIn)
            {
                _zoom += _zoom / 16;
            }

            if (_zoomOut)
            {
                _zoom -= _zoom / 16;
            }

            if (_zoom < Fixed.One / 2)
            {
                _zoom = Fixed.One / 2;
            }
            else if (_zoom > Fixed.One * 32)
            {
                _zoom = Fixed.One * 32;
            }

            if (_left)
            {
                _viewX -= 64 / _zoom;
            }

            if (_right)
            {
                _viewX += 64 / _zoom;
            }

            if (_up)
            {
                _viewY += 64 / _zoom;
            }

            if (_down)
            {
                _viewY -= 64 / _zoom;
            }

            if (_viewX < _minX)
            {
                _viewX = _minX;
            }
            else if (_viewX > _maxX)
            {
                _viewX = _maxX;
            }

            if (_viewY < _minY)
            {
                _viewY = _minY;
            }
            else if (_viewY > _maxY)
            {
                _viewY = _maxY;
            }

            if (_follow)
            {
                Mobj? player = _world.ConsolePlayer.Mobj;
                _viewX = player.X;
                _viewY = player.Y;
            }
        }

        public bool DoEvent(DoomEvent e)
        {
            if (e.Key == DoomKey.Add || e.Key == DoomKey.Quote || e.Key == DoomKey.Equal)
            {
                if (e.Type == EventType.KeyDown)
                {
                    _zoomIn = true;
                }
                else if (e.Type == EventType.KeyUp)
                {
                    _zoomIn = false;
                }

                return true;
            }
            else if (e.Key == DoomKey.Subtract || e.Key == DoomKey.Hyphen || e.Key == DoomKey.Semicolon)
            {
                if (e.Type == EventType.KeyDown)
                {
                    _zoomOut = true;
                }
                else if (e.Type == EventType.KeyUp)
                {
                    _zoomOut = false;
                }

                return true;
            }
            else if (e.Key == DoomKey.Left)
            {
                if (e.Type == EventType.KeyDown)
                {
                    _left = true;
                }
                else if (e.Type == EventType.KeyUp)
                {
                    _left = false;
                }

                return true;
            }
            else if (e.Key == DoomKey.Right)
            {
                if (e.Type == EventType.KeyDown)
                {
                    _right = true;
                }
                else if (e.Type == EventType.KeyUp)
                {
                    _right = false;
                }

                return true;
            }
            else if (e.Key == DoomKey.Up)
            {
                if (e.Type == EventType.KeyDown)
                {
                    _up = true;
                }
                else if (e.Type == EventType.KeyUp)
                {
                    _up = false;
                }

                return true;
            }
            else if (e.Key == DoomKey.Down)
            {
                if (e.Type == EventType.KeyDown)
                {
                    _down = true;
                }
                else if (e.Type == EventType.KeyUp)
                {
                    _down = false;
                }

                return true;
            }
            else if (e.Key == DoomKey.F)
            {
                if (e.Type == EventType.KeyDown)
                {
                    _follow = !_follow;
                    if (_follow)
                    {
                        _world.ConsolePlayer.SendMessage(DoomInfo.Strings.AMSTR_FOLLOWON);
                    }
                    else
                    {
                        _world.ConsolePlayer.SendMessage(DoomInfo.Strings.AMSTR_FOLLOWOFF);
                    }
                    return true;
                }
            }
            else if (e.Key == DoomKey.M)
            {
                if (e.Type == EventType.KeyDown)
                {
                    if (_marks.Count < 10)
                    {
                        _marks.Add(new Vertex(_viewX, _viewY));
                    }
                    else
                    {
                        _marks[_nextMarkNumber] = new Vertex(_viewX, _viewY);
                    }
                    _nextMarkNumber++;
                    if (_nextMarkNumber == 10)
                    {
                        _nextMarkNumber = 0;
                    }
                    _world.ConsolePlayer.SendMessage(DoomInfo.Strings.AMSTR_MARKEDSPOT);
                    return true;
                }
            }
            else if (e.Key == DoomKey.C)
            {
                if (e.Type == EventType.KeyDown)
                {
                    _marks.Clear();
                    _nextMarkNumber = 0;
                    _world.ConsolePlayer.SendMessage(DoomInfo.Strings.AMSTR_MARKSCLEARED);
                    return true;
                }
            }

            return false;
        }

        public void Open()
        {
            _visible = true;
        }

        public void Close()
        {
            _visible = false;
            _zoomIn = false;
            _zoomOut = false;
            _left = false;
            _right = false;
            _up = false;
            _down = false;
        }

        public void ToggleCheat()
        {
            _state++;
            if ((int)_state == 3)
            {
                _state = AutoMapState.None;
            }
        }

        public Fixed MinX => _minX;
        public Fixed MaxX => _maxX;
        public Fixed MinY => _minY;
        public Fixed MaxY => _maxY;
        public Fixed ViewX => _viewX;
        public Fixed ViewY => _viewY;
        public Fixed Zoom => _zoom;
        public bool Follow => _follow;
        public bool Visible => _visible;
        public AutoMapState State => _state;
        public IReadOnlyList<Vertex> Marks => _marks;
    }
}

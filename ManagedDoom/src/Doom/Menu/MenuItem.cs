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
    public abstract class MenuItem
    {
        private readonly int _skullX;
        private readonly int _skullY;
        private readonly MenuDef? _next;

        private MenuItem()
        {
        }

        public MenuItem(int skullX, int skullY, MenuDef? next)
        {
            _skullX = skullX;
            _skullY = skullY;
            _next = next;
        }

        public int SkullX => _skullX;
        public int SkullY => _skullY;
        public MenuDef? Next => _next;
    }
}

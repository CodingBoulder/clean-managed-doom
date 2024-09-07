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
    public class ToggleMenuItem : MenuItem
    {
        private readonly string name;
        private readonly int itemX;
        private readonly int itemY;

        private readonly string[] states;
        private readonly int stateX;

        private int stateNumber;

        private readonly Func<int> reset;
        private readonly Action<int> action;

        public ToggleMenuItem(
            string name,
            int skullX, int skullY,
            int itemX, int itemY,
            string state1, string state2,
            int stateX,
            Func<int> reset,
            Action<int> action)
            : base(skullX, skullY, null)
        {
            this.name = name;
            this.itemX = itemX;
            this.itemY = itemY;

            states = [state1, state2];
            this.stateX = stateX;

            stateNumber = 0;

            this.action = action;
            this.reset = reset;
        }

        public void Reset()
        {
            if (reset != null)
            {
                stateNumber = reset();
            }
        }

        public void Up()
        {
            stateNumber++;
            if (stateNumber == states.Length)
            {
                stateNumber = 0;
            }

            action?.Invoke(stateNumber);
        }

        public void Down()
        {
            stateNumber--;
            if (stateNumber == -1)
            {
                stateNumber = states.Length - 1;
            }

            action?.Invoke(stateNumber);
        }

        public string Name => name;
        public int ItemX => itemX;
        public int ItemY => itemY;

        public string State => states[stateNumber];
        public int StateX => stateX;
    }
}

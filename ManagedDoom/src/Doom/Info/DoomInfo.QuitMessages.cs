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
    public static partial class DoomInfo
    {
        public static class QuitMessages
        {
            public static readonly IReadOnlyList<DoomString> Doom =
            [
                Strings.QUITMSG,
                new("please don't leave, there's more\ndemons to toast!"),
                new("let's beat it -- this is turning\ninto a bloodbath!"),
                new("i wouldn't leave if i were you.\ndos is much worse."),
                new("you're trying to say you like dos\nbetter than me, right?"),
                new("don't leave yet -- there's a\ndemon around that corner!"),
                new("ya know, next time you come in here\ni'm gonna toast ya."),
                new("go ahead and leave. see if i care.")
            ];

            public static readonly IReadOnlyList<DoomString> Doom2 =
            [
                new("you want to quit?\nthen, thou hast lost an eighth!"),
                new("don't go now, there's a \ndimensional shambler waiting\nat the dos prompt!"),
                new("get outta here and go back\nto your boring programs."),
                new("if i were your boss, i'd \n deathmatch ya in a minute!"),
                new("look, bud. you leave now\nand you forfeit your body count!"),
                new("just leave. when you come\nback, i'll be waiting with a bat."),
                new("you're lucky i don't smack\nyou for thinking about leaving.")
            ];

            public static readonly IReadOnlyList<DoomString> FinalDoom =
            [
                new("fuck you, pussy!\nget the fuck out!"),
                new("you quit and i'll jizz\nin your cystholes!"),
                new("if you leave, i'll make\nthe lord drink my jizz."),
                new("hey, ron! can we say\n'fuck' in the game?"),
                new("i'd leave: this is just\nmore monsters and levels.\nwhat a load."),
                new("suck it down, asshole!\nyou're a fucking wimp!"),
                new("don't quit now! we're \nstill spending your money!")
            ];
        }
    }
}

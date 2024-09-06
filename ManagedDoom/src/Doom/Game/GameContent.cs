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
using System.Threading;
using System.Threading.Tasks;

namespace ManagedDoom
{
    public sealed class GameContent : IDisposable
    {
        private Wad wad;
        private Palette palette;
        private ColorMap colorMap;
        private ITextureLookup textures;
        private IFlatLookup flats;
        private ISpriteLookup sprites;
        private TextureAnimation animation;

        public GameContent(Wad wad)
        {
            this.wad = wad;
        }

        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            textures = new TextureLookup();
            textures.Initialize(wad);

            flats = new FlatLookup(wad);

            await Task.WhenAll(
                Task.Run(() => palette = new Palette(wad), cancellationToken),
                Task.Run(() => colorMap = new ColorMap(wad), cancellationToken),
                Task.Run(() => sprites = new SpriteLookup(wad), cancellationToken),
                Task.Run(() => animation = new TextureAnimation(textures, flats), cancellationToken));
        }

        public static GameContent CreateDummy(params string[] wadPaths)
        {
            var gc = new GameContent(new Wad(wadPaths));

            gc.InitializeAsync(CancellationToken.None)
                .Wait();

            return gc;
        }

        public void Dispose()
        {
            if (wad != null)
            {
                wad.Dispose();
                wad = null;
            }
        }

        public Wad Wad => wad;
        public Palette Palette => palette;
        public ColorMap ColorMap => colorMap;
        public ITextureLookup Textures => textures;
        public IFlatLookup Flats => flats;
        public ISpriteLookup Sprites => sprites;
        public TextureAnimation Animation => animation;
    }
}

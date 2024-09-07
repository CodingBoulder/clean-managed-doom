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
        private Wad? _wad;
        private Palette _palette;
        private ColorMap _colorMap;
        private TextureLookup _textures;
        private FlatLookup _flats;
        private SpriteLookup _sprites;
        private TextureAnimation _animation;

        public GameContent(Wad wad)
        {
            _wad = wad;
        }

        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            _textures = new TextureLookup();
            _textures.Initialize(_wad);

            _flats = new FlatLookup(_wad);

            await Task.WhenAll(
                Task.Run(() => _palette = new Palette(_wad), cancellationToken),
                Task.Run(() => _colorMap = new ColorMap(_wad), cancellationToken),
                Task.Run(() => _sprites = new SpriteLookup(_wad), cancellationToken),
                Task.Run(() => _animation = new TextureAnimation(_textures, _flats), cancellationToken));
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
            if (_wad != null)
            {
                _wad.Dispose();
                _wad = null;
            }
        }

        public Wad? Wad => _wad;
        public Palette Palette => _palette;
        public ColorMap ColorMap => _colorMap;
        public ITextureLookup Textures => _textures;
        public IFlatLookup Flats => _flats;
        public ISpriteLookup Sprites => _sprites;
        public TextureAnimation Animation => _animation;
    }
}

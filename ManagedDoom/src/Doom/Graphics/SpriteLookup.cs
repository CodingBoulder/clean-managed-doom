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
using System.Runtime.ExceptionServices;

namespace ManagedDoom
{
    public sealed class SpriteLookup : ISpriteLookup
    {
        private readonly SpriteDef[] _spriteDefs;

        public SpriteLookup(Wad wad)
        {
            try
            {
                Console.Write("Load sprites: ");

                var temp = new Dictionary<string, List<SpriteInfo>>();
                for (int i = 0; i < (int)Sprite.Count; i++)
                {
                    temp.TryAdd(DoomInfo.SpriteNames[i], []);
                }

                var cache = new Dictionary<int, Patch>();

                foreach (int lump in EnumerateSprites(wad))
                {
                    string name = wad.LumpInfos[lump].Name.Substring(0, 4);

                    if (!temp.TryGetValue(name, out List<SpriteInfo>? sprites))
                    {
                        continue;
                    }

                    {
                        int frame = wad.LumpInfos[lump].Name[4] - 'A';
                        int rotation = wad.LumpInfos[lump].Name[5] - '0';

                        while (sprites.Count < frame + 1)
                        {
                            sprites.Add(new SpriteInfo());
                        }

                        if (rotation == 0)
                        {
                            for (int i = 0; i < 8; i++)
                            {
                                if (sprites[frame].Patches[i] == null)
                                {
                                    sprites[frame].Patches[i] = CachedRead(lump, wad, cache);
                                    sprites[frame].Flip[i] = false;
                                }
                            }
                        }
                        else
                        {
                            if (sprites[frame].Patches[rotation - 1] == null)
                            {
                                sprites[frame].Patches[rotation - 1] = CachedRead(lump, wad, cache);
                                sprites[frame].Flip[rotation - 1] = false;
                            }
                        }
                    }

                    if (wad.LumpInfos[lump].Name.Length == 8)
                    {
                        int frame = wad.LumpInfos[lump].Name[6] - 'A';
                        int rotation = wad.LumpInfos[lump].Name[7] - '0';

                        while (sprites.Count < frame + 1)
                        {
                            sprites.Add(new SpriteInfo());
                        }

                        if (rotation == 0)
                        {
                            for (int i = 0; i < 8; i++)
                            {
                                if (sprites[frame].Patches[i] == null)
                                {
                                    sprites[frame].Patches[i] = CachedRead(lump, wad, cache);
                                    sprites[frame].Flip[i] = true;
                                }
                            }
                        }
                        else
                        {
                            if (sprites[frame].Patches[rotation - 1] == null)
                            {
                                sprites[frame].Patches[rotation - 1] = CachedRead(lump, wad, cache);
                                sprites[frame].Flip[rotation - 1] = true;
                            }
                        }
                    }
                }

                _spriteDefs = new SpriteDef[(int)Sprite.Count];
                for (int i = 0; i < _spriteDefs.Length; i++)
                {
                    List<SpriteInfo> list = temp[DoomInfo.SpriteNames[i]];

                    var frames = new SpriteFrame[list.Count];
                    for (int j = 0; j < frames.Length; j++)
                    {
                        list[j].CheckCompletion();

                        var frame = new SpriteFrame(list[j].HasRotation(), list[j].Patches, list[j].Flip);
                        frames[j] = frame;
                    }

                    _spriteDefs[i] = new SpriteDef(frames);
                }

                Console.WriteLine("OK (" + cache.Count + " sprites)");
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed");
                ExceptionDispatchInfo.Throw(e);
            }
        }

        private static IEnumerable<int> EnumerateSprites(Wad wad)
        {
            bool spriteSection = false;

            for (int lump = wad.LumpInfos.Count - 1; lump >= 0; lump--)
            {
                string name = wad.LumpInfos[lump].Name;

                if (name.StartsWith("S"))
                {
                    if (name.EndsWith("_END"))
                    {
                        spriteSection = true;
                        continue;
                    }
                    else if (name.EndsWith("_START"))
                    {
                        spriteSection = false;
                        continue;
                    }
                }

                if (spriteSection)
                {
                    if (wad.LumpInfos[lump].Size > 0)
                    {
                        yield return lump;
                    }
                }
            }
        }

        private static Patch CachedRead(int lump, Wad wad, Dictionary<int, Patch> cache)
        {
            if (!cache.TryGetValue(lump, out Patch? patch))
            {
                string name = wad.LumpInfos[lump].Name;

                patch = Patch.FromData(name, wad.ReadLump(lump));

                cache.Add(lump, patch);
            }

            return patch;
        }

        public SpriteDef this[Sprite sprite]
        {
            get
            {
                return _spriteDefs[(int)sprite];
            }
        }



        private class SpriteInfo
        {
            public Patch[] Patches;
            public bool[] Flip;

            public SpriteInfo()
            {
                Patches = new Patch[8];
                Flip = new bool[8];
            }

            public void CheckCompletion()
            {
                for (int i = 0; i < Patches.Length; i++)
                {
                    if (Patches[i] == null)
                    {
                        throw new Exception("Missing sprite!");
                    }
                }
            }

            public bool HasRotation()
            {
                for (int i = 1; i < Patches.Length; i++)
                {
                    if (Patches[i] != Patches[0])
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }
}

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
    public sealed class Map
    {
        private readonly ITextureLookup _textures;
        private readonly IFlatLookup _flats;
        private readonly TextureAnimation _animation;

        private readonly World _world;

        private readonly Vertex[] _vertices;
        private readonly Sector[] _sectors;
        private readonly SideDef[] _sides;
        private readonly LineDef[] _lines;
        private readonly Seg[] _segs;
        private readonly Subsector[] _subsectors;
        private readonly Node[] _nodes;
        private readonly MapThing[] _things;
        private readonly BlockMap _blockMap;
        private readonly Reject _reject;

        private readonly Texture _skyTexture;

        private readonly string _title;

        public Map(GameContent resorces, World world)
            : this(resorces.Wad, resorces.Textures, resorces.Flats, resorces.Animation, world)
        {
        }

        public Map(Wad wad, ITextureLookup textures, IFlatLookup flats, TextureAnimation animation, World world)
        {
            try
            {
                _textures = textures;
                _flats = flats;
                _animation = animation;
                _world = world;

                GameOptions options = world.Options;

                string name;
                if (wad.GameMode == GameMode.Commercial)
                {
                    name = "MAP" + options.Map.ToString("00");
                }
                else
                {
                    name = "E" + options.Episode + "M" + options.Map;
                }

                Console.Write("Load map '" + name + "': ");

                int map = wad.GetLumpNumber(name);

                if (map == -1)
                {
                    throw new Exception("Map '" + name + "' was not found!");
                }

                _vertices = Vertex.FromWad(wad, map + 4);
                _sectors = Sector.FromWad(wad, map + 8, flats);
                _sides = SideDef.FromWad(wad, map + 3, textures, _sectors);
                _lines = LineDef.FromWad(wad, map + 2, _vertices, _sides);
                _segs = Seg.FromWad(wad, map + 5, _vertices, _lines);
                _subsectors = Subsector.FromWad(wad, map + 6, _segs);
                _nodes = Node.FromWad(wad, map + 7);
                _things = MapThing.FromWad(wad, map + 1);
                _blockMap = BlockMap.FromWad(wad, map + 10, _lines);
                _reject = Reject.FromWad(wad, map + 9, _sectors);

                GroupLines();

                _skyTexture = GetSkyTextureByMapName(name);

                if (options.GameMode == GameMode.Commercial)
                {
                    switch (options.MissionPack)
                    {
                        case MissionPack.Plutonia:
                            _title = DoomInfo.MapTitles.Plutonia[options.Map - 1];
                            break;
                        case MissionPack.Tnt:
                            _title = DoomInfo.MapTitles.Tnt[options.Map - 1];
                            break;
                        default:
                            _title = DoomInfo.MapTitles.Doom2[options.Map - 1];
                            break;
                    }
                }
                else
                {
                    _title = DoomInfo.MapTitles.Doom[options.Episode - 1][options.Map - 1];
                }

                Console.WriteLine("OK");
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed");
                ExceptionDispatchInfo.Throw(e);
            }
        }

        private void GroupLines()
        {
            var sectorLines = new List<LineDef>();
            var boundingBox = new Fixed[4];

            foreach (LineDef line in _lines)
            {
                if (line.Special != 0)
                {
                    var so = new Mobj(_world)
                    {
                        X = (line.Vertex1.X + line.Vertex2.X) / 2,
                        Y = (line.Vertex1.Y + line.Vertex2.Y) / 2
                    };
                    line.SoundOrigin = so;
                }
            }

            foreach (Sector sector in _sectors)
            {
                sectorLines.Clear();
                Box.Clear(boundingBox);

                foreach (LineDef line in _lines)
                {
                    if (line.FrontSector == sector || line.BackSector == sector)
                    {
                        sectorLines.Add(line);
                        Box.AddPoint(boundingBox, line.Vertex1.X, line.Vertex1.Y);
                        Box.AddPoint(boundingBox, line.Vertex2.X, line.Vertex2.Y);
                    }
                }

                sector.Lines = sectorLines.ToArray();

                // Set the degenmobj_t to the middle of the bounding box.
                sector.SoundOrigin = new Mobj(_world)
                {
                    X = (boundingBox[Box.Right] + boundingBox[Box.Left]) / 2,
                    Y = (boundingBox[Box.Top] + boundingBox[Box.Bottom]) / 2
                };

                sector.BlockBox = new int[4];
                int block;

                // Adjust bounding box to map blocks.
                block = (boundingBox[Box.Top] - _blockMap.OriginY + GameConst.MaxThingRadius).Data >> BlockMap.FracToBlockShift;
                block = block >= _blockMap.Height ? _blockMap.Height - 1 : block;
                sector.BlockBox[Box.Top] = block;

                block = (boundingBox[Box.Bottom] - _blockMap.OriginY - GameConst.MaxThingRadius).Data >> BlockMap.FracToBlockShift;
                block = block < 0 ? 0 : block;
                sector.BlockBox[Box.Bottom] = block;

                block = (boundingBox[Box.Right] - _blockMap.OriginX + GameConst.MaxThingRadius).Data >> BlockMap.FracToBlockShift;
                block = block >= _blockMap.Width ? _blockMap.Width - 1 : block;
                sector.BlockBox[Box.Right] = block;

                block = (boundingBox[Box.Left] - _blockMap.OriginX - GameConst.MaxThingRadius).Data >> BlockMap.FracToBlockShift;
                block = block < 0 ? 0 : block;
                sector.BlockBox[Box.Left] = block;
            }
        }

        private Texture GetSkyTextureByMapName(string name)
        {
            if (name.Length == 4)
            {
                switch (name[1])
                {
                    case '1':
                        return _textures["SKY1"];
                    case '2':
                        return _textures["SKY2"];
                    case '3':
                        return _textures["SKY3"];
                    default:
                        return _textures["SKY4"];
                }
            }
            else
            {
                int number = int.Parse(name.Substring(3));
                if (number <= 11)
                {
                    return _textures["SKY1"];
                }
                else if (number <= 21)
                {
                    return _textures["SKY2"];
                }
                else
                {
                    return _textures["SKY3"];
                }
            }
        }

        public ITextureLookup Textures => _textures;
        public IFlatLookup Flats => _flats;
        public TextureAnimation Animation => _animation;

        public Vertex[] Vertices => _vertices;
        public Sector[] Sectors => _sectors;
        public SideDef[] Sides => _sides;
        public LineDef[] Lines => _lines;
        public Seg[] Segs => _segs;
        public Subsector[] Subsectors => _subsectors;
        public Node[] Nodes => _nodes;
        public MapThing[] Things => _things;
        public BlockMap BlockMap => _blockMap;
        public Reject Reject => _reject;
        public Texture SkyTexture => _skyTexture;
        public int SkyFlatNumber => _flats.SkyFlatNumber;
        public string Title => _title;



        private static readonly Bgm[] _e4BgmList =
        [
            Bgm.E3M4, // American   e4m1
            Bgm.E3M2, // Romero     e4m2
            Bgm.E3M3, // Shawn      e4m3
            Bgm.E1M5, // American   e4m4
            Bgm.E2M7, // Tim        e4m5
            Bgm.E2M4, // Romero     e4m6
            Bgm.E2M6, // J.Anderson e4m7 CHIRON.WAD
            Bgm.E2M5, // Shawn      e4m8
            Bgm.E1M9  // Tim        e4m9
        ];

        public static Bgm GetMapBgm(GameOptions options)
        {
            Bgm bgm;
            if (options.GameMode == GameMode.Commercial)
            {
                bgm = Bgm.RUNNIN + options.Map - 1;
            }
            else
            {
                if (options.Episode < 4)
                {
                    bgm = Bgm.E1M1 + (options.Episode - 1) * 9 + options.Map - 1;
                }
                else
                {
                    bgm = _e4BgmList[options.Map - 1];
                }
            }

            return bgm;
        }
    }
}

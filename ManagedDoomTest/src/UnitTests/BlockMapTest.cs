using ManagedDoom;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ManagedDoomTest.UnitTests
{
    public class BlockMapTest
    {
        [Fact]
        public void LoadE1M1()
        {
            using var wad = new Wad(WadPath.Doom1);
            var flats = new FlatLookup(wad);
            var textures = new TextureLookup();
            textures.Initialize(wad);
            int map = wad.GetLumpNumber("E1M1");
            Vertex[] vertices = Vertex.FromWad(wad, map + 4);
            Sector[] sectors = Sector.FromWad(wad, map + 8, flats);
            SideDef[] sides = SideDef.FromWad(wad, map + 3, textures, sectors);
            LineDef[] lines = LineDef.FromWad(wad, map + 2, vertices, sides);
            var blockMap = BlockMap.FromWad(wad, map + 10, lines);

            {
                double minX = vertices.Select(v => v.X.ToDouble()).Min();
                double maxX = vertices.Select(v => v.X.ToDouble()).Max();
                double minY = vertices.Select(v => v.Y.ToDouble()).Min();
                double maxY = vertices.Select(v => v.Y.ToDouble()).Max();

                Assert.Equal(blockMap.OriginX.ToDouble(), minX, 64d);
                Assert.Equal(blockMap.OriginY.ToDouble(), minY, 64d);
                Assert.Equal((blockMap.OriginX + BlockMap.BlockSize * blockMap.Width).ToDouble(), maxX, 128d);
                Assert.Equal((blockMap.OriginY + BlockMap.BlockSize * blockMap.Height).ToDouble(), maxY, 128d);
            }

            var spots = new List<Tuple<int, int>>();
            for (int blockY = -2; blockY < blockMap.Height + 2; blockY++)
            {
                for (int blockX = -2; blockX < blockMap.Width + 2; blockX++)
                {
                    spots.Add(Tuple.Create(blockX, blockY));
                }
            }

            var random = new Random(666);

            for (int i = 0; i < 50; i++)
            {
                Tuple<int, int>[] ordered = spots.OrderBy(spot => random.NextDouble()).ToArray();

                int total = 0;

                foreach (Tuple<int, int>? spot in ordered)
                {
                    int blockX = spot.Item1;
                    int blockY = spot.Item2;

                    double minX = double.MaxValue;
                    double maxX = double.MinValue;
                    double minY = double.MaxValue;
                    double maxY = double.MinValue;
                    int count = 0;

                    blockMap.IterateLines(
                        blockX,
                        blockY,
                        line =>
                        {
                            if (count != 0)
                            {
                                minX = Math.Min(Math.Min(line.Vertex1.X.ToDouble(), line.Vertex2.X.ToDouble()), minX);
                                maxX = Math.Max(Math.Max(line.Vertex1.X.ToDouble(), line.Vertex2.X.ToDouble()), maxX);
                                minY = Math.Min(Math.Min(line.Vertex1.Y.ToDouble(), line.Vertex2.Y.ToDouble()), minY);
                                maxY = Math.Max(Math.Max(line.Vertex1.Y.ToDouble(), line.Vertex2.Y.ToDouble()), maxY);
                            }
                            count++;
                            return true;
                        },
                        i + 1);

                    if (count > 1)
                    {
                        Assert.True(minX <= (blockMap.OriginX + BlockMap.BlockSize * (blockX + 1)).ToDouble());
                        Assert.True(maxX >= (blockMap.OriginX + BlockMap.BlockSize * blockX).ToDouble());
                        Assert.True(minY <= (blockMap.OriginY + BlockMap.BlockSize * (blockY + 1)).ToDouble());
                        Assert.True(maxY >= (blockMap.OriginY + BlockMap.BlockSize * blockY).ToDouble());
                    }

                    total += count;
                }

                Assert.Equal(lines.Length, total);
            }
        }

        [Fact]
        public void LoadMap01()
        {
            using var wad = new Wad(WadPath.Doom2);
            var flats = new FlatLookup(wad);
            var textures = new TextureLookup();
            textures.Initialize(wad);
            int map = wad.GetLumpNumber("MAP01");
            Vertex[] vertices = Vertex.FromWad(wad, map + 4);
            Sector[] sectors = Sector.FromWad(wad, map + 8, flats);
            SideDef[] sides = SideDef.FromWad(wad, map + 3, textures, sectors);
            LineDef[] lines = LineDef.FromWad(wad, map + 2, vertices, sides);
            var blockMap = BlockMap.FromWad(wad, map + 10, lines);

            {
                double minX = vertices.Select(v => v.X.ToDouble()).Min();
                double maxX = vertices.Select(v => v.X.ToDouble()).Max();
                double minY = vertices.Select(v => v.Y.ToDouble()).Min();
                double maxY = vertices.Select(v => v.Y.ToDouble()).Max();

                Assert.Equal(blockMap.OriginX.ToDouble(), minX, 64d);
                Assert.Equal(blockMap.OriginY.ToDouble(), minY, 64d);
                Assert.Equal((blockMap.OriginX + BlockMap.BlockSize * blockMap.Width).ToDouble(), maxX, 128d);
                Assert.Equal((blockMap.OriginY + BlockMap.BlockSize * blockMap.Height).ToDouble(), maxY, 128d);
            }

            var spots = new List<Tuple<int, int>>();
            for (int blockY = -2; blockY < blockMap.Height + 2; blockY++)
            {
                for (int blockX = -2; blockX < blockMap.Width + 2; blockX++)
                {
                    spots.Add(Tuple.Create(blockX, blockY));
                }
            }

            var random = new Random(666);

            for (int i = 0; i < 50; i++)
            {
                Tuple<int, int>[] ordered = spots.OrderBy(spot => random.NextDouble()).ToArray();

                int total = 0;

                foreach (Tuple<int, int>? spot in ordered)
                {
                    int blockX = spot.Item1;
                    int blockY = spot.Item2;

                    double minX = double.MaxValue;
                    double maxX = double.MinValue;
                    double minY = double.MaxValue;
                    double maxY = double.MinValue;
                    int count = 0;

                    blockMap.IterateLines(
                        blockX,
                        blockY,
                        line =>
                        {
                            if (count != 0)
                            {
                                minX = Math.Min(Math.Min(line.Vertex1.X.ToDouble(), line.Vertex2.X.ToDouble()), minX);
                                maxX = Math.Max(Math.Max(line.Vertex1.X.ToDouble(), line.Vertex2.X.ToDouble()), maxX);
                                minY = Math.Min(Math.Min(line.Vertex1.Y.ToDouble(), line.Vertex2.Y.ToDouble()), minY);
                                maxY = Math.Max(Math.Max(line.Vertex1.Y.ToDouble(), line.Vertex2.Y.ToDouble()), maxY);
                            }
                            count++;
                            return true;
                        },
                        i + 1);

                    if (count > 1)
                    {
                        Assert.True(minX <= (blockMap.OriginX + BlockMap.BlockSize * (blockX + 1)).ToDouble());
                        Assert.True(maxX >= (blockMap.OriginX + BlockMap.BlockSize * blockX).ToDouble());
                        Assert.True(minY <= (blockMap.OriginY + BlockMap.BlockSize * (blockY + 1)).ToDouble());
                        Assert.True(maxY >= (blockMap.OriginY + BlockMap.BlockSize * blockY).ToDouble());
                    }

                    total += count;
                }

                Assert.Equal(lines.Length, total);
            }
        }
    }
}

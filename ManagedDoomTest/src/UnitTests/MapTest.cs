using ManagedDoom;
using System;
using System.Linq;
using Xunit;

namespace ManagedDoomTest.UnitTests
{
    public class MapTest
    {
        private static readonly double maxRadius = 32;

        [Fact]
        public void LoadE1M1()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom1);
            var options = new GameOptions();
            var world = new World(content, options, null);
            var map = new Map(content, world);

            double mapMinX = map.Lines.Min(line => Fixed.Min(line.Vertex1.X, line.Vertex2.X).ToDouble());
            double mapMaxX = map.Lines.Max(line => Fixed.Max(line.Vertex1.X, line.Vertex2.X).ToDouble());
            double mapMinY = map.Lines.Min(line => Fixed.Min(line.Vertex1.Y, line.Vertex2.Y).ToDouble());
            double mapMaxY = map.Lines.Max(line => Fixed.Max(line.Vertex1.Y, line.Vertex2.Y).ToDouble());

            for (int i = 0; i < map.Sectors.Length; i++)
            {
                Sector sector = map.Sectors[i];
                LineDef[] sLines = map.Lines.Where(line => line.FrontSector == sector || line.BackSector == sector).ToArray();

                Assert.Equal(sLines, sector.Lines);

                double minX = sLines.Min(line => Fixed.Min(line.Vertex1.X, line.Vertex2.X).ToDouble()) - maxRadius;
                minX = Math.Max(minX, mapMinX);
                double maxX = sLines.Max(line => Fixed.Max(line.Vertex1.X, line.Vertex2.X).ToDouble()) + maxRadius;
                maxX = Math.Min(maxX, mapMaxX);
                double minY = sLines.Min(line => Fixed.Min(line.Vertex1.Y, line.Vertex2.Y).ToDouble()) - maxRadius;
                minY = Math.Max(minY, mapMinY);
                double maxY = sLines.Max(line => Fixed.Max(line.Vertex1.Y, line.Vertex2.Y).ToDouble()) + maxRadius;
                maxY = Math.Min(maxY, mapMaxY);

                double bboxTop = (map.BlockMap.OriginY + BlockMap.BlockSize * (sector.BlockBox[Box.Top] + 1)).ToDouble();
                double bboxBottom = (map.BlockMap.OriginY + BlockMap.BlockSize * sector.BlockBox[Box.Bottom]).ToDouble();
                double bboxLeft = (map.BlockMap.OriginX + BlockMap.BlockSize * sector.BlockBox[Box.Left]).ToDouble();
                double bboxRight = (map.BlockMap.OriginX + BlockMap.BlockSize * (sector.BlockBox[Box.Right] + 1)).ToDouble();

                Assert.True(bboxLeft <= minX);
                Assert.True(bboxRight >= maxX);
                Assert.True(bboxTop >= maxY);
                Assert.True(bboxBottom <= minY);

                Assert.True(Math.Abs(bboxLeft - minX) <= 128);
                Assert.True(Math.Abs(bboxRight - maxX) <= 128);
                Assert.True(Math.Abs(bboxTop - maxY) <= 128);
                Assert.True(Math.Abs(bboxBottom - minY) <= 128);
            }
        }

        [Fact]
        public void LoadMap01()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2);
            var options = new GameOptions();
            var world = new World(content, options, null);
            var map = new Map(content, world);

            double mapMinX = map.Lines.Min(line => Fixed.Min(line.Vertex1.X, line.Vertex2.X).ToDouble());
            double mapMaxX = map.Lines.Max(line => Fixed.Max(line.Vertex1.X, line.Vertex2.X).ToDouble());
            double mapMinY = map.Lines.Min(line => Fixed.Min(line.Vertex1.Y, line.Vertex2.Y).ToDouble());
            double mapMaxY = map.Lines.Max(line => Fixed.Max(line.Vertex1.Y, line.Vertex2.Y).ToDouble());

            for (int i = 0; i < map.Sectors.Length; i++)
            {
                Sector sector = map.Sectors[i];
                LineDef[] sLines = map.Lines.Where(line => line.FrontSector == sector || line.BackSector == sector).ToArray();

                Assert.Equal(sLines, sector.Lines);

                double minX = sLines.Min(line => Fixed.Min(line.Vertex1.X, line.Vertex2.X).ToDouble()) - maxRadius;
                minX = Math.Max(minX, mapMinX);
                double maxX = sLines.Max(line => Fixed.Max(line.Vertex1.X, line.Vertex2.X).ToDouble()) + maxRadius;
                maxX = Math.Min(maxX, mapMaxX);
                double minY = sLines.Min(line => Fixed.Min(line.Vertex1.Y, line.Vertex2.Y).ToDouble()) - maxRadius;
                minY = Math.Max(minY, mapMinY);
                double maxY = sLines.Max(line => Fixed.Max(line.Vertex1.Y, line.Vertex2.Y).ToDouble()) + maxRadius;
                maxY = Math.Min(maxY, mapMaxY);

                double bboxTop = (map.BlockMap.OriginY + BlockMap.BlockSize * (sector.BlockBox[Box.Top] + 1)).ToDouble();
                double bboxBottom = (map.BlockMap.OriginY + BlockMap.BlockSize * sector.BlockBox[Box.Bottom]).ToDouble();
                double bboxLeft = (map.BlockMap.OriginX + BlockMap.BlockSize * sector.BlockBox[Box.Left]).ToDouble();
                double bboxRight = (map.BlockMap.OriginX + BlockMap.BlockSize * (sector.BlockBox[Box.Right] + 1)).ToDouble();

                Assert.True(bboxLeft <= minX);
                Assert.True(bboxRight >= maxX);
                Assert.True(bboxTop >= maxY);
                Assert.True(bboxBottom <= minY);

                Assert.True(Math.Abs(bboxLeft - minX) <= 128);
                Assert.True(Math.Abs(bboxRight - maxX) <= 128);
                Assert.True(Math.Abs(bboxTop - maxY) <= 128);
                Assert.True(Math.Abs(bboxBottom - minY) <= 128);
            }
        }
    }
}

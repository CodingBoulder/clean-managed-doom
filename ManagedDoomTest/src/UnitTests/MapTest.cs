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

            var mapMinX = map.Lines.Min(line => Fixed.Min(line.Vertex1.X, line.Vertex2.X).ToDouble());
            var mapMaxX = map.Lines.Max(line => Fixed.Max(line.Vertex1.X, line.Vertex2.X).ToDouble());
            var mapMinY = map.Lines.Min(line => Fixed.Min(line.Vertex1.Y, line.Vertex2.Y).ToDouble());
            var mapMaxY = map.Lines.Max(line => Fixed.Max(line.Vertex1.Y, line.Vertex2.Y).ToDouble());

            for (var i = 0; i < map.Sectors.Length; i++)
            {
                var sector = map.Sectors[i];
                var sLines = map.Lines.Where(line => line.FrontSector == sector || line.BackSector == sector).ToArray();

                Assert.Equal(sLines, sector.Lines);

                var minX = sLines.Min(line => Fixed.Min(line.Vertex1.X, line.Vertex2.X).ToDouble()) - maxRadius;
                minX = Math.Max(minX, mapMinX);
                var maxX = sLines.Max(line => Fixed.Max(line.Vertex1.X, line.Vertex2.X).ToDouble()) + maxRadius;
                maxX = Math.Min(maxX, mapMaxX);
                var minY = sLines.Min(line => Fixed.Min(line.Vertex1.Y, line.Vertex2.Y).ToDouble()) - maxRadius;
                minY = Math.Max(minY, mapMinY);
                var maxY = sLines.Max(line => Fixed.Max(line.Vertex1.Y, line.Vertex2.Y).ToDouble()) + maxRadius;
                maxY = Math.Min(maxY, mapMaxY);

                var bboxTop = (map.BlockMap.OriginY + BlockMap.BlockSize * (sector.BlockBox[Box.Top] + 1)).ToDouble();
                var bboxBottom = (map.BlockMap.OriginY + BlockMap.BlockSize * sector.BlockBox[Box.Bottom]).ToDouble();
                var bboxLeft = (map.BlockMap.OriginX + BlockMap.BlockSize * sector.BlockBox[Box.Left]).ToDouble();
                var bboxRight = (map.BlockMap.OriginX + BlockMap.BlockSize * (sector.BlockBox[Box.Right] + 1)).ToDouble();

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

            var mapMinX = map.Lines.Min(line => Fixed.Min(line.Vertex1.X, line.Vertex2.X).ToDouble());
            var mapMaxX = map.Lines.Max(line => Fixed.Max(line.Vertex1.X, line.Vertex2.X).ToDouble());
            var mapMinY = map.Lines.Min(line => Fixed.Min(line.Vertex1.Y, line.Vertex2.Y).ToDouble());
            var mapMaxY = map.Lines.Max(line => Fixed.Max(line.Vertex1.Y, line.Vertex2.Y).ToDouble());

            for (var i = 0; i < map.Sectors.Length; i++)
            {
                var sector = map.Sectors[i];
                var sLines = map.Lines.Where(line => line.FrontSector == sector || line.BackSector == sector).ToArray();

                Assert.Equal(sLines, sector.Lines);

                var minX = sLines.Min(line => Fixed.Min(line.Vertex1.X, line.Vertex2.X).ToDouble()) - maxRadius;
                minX = Math.Max(minX, mapMinX);
                var maxX = sLines.Max(line => Fixed.Max(line.Vertex1.X, line.Vertex2.X).ToDouble()) + maxRadius;
                maxX = Math.Min(maxX, mapMaxX);
                var minY = sLines.Min(line => Fixed.Min(line.Vertex1.Y, line.Vertex2.Y).ToDouble()) - maxRadius;
                minY = Math.Max(minY, mapMinY);
                var maxY = sLines.Max(line => Fixed.Max(line.Vertex1.Y, line.Vertex2.Y).ToDouble()) + maxRadius;
                maxY = Math.Min(maxY, mapMaxY);

                var bboxTop = (map.BlockMap.OriginY + BlockMap.BlockSize * (sector.BlockBox[Box.Top] + 1)).ToDouble();
                var bboxBottom = (map.BlockMap.OriginY + BlockMap.BlockSize * sector.BlockBox[Box.Bottom]).ToDouble();
                var bboxLeft = (map.BlockMap.OriginX + BlockMap.BlockSize * sector.BlockBox[Box.Left]).ToDouble();
                var bboxRight = (map.BlockMap.OriginX + BlockMap.BlockSize * (sector.BlockBox[Box.Right] + 1)).ToDouble();

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

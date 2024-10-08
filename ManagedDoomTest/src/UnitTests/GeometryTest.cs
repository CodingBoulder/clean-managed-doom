﻿using ManagedDoom;
using System;
using Xunit;

namespace ManagedDoomTest.UnitTests
{
    public class GeometryTest
    {
        [Fact]
        public void PointOnSide1()
        {
            var random = new Random(666);
            for (int i = 0; i < 1000; i++)
            {
                double startX = -1 - 666 * random.NextDouble();
                double endX = +1 + 666 * random.NextDouble();

                double pointX = 666 * random.NextDouble() - 333;
                double frontSideY = -1 - 666 * random.NextDouble();
                double backSideY = -frontSideY;

                var node = new Node(
                    Fixed.FromDouble(startX),
                    Fixed.Zero,
                    Fixed.FromDouble(endX - startX),
                    Fixed.Zero,
                    Fixed.Zero, Fixed.Zero, Fixed.Zero, Fixed.Zero,
                    Fixed.Zero, Fixed.Zero, Fixed.Zero, Fixed.Zero,
                    0, 0);

                var x = Fixed.FromDouble(pointX);
                {
                    var y = Fixed.FromDouble(frontSideY);
                    Assert.Equal(0, Geometry.PointOnSide(x, y, node));
                }
                {
                    var y = Fixed.FromDouble(backSideY);
                    Assert.Equal(1, Geometry.PointOnSide(x, y, node));
                }
            }
        }

        [Fact]
        public void PointOnSide2()
        {
            var random = new Random(666);
            for (int i = 0; i < 1000; i++)
            {
                double startY = +1 + 666 * random.NextDouble();
                double endY = -1 - 666 * random.NextDouble();

                double pointY = 666 * random.NextDouble() - 333;
                double frontSideX = -1 - 666 * random.NextDouble();
                double backSideX = -frontSideX;

                var node = new Node(
                    Fixed.Zero,
                    Fixed.FromDouble(startY),
                    Fixed.Zero,
                    Fixed.FromDouble(endY - startY),
                    Fixed.Zero, Fixed.Zero, Fixed.Zero, Fixed.Zero,
                    Fixed.Zero, Fixed.Zero, Fixed.Zero, Fixed.Zero,
                    0, 0);

                var y = Fixed.FromDouble(pointY);
                {
                    var x = Fixed.FromDouble(frontSideX);
                    Assert.Equal(0, Geometry.PointOnSide(x, y, node));
                }
                {
                    var x = Fixed.FromDouble(backSideX);
                    Assert.Equal(1, Geometry.PointOnSide(x, y, node));
                }
            }
        }

        [Fact]
        public void PointOnSide3()
        {
            var random = new Random(666);
            for (int i = 0; i < 1000; i++)
            {
                double startX = -1 - 666 * random.NextDouble();
                double endX = +1 + 666 * random.NextDouble();

                double pointX = 666 * random.NextDouble() - 333;
                double frontSideY = -1 - 666 * random.NextDouble();
                double backSideY = -frontSideY;

                for (int j = 0; j < 100; j++)
                {
                    double theta = 2 * Math.PI * random.NextDouble();
                    double ox = 666 * random.NextDouble() - 333;
                    double oy = 666 * random.NextDouble() - 333;

                    var node = new Node(
                        Fixed.FromDouble(ox + startX * Math.Cos(theta)),
                        Fixed.FromDouble(oy + startX * Math.Sin(theta)),
                        Fixed.FromDouble((endX - startX) * Math.Cos(theta)),
                        Fixed.FromDouble((endX - startX) * Math.Sin(theta)),
                        Fixed.Zero, Fixed.Zero, Fixed.Zero, Fixed.Zero,
                        Fixed.Zero, Fixed.Zero, Fixed.Zero, Fixed.Zero,
                        0, 0);

                    {
                        var x = Fixed.FromDouble(ox + pointX * Math.Cos(theta) - frontSideY * Math.Sin(theta));
                        var y = Fixed.FromDouble(oy + pointX * Math.Sin(theta) + frontSideY * Math.Cos(theta));
                        Assert.Equal(0, Geometry.PointOnSide(x, y, node));
                    }
                    {
                        var x = Fixed.FromDouble(ox + pointX * Math.Cos(theta) - backSideY * Math.Sin(theta));
                        var y = Fixed.FromDouble(oy + pointX * Math.Sin(theta) + backSideY * Math.Cos(theta));
                        Assert.Equal(1, Geometry.PointOnSide(x, y, node));
                    }
                }
            }
        }

        [Fact]
        public void PointToDist()
        {
            var random = new Random(666);
            for (int i = 0; i < 1000; i += 3)
            {
                int expected = i;
                for (int j = 0; j < 100; j++)
                {
                    int r = i;
                    double theta = 2 * Math.PI * random.NextDouble();
                    double ox = 666 * random.NextDouble() - 333;
                    double oy = 666 * random.NextDouble() - 333;
                    double x = ox + r * Math.Cos(theta);
                    double y = oy + r * Math.Sin(theta);
                    var fromX = Fixed.FromDouble(ox);
                    var fromY = Fixed.FromDouble(oy);
                    var toX = Fixed.FromDouble(x);
                    var toY = Fixed.FromDouble(y);
                    Fixed dist = Geometry.PointToDist(fromX, fromY, toX, toY);
                    Assert.Equal(expected, dist.ToDouble(), (double)i / 100);
                }
            }
        }

        [Fact]
        public void PointToAngle()
        {
            var random = new Random(666);
            for (int i = 0; i < 100; i++)
            {
                double expected = 2 * Math.PI * random.NextDouble();
                for (int j = 0; j < 100; j++)
                {
                    double r = 666 * random.NextDouble();
                    double ox = 666 * random.NextDouble() - 333;
                    double oy = 666 * random.NextDouble() - 333;
                    double x = ox + r * Math.Cos(expected);
                    double y = oy + r * Math.Sin(expected);
                    var fromX = Fixed.FromDouble(ox);
                    var fromY = Fixed.FromDouble(oy);
                    var toX = Fixed.FromDouble(x);
                    var toY = Fixed.FromDouble(y);
                    Angle angle = Geometry.PointToAngle(fromX, fromY, toX, toY);
                    double actual = angle.ToRadian();
                    Assert.Equal(expected, actual, 0.01);
                }
            }
        }

        [Fact]
        public void PointInSubsectorE1M1()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom1);
            var options = new GameOptions();
            var world = new World(content, options, null);
            var map = new Map(content, world);

            int ok = 0;
            int count = 0;

            foreach (Subsector subsector in map.Subsectors)
            {
                for (int i = 0; i < subsector.SegCount; i++)
                {
                    Seg seg = map.Segs[subsector.FirstSeg + i];

                    double p1x = seg.Vertex1.X.ToDouble();
                    double p1y = seg.Vertex1.Y.ToDouble();
                    double p2x = seg.Vertex2.X.ToDouble();
                    double p2y = seg.Vertex2.Y.ToDouble();

                    double dx = p2x - p1x;
                    double dy = p2y - p1y;
                    double length = Math.Sqrt(dx * dx + dy * dy);

                    double centerX = (p1x + p2x) / 2;
                    double centerY = (p1y + p2y) / 2;
                    double stepX = dy / length;
                    double stepY = -dx / length;

                    double targetX = centerX + 3 * stepX;
                    double targetY = centerY + 3 * stepY;

                    var fx = Fixed.FromDouble(targetX);
                    var fy = Fixed.FromDouble(targetY);

                    Subsector result = Geometry.PointInSubsector(fx, fy, map);

                    if (result == subsector)
                    {
                        ok++;
                    }
                    count++;
                }
            }

            Assert.True((double)ok / count >= 0.995);
        }

        [Fact]
        public void PointInSubsectorMap01()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2);
            var options = new GameOptions();
            var world = new World(content, options, null);
            var map = new Map(content, world);

            int ok = 0;
            int count = 0;

            foreach (Subsector subsector in map.Subsectors)
            {
                for (int i = 0; i < subsector.SegCount; i++)
                {
                    Seg seg = map.Segs[subsector.FirstSeg + i];

                    double p1x = seg.Vertex1.X.ToDouble();
                    double p1y = seg.Vertex1.Y.ToDouble();
                    double p2x = seg.Vertex2.X.ToDouble();
                    double p2y = seg.Vertex2.Y.ToDouble();

                    double dx = p2x - p1x;
                    double dy = p2y - p1y;
                    double length = Math.Sqrt(dx * dx + dy * dy);

                    double centerX = (p1x + p2x) / 2;
                    double centerY = (p1y + p2y) / 2;
                    double stepX = dy / length;
                    double stepY = -dx / length;

                    double targetX = centerX + 3 * stepX;
                    double targetY = centerY + 3 * stepY;

                    var fx = Fixed.FromDouble(targetX);
                    var fy = Fixed.FromDouble(targetY);

                    Subsector result = Geometry.PointInSubsector(fx, fy, map);

                    if (result == subsector)
                    {
                        ok++;
                    }
                    count++;
                }
            }

            Assert.True((double)ok / count >= 0.995);
        }

        [Fact]
        public void PointOnSegSide1()
        {
            var random = new Random(666);
            for (int i = 0; i < 1000; i++)
            {
                double startX = -1 - 666 * random.NextDouble();
                double endX = +1 + 666 * random.NextDouble();

                double pointX = 666 * random.NextDouble() - 333;
                double frontSideY = -1 - 666 * random.NextDouble();
                double backSideY = -frontSideY;

                var vertex1 = new Vertex(Fixed.FromDouble(startX), Fixed.Zero);
                var vertex2 = new Vertex(Fixed.FromDouble(endX - startX), Fixed.Zero);

                var seg = new Seg(
                    vertex1,
                    vertex2,
                    Fixed.Zero, Angle.Ang0, null, null, null, null);

                var x = Fixed.FromDouble(pointX);
                {
                    var y = Fixed.FromDouble(frontSideY);
                    Assert.Equal(0, Geometry.PointOnSegSide(x, y, seg));
                }
                {
                    var y = Fixed.FromDouble(backSideY);
                    Assert.Equal(1, Geometry.PointOnSegSide(x, y, seg));
                }
            }
        }

        [Fact]
        public void PointOnSegSide2()
        {
            var random = new Random(666);
            for (int i = 0; i < 1000; i++)
            {
                double startY = +1 + 666 * random.NextDouble();
                double endY = -1 - 666 * random.NextDouble();

                double pointY = 666 * random.NextDouble() - 333;
                double frontSideX = -1 - 666 * random.NextDouble();
                double backSideX = -frontSideX;

                var vertex1 = new Vertex(Fixed.Zero, Fixed.FromDouble(startY));
                var vertex2 = new Vertex(Fixed.Zero, Fixed.FromDouble(endY - startY));

                var seg = new Seg(
                    vertex1,
                    vertex2,
                    Fixed.Zero, Angle.Ang0, null, null, null, null);

                var y = Fixed.FromDouble(pointY);
                {
                    var x = Fixed.FromDouble(frontSideX);
                    Assert.Equal(0, Geometry.PointOnSegSide(x, y, seg));
                }
                {
                    var x = Fixed.FromDouble(backSideX);
                    Assert.Equal(1, Geometry.PointOnSegSide(x, y, seg));
                }
            }
        }

        [Fact]
        public void PointOnSegSide3()
        {
            var random = new Random(666);
            for (int i = 0; i < 1000; i++)
            {
                double startX = -1 - 666 * random.NextDouble();
                double endX = +1 + 666 * random.NextDouble();

                double pointX = 666 * random.NextDouble() - 333;
                double frontSideY = -1 - 666 * random.NextDouble();
                double backSideY = -frontSideY;

                for (int j = 0; j < 100; j++)
                {
                    double theta = 2 * Math.PI * random.NextDouble();
                    double ox = 666 * random.NextDouble() - 333;
                    double oy = 666 * random.NextDouble() - 333;

                    var vertex1 = new Vertex(
                        Fixed.FromDouble(ox + startX * Math.Cos(theta)),
                        Fixed.FromDouble(oy + startX * Math.Sin(theta)));

                    var vertex2 = new Vertex(
                        vertex1.X + Fixed.FromDouble((endX - startX) * Math.Cos(theta)),
                        vertex1.Y + Fixed.FromDouble((endX - startX) * Math.Sin(theta)));

                    var seg = new Seg(
                        vertex1,
                        vertex2,
                        Fixed.Zero, Angle.Ang0, null, null, null, null);

                    {
                        var x = Fixed.FromDouble(ox + pointX * Math.Cos(theta) - frontSideY * Math.Sin(theta));
                        var y = Fixed.FromDouble(oy + pointX * Math.Sin(theta) + frontSideY * Math.Cos(theta));
                        Assert.Equal(0, Geometry.PointOnSegSide(x, y, seg));
                    }
                    {
                        var x = Fixed.FromDouble(ox + pointX * Math.Cos(theta) - backSideY * Math.Sin(theta));
                        var y = Fixed.FromDouble(oy + pointX * Math.Sin(theta) + backSideY * Math.Cos(theta));
                        Assert.Equal(1, Geometry.PointOnSegSide(x, y, seg));
                    }
                }
            }
        }

        [Fact]
        public void PointOnLineSide1()
        {
            var random = new Random(666);
            for (int i = 0; i < 1000; i++)
            {
                double startX = -1 - 666 * random.NextDouble();
                double endX = +1 + 666 * random.NextDouble();

                double pointX = 666 * random.NextDouble() - 333;
                double frontSideY = -1 - 666 * random.NextDouble();
                double backSideY = -frontSideY;

                var vertex1 = new Vertex(Fixed.FromDouble(startX), Fixed.Zero);
                var vertex2 = new Vertex(Fixed.FromDouble(endX - startX), Fixed.Zero);

                var line = new LineDef(
                    vertex1,
                    vertex2,
                    0, 0, 0, null, null);

                var x = Fixed.FromDouble(pointX);
                {
                    var y = Fixed.FromDouble(frontSideY);
                    Assert.Equal(0, Geometry.PointOnLineSide(x, y, line));
                }
                {
                    var y = Fixed.FromDouble(backSideY);
                    Assert.Equal(1, Geometry.PointOnLineSide(x, y, line));
                }
            }
        }

        [Fact]
        public void PointOnLineSide2()
        {
            var random = new Random(666);
            for (int i = 0; i < 1000; i++)
            {
                double startY = +1 + 666 * random.NextDouble();
                double endY = -1 - 666 * random.NextDouble();

                double pointY = 666 * random.NextDouble() - 333;
                double frontSideX = -1 - 666 * random.NextDouble();
                double backSideX = -frontSideX;

                var vertex1 = new Vertex(Fixed.Zero, Fixed.FromDouble(startY));
                var vertex2 = new Vertex(Fixed.Zero, Fixed.FromDouble(endY - startY));

                var line = new LineDef(
                    vertex1,
                    vertex2,
                    0, 0, 0, null, null);

                var y = Fixed.FromDouble(pointY);
                {
                    var x = Fixed.FromDouble(frontSideX);
                    Assert.Equal(0, Geometry.PointOnLineSide(x, y, line));
                }
                {
                    var x = Fixed.FromDouble(backSideX);
                    Assert.Equal(1, Geometry.PointOnLineSide(x, y, line));
                }
            }
        }

        [Fact]
        public void PointOnLineSide3()
        {
            var random = new Random(666);
            for (int i = 0; i < 1000; i++)
            {
                double startX = -1 - 666 * random.NextDouble();
                double endX = +1 + 666 * random.NextDouble();

                double pointX = 666 * random.NextDouble() - 333;
                double frontSideY = -1 - 666 * random.NextDouble();
                double backSideY = -frontSideY;

                for (int j = 0; j < 100; j++)
                {
                    double theta = 2 * Math.PI * random.NextDouble();
                    double ox = 666 * random.NextDouble() - 333;
                    double oy = 666 * random.NextDouble() - 333;

                    var vertex1 = new Vertex(
                        Fixed.FromDouble(ox + startX * Math.Cos(theta)),
                        Fixed.FromDouble(oy + startX * Math.Sin(theta)));

                    var vertex2 = new Vertex(
                        vertex1.X + Fixed.FromDouble((endX - startX) * Math.Cos(theta)),
                        vertex1.Y + Fixed.FromDouble((endX - startX) * Math.Sin(theta)));

                    var line = new LineDef(
                        vertex1,
                        vertex2,
                        0, 0, 0, null, null);

                    {
                        var x = Fixed.FromDouble(ox + pointX * Math.Cos(theta) - frontSideY * Math.Sin(theta));
                        var y = Fixed.FromDouble(oy + pointX * Math.Sin(theta) + frontSideY * Math.Cos(theta));
                        Assert.Equal(0, Geometry.PointOnLineSide(x, y, line));
                    }
                    {
                        var x = Fixed.FromDouble(ox + pointX * Math.Cos(theta) - backSideY * Math.Sin(theta));
                        var y = Fixed.FromDouble(oy + pointX * Math.Sin(theta) + backSideY * Math.Cos(theta));
                        Assert.Equal(1, Geometry.PointOnLineSide(x, y, line));
                    }
                }
            }
        }

        [Fact]
        public void BoxOnLineSide1()
        {
            var random = new Random(666);
            for (int i = 0; i < 1000; i++)
            {
                double radius = 33 + 33 * random.NextDouble();

                double startX = -1 - 666 * random.NextDouble();
                double endX = +1 + 666 * random.NextDouble();

                double pointX = 666 * random.NextDouble() - 333;
                double frontSideY = -1 - radius - 666 * random.NextDouble();
                double backSideY = -frontSideY;
                double crossingY = radius * 1.9 * (random.NextDouble() - 0.5);

                var frontBox = new Fixed[]
                {
                    Fixed.FromDouble(frontSideY + radius),
                    Fixed.FromDouble(frontSideY - radius),
                    Fixed.FromDouble(pointX - radius),
                    Fixed.FromDouble(pointX + radius)
                };

                var backBox = new Fixed[]
                {
                    Fixed.FromDouble(backSideY + radius),
                    Fixed.FromDouble(backSideY - radius),
                    Fixed.FromDouble(pointX - radius),
                    Fixed.FromDouble(pointX + radius)
                };

                var crossingBox = new Fixed[]
                {
                    Fixed.FromDouble(crossingY + radius),
                    Fixed.FromDouble(crossingY - radius),
                    Fixed.FromDouble(pointX - radius),
                    Fixed.FromDouble(pointX + radius)
                };

                var vertex1 = new Vertex(Fixed.FromDouble(startX), Fixed.Zero);
                var vertex2 = new Vertex(Fixed.FromDouble(endX - startX), Fixed.Zero);

                var line = new LineDef(
                    vertex1,
                    vertex2,
                    0, 0, 0, null, null);

                Assert.Equal(0, Geometry.BoxOnLineSide(frontBox, line));
                Assert.Equal(1, Geometry.BoxOnLineSide(backBox, line));
                Assert.Equal(-1, Geometry.BoxOnLineSide(crossingBox, line));
            }
        }

        [Fact]
        public void BoxOnLineSide2()
        {
            var random = new Random(666);
            for (int i = 0; i < 1000; i++)
            {
                double radius = 33 + 33 * random.NextDouble();

                double startY = +1 + 666 * random.NextDouble();
                double endY = -1 - 666 * random.NextDouble();

                double pointY = 666 * random.NextDouble() - 333;
                double frontSideX = -1 - radius - 666 * random.NextDouble();
                double backSideX = -frontSideX;
                double crossingX = radius * 1.9 * (random.NextDouble() - 0.5);

                var frontBox = new Fixed[]
                {
                    Fixed.FromDouble(pointY + radius),
                    Fixed.FromDouble(pointY - radius),
                    Fixed.FromDouble(frontSideX - radius),
                    Fixed.FromDouble(frontSideX + radius)
                };

                var backBox = new Fixed[]
                {
                    Fixed.FromDouble(pointY + radius),
                    Fixed.FromDouble(pointY - radius),
                    Fixed.FromDouble(backSideX - radius),
                    Fixed.FromDouble(backSideX + radius)
                };

                var crossingBox = new Fixed[]
                {
                    Fixed.FromDouble(pointY + radius),
                    Fixed.FromDouble(pointY - radius),
                    Fixed.FromDouble(crossingX - radius),
                    Fixed.FromDouble(crossingX + radius)
                };

                var vertex1 = new Vertex(Fixed.Zero, Fixed.FromDouble(startY));
                var vertex2 = new Vertex(Fixed.Zero, Fixed.FromDouble(endY - startY));

                var line = new LineDef(
                    vertex1,
                    vertex2,
                    0, 0, 0, null, null);

                Assert.Equal(0, Geometry.BoxOnLineSide(frontBox, line));
                Assert.Equal(1, Geometry.BoxOnLineSide(backBox, line));
                Assert.Equal(-1, Geometry.BoxOnLineSide(crossingBox, line));
            }
        }

        [Fact]
        public void BoxOnLineSide3()
        {
            var random = new Random(666);
            for (int i = 0; i < 1000; i++)
            {
                double radius = 33 + 33 * random.NextDouble();

                double startX = -1 - 666 * random.NextDouble();
                double endX = +1 + 666 * random.NextDouble();

                double pointX = 666 * random.NextDouble() - 333;
                double frontSideY = -1 - 1.5 * radius - 666 * random.NextDouble();
                double backSideY = -frontSideY;
                double crossingY = radius * 1.9 * (random.NextDouble() - 0.5);

                for (int j = 0; j < 100; j++)
                {
                    double theta = 2 * Math.PI * random.NextDouble();
                    double ox = 666 * random.NextDouble() - 333;
                    double oy = 666 * random.NextDouble() - 333;

                    var frontBox = new Fixed[]
                    {
                        Fixed.FromDouble(oy + pointX * Math.Sin(theta) + frontSideY * Math.Cos(theta) + radius),
                        Fixed.FromDouble(oy + pointX * Math.Sin(theta) + frontSideY * Math.Cos(theta) - radius),
                        Fixed.FromDouble(ox + pointX * Math.Cos(theta) - frontSideY * Math.Sin(theta) - radius),
                        Fixed.FromDouble(ox + pointX * Math.Cos(theta) - frontSideY * Math.Sin(theta) + radius)
                    };

                    var backBox = new Fixed[]
                    {
                        Fixed.FromDouble(oy + pointX * Math.Sin(theta) + backSideY * Math.Cos(theta) + radius),
                        Fixed.FromDouble(oy + pointX * Math.Sin(theta) + backSideY * Math.Cos(theta) - radius),
                        Fixed.FromDouble(ox + pointX * Math.Cos(theta) - backSideY * Math.Sin(theta) - radius),
                        Fixed.FromDouble(ox + pointX * Math.Cos(theta) - backSideY * Math.Sin(theta) + radius)
                    };

                    var crossingBox = new Fixed[]
                    {
                        Fixed.FromDouble(oy + pointX * Math.Sin(theta) + crossingY * Math.Cos(theta) + radius),
                        Fixed.FromDouble(oy + pointX * Math.Sin(theta) + crossingY * Math.Cos(theta) - radius),
                        Fixed.FromDouble(ox + pointX * Math.Cos(theta) - crossingY * Math.Sin(theta) - radius),
                        Fixed.FromDouble(ox + pointX * Math.Cos(theta) - crossingY * Math.Sin(theta) + radius)
                    };

                    var vertex1 = new Vertex(
                        Fixed.FromDouble(ox + startX * Math.Cos(theta)),
                        Fixed.FromDouble(oy + startX * Math.Sin(theta)));

                    var vertex2 = new Vertex(
                        vertex1.X + Fixed.FromDouble((endX - startX) * Math.Cos(theta)),
                        vertex1.Y + Fixed.FromDouble((endX - startX) * Math.Sin(theta)));

                    var line = new LineDef(
                        vertex1,
                        vertex2,
                        0, 0, 0, null, null);

                    Assert.Equal(0, Geometry.BoxOnLineSide(frontBox, line));
                    Assert.Equal(1, Geometry.BoxOnLineSide(backBox, line));
                    Assert.Equal(-1, Geometry.BoxOnLineSide(crossingBox, line));
                }
            }
        }

        [Fact]
        public void PointOnDivLineSide1()
        {
            var random = new Random(666);
            for (int i = 0; i < 1000; i++)
            {
                double startX = -1 - 666 * random.NextDouble();
                double endX = +1 + 666 * random.NextDouble();

                double pointX = 666 * random.NextDouble() - 333;
                double frontSideY = -1 - 666 * random.NextDouble();
                double backSideY = -frontSideY;

                var vertex1 = new Vertex(Fixed.FromDouble(startX), Fixed.Zero);
                var vertex2 = new Vertex(Fixed.FromDouble(endX - startX), Fixed.Zero);

                var line = new LineDef(
                    vertex1,
                    vertex2,
                    0, 0, 0, null, null);

                var divLine = new DivLine();
                divLine.MakeFrom(line);

                var x = Fixed.FromDouble(pointX);
                {
                    var y = Fixed.FromDouble(frontSideY);
                    Assert.Equal(0, Geometry.PointOnDivLineSide(x, y, divLine));
                }
                {
                    var y = Fixed.FromDouble(backSideY);
                    Assert.Equal(1, Geometry.PointOnDivLineSide(x, y, divLine));
                }
            }
        }

        [Fact]
        public void PointOnDivLineSide2()
        {
            var random = new Random(666);
            for (int i = 0; i < 1000; i++)
            {
                double startY = +1 + 666 * random.NextDouble();
                double endY = -1 - 666 * random.NextDouble();

                double pointY = 666 * random.NextDouble() - 333;
                double frontSideX = -1 - 666 * random.NextDouble();
                double backSideX = -frontSideX;

                var vertex1 = new Vertex(Fixed.Zero, Fixed.FromDouble(startY));
                var vertex2 = new Vertex(Fixed.Zero, Fixed.FromDouble(endY - startY));

                var line = new LineDef(
                    vertex1,
                    vertex2,
                    0, 0, 0, null, null);

                var divLine = new DivLine();
                divLine.MakeFrom(line);

                var y = Fixed.FromDouble(pointY);
                {
                    var x = Fixed.FromDouble(frontSideX);
                    Assert.Equal(0, Geometry.PointOnDivLineSide(x, y, divLine));
                }
                {
                    var x = Fixed.FromDouble(backSideX);
                    Assert.Equal(1, Geometry.PointOnDivLineSide(x, y, divLine));
                }
            }
        }

        [Fact]
        public void PointOnDivLineSide3()
        {
            var random = new Random(666);
            for (int i = 0; i < 1000; i++)
            {
                double startX = -1 - 666 * random.NextDouble();
                double endX = +1 + 666 * random.NextDouble();

                double pointX = 666 * random.NextDouble() - 333;
                double frontSideY = -1 - 666 * random.NextDouble();
                double backSideY = -frontSideY;

                for (int j = 0; j < 100; j++)
                {
                    double theta = 2 * Math.PI * random.NextDouble();
                    double ox = 666 * random.NextDouble() - 333;
                    double oy = 666 * random.NextDouble() - 333;

                    var vertex1 = new Vertex(
                        Fixed.FromDouble(ox + startX * Math.Cos(theta)),
                        Fixed.FromDouble(oy + startX * Math.Sin(theta)));

                    var vertex2 = new Vertex(
                        vertex1.X + Fixed.FromDouble((endX - startX) * Math.Cos(theta)),
                        vertex1.Y + Fixed.FromDouble((endX - startX) * Math.Sin(theta)));

                    var line = new LineDef(
                        vertex1,
                        vertex2,
                        0, 0, 0, null, null);

                    var divLine = new DivLine();
                    divLine.MakeFrom(line);

                    {
                        var x = Fixed.FromDouble(ox + pointX * Math.Cos(theta) - frontSideY * Math.Sin(theta));
                        var y = Fixed.FromDouble(oy + pointX * Math.Sin(theta) + frontSideY * Math.Cos(theta));
                        Assert.Equal(0, Geometry.PointOnDivLineSide(x, y, divLine));
                    }
                    {
                        var x = Fixed.FromDouble(ox + pointX * Math.Cos(theta) - backSideY * Math.Sin(theta));
                        var y = Fixed.FromDouble(oy + pointX * Math.Sin(theta) + backSideY * Math.Cos(theta));
                        Assert.Equal(1, Geometry.PointOnDivLineSide(x, y, divLine));
                    }
                }
            }
        }

        [Fact]
        public void AproxDistance()
        {
            var random = new Random(666);
            for (int i = 0; i < 1000; i++)
            {
                double dx = 666 * random.NextDouble() - 333;
                double dy = 666 * random.NextDouble() - 333;

                double adx = Math.Abs(dx);
                double ady = Math.Abs(dy);
                double expected = adx + ady - Math.Min(adx, ady) / 2;

                Fixed actual = Geometry.AproxDistance(Fixed.FromDouble(dx), Fixed.FromDouble(dy));

                Assert.Equal(expected, actual.ToDouble(), 1.0E-3);
            }
        }

        [Fact]
        public void DivLineSide1()
        {
            var random = new Random(666);
            for (int i = 0; i < 1000; i++)
            {
                double startX = -1 - 666 * random.NextDouble();
                double endX = +1 + 666 * random.NextDouble();

                double pointX = 666 * random.NextDouble() - 333;
                double frontSideY = -1 - 666 * random.NextDouble();
                double backSideY = -frontSideY;

                var vertex1 = new Vertex(Fixed.FromDouble(startX), Fixed.Zero);
                var vertex2 = new Vertex(Fixed.FromDouble(endX - startX), Fixed.Zero);

                var line = new LineDef(
                    vertex1,
                    vertex2,
                    0, 0, 0, null, null);

                var divLine = new DivLine();
                divLine.MakeFrom(line);

                var node = new Node(
                    Fixed.FromDouble(startX),
                    Fixed.Zero,
                    Fixed.FromDouble(endX - startX),
                    Fixed.Zero,
                    Fixed.Zero, Fixed.Zero, Fixed.Zero, Fixed.Zero,
                    Fixed.Zero, Fixed.Zero, Fixed.Zero, Fixed.Zero,
                    0, 0);

                var x = Fixed.FromDouble(pointX);
                {
                    var y = Fixed.FromDouble(frontSideY);
                    Assert.Equal(0, Geometry.DivLineSide(x, y, divLine));
                    Assert.Equal(0, Geometry.DivLineSide(x, y, node));
                }
                {
                    var y = Fixed.FromDouble(backSideY);
                    Assert.Equal(1, Geometry.DivLineSide(x, y, divLine));
                    Assert.Equal(1, Geometry.DivLineSide(x, y, node));
                }
            }
        }

        [Fact]
        public void DivLineSide2()
        {
            var random = new Random(666);
            for (int i = 0; i < 1000; i++)
            {
                double startY = +1 + 666 * random.NextDouble();
                double endY = -1 - 666 * random.NextDouble();

                double pointY = 666 * random.NextDouble() - 333;
                double frontSideX = -1 - 666 * random.NextDouble();
                double backSideX = -frontSideX;

                var vertex1 = new Vertex(Fixed.Zero, Fixed.FromDouble(startY));
                var vertex2 = new Vertex(Fixed.Zero, Fixed.FromDouble(endY - startY));

                var line = new LineDef(
                    vertex1,
                    vertex2,
                    0, 0, 0, null, null);

                var divLine = new DivLine();
                divLine.MakeFrom(line);

                var node = new Node(
                    Fixed.Zero,
                    Fixed.FromDouble(startY),
                    Fixed.Zero,
                    Fixed.FromDouble(endY - startY),
                    Fixed.Zero, Fixed.Zero, Fixed.Zero, Fixed.Zero,
                    Fixed.Zero, Fixed.Zero, Fixed.Zero, Fixed.Zero,
                    0, 0);

                var y = Fixed.FromDouble(pointY);
                {
                    var x = Fixed.FromDouble(frontSideX);
                    Assert.Equal(0, Geometry.DivLineSide(x, y, divLine));
                    Assert.Equal(0, Geometry.DivLineSide(x, y, node));
                }
                {
                    var x = Fixed.FromDouble(backSideX);
                    Assert.Equal(1, Geometry.DivLineSide(x, y, divLine));
                    Assert.Equal(1, Geometry.DivLineSide(x, y, node));
                }
            }
        }

        [Fact]
        public void DivLineSide3()
        {
            var random = new Random(666);
            for (int i = 0; i < 1000; i++)
            {
                double startX = -1 - 666 * random.NextDouble();
                double endX = +1 + 666 * random.NextDouble();

                double pointX = 666 * random.NextDouble() - 333;
                double frontSideY = -1 - 666 * random.NextDouble();
                double backSideY = -frontSideY;

                for (int j = 0; j < 100; j++)
                {
                    double theta = 2 * Math.PI * random.NextDouble();
                    double ox = 666 * random.NextDouble() - 333;
                    double oy = 666 * random.NextDouble() - 333;

                    var vertex1 = new Vertex(
                        Fixed.FromDouble(ox + startX * Math.Cos(theta)),
                        Fixed.FromDouble(oy + startX * Math.Sin(theta)));

                    var vertex2 = new Vertex(
                        vertex1.X + Fixed.FromDouble((endX - startX) * Math.Cos(theta)),
                        vertex1.Y + Fixed.FromDouble((endX - startX) * Math.Sin(theta)));

                    var line = new LineDef(
                        vertex1,
                        vertex2,
                        0, 0, 0, null, null);

                    var divLine = new DivLine();
                    divLine.MakeFrom(line);

                    var node = new Node(
                        Fixed.FromDouble(ox + startX * Math.Cos(theta)),
                        Fixed.FromDouble(oy + startX * Math.Sin(theta)),
                        Fixed.FromDouble((endX - startX) * Math.Cos(theta)),
                        Fixed.FromDouble((endX - startX) * Math.Sin(theta)),
                        Fixed.Zero, Fixed.Zero, Fixed.Zero, Fixed.Zero,
                        Fixed.Zero, Fixed.Zero, Fixed.Zero, Fixed.Zero,
                        0, 0);

                    {
                        var x = Fixed.FromDouble(ox + pointX * Math.Cos(theta) - frontSideY * Math.Sin(theta));
                        var y = Fixed.FromDouble(oy + pointX * Math.Sin(theta) + frontSideY * Math.Cos(theta));
                        Assert.Equal(0, Geometry.DivLineSide(x, y, divLine));
                        Assert.Equal(0, Geometry.DivLineSide(x, y, node));
                    }
                    {
                        var x = Fixed.FromDouble(ox + pointX * Math.Cos(theta) - backSideY * Math.Sin(theta));
                        var y = Fixed.FromDouble(oy + pointX * Math.Sin(theta) + backSideY * Math.Cos(theta));
                        Assert.Equal(1, Geometry.DivLineSide(x, y, divLine));
                        Assert.Equal(1, Geometry.DivLineSide(x, y, node));
                    }
                }
            }
        }
    }
}

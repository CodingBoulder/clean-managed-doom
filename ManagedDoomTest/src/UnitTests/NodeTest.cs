using ManagedDoom;
using Xunit;

namespace ManagedDoomTest.UnitTests
{
    public class NodeTest
    {
        private const double _delta = 1.0E-9;

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
            Seg[] segs = Seg.FromWad(wad, map + 5, vertices, lines);
            Subsector[] subsectors = Subsector.FromWad(wad, map + 6, segs);
            Node[] nodes = Node.FromWad(wad, map + 7);

            Assert.Equal(238, nodes.Length);

            Assert.Equal(1784, nodes[0].X.ToDouble(), _delta);
            Assert.Equal(-3448, nodes[0].Y.ToDouble(), _delta);
            Assert.Equal(-240, nodes[0].Dx.ToDouble(), _delta);
            Assert.Equal(64, nodes[0].Dy.ToDouble(), _delta);
            Assert.Equal(-3104, nodes[0].BoundingBox[0][Box.Top].ToDouble(), _delta);
            Assert.Equal(-3448, nodes[0].BoundingBox[0][Box.Bottom].ToDouble(), _delta);
            Assert.Equal(1520, nodes[0].BoundingBox[0][Box.Left].ToDouble(), _delta);
            Assert.Equal(2128, nodes[0].BoundingBox[0][Box.Right].ToDouble(), _delta);
            Assert.Equal(-3384, nodes[0].BoundingBox[1][Box.Top].ToDouble(), _delta);
            Assert.Equal(-3448, nodes[0].BoundingBox[1][Box.Bottom].ToDouble(), _delta);
            Assert.Equal(1544, nodes[0].BoundingBox[1][Box.Left].ToDouble(), _delta);
            Assert.Equal(1784, nodes[0].BoundingBox[1][Box.Right].ToDouble(), _delta);
            Assert.Equal(32768, nodes[0].Children[0] + 0x10000);
            Assert.Equal(32769, nodes[0].Children[1] + 0x10000);

            Assert.Equal(928, nodes[57].X.ToDouble(), _delta);
            Assert.Equal(-3360, nodes[57].Y.ToDouble(), _delta);
            Assert.Equal(0, nodes[57].Dx.ToDouble(), _delta);
            Assert.Equal(256, nodes[57].Dy.ToDouble(), _delta);
            Assert.Equal(-3104, nodes[57].BoundingBox[0][Box.Top].ToDouble(), _delta);
            Assert.Equal(-3360, nodes[57].BoundingBox[0][Box.Bottom].ToDouble(), _delta);
            Assert.Equal(928, nodes[57].BoundingBox[0][Box.Left].ToDouble(), _delta);
            Assert.Equal(1344, nodes[57].BoundingBox[0][Box.Right].ToDouble(), _delta);
            Assert.Equal(-3104, nodes[57].BoundingBox[1][Box.Top].ToDouble(), _delta);
            Assert.Equal(-3360, nodes[57].BoundingBox[1][Box.Bottom].ToDouble(), _delta);
            Assert.Equal(704, nodes[57].BoundingBox[1][Box.Left].ToDouble(), _delta);
            Assert.Equal(928, nodes[57].BoundingBox[1][Box.Right].ToDouble(), _delta);
            Assert.Equal(32825, nodes[57].Children[0] + 0x10000);
            Assert.Equal(56, nodes[57].Children[1]);

            Assert.Equal(2176, nodes[237].X.ToDouble(), _delta);
            Assert.Equal(-2304, nodes[237].Y.ToDouble(), _delta);
            Assert.Equal(0, nodes[237].Dx.ToDouble(), _delta);
            Assert.Equal(-256, nodes[237].Dy.ToDouble(), _delta);
            Assert.Equal(-2048, nodes[237].BoundingBox[0][Box.Top].ToDouble(), _delta);
            Assert.Equal(-4064, nodes[237].BoundingBox[0][Box.Bottom].ToDouble(), _delta);
            Assert.Equal(-768, nodes[237].BoundingBox[0][Box.Left].ToDouble(), _delta);
            Assert.Equal(2176, nodes[237].BoundingBox[0][Box.Right].ToDouble(), _delta);
            Assert.Equal(-2048, nodes[237].BoundingBox[1][Box.Top].ToDouble(), _delta);
            Assert.Equal(-4864, nodes[237].BoundingBox[1][Box.Bottom].ToDouble(), _delta);
            Assert.Equal(2176, nodes[237].BoundingBox[1][Box.Left].ToDouble(), _delta);
            Assert.Equal(3808, nodes[237].BoundingBox[1][Box.Right].ToDouble(), _delta);
            Assert.Equal(131, nodes[237].Children[0]);
            Assert.Equal(236, nodes[237].Children[1]);
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
            Seg[] segs = Seg.FromWad(wad, map + 5, vertices, lines);
            Subsector[] subsectors = Subsector.FromWad(wad, map + 6, segs);
            Node[] nodes = Node.FromWad(wad, map + 7);

            Assert.Equal(193, nodes.Length);

            Assert.Equal(64, nodes[0].X.ToDouble(), _delta);
            Assert.Equal(1024, nodes[0].Y.ToDouble(), _delta);
            Assert.Equal(0, nodes[0].Dx.ToDouble(), _delta);
            Assert.Equal(-64, nodes[0].Dy.ToDouble(), _delta);
            Assert.Equal(1173, nodes[0].BoundingBox[0][Box.Top].ToDouble(), _delta);
            Assert.Equal(960, nodes[0].BoundingBox[0][Box.Bottom].ToDouble(), _delta);
            Assert.Equal(-64, nodes[0].BoundingBox[0][Box.Left].ToDouble(), _delta);
            Assert.Equal(64, nodes[0].BoundingBox[0][Box.Right].ToDouble(), _delta);
            Assert.Equal(1280, nodes[0].BoundingBox[1][Box.Top].ToDouble(), _delta);
            Assert.Equal(1024, nodes[0].BoundingBox[1][Box.Bottom].ToDouble(), _delta);
            Assert.Equal(64, nodes[0].BoundingBox[1][Box.Left].ToDouble(), _delta);
            Assert.Equal(128, nodes[0].BoundingBox[1][Box.Right].ToDouble(), _delta);
            Assert.Equal(32770, nodes[0].Children[0] + 0x10000);
            Assert.Equal(32771, nodes[0].Children[1] + 0x10000);

            Assert.Equal(640, nodes[57].X.ToDouble(), _delta);
            Assert.Equal(856, nodes[57].Y.ToDouble(), _delta);
            Assert.Equal(-88, nodes[57].Dx.ToDouble(), _delta);
            Assert.Equal(-16, nodes[57].Dy.ToDouble(), _delta);
            Assert.Equal(856, nodes[57].BoundingBox[0][Box.Top].ToDouble(), _delta);
            Assert.Equal(840, nodes[57].BoundingBox[0][Box.Bottom].ToDouble(), _delta);
            Assert.Equal(552, nodes[57].BoundingBox[0][Box.Left].ToDouble(), _delta);
            Assert.Equal(640, nodes[57].BoundingBox[0][Box.Right].ToDouble(), _delta);
            Assert.Equal(856, nodes[57].BoundingBox[1][Box.Top].ToDouble(), _delta);
            Assert.Equal(760, nodes[57].BoundingBox[1][Box.Bottom].ToDouble(), _delta);
            Assert.Equal(536, nodes[57].BoundingBox[1][Box.Left].ToDouble(), _delta);
            Assert.Equal(704, nodes[57].BoundingBox[1][Box.Right].ToDouble(), _delta);
            Assert.Equal(32829, nodes[57].Children[0] + 0x10000);
            Assert.Equal(56, nodes[57].Children[1]);

            Assert.Equal(96, nodes[192].X.ToDouble(), _delta);
            Assert.Equal(1280, nodes[192].Y.ToDouble(), _delta);
            Assert.Equal(32, nodes[192].Dx.ToDouble(), _delta);
            Assert.Equal(0, nodes[192].Dy.ToDouble(), _delta);
            Assert.Equal(1280, nodes[192].BoundingBox[0][Box.Top].ToDouble(), _delta);
            Assert.Equal(-960, nodes[192].BoundingBox[0][Box.Bottom].ToDouble(), _delta);
            Assert.Equal(-1304, nodes[192].BoundingBox[0][Box.Left].ToDouble(), _delta);
            Assert.Equal(2072, nodes[192].BoundingBox[0][Box.Right].ToDouble(), _delta);
            Assert.Equal(2688, nodes[192].BoundingBox[1][Box.Top].ToDouble(), _delta);
            Assert.Equal(1280, nodes[192].BoundingBox[1][Box.Bottom].ToDouble(), _delta);
            Assert.Equal(-1304, nodes[192].BoundingBox[1][Box.Left].ToDouble(), _delta);
            Assert.Equal(2072, nodes[192].BoundingBox[1][Box.Right].ToDouble(), _delta);
            Assert.Equal(147, nodes[192].Children[0]);
            Assert.Equal(191, nodes[192].Children[1]);
        }
    }
}

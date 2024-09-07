using ManagedDoom;
using Xunit;

namespace ManagedDoomTest.UnitTests
{
    public class SubsectorTest
    {
        [Fact]
        public void LoadE1M1()
        {
            using var wad = new Wad(WadPath.Doom1);
            var flats = new DummyFlatLookup(wad);
            var textures = new DummyTextureLookup(wad);
            int map = wad.GetLumpNumber("E1M1");
            Vertex[] vertices = Vertex.FromWad(wad, map + 4);
            Sector[] sectors = Sector.FromWad(wad, map + 8, flats);
            SideDef[] sides = SideDef.FromWad(wad, map + 3, textures, sectors);
            LineDef[] lines = LineDef.FromWad(wad, map + 2, vertices, sides);
            Seg[] segs = Seg.FromWad(wad, map + 5, vertices, lines);
            Subsector[] subsectors = Subsector.FromWad(wad, map + 6, segs);

            Assert.Equal(239, subsectors.Length);

            Assert.Equal(8, subsectors[0].SegCount);
            for (int i = 0; i < 8; i++)
            {
                Assert.True(segs[subsectors[0].FirstSeg + i] == segs[0 + i]);
            }

            Assert.Equal(1, subsectors[54].SegCount);
            for (int i = 0; i < 1; i++)
            {
                Assert.True(segs[subsectors[54].FirstSeg + i] == segs[181 + i]);
            }

            Assert.Equal(2, subsectors[238].SegCount);
            for (int i = 0; i < 2; i++)
            {
                Assert.True(segs[subsectors[238].FirstSeg + i] == segs[745 + i]);
            }
        }

        [Fact]
        public void LoadMap01()
        {
            using var wad = new Wad(WadPath.Doom2);
            var flats = new DummyFlatLookup(wad);
            var textures = new DummyTextureLookup(wad);
            int map = wad.GetLumpNumber("MAP01");
            Vertex[] vertices = Vertex.FromWad(wad, map + 4);
            Sector[] sectors = Sector.FromWad(wad, map + 8, flats);
            SideDef[] sides = SideDef.FromWad(wad, map + 3, textures, sectors);
            LineDef[] lines = LineDef.FromWad(wad, map + 2, vertices, sides);
            Seg[] segs = Seg.FromWad(wad, map + 5, vertices, lines);
            Subsector[] subsectors = Subsector.FromWad(wad, map + 6, segs);

            Assert.Equal(194, subsectors.Length);

            Assert.Equal(4, subsectors[0].SegCount);
            for (int i = 0; i < 4; i++)
            {
                Assert.True(segs[subsectors[0].FirstSeg + i] == segs[i]);
            }

            Assert.Equal(4, subsectors[57].SegCount);
            for (int i = 0; i < 4; i++)
            {
                Assert.True(segs[subsectors[57].FirstSeg + i] == segs[179 + i]);
            }

            Assert.Equal(4, subsectors[193].SegCount);
            for (int i = 0; i < 4; i++)
            {
                Assert.True(segs[subsectors[193].FirstSeg + i] == segs[597 + i]);
            }
        }
    }
}

using ManagedDoom;
using System;
using Xunit;

namespace ManagedDoomTest.UnitTests
{
    public class SegTest
    {
        private const double delta = 1.0E-9;

        private static double ToRadian(int angle)
        {
            if (angle < 0)
            {
                angle += 0x10000;
            }
            return 2 * Math.PI * ((double)angle / 0x10000);
        }

        [Fact]
        public void LoadE1M1()
        {
            using var wad = new Wad(WadPath.Doom1);
            var flats = new DummyFlatLookup(wad);
            var textures = new DummyTextureLookup(wad);
            var map = wad.GetLumpNumber("E1M1");
            var vertices = Vertex.FromWad(wad, map + 4);
            var sectors = Sector.FromWad(wad, map + 8, flats);
            var sides = SideDef.FromWad(wad, map + 3, textures, sectors);
            var lines = LineDef.FromWad(wad, map + 2, vertices, sides);
            var segs = Seg.FromWad(wad, map + 5, vertices, lines);

            Assert.Equal(747, segs.Length);

            Assert.True(segs[0].Vertex1 == vertices[132]);
            Assert.True(segs[0].Vertex2 == vertices[133]);
            Assert.Equal(ToRadian(4156), segs[0].Angle.ToRadian(), delta);
            Assert.True(segs[0].LineDef == lines[160]);
            Assert.NotEqual(0, (int)(segs[0].LineDef.Flags & LineFlags.TwoSided));
            Assert.True(segs[0].FrontSector == segs[0].LineDef.FrontSide.Sector);
            Assert.True(segs[0].BackSector == segs[0].LineDef.BackSide.Sector);
            Assert.Equal(0, segs[0].Offset.ToDouble(), delta);

            Assert.True(segs[28].Vertex1 == vertices[390]);
            Assert.True(segs[28].Vertex2 == vertices[131]);
            Assert.Equal(ToRadian(-32768), segs[28].Angle.ToRadian(), delta);
            Assert.True(segs[28].LineDef == lines[480]);
            Assert.NotEqual(0, (int)(segs[0].LineDef.Flags & LineFlags.TwoSided));
            Assert.True(segs[28].FrontSector == segs[28].LineDef.BackSide.Sector);
            Assert.True(segs[28].BackSector == segs[28].LineDef.FrontSide.Sector);
            Assert.Equal(0, segs[28].Offset.ToDouble(), delta);

            Assert.True(segs[744].Vertex1 == vertices[446]);
            Assert.True(segs[744].Vertex2 == vertices[374]);
            Assert.Equal(ToRadian(-16384), segs[744].Angle.ToRadian(), delta);
            Assert.True(segs[744].LineDef == lines[452]);
            Assert.Equal(0, (int)(segs[744].LineDef.Flags & LineFlags.TwoSided));
            Assert.True(segs[744].FrontSector == segs[744].LineDef.FrontSide.Sector);
            Assert.Null(segs[744].BackSector);
            Assert.Equal(154, segs[744].Offset.ToDouble(), delta);

            Assert.True(segs[746].Vertex1 == vertices[374]);
            Assert.True(segs[746].Vertex2 == vertices[368]);
            Assert.Equal(ToRadian(-13828), segs[746].Angle.ToRadian(), delta);
            Assert.True(segs[746].LineDef == lines[451]);
            Assert.Equal(0, (int)(segs[746].LineDef.Flags & LineFlags.TwoSided));
            Assert.True(segs[746].FrontSector == segs[746].LineDef.FrontSide.Sector);
            Assert.Null(segs[746].BackSector);
            Assert.Equal(0, segs[746].Offset.ToDouble(), delta);
        }

        [Fact]
        public void LoadMap01()
        {
            using var wad = new Wad(WadPath.Doom2);
            var flats = new DummyFlatLookup(wad);
            var textures = new DummyTextureLookup(wad);
            var map = wad.GetLumpNumber("MAP01");
            var vertices = Vertex.FromWad(wad, map + 4);
            var sectors = Sector.FromWad(wad, map + 8, flats);
            var sides = SideDef.FromWad(wad, map + 3, textures, sectors);
            var lines = LineDef.FromWad(wad, map + 2, vertices, sides);
            var segs = Seg.FromWad(wad, map + 5, vertices, lines);

            Assert.Equal(601, segs.Length);

            Assert.True(segs[0].Vertex1 == vertices[9]);
            Assert.True(segs[0].Vertex2 == vertices[316]);
            Assert.Equal(ToRadian(-32768), segs[0].Angle.ToRadian(), delta);
            Assert.True(segs[0].LineDef == lines[8]);
            Assert.NotEqual(0, (int)(segs[0].LineDef.Flags & LineFlags.TwoSided));
            Assert.True(segs[0].FrontSector == segs[0].LineDef.FrontSide.Sector);
            Assert.True(segs[0].BackSector == segs[0].LineDef.BackSide.Sector);
            Assert.Equal(0, segs[0].Offset.ToDouble(), delta);

            Assert.True(segs[42].Vertex1 == vertices[26]);
            Assert.True(segs[42].Vertex2 == vertices[320]);
            Assert.Equal(ToRadian(-22209), segs[42].Angle.ToRadian(), delta);
            Assert.True(segs[42].LineDef == lines[33]);
            Assert.NotEqual(0, (int)(segs[42].LineDef.Flags & LineFlags.TwoSided));
            Assert.True(segs[42].FrontSector == segs[42].LineDef.BackSide.Sector);
            Assert.True(segs[42].BackSector == segs[42].LineDef.FrontSide.Sector);
            Assert.Equal(0, segs[42].Offset.ToDouble(), delta);

            Assert.True(segs[103].Vertex1 == vertices[331]);
            Assert.True(segs[103].Vertex2 == vertices[329]);
            Assert.Equal(ToRadian(16384), segs[103].Angle.ToRadian(), delta);
            Assert.True(segs[103].LineDef == lines[347]);
            Assert.Equal(0, (int)(segs[103].LineDef.Flags & LineFlags.TwoSided));
            Assert.True(segs[103].FrontSector == segs[103].LineDef.FrontSide.Sector);
            Assert.Null(segs[103].BackSector);
            Assert.Equal(64, segs[103].Offset.ToDouble(), delta);

            Assert.True(segs[600].Vertex1 == vertices[231]);
            Assert.True(segs[600].Vertex2 == vertices[237]);
            Assert.Equal(ToRadian(-16384), segs[600].Angle.ToRadian(), delta);
            Assert.True(segs[600].LineDef == lines[271]);
            Assert.NotEqual(0, (int)(segs[600].LineDef.Flags & LineFlags.TwoSided));
            Assert.True(segs[600].FrontSector == segs[600].LineDef.BackSide.Sector);
            Assert.True(segs[600].BackSector == segs[600].LineDef.FrontSide.Sector);
            Assert.Equal(0, segs[600].Offset.ToDouble(), delta);
        }
    }
}

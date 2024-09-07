using ManagedDoom;
using Xunit;

namespace ManagedDoomTest.UnitTests
{
    public class LineDefTest
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

            Assert.Equal(486, lines.Length);

            Assert.True(lines[0].Vertex1 == vertices[0]);
            Assert.True(lines[0].Vertex2 == vertices[1]);
            Assert.Equal(1, (int)lines[0].Flags);
            Assert.Equal(0, (int)lines[0].Special);
            Assert.Equal(0, lines[0].Tag);
            Assert.True(lines[0].FrontSide == sides[0]);
            Assert.Null(lines[0].BackSide);
            Assert.True(lines[0].FrontSector == sides[0].Sector);
            Assert.Null(lines[0].BackSector);

            Assert.True(lines[136].Vertex1 == vertices[110]);
            Assert.True(lines[136].Vertex2 == vertices[111]);
            Assert.Equal(28, (int)lines[136].Flags);
            Assert.Equal(63, (int)lines[136].Special);
            Assert.Equal(3, lines[136].Tag);
            Assert.True(lines[136].FrontSide == sides[184]);
            Assert.True(lines[136].BackSide == sides[185]);
            Assert.True(lines[136].FrontSector == sides[184].Sector);
            Assert.True(lines[136].BackSector == sides[185].Sector);

            Assert.True(lines[485].Vertex1 == vertices[309]);
            Assert.True(lines[485].Vertex2 == vertices[294]);
            Assert.Equal(12, (int)lines[485].Flags);
            Assert.Equal(0, (int)lines[485].Special);
            Assert.Equal(0, lines[485].Tag);
            Assert.True(lines[485].FrontSide == sides[664]);
            Assert.True(lines[485].BackSide == sides[665]);
            Assert.True(lines[485].FrontSector == sides[664].Sector);
            Assert.True(lines[485].BackSector == sides[665].Sector);
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

            Assert.Equal(370, lines.Length);

            Assert.True(lines[0].Vertex1 == vertices[0]);
            Assert.True(lines[0].Vertex2 == vertices[1]);
            Assert.Equal(1, (int)lines[0].Flags);
            Assert.Equal(0, (int)lines[0].Special);
            Assert.Equal(0, lines[0].Tag);
            Assert.True(lines[0].FrontSide == sides[0]);
            Assert.Null(lines[0].BackSide);
            Assert.True(lines[0].FrontSector == sides[0].Sector);
            Assert.Null(lines[0].BackSector);

            Assert.True(lines[75].Vertex1 == vertices[73]);
            Assert.True(lines[75].Vertex2 == vertices[74]);
            Assert.Equal(4, (int)lines[75].Flags);
            Assert.Equal(103, (int)lines[75].Special);
            Assert.Equal(4, lines[75].Tag);
            Assert.True(lines[75].FrontSide == sides[97]);
            Assert.True(lines[75].BackSide == sides[98]);
            Assert.True(lines[75].FrontSector == sides[97].Sector);
            Assert.True(lines[75].BackSector == sides[98].Sector);

            Assert.True(lines[369].Vertex1 == vertices[293]);
            Assert.True(lines[369].Vertex2 == vertices[299]);
            Assert.Equal(21, (int)lines[369].Flags);
            Assert.Equal(0, (int)lines[369].Special);
            Assert.Equal(0, lines[369].Tag);
            Assert.True(lines[369].FrontSide == sides[527]);
            Assert.True(lines[369].BackSide == sides[528]);
            Assert.True(lines[369].FrontSector == sides[527].Sector);
            Assert.True(lines[369].BackSector == sides[528].Sector);
        }
    }
}

using ManagedDoom;
using Xunit;

namespace ManagedDoomTest.UnitTests
{
    public class SideDefTest
    {
        private const double delta = 1.0E-9;

        [Fact]
        public void LoadE1M1()
        {
            using var wad = new Wad(WadPath.Doom1);
            var flats = new DummyFlatLookup(wad);
            ITextureLookup textures = new TextureLookup();
            textures.Initialize(wad);
            var map = wad.GetLumpNumber("E1M1");
            var vertices = Vertex.FromWad(wad, map + 4);
            var sectors = Sector.FromWad(wad, map + 8, flats);
            var sides = SideDef.FromWad(wad, map + 3, textures, sectors);

            Assert.Equal(666, sides.Length);

            Assert.Equal(0, sides[0].TextureOffset.ToDouble(), delta);
            Assert.Equal(0, sides[0].RowOffset.ToDouble(), delta);
            Assert.Equal(0, sides[0].TopTexture);
            Assert.Equal(0, sides[0].BottomTexture);
            Assert.Equal("DOOR3", textures[sides[0].MiddleTexture].Name);
            Assert.True(sides[0].Sector == sectors[30]);

            Assert.Equal(32, sides[480].TextureOffset.ToDouble(), delta);
            Assert.Equal(0, sides[480].RowOffset.ToDouble(), delta);
            Assert.Equal("EXITSIGN", textures[sides[480].TopTexture].Name);
            Assert.Equal(0, sides[480].BottomTexture);
            Assert.Equal(0, sides[480].MiddleTexture);
            Assert.True(sides[480].Sector == sectors[70]);

            Assert.Equal(0, sides[650].TextureOffset.ToDouble(), delta);
            Assert.Equal(88, sides[650].RowOffset.ToDouble(), delta);
            Assert.Equal("STARTAN3", textures[sides[650].TopTexture].Name);
            Assert.Equal("STARTAN3", textures[sides[650].BottomTexture].Name);
            Assert.Equal(0, sides[650].MiddleTexture);
            Assert.True(sides[650].Sector == sectors[1]);

            Assert.Equal(0, sides[665].TextureOffset.ToDouble(), delta);
            Assert.Equal(0, sides[665].RowOffset.ToDouble(), delta);
            Assert.Equal(0, sides[665].TopTexture);
            Assert.Equal(0, sides[665].BottomTexture);
            Assert.Equal(0, sides[665].MiddleTexture);
            Assert.True(sides[665].Sector == sectors[23]);
        }

        [Fact]
        public void LoadMap01()
        {
            using var wad = new Wad(WadPath.Doom2);
            var flats = new DummyFlatLookup(wad);
            ITextureLookup textures = new TextureLookup();
            textures.Initialize(wad);
            var map = wad.GetLumpNumber("MAP01");
            var vertices = Vertex.FromWad(wad, map + 4);
            var sectors = Sector.FromWad(wad, map + 8, flats);
            var sides = SideDef.FromWad(wad, map + 3, textures, sectors);

            Assert.Equal(529, sides.Length);

            Assert.Equal(0, sides[0].TextureOffset.ToDouble(), delta);
            Assert.Equal(0, sides[0].RowOffset.ToDouble(), delta);
            Assert.Equal(0, sides[0].TopTexture);
            Assert.Equal(0, sides[0].BottomTexture);
            Assert.Equal("BRONZE1", textures[sides[0].MiddleTexture].Name);
            Assert.True(sides[0].Sector == sectors[9]);

            Assert.Equal(0, sides[312].TextureOffset.ToDouble(), delta);
            Assert.Equal(0, sides[312].RowOffset.ToDouble(), delta);
            Assert.Equal(0, sides[312].TopTexture);
            Assert.Equal(0, sides[312].BottomTexture);
            Assert.Equal("DOORTRAK", textures[sides[312].MiddleTexture].Name);
            Assert.True(sides[312].Sector == sectors[31]);

            Assert.Equal(24, sides[512].TextureOffset.ToDouble(), delta);
            Assert.Equal(0, sides[512].RowOffset.ToDouble(), delta);
            Assert.Equal(0, sides[512].TopTexture);
            Assert.Equal(0, sides[512].BottomTexture);
            Assert.Equal("SUPPORT2", textures[sides[512].MiddleTexture].Name);
            Assert.True(sides[512].Sector == sectors[52]);

            Assert.Equal(0, sides[528].TextureOffset.ToDouble(), delta);
            Assert.Equal(0, sides[528].RowOffset.ToDouble(), delta);
            Assert.Equal(0, sides[528].TopTexture);
            Assert.Equal(0, sides[528].BottomTexture);
            Assert.Equal(0, sides[528].MiddleTexture);
            Assert.True(sides[528].Sector == sectors[11]);
        }
    }
}

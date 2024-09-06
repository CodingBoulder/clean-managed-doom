using ManagedDoom;
using Xunit;

namespace ManagedDoomTest.UnitTests
{
    public class SectorTest
    {
        private const double delta = 1.0E-9;

        [Fact]
        public void LoadE1M1()
        {
            using var wad = new Wad(WadPath.Doom1);
            var map = wad.GetLumpNumber("E1M1");
            var flats = new FlatLookup(wad);
            var sectors = Sector.FromWad(wad, map + 8, flats);

            Assert.Equal(88, sectors.Length);

            Assert.Equal(-80, sectors[0].FloorHeight.ToDouble(), delta);
            Assert.Equal(216, sectors[0].CeilingHeight.ToDouble(), delta);
            Assert.Equal("NUKAGE3", flats[sectors[0].FloorFlat].Name);
            Assert.Equal("F_SKY1", flats[sectors[0].CeilingFlat].Name);
            Assert.Equal(255, sectors[0].LightLevel);
            Assert.Equal((SectorSpecial)7, sectors[0].Special);
            Assert.Equal(0, sectors[0].Tag);

            Assert.Equal(0, sectors[42].FloorHeight.ToDouble(), delta);
            Assert.Equal(264, sectors[42].CeilingHeight.ToDouble(), delta);
            Assert.Equal("FLOOR7_1", flats[sectors[42].FloorFlat].Name);
            Assert.Equal("F_SKY1", flats[sectors[42].CeilingFlat].Name);
            Assert.Equal(255, sectors[42].LightLevel);
            Assert.Equal((SectorSpecial)0, sectors[42].Special);
            Assert.Equal(0, sectors[42].Tag);

            Assert.Equal(104, sectors[87].FloorHeight.ToDouble(), delta);
            Assert.Equal(184, sectors[87].CeilingHeight.ToDouble(), delta);
            Assert.Equal("FLOOR4_8", flats[sectors[87].FloorFlat].Name);
            Assert.Equal("FLOOR6_2", flats[sectors[87].CeilingFlat].Name);
            Assert.Equal(128, sectors[87].LightLevel);
            Assert.Equal((SectorSpecial)9, sectors[87].Special);
            Assert.Equal(2, sectors[87].Tag);
        }

        [Fact]
        public void LoadMap01()
        {
            using var wad = new Wad(WadPath.Doom2);
            var map = wad.GetLumpNumber("MAP01");
            var flats = new FlatLookup(wad);
            var sectors = Sector.FromWad(wad, map + 8, flats);

            Assert.Equal(59, sectors.Length);

            Assert.Equal(8, sectors[0].FloorHeight.ToDouble(), delta);
            Assert.Equal(264, sectors[0].CeilingHeight.ToDouble(), delta);
            Assert.Equal("FLOOR0_1", flats[sectors[0].FloorFlat].Name);
            Assert.Equal("FLOOR4_1", flats[sectors[0].CeilingFlat].Name);
            Assert.Equal(128, sectors[0].LightLevel);
            Assert.Equal(SectorSpecial.Normal, sectors[0].Special);
            Assert.Equal(0, sectors[0].Tag);

            Assert.Equal(56, sectors[57].FloorHeight.ToDouble(), delta);
            Assert.Equal(184, sectors[57].CeilingHeight.ToDouble(), delta);
            Assert.Equal("FLOOR3_3", flats[sectors[57].FloorFlat].Name);
            Assert.Equal("CEIL3_3", flats[sectors[57].CeilingFlat].Name);
            Assert.Equal(144, sectors[57].LightLevel);
            Assert.Equal((SectorSpecial)9, sectors[57].Special);
            Assert.Equal(0, sectors[57].Tag);

            Assert.Equal(56, sectors[58].FloorHeight.ToDouble(), delta);
            Assert.Equal(56, sectors[58].CeilingHeight.ToDouble(), delta);
            Assert.Equal("FLOOR3_3", flats[sectors[58].FloorFlat].Name);
            Assert.Equal("FLAT20", flats[sectors[58].CeilingFlat].Name);
            Assert.Equal(144, sectors[58].LightLevel);
            Assert.Equal(SectorSpecial.Normal, sectors[58].Special);
            Assert.Equal(6, sectors[58].Tag);
        }
    }
}

using ManagedDoom;
using Xunit;

namespace ManagedDoomTest.UnitTests
{
    public class WadTest
    {
        [Fact]
        public void LumpNumberDoom1()
        {
            using var wad = new Wad(WadPath.Doom1);
            Assert.Equal(0, wad.GetLumpNumber("PLAYPAL"));
            Assert.Equal(1, wad.GetLumpNumber("COLORMAP"));
            Assert.Equal(7, wad.GetLumpNumber("E1M1"));
            Assert.Equal(2305, wad.GetLumpNumber("F_END"));
            Assert.Equal(2306, wad.LumpInfos.Count);
        }

        [Fact]
        public void LumpNumberDoom2()
        {
            using var wad = new Wad(WadPath.Doom2);
            Assert.Equal(0, wad.GetLumpNumber("PLAYPAL"));
            Assert.Equal(1, wad.GetLumpNumber("COLORMAP"));
            Assert.Equal(6, wad.GetLumpNumber("MAP01"));
            Assert.Equal(2918, wad.GetLumpNumber("F_END"));
            Assert.Equal(2919, wad.LumpInfos.Count);
        }

        [Fact]
        public void FlatSizeDoom1()
        {
            using var wad = new Wad(WadPath.Doom1);
            var start = wad.GetLumpNumber("F_START") + 1;
            var end = wad.GetLumpNumber("F_END");
            for (var lump = start; lump < end; lump++)
            {
                var size = wad.GetLumpSize(lump);
                Assert.True(size == 0 || size == 4096);
            }
        }

        [Fact]
        public void FlatSizeDoom2()
        {
            using var wad = new Wad(WadPath.Doom2);
            var start = wad.GetLumpNumber("F_START") + 1;
            var end = wad.GetLumpNumber("F_END");
            for (var lump = start; lump < end; lump++)
            {
                var size = wad.GetLumpSize(lump);
                Assert.True(size == 0 || size == 4096);
            }
        }
    }
}

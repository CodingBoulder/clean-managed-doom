using ManagedDoom;
using Xunit;

namespace ManagedDoomTest.UnitTests
{
    public class ThingTest
    {
        private const double delta = 1.0E-9;

        [Fact]
        public void LoadE1M1()
        {
            using var wad = new Wad(WadPath.Doom1);
            var map = wad.GetLumpNumber("E1M1");
            var things = MapThing.FromWad(wad, map + 1);

            Assert.Equal(143, things.Length);

            Assert.Equal(1056, things[0].X.ToDouble(), delta);
            Assert.Equal(-3616, things[0].Y.ToDouble(), delta);
            Assert.Equal(90, things[0].Angle.ToDegree(), delta);
            Assert.Equal(1, things[0].Type);
            Assert.Equal(7, (int)things[0].Flags);

            Assert.Equal(3072, things[57].X.ToDouble(), delta);
            Assert.Equal(-4832, things[57].Y.ToDouble(), delta);
            Assert.Equal(180, things[57].Angle.ToDegree(), delta);
            Assert.Equal(2015, things[57].Type);
            Assert.Equal(7, (int)things[57].Flags);

            Assert.Equal(736, things[142].X.ToDouble(), delta);
            Assert.Equal(-2976, things[142].Y.ToDouble(), delta);
            Assert.Equal(90, things[142].Angle.ToDegree(), delta);
            Assert.Equal(2001, things[142].Type);
            Assert.Equal(23, (int)things[142].Flags);
        }

        [Fact]
        public void LoadMap01()
        {
            using var wad = new Wad(WadPath.Doom2);
            var map = wad.GetLumpNumber("MAP01");
            var things = MapThing.FromWad(wad, map + 1);

            Assert.Equal(69, things.Length);

            Assert.Equal(-96, things[0].X.ToDouble(), delta);
            Assert.Equal(784, things[0].Y.ToDouble(), delta);
            Assert.Equal(90, things[0].Angle.ToDegree(), delta);
            Assert.Equal(1, things[0].Type);
            Assert.Equal(7, (int)things[0].Flags);

            Assert.Equal(-288, things[57].X.ToDouble(), delta);
            Assert.Equal(976, things[57].Y.ToDouble(), delta);
            Assert.Equal(270, things[57].Angle.ToDegree(), delta);
            Assert.Equal(2006, things[57].Type);
            Assert.Equal(23, (int)things[57].Flags);

            Assert.Equal(-480, things[68].X.ToDouble(), delta);
            Assert.Equal(848, things[68].Y.ToDouble(), delta);
            Assert.Equal(0, things[68].Angle.ToDegree(), delta);
            Assert.Equal(2005, things[68].Type);
            Assert.Equal(7, (int)things[68].Flags);
        }
    }
}

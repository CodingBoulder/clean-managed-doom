using ManagedDoom;
using Xunit;

namespace ManagedDoomTest.UnitTests
{
    public class VertexTest
    {
        private const double delta = 1.0E-9;

        [Fact]
        public void LoadE1M1()
        {
            using var wad = new Wad(WadPath.Doom1);
            var map = wad.GetLumpNumber("E1M1");
            var vertices = Vertex.FromWad(wad, map + 4);

            Assert.Equal(470, vertices.Length);

            Assert.Equal(1088, vertices[0].X.ToDouble(), delta);
            Assert.Equal(-3680, vertices[0].Y.ToDouble(), delta);

            Assert.Equal(128, vertices[57].X.ToDouble(), delta);
            Assert.Equal(-3008, vertices[57].Y.ToDouble(), delta);

            Assert.Equal(2435, vertices[469].X.ToDouble(), delta);
            Assert.Equal(-3920, vertices[469].Y.ToDouble(), delta);
        }

        [Fact]
        public void LoadMap01()
        {
            using var wad = new Wad(WadPath.Doom2);
            var map = wad.GetLumpNumber("MAP01");
            var vertices = Vertex.FromWad(wad, map + 4);

            Assert.Equal(383, vertices.Length);

            Assert.Equal(-448, vertices[0].X.ToDouble(), delta);
            Assert.Equal(768, vertices[0].Y.ToDouble(), delta);

            Assert.Equal(128, vertices[57].X.ToDouble(), delta);
            Assert.Equal(1808, vertices[57].Y.ToDouble(), delta);

            Assert.Equal(-64, vertices[382].X.ToDouble(), delta);
            Assert.Equal(2240, vertices[382].Y.ToDouble(), delta);
        }
    }
}

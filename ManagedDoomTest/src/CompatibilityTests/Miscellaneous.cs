using ManagedDoom;
using System.Linq;
using Xunit;

namespace ManagedDoomTest.CompatibilityTests
{
    public class Miscellaneous
    {
        [Fact]
        public void Altdeath()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2);
            var demo = new Demo(@"data\altdeath_test.lmp");
            var cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
            var game = new DoomGame(content, demo.Options);
            game.DeferedInitNew();

            var lastMobjHash = 0;
            var aggMobjHash = 0;

            while (true)
            {
                if (!demo.ReadCmd(cmds))
                {
                    break;
                }

                game.Update(cmds);
                lastMobjHash = DoomDebug.GetMobjHash(game.World);
                aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);
            }

            Assert.Equal(0xf598b1d9u, (uint)lastMobjHash);
            Assert.Equal(0x9f716cfau, (uint)aggMobjHash);
        }
    }
}

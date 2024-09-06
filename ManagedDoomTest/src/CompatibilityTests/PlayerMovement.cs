using ManagedDoom;
using System.Linq;
using Xunit;

namespace ManagedDoomTest.CompatibilityTests
{
    public class PlayerMovement
    {
        [Fact]
        public void PlayerMovementTest()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2, @"data\player_movement_test.wad");
            var demo = new Demo(@"data\player_movement_test.lmp");
            var cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
            var game = new DoomGame(content, demo.Options);
            game.DeferedInitNew();

            var lastHash = 0;
            var aggHash = 0;

            while (true)
            {
                if (!demo.ReadCmd(cmds))
                {
                    break;
                }

                game.Update(cmds);
                lastHash = DoomDebug.GetMobjHash(game.World);
                aggHash = DoomDebug.CombineHash(aggHash, lastHash);
            }

            Assert.Equal(0xe9a6d7d2u, (uint)lastHash);
            Assert.Equal(0x5e70c62du, (uint)aggHash);
        }

        [Fact]
        public void ThingCollisionTest()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2, @"data\thing_collision_test.wad");
            var demo = new Demo(@"data\thing_collision_test.lmp");
            var cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
            var game = new DoomGame(content, demo.Options);
            game.DeferedInitNew();

            var lastHash = 0;
            var aggHash = 0;

            while (true)
            {
                if (!demo.ReadCmd(cmds))
                {
                    break;
                }

                game.Update(cmds);
                lastHash = DoomDebug.GetMobjHash(game.World);
                aggHash = DoomDebug.CombineHash(aggHash, lastHash);
            }

            Assert.Equal(0x63ff9173u, (uint)lastHash);
            Assert.Equal(0xb9cd0f6fu, (uint)aggHash);
        }

        [Fact]
        public void AutoAimTest()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2, @"data\autoaim_test.wad");
            var demo = new Demo(@"data\autoaim_test.lmp");
            var cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
            var game = new DoomGame(content, demo.Options);
            game.DeferedInitNew();

            var lastHash = 0;
            var aggHash = 0;

            while (true)
            {
                if (!demo.ReadCmd(cmds))
                {
                    break;
                }

                game.Update(cmds);
                lastHash = DoomDebug.GetMobjHash(game.World);
                aggHash = DoomDebug.CombineHash(aggHash, lastHash);
            }

            Assert.Equal(0xe0d5d327u, (uint)lastHash);
            Assert.Equal(0x1a00fde9u, (uint)aggHash);
        }
    }
}

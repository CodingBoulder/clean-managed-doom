using ManagedDoom;
using System.Linq;
using Xunit;

namespace ManagedDoomTest.CompatibilityTests
{
    public class MapGimmicks
    {
        [Fact]
        public void E1M2Donut()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom1);
            var demo = new Demo(@"demos\e1m2_donut_test.lmp");
            demo.Options.GameMode = content.Wad.GameMode;
            var cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
            var game = new DoomGame(content, demo.Options);
            game.DeferedInitNew();

            var lastMobjHash = 0;
            var aggMobjHash = 0;
            var lastSectorHash = 0;
            var aggSectorHash = 0;

            while (true)
            {
                if (!demo.ReadCmd(cmds))
                {
                    break;
                }

                game.Update(cmds);
                lastMobjHash = DoomDebug.GetMobjHash(game.World);
                aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);
                lastSectorHash = DoomDebug.GetSectorHash(game.World);
                aggSectorHash = DoomDebug.CombineHash(aggSectorHash, lastSectorHash);
            }

            Assert.Equal(0xcfa012d2u, (uint)lastMobjHash);
            Assert.Equal(0xdd1a0d72u, (uint)aggMobjHash);
            Assert.Equal(0x76aad5fbu, (uint)lastSectorHash);
            Assert.Equal(0xaa2757eau, (uint)aggSectorHash);
        }

        [Fact]
        public void E1M8Boss()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom1);
            var demo = new Demo(@"demos\e1m8_boss_test.lmp");
            demo.Options.GameMode = content.Wad.GameMode;
            var cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
            var game = new DoomGame(content, demo.Options);
            game.DeferedInitNew();

            var lastMobjHash = 0;
            var aggMobjHash = 0;
            var lastSectorHash = 0;
            var aggSectorHash = 0;

            while (true)
            {
                if (!demo.ReadCmd(cmds))
                {
                    break;
                }

                game.Update(cmds);
                lastMobjHash = DoomDebug.GetMobjHash(game.World);
                aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);
                lastSectorHash = DoomDebug.GetSectorHash(game.World);
                aggSectorHash = DoomDebug.CombineHash(aggSectorHash, lastSectorHash);
            }

            Assert.Equal(0xb01c44f9u, (uint)lastMobjHash);
            Assert.Equal(0x1a4918bbu, (uint)aggMobjHash);
            Assert.Equal(0xa7bac3ceu, (uint)lastSectorHash);
            Assert.Equal(0xda0067d0u, (uint)aggSectorHash);
        }

        [Fact]
        public void Map06Crusher()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2);
            var demo = new Demo(@"demos\map06_crusher_test.lmp");
            var cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
            var game = new DoomGame(content, demo.Options);
            game.DeferedInitNew();

            var lastMobjHash = 0;
            var aggMobjHash = 0;
            var lastSectorHash = 0;
            var aggSectorHash = 0;

            while (true)
            {
                if (!demo.ReadCmd(cmds))
                {
                    break;
                }

                game.Update(cmds);
                lastMobjHash = DoomDebug.GetMobjHash(game.World);
                aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);
                lastSectorHash = DoomDebug.GetSectorHash(game.World);
                aggSectorHash = DoomDebug.CombineHash(aggSectorHash, lastSectorHash);
            }

            Assert.Equal(0x302bc4e3u, (uint)lastMobjHash);
            Assert.Equal(0xe4050462u, (uint)aggMobjHash);
            Assert.Equal(0x3ce914d8u, (uint)lastSectorHash);
            Assert.Equal(0x549ea480u, (uint)aggSectorHash);
        }

        [Fact]
        public void Map07Boss()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2);
            var demo = new Demo(@"demos\map07_boss_test.lmp");
            var cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
            var game = new DoomGame(content, demo.Options);
            game.DeferedInitNew();

            var lastMobjHash = 0;
            var aggMobjHash = 0;
            var lastSectorHash = 0;
            var aggSectorHash = 0;

            while (true)
            {
                if (!demo.ReadCmd(cmds))
                {
                    break;
                }

                game.Update(cmds);
                lastMobjHash = DoomDebug.GetMobjHash(game.World);
                aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);
                lastSectorHash = DoomDebug.GetSectorHash(game.World);
                aggSectorHash = DoomDebug.CombineHash(aggSectorHash, lastSectorHash);
            }

            Assert.Equal(0x4c4e952fu, (uint)lastMobjHash);
            Assert.Equal(0x56d1d836u, (uint)aggMobjHash);
            Assert.Equal(0x44469690u, (uint)lastSectorHash);
            Assert.Equal(0x1b989de0u, (uint)aggSectorHash);
        }

        [Fact]
        public void Map30Brain()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2);
            var demo = new Demo(@"demos\map30_brain_test.lmp");
            var cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
            var game = new DoomGame(content, demo.Options);
            game.DeferedInitNew();

            var lastMobjHash = 0;
            var aggMobjHash = 0;
            var lastSectorHash = 0;
            var aggSectorHash = 0;

            while (true)
            {
                if (!demo.ReadCmd(cmds))
                {
                    break;
                }

                game.Update(cmds);
                lastMobjHash = DoomDebug.GetMobjHash(game.World);
                aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);
                lastSectorHash = DoomDebug.GetSectorHash(game.World);
                aggSectorHash = DoomDebug.CombineHash(aggSectorHash, lastSectorHash);
            }

            Assert.Equal(0xfc44fcceu, (uint)lastMobjHash);
            Assert.Equal(0x72c0e74fu, (uint)aggMobjHash);
            Assert.Equal(0x0a37e32au, (uint)lastSectorHash);
            Assert.Equal(0xb640d706u, (uint)aggSectorHash);
        }

        [Fact]
        public void Map32Keen()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2);
            var demo = new Demo(@"demos\map32_keen_test.lmp");
            var cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
            var game = new DoomGame(content, demo.Options);
            game.DeferedInitNew();

            var lastMobjHash = 0;
            var aggMobjHash = 0;
            var lastSectorHash = 0;
            var aggSectorHash = 0;

            while (true)
            {
                if (!demo.ReadCmd(cmds))
                {
                    break;
                }

                game.Update(cmds);
                lastMobjHash = DoomDebug.GetMobjHash(game.World);
                aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);
                lastSectorHash = DoomDebug.GetSectorHash(game.World);
                aggSectorHash = DoomDebug.CombineHash(aggSectorHash, lastSectorHash);
            }

            Assert.Equal(0xd0b51d34u, (uint)lastMobjHash);
            Assert.Equal(0xec0bc144u, (uint)aggMobjHash);
            Assert.Equal(0xab146fa3u, (uint)lastSectorHash);
            Assert.Equal(0x3ede64ccu, (uint)aggSectorHash);
        }
    }
}

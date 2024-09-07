using ManagedDoom;
using System.Linq;
using Xunit;

namespace ManagedDoomTest.CompatibilityTests
{
    public class SectorAction
    {
        [Fact]
        public void TeleporterTest()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2, @"data\teleporter_test.wad");
            var demo = new Demo(@"data\teleporter_test.lmp");
            TicCmd[] cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
            var game = new DoomGame(content, demo.Options);
            game.DeferedInitNew();

            int lastMobjHash = 0;
            int aggMobjHash = 0;

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

            Assert.Equal(0x3450bb23u, (uint)lastMobjHash);
            Assert.Equal(0x2669e089u, (uint)aggMobjHash);
        }

        [Fact]
        public void LocalDoorTest()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2, @"data\localdoor_test.wad");
            var demo = new Demo(@"data\localdoor_test.lmp");
            TicCmd[] cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
            var game = new DoomGame(content, demo.Options);
            game.DeferedInitNew();

            int lastMobjHash = 0;
            int aggMobjHash = 0;
            int lastSectorHash = 0;
            int aggSectorHash = 0;

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

            Assert.Equal(0x9d6c0abeu, (uint)lastMobjHash);
            Assert.Equal(0x7e1bb5f2u, (uint)aggMobjHash);
            Assert.Equal(0xfdf3e7a0u, (uint)lastSectorHash);
            Assert.Equal(0x0a0f1980u, (uint)aggSectorHash);
        }

        [Fact]
        public void PlatformTest()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2, @"data\platform_test.wad");
            var demo = new Demo(@"data\platform_test.lmp");
            TicCmd[] cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
            var game = new DoomGame(content, demo.Options);
            game.DeferedInitNew();

            int lastMobjHash = 0;
            int aggMobjHash = 0;
            int lastSectorHash = 0;
            int aggSectorHash = 0;

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

            Assert.Equal(0x3da2f507u, (uint)lastMobjHash);
            Assert.Equal(0x3402f715u, (uint)aggMobjHash);
            Assert.Equal(0xc71b4d00u, (uint)lastSectorHash);
            Assert.Equal(0x2fb8dd00u, (uint)aggSectorHash);
        }

        [Fact]
        public void SilentCrusherTest()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2, @"data\silent_crusher_test.wad");
            var demo = new Demo(@"data\silent_crusher_test.lmp");
            TicCmd[] cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
            var game = new DoomGame(content, demo.Options);
            game.DeferedInitNew();

            int lastMobjHash = 0;
            int aggMobjHash = 0;
            int lastSectorHash = 0;
            int aggSectorHash = 0;

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

            Assert.Equal(0xee31a164u, (uint)lastMobjHash);
            Assert.Equal(0x1f3fc7b4u, (uint)aggMobjHash);
            Assert.Equal(0x6d6a1f20u, (uint)lastSectorHash);
            Assert.Equal(0x34b4f740u, (uint)aggSectorHash);
        }
    }
}

using ManagedDoom;
using System.Linq;
using Xunit;

namespace ManagedDoomTest.CompatibilityTests
{
    public class TNTBlood
    {
        [Fact]
        public void Demo1()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2, WadPath.TntBlood);
            var demo = new Demo(content.Wad.ReadLump("DEMO1"));
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

            Assert.Equal(0xa8343166u, (uint)lastMobjHash);
            Assert.Equal(0xd1d5c433u, (uint)aggMobjHash);
            Assert.Equal(0x9e70ce46u, (uint)lastSectorHash);
            Assert.Equal(0x71eb6e2cu, (uint)aggSectorHash);
        }

        [Fact]
        public void Demo2()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2, WadPath.TntBlood);
            var demo = new Demo(content.Wad.ReadLump("DEMO2"));
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

            Assert.Equal(0x6fde0422u, (uint)lastMobjHash);
            Assert.Equal(0xbae1086eu, (uint)aggMobjHash);
            Assert.Equal(0x9708f97du, (uint)lastSectorHash);
            Assert.Equal(0xfc771056u, (uint)aggSectorHash);
        }

        [Fact]
        public void Demo3()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2, WadPath.TntBlood);
            var demo = new Demo(content.Wad.ReadLump("DEMO3"));
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

            Assert.Equal(0x9d24c7d8u, (uint)lastMobjHash);
            Assert.Equal(0xd37240f4u, (uint)aggMobjHash);
            Assert.Equal(0xf3f4db97u, (uint)lastSectorHash);
            Assert.Equal(0xa0acc43eu, (uint)aggSectorHash);
        }
    }
}

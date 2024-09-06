using ManagedDoom;
using System.Linq;
using Xunit;

namespace ManagedDoomTest.CompatibilityTests
{
    public class MementoMori
    {
        [Fact]
        public void Demo1()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2, WadPath.MementoMori);
            var demo = new Demo(content.Wad.ReadLump("DEMO1"));
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

            Assert.Equal(0x9c24cf97u, (uint)lastMobjHash);
            Assert.Equal(0x58a33c2au, (uint)aggMobjHash);
            Assert.Equal(0xf0f84e3du, (uint)lastSectorHash);
            Assert.Equal(0x563d30fbu, (uint)aggSectorHash);
        }

        [Fact]
        public void Demo2()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2, WadPath.MementoMori);
            var demo = new Demo(content.Wad.ReadLump("DEMO2"));
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

            Assert.Equal(0x02bdcde5u, (uint)lastMobjHash);
            Assert.Equal(0x228756a5u, (uint)aggMobjHash);
            Assert.Equal(0xac3d6ccfu, (uint)lastSectorHash);
            Assert.Equal(0xb9311befu, (uint)aggSectorHash);
        }

        [Fact]
        public void Demo3()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2, WadPath.MementoMori);
            var demo = new Demo(content.Wad.ReadLump("DEMO3"));
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

            Assert.Equal(0x2c3bf1e3u, (uint)lastMobjHash);
            Assert.Equal(0x40d3fc5cu, (uint)aggMobjHash);
            Assert.Equal(0xdc871ca2u, (uint)lastSectorHash);
            Assert.Equal(0x388e5e4fu, (uint)aggSectorHash);
        }
    }
}

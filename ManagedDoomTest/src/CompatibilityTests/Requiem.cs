using ManagedDoom;
using System.Linq;
using Xunit;

namespace ManagedDoomTest.CompatibilityTests
{
    public class Requiem
    {
        [Fact]
        public void Demo1_Final2()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2, WadPath.Requiem);
            var demo = new Demo(content.Wad.ReadLump("DEMO1"));
            demo.Options.GameVersion = GameVersion.Final2;
            TicCmd[] cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
            var game = new DoomGame(content, demo.Options);
            game.DeferedInitNew();

            int lastMobjHash = 0;
            int aggMobjHash = 0;
            int lastSectorHash = 0;
            int aggSectorHash = 0;

            while (true)
            {
                demo.ReadCmd(cmds);
                game.Update(cmds);
                lastMobjHash = DoomDebug.GetMobjHash(game.World);
                aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);
                lastSectorHash = DoomDebug.GetSectorHash(game.World);
                aggSectorHash = DoomDebug.CombineHash(aggSectorHash, lastSectorHash);

                if (game.World.LevelTime == 18003)
                {
                    break;
                }
            }

            Assert.Equal(0x62d5d8f5u, (uint)lastMobjHash);
            Assert.Equal(0x05ce9c00u, (uint)aggMobjHash);
            Assert.Equal(0x94015cdau, (uint)lastSectorHash);
            Assert.Equal(0x1ae3ca8eu, (uint)aggSectorHash);
        }

        [Fact]
        public void Demo2()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2, WadPath.Requiem);
            var demo = new Demo(content.Wad.ReadLump("DEMO2"));
            demo.Options.Players[0].PlayerState = PlayerState.Reborn;
            TicCmd[] cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
            var game = new DoomGame(content, demo.Options);
            game.DeferedInitNew();

            int lastMobjHash = 0;
            int aggMobjHash = 0;
            int lastSectorHash = 0;
            int aggSectorHash = 0;

            while (true)
            {
                demo.ReadCmd(cmds);
                game.Update(cmds);
                lastMobjHash = DoomDebug.GetMobjHash(game.World);
                aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);
                lastSectorHash = DoomDebug.GetSectorHash(game.World);
                aggSectorHash = DoomDebug.CombineHash(aggSectorHash, lastSectorHash);

                if (game.World.LevelTime == 24659)
                {
                    break;
                }
            }

            Assert.Equal(0x083125a6u, (uint)lastMobjHash);
            Assert.Equal(0x50237ab4u, (uint)aggMobjHash);
            Assert.Equal(0x732a5645u, (uint)lastSectorHash);
            Assert.Equal(0x36f64dd0u, (uint)aggSectorHash);
        }

        [Fact]
        public void Demo3()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2, WadPath.Requiem);
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
                demo.ReadCmd(cmds);
                game.Update(cmds);
                lastMobjHash = DoomDebug.GetMobjHash(game.World);
                aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);
                lastSectorHash = DoomDebug.GetSectorHash(game.World);
                aggSectorHash = DoomDebug.CombineHash(aggSectorHash, lastSectorHash);

                if (game.World.LevelTime == 52487)
                {
                    break;
                }
            }

            Assert.Equal(0xb76035c8u, (uint)lastMobjHash);
            Assert.Equal(0x87651774u, (uint)aggMobjHash);
            Assert.Equal(0xa2d7d335u, (uint)lastSectorHash);
            Assert.Equal(0xabf7609au, (uint)aggSectorHash);
        }
    }
}

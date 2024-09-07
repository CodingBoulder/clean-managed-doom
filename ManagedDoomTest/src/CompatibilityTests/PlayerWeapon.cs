using ManagedDoom;
using System.Linq;
using Xunit;

namespace ManagedDoomTest.CompatibilityTests
{
    public class PlayerWeapon
    {
        [Fact]
        public void PunchTest()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2, @"data\punch_test.wad");
            var demo = new Demo(@"data\punch_test.lmp");
            TicCmd[] cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
            var game = new DoomGame(content, demo.Options);
            game.DeferedInitNew();

            int lastHash = 0;
            int aggHash = 0;

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

            Assert.Equal(0x3d6c0f49u, (uint)lastHash);
            Assert.Equal(0x97d3aa02u, (uint)aggHash);
        }

        [Fact]
        public void ChainsawTest()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2, @"data\chainsaw_test.wad");
            var demo = new Demo(@"data\chainsaw_test.lmp");
            TicCmd[] cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
            var game = new DoomGame(content, demo.Options);
            game.DeferedInitNew();

            int lastHash = 0;
            int aggHash = 0;

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

            Assert.Equal(0x5db30e69u, (uint)lastHash);
            Assert.Equal(0xed598949u, (uint)aggHash);
        }

        [Fact]
        public void ShotgunTest()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2, @"data\shotgun_test.wad");
            var demo = new Demo(@"data\shotgun_test.lmp");
            TicCmd[] cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
            var game = new DoomGame(content, demo.Options);
            game.DeferedInitNew();

            int lastHash = 0;
            int aggHash = 0;

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

            Assert.Equal(0x3dd50799u, (uint)lastHash);
            Assert.Equal(0x4ddd814fu, (uint)aggHash);
        }

        [Fact]
        public void SuperShotgunTest()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2, @"data\supershotgun_test.wad");
            var demo = new Demo(@"data\supershotgun_test.lmp");
            TicCmd[] cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
            var game = new DoomGame(content, demo.Options);
            game.DeferedInitNew();

            int lastHash = 0;
            int aggHash = 0;

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

            Assert.Equal(0xe2f7936eu, (uint)lastHash);
            Assert.Equal(0x538061e4u, (uint)aggHash);
        }

        [Fact]
        public void ChaingunTest()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2, @"data\chaingun_test.wad");
            var demo = new Demo(@"data\chaingun_test.lmp");
            TicCmd[] cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
            var game = new DoomGame(content, demo.Options);
            game.DeferedInitNew();

            int lastHash = 0;
            int aggHash = 0;

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

            Assert.Equal(0x0b30e14bu, (uint)lastHash);
            Assert.Equal(0xb2104158u, (uint)aggHash);
        }

        [Fact]
        public void RocketTest()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2, @"data\rocket_test.wad");
            var demo = new Demo(@"data\rocket_test.lmp");
            TicCmd[] cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
            var game = new DoomGame(content, demo.Options);
            game.DeferedInitNew();

            int lastHash = 0;
            int aggHash = 0;

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

            Assert.Equal(0x8dce774bu, (uint)lastHash);
            Assert.Equal(0x87f45b5bu, (uint)aggHash);
        }

        [Fact]
        public void PlasmaTest()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2, @"data\plasma_test.wad");
            var demo = new Demo(@"data\plasma_test.lmp");
            TicCmd[] cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
            var game = new DoomGame(content, demo.Options);
            game.DeferedInitNew();

            int lastHash = 0;
            int aggHash = 0;

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

            Assert.Equal(0x116924d3u, (uint)lastHash);
            Assert.Equal(0x88fc9e68u, (uint)aggHash);
        }

        [Fact]
        public void BfgTest()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2, @"data\bfg_test.wad");
            var demo = new Demo(@"data\bfg_test.lmp");
            TicCmd[] cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
            var game = new DoomGame(content, demo.Options);
            game.DeferedInitNew();

            int lastHash = 0;
            int aggHash = 0;

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

            Assert.Equal(0xdeaf403fu, (uint)lastHash);
            Assert.Equal(0xb2c67368u, (uint)aggHash);
        }

        [Fact]
        public void SkyShootTest()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2, @"data\sky_shoot_test.wad");
            var demo = new Demo(@"data\sky_shoot_test.lmp");
            TicCmd[] cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
            var game = new DoomGame(content, demo.Options);
            game.DeferedInitNew();

            int lastHash = 0;
            int aggHash = 0;

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

            Assert.Equal(0xfe794466u, (uint)lastHash);
            Assert.Equal(0xc71f30b2u, (uint)aggHash);
        }
    }
}

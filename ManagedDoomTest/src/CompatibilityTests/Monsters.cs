using ManagedDoom;
using System.Linq;
using Xunit;

namespace ManagedDoomTest.CompatibilityTests
{
    public class Monsters
    {
        [Fact]
        public void NightmareTest()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2);
            var demo = new Demo(@"data\nightmare_test.lmp");
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

            Assert.Equal(0x9278a07au, (uint)lastHash);
            Assert.Equal(0xb2d9a9a0u, (uint)aggHash);
        }

        [Fact]
        public void BarrelTest()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2, @"data\barrel_test.wad");
            var demo = new Demo(@"data\barrel_test.lmp");
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

            Assert.Equal(0xfb76dc03u, (uint)lastHash);
            Assert.Equal(0xccc38bc3u, (uint)aggHash);
        }

        [Fact]
        public void ZombiemanTest()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2, @"data\zombieman_test.wad");
            var demo = new Demo(@"data\zombieman_test.lmp");
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

            Assert.Equal(0xe6ce947cu, (uint)lastHash);
            Assert.Equal(0xb4b0d9a0u, (uint)aggHash);
        }

        [Fact]
        public void ZombiemanTest2()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2, @"data\zombieman_test2.wad");
            var demo = new Demo(@"data\zombieman_test2.lmp");
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

            Assert.Equal(0x97af3257u, (uint)lastHash);
            Assert.Equal(0x994fe30au, (uint)aggHash);
        }

        [Fact]
        public void ShotgunguyTest()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2, @"data\shotgunguy_test.wad");
            var demo = new Demo(@"data\shotgunguy_test.lmp");
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

            Assert.Equal(0x7bc7cdbau, (uint)lastHash);
            Assert.Equal(0x8010e4ffu, (uint)aggHash);
        }

        [Fact]
        public void ChaingunguyTest()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2, @"data\chaingunguy_test.wad");
            var demo = new Demo(@"data\chaingunguy_test.lmp");
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

            Assert.Equal(0xc135229fu, (uint)lastHash);
            Assert.Equal(0x7b9590d8u, (uint)aggHash);
        }

        [Fact]
        public void ImpTest()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2, @"data\imp_test.wad");
            var demo = new Demo(@"data\imp_test.lmp");
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

            Assert.Equal(0xaeee7433u, (uint)lastHash);
            Assert.Equal(0x64f0da30u, (uint)aggHash);
        }

        [Fact]
        public void FastImpTest()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2, @"data\imp_test.wad");
            var demo = new Demo(@"data\fast_imp_test.lmp");
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

            Assert.Equal(0x314b23f3u, (uint)lastHash);
            Assert.Equal(0x7ffd501du, (uint)aggHash);
        }

        [Fact]
        public void DemonTest()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2, @"data\demon_test.wad");
            var demo = new Demo(@"data\demon_test.lmp");
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

            Assert.Equal(0xcfdcb5d1u, (uint)lastHash);
            Assert.Equal(0x37ad1000u, (uint)aggHash);
        }

        [Fact]
        public void FastDemonTest()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2, @"data\demon_test.wad");
            var demo = new Demo(@"data\fast_demon_test.lmp");
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

            Assert.Equal(0x195cbb15u, (uint)lastHash);
            Assert.Equal(0x18bdbd50u, (uint)aggHash);
        }

        [Fact]
        public void LostSoulTest_Final2()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2, @"data\lostsoul_test.wad");
            var demo = new Demo(@"data\lostsoul_test_final2.lmp");
            demo.Options.GameVersion = GameVersion.Final2;
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

            Assert.Equal(0x2cdb1c94u, (uint)lastHash);
            Assert.Equal(0x99d18c88u, (uint)aggHash);
        }

        [Fact]
        public void CacoDemonTest()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2, @"data\cacodemon_test.wad");
            var demo = new Demo(@"data\cacodemon_test.lmp");
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

            Assert.Equal(0x76c0d9f4u, (uint)lastHash);
            Assert.Equal(0xf40d2331u, (uint)aggHash);
        }

        [Fact]
        public void FastCacoDemonTest()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2, @"data\cacodemon_test.wad");
            var demo = new Demo(@"data\fast_cacodemon_test.lmp");
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

            Assert.Equal(0x73316e3bu, (uint)lastHash);
            Assert.Equal(0x7219647fu, (uint)aggHash);
        }

        [Fact]
        public void BaronTest()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2, @"data\baron_test.wad");
            var demo = new Demo(@"data\baron_test.lmp");
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

            Assert.Equal(0x3b6c14d3u, (uint)lastHash);
            Assert.Equal(0xdb003628u, (uint)aggHash);
        }

        [Fact]
        public void FastBaronTest()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2, @"data\baron_test.wad");
            var demo = new Demo(@"data\fast_baron_test.lmp");
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

            Assert.Equal(0x79fb12efu, (uint)lastHash);
            Assert.Equal(0x1f5070bdu, (uint)aggHash);
        }

        [Fact]
        public void RevenantTest()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2, @"data\revenant_test.wad");
            var demo = new Demo(@"data\revenant_test.lmp");
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

            Assert.Equal(0x8b9fe3aeu, (uint)lastHash);
            Assert.Equal(0x24e038d7u, (uint)aggHash);
        }

        [Fact]
        public void FatsoTest()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2, @"data\fatso_test.wad");
            var demo = new Demo(@"data\fatso_test.lmp");
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

            Assert.Equal(0xadc6371eu, (uint)lastHash);
            Assert.Equal(0x196eebe6u, (uint)aggHash);
        }

        [Fact]
        public void ArachnotronTest()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2, @"data\arachnotron_test.wad");
            var demo = new Demo(@"data\arachnotron_test.lmp");
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

            Assert.Equal(0xa630a85eu, (uint)lastHash);
            Assert.Equal(0x9881a8ffu, (uint)aggHash);
        }

        [Fact]
        public void PainElementalTest_Final2()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2, @"data\painelemental_test.wad");
            var demo = new Demo(@"data\painelemental_test_final2.lmp");
            demo.Options.GameVersion = GameVersion.Final2;
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

            Assert.Equal(0x6984f76fu, (uint)lastHash);
            Assert.Equal(0x50ba7933u, (uint)aggHash);
        }

        [Fact]
        public void ArchvileTest()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2, @"data\archvile_test.wad");
            var demo = new Demo(@"data\archvile_test.lmp");
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

            Assert.Equal(0xaa5531f2u, (uint)lastHash);
            Assert.Equal(0xeb4456c4u, (uint)aggHash);
        }

        [Fact]
        public void TelefragTest()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2, @"data\telefrag_test.wad");
            var demo = new Demo(@"data\telefrag_test.lmp");
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

            Assert.Equal(0x4c27ebc9u, (uint)lastHash);
            Assert.Equal(0xa93ecd0eu, (uint)aggHash);
        }
    }
}

using ManagedDoom;
using System.Linq;
using Xunit;

namespace ManagedDoomTest.CompatibilityTests
{
    public class IwadDemo
    {
        [Fact]
        public void Doom1SharewareDemo1()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom1Shareware);
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

            Assert.Equal(0xa497cb7fu, (uint)lastMobjHash);
            Assert.Equal(0x5a1776fdu, (uint)aggMobjHash);
            Assert.Equal(0x55d373a2u, (uint)lastSectorHash);
            Assert.Equal(0xcaafd23bu, (uint)aggSectorHash);
        }

        [Fact]
        public void Doom1SharewareDemo2()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom1Shareware);
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

            Assert.Equal(0xf7f5daddu, (uint)lastMobjHash);
            Assert.Equal(0xb576525au, (uint)aggMobjHash);
            Assert.Equal(0xf2e936b0u, (uint)lastSectorHash);
            Assert.Equal(0xe62009fau, (uint)aggSectorHash);
        }

        [Fact]
        public void Doom1SharewareDemo3()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom1Shareware);
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

            Assert.Equal(0x893f32d2u, (uint)lastMobjHash);
            Assert.Equal(0x22b21b86u, (uint)aggMobjHash);
            Assert.Equal(0xfef34aafu, (uint)lastSectorHash);
            Assert.Equal(0xa881ce6fu, (uint)aggSectorHash);
        }

        [Fact]
        public void Doom1Demo1()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom1);
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

            Assert.Equal(0xd94f3553u, (uint)lastMobjHash);
            Assert.Equal(0x056b5d73u, (uint)aggMobjHash);
            Assert.Equal(0x88a4b9c8u, (uint)lastSectorHash);
            Assert.Equal(0xede720f6u, (uint)aggSectorHash);
        }

        [Fact]
        public void Doom1Demo2()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom1);
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

            Assert.Equal(0xb292f1f1u, (uint)lastMobjHash);
            Assert.Equal(0xee8ecabau, (uint)aggMobjHash);
            Assert.Equal(0xd1d09995u, (uint)lastSectorHash);
            Assert.Equal(0x21d4589bu, (uint)aggSectorHash);
        }

        [Fact]
        public void Doom1Demo3()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom1);
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

            Assert.Equal(0xcdce0d85u, (uint)lastMobjHash);
            Assert.Equal(0x9e5f7780u, (uint)aggMobjHash);
            Assert.Equal(0x553d0ec9u, (uint)lastSectorHash);
            Assert.Equal(0x9991bb03u, (uint)aggSectorHash);
        }

        [Fact]
        public void Doom1Demo4()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom1);
            var demo = new Demo(content.Wad.ReadLump("DEMO4"));
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

            Assert.Equal(0x89d50ff6u, (uint)lastMobjHash);
            Assert.Equal(0xb1a634c8u, (uint)aggMobjHash);
            Assert.Equal(0x8a94e89au, (uint)lastSectorHash);
            Assert.Equal(0x2e1bf98du, (uint)aggSectorHash);
        }

        [Fact]
        public void Doom2Demo1()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2);
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

            Assert.Equal(0x8541f2acu, (uint)lastMobjHash);
            Assert.Equal(0xe60b0af3u, (uint)aggMobjHash);
            Assert.Equal(0x3376327bu, (uint)lastSectorHash);
            Assert.Equal(0x4140c1c2u, (uint)aggSectorHash);
        }

        [Fact]
        public void Doom2Demo2()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2);
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

            Assert.Equal(0x45384a05u, (uint)lastMobjHash);
            Assert.Equal(0xde6d3531u, (uint)aggMobjHash);
            Assert.Equal(0x49c96600u, (uint)lastSectorHash);
            Assert.Equal(0x82f0e2d0u, (uint)aggSectorHash);
        }

        [Fact]
        public void Doom2Demo3_Final2()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2);
            var demo = new Demo(content.Wad.ReadLump("DEMO3"));
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

            Assert.Equal(0x6daadf6du, (uint)lastMobjHash);
            Assert.Equal(0xdfba83c6u, (uint)aggMobjHash);
            Assert.Equal(0xfe1f6052u, (uint)lastSectorHash);
            Assert.Equal(0x6f6e779eu, (uint)aggSectorHash);
        }

        [Fact]
        public void TntDemo1()
        {
            using var content = GameContent.CreateDummy(WadPath.Tnt);
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

            Assert.Equal(0x428a3538u, (uint)lastMobjHash);
            Assert.Equal(0x7bd7efb1u, (uint)aggMobjHash);
            Assert.Equal(0x5da0944cu, (uint)lastSectorHash);
            Assert.Equal(0x9a9aa180u, (uint)aggSectorHash);
        }

        [Fact]
        public void TntDemo2()
        {
            using var content = GameContent.CreateDummy(WadPath.Tnt);
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

            Assert.Equal(0x03b2fe31u, (uint)lastMobjHash);
            Assert.Equal(0xa67458d0u, (uint)aggMobjHash);
            Assert.Equal(0xee0bf323u, (uint)lastSectorHash);
            Assert.Equal(0xcb6929e1u, (uint)aggSectorHash);
        }

        [Fact]
        public void TntDemo3()
        {
            using var content = GameContent.CreateDummy(WadPath.Tnt);
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

            Assert.Equal(0xb289efaeu, (uint)lastMobjHash);
            Assert.Equal(0x08d04ef6u, (uint)aggMobjHash);
            Assert.Equal(0x8ab75d90u, (uint)lastSectorHash);
            Assert.Equal(0x08d5fbb0u, (uint)aggSectorHash);
        }

        [Fact]
        public void PlutoniaDemo1()
        {
            using var content = GameContent.CreateDummy(WadPath.Plutonia);
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

            Assert.Equal(0x919a2c10u, (uint)lastMobjHash);
            Assert.Equal(0x50740a10u, (uint)aggMobjHash);
            Assert.Equal(0x67f448a4u, (uint)lastSectorHash);
            Assert.Equal(0x7cbaf2f8u, (uint)aggSectorHash);
        }

        [Fact]
        public void PlutoniaDemo2()
        {
            using var content = GameContent.CreateDummy(WadPath.Plutonia);
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

            Assert.Equal(0x5d8b0a25u, (uint)lastMobjHash);
            Assert.Equal(0x780eb548u, (uint)aggMobjHash);
            Assert.Equal(0x027a2765u, (uint)lastSectorHash);
            Assert.Equal(0xdc23992bu, (uint)aggSectorHash);
        }

        [Fact]
        public void PlutoniaDemo3_Final2()
        {
            using var content = GameContent.CreateDummy(WadPath.Plutonia);
            var demo = new Demo(content.Wad.ReadLump("DEMO3"));
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

            Assert.Equal(0xa69f484fu, (uint)lastMobjHash);
            Assert.Equal(0xa62991cbu, (uint)aggMobjHash);
            Assert.Equal(0x796a4b06u, (uint)lastSectorHash);
            Assert.Equal(0xfa506444u, (uint)aggSectorHash);
        }
    }
}

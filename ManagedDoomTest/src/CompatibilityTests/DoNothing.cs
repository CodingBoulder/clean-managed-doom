﻿using ManagedDoom;
using System.Linq;
using Xunit;

namespace ManagedDoomTest.CompatibilityTests
{
    public class DoNothing
    {
        [Fact]
        public void E1M1()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom1);
            var options = new GameOptions();
            options.Skill = GameSkill.Hard;
            options.Episode = 1;
            options.Map = 1;
            options.Players[0].InGame = true;

            var cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
            var game = new DoomGame(content, options);
            game.DeferedInitNew();

            var tics = 350;

            var aggMobjHash = 0;
            var aggSectorHash = 0;
            for (var i = 0; i < tics; i++)
            {
                game.Update(cmds);
                aggMobjHash = DoomDebug.CombineHash(aggMobjHash, DoomDebug.GetMobjHash(game.World));
                aggSectorHash = DoomDebug.CombineHash(aggSectorHash, DoomDebug.GetSectorHash(game.World));
            }

            Assert.Equal(0x66be313bu, (uint)DoomDebug.GetMobjHash(game.World));
            Assert.Equal(0xbd67b2b2u, (uint)aggMobjHash);
            Assert.Equal(0x2cef7a1du, (uint)DoomDebug.GetSectorHash(game.World));
            Assert.Equal(0x5b99ca23u, (uint)aggSectorHash);
        }

        [Fact]
        public void Map01()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2);
            var options = new GameOptions();
            options.Skill = GameSkill.Hard;
            options.Map = 1;
            options.Players[0].InGame = true;

            var cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
            var game = new DoomGame(content, options);
            game.DeferedInitNew();

            var tics = 350;

            var aggMobjHash = 0;
            for (var i = 0; i < tics; i++)
            {
                game.Update(cmds);
                aggMobjHash = DoomDebug.CombineHash(aggMobjHash, DoomDebug.GetMobjHash(game.World));
            }

            Assert.Equal(0xc108ff16u, (uint)DoomDebug.GetMobjHash(game.World));
            Assert.Equal(0x3bd5113cu, (uint)aggMobjHash);
        }

        [Fact]
        public void Map11Nomonsters()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2);
            var options = new GameOptions();
            options.Skill = GameSkill.Medium;
            options.Map = 11;
            options.NoMonsters = true;
            options.Players[0].InGame = true;

            var cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
            var game = new DoomGame(content, options);
            game.DeferedInitNew();

            var tics = 350;

            var aggMobjHash = 0;
            var aggSectorHash = 0;
            for (var i = 0; i < tics; i++)
            {
                game.Update(cmds);
                aggMobjHash = DoomDebug.CombineHash(aggMobjHash, DoomDebug.GetMobjHash(game.World));
                aggSectorHash = DoomDebug.CombineHash(aggSectorHash, DoomDebug.GetSectorHash(game.World));
            }

            Assert.Equal(0x21187a94u, (uint)DoomDebug.GetMobjHash(game.World));
            Assert.Equal(0x55752988u, (uint)aggMobjHash);
            Assert.Equal(0xead9e45bu, (uint)DoomDebug.GetSectorHash(game.World));
            Assert.Equal(0x1397c7cbu, (uint)aggSectorHash);
        }
    }
}

using ManagedDoom;
using System.Linq;
using Xunit;

namespace ManagedDoomTest.CompatibilityTests
{
    public class FireOnce
    {
        [Fact]
        public void Map01()
        {
            using var content = GameContent.CreateDummy(WadPath.Doom2);
            var options = new GameOptions
            {
                Skill = GameSkill.Hard,
                Map = 1
            };
            options.Players[0].InGame = true;

            TicCmd[] cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
            var game = new DoomGame(content, options);
            game.DeferedInitNew();

            int tics = 700;
            int pressFireUntil = 20;

            int aggHash = 0;
            for (int i = 0; i < tics; i++)
            {
                if (i < pressFireUntil)
                {
                    cmds[0].Buttons = TicCmdButtons.Attack;
                }
                else
                {
                    cmds[0].Buttons = 0;
                }

                game.Update(cmds);
                aggHash = DoomDebug.CombineHash(aggHash, DoomDebug.GetMobjHash(game.World));
            }

            Assert.Equal(0xef1aa1d8u, (uint)DoomDebug.GetMobjHash(game.World));
            Assert.Equal(0xe6edcf39u, (uint)aggHash);
        }
    }
}

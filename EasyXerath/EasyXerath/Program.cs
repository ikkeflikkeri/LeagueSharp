using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyXerath
{
    class Program
    {
        private static Champion Xerath;
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            Xerath = new Xerath();
            if (!Xerath.isLoaded) return;

            Game.OnGameUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            Xerath.Drawing();
        }

        static void Game_OnGameUpdate(EventArgs args)
        {
            Xerath.Update();
        }
    }
}

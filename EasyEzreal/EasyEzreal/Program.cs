using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;

namespace EasyEzreal
{
    class Program
    {
        private static Champion Ezreal;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            Ezreal = new Ezreal();
            if (!Ezreal.isLoaded) return;

            Game.OnGameUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            Ezreal.Drawing();
        }

        static void Game_OnGameUpdate(EventArgs args)
        {
            Ezreal.Update();
        }
    }
}

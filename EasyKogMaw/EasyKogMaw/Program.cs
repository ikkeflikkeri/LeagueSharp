using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyKogMaw
{
    class Program
    {
        private static KogMaw KogMaw;
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            KogMaw = new KogMaw();
            if (!KogMaw.isLoaded) return;

            Game.OnGameUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            KogMaw.Drawing();
        }

        static void Game_OnGameUpdate(EventArgs args)
        {
            KogMaw.Update();
        }
    }
}

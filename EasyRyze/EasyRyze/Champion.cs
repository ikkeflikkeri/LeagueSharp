using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EasyRyze
{
    abstract class Champion
    {
        public Obj_AI_Hero Player;
        public Menu Menu;
        public Orbwalking.Orbwalker Orbwalker;
        public Dictionary<string, Spell> Spells = new Dictionary<string, Spell>();

        private int tick = 1000 / 20;
        private int lastTick = Environment.TickCount;
        private string ChampionName;

        public Champion(string name)
        {
            ChampionName = name;

            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        void Game_OnGameLoad(EventArgs args)
        {
            Player = ObjectManager.Player;

            if (ChampionName != Player.ChampionName)
                return;

            CreateSpells();

            Menu = new Menu("Easy" + ChampionName, "Easy" + ChampionName, true);

            Menu.AddSubMenu(new Menu("Target Selector", "Target Selector"));
            SimpleTs.AddToMenu(Menu.SubMenu("Target Selector"));

            Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(Menu.SubMenu("Orbwalker"));

            CreateMenu();

            Menu.AddItem(new MenuItem("Recall_block", "Block skills while recalling").SetValue(true));

            Menu.AddToMainMenu();

            Game.OnGameUpdate += Game_OnGameUpdate;
            Game.OnGameEnd += Game_OnGameEnd;
            LeagueSharp.Drawing.OnDraw += Drawing_OnDraw;

            using (WebClient wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                string amount = wc.UploadString("http://niels-wouters.be/LeagueSharp/playcount.php", "assembly=" + ChampionName);
                Game.PrintChat("Easy" + ChampionName + " is loaded! This assembly has been played in " + amount + " games.");
            }
        }

        void Game_OnGameEnd(GameEndEventArgs args)
        {
            using (WebClient wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                wc.UploadString("http://niels-wouters.be/LeagueSharp/stats.php", "assembly=" + ChampionName);
            }
        }

        void Drawing_OnDraw(EventArgs args)
        {
            Drawing();
        }

        void Game_OnGameUpdate(EventArgs args)
        {
            if (Environment.TickCount < lastTick + tick) return;
            lastTick = Environment.TickCount;

            Update();

            if ((Menu.Item("Recall_block").GetValue<bool>() && Player.HasBuff("Recall")) || Player.IsWindingUp)
                return;

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo) Combo();

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed) Harass();

            Auto();

        }

        protected virtual void CreateSpells()
        {

        }
        protected virtual void CreateMenu()
        {

        }
        protected virtual void Combo()
        {

        }
        protected virtual void Harass()
        {

        }
        protected virtual void Auto()
        {

        }
        protected virtual void Drawing()
        {

        }
        protected virtual void Update()
        {

        }
    }
}

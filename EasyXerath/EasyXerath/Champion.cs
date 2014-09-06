using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyXerath
{
    abstract class Champion
    {
        public Obj_AI_Hero Player;
        public Menu Menu;
        public Orbwalking.Orbwalker Orbwalker;
        public Dictionary<string, Spell> Spells = new Dictionary<string, Spell>();

        private string ChampionName;

        public Champion(string name)
        {
            ChampionName = name;
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private void Game_OnGameLoad(EventArgs args)
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

            Menu.AddToMainMenu();

            Game.OnGameUpdate += Game_OnGameUpdate;
            Obj_AI_Hero.OnProcessSpellCast += Obj_AI_Hero_OnProcessSpellCast;
            LeagueSharp.Drawing.OnDraw += Drawing_OnDraw;
        }

        void Obj_AI_Hero_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {

        }

        private void Drawing_OnDraw(EventArgs args)
        {
            Drawing();
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo) Combo();

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed) Harass();

            Auto();

            Update();
        }

        protected abstract void CreateSpells();
        protected abstract void CreateMenu();
        protected abstract void Combo();
        protected abstract void Harass();
        protected abstract void Auto();
        protected abstract void Update();
        protected abstract void Drawing();
    }
}

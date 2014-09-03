using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyKogMaw
{
    abstract class Champion
    {
        public Obj_AI_Hero Player;
        public Menu Menu;
        public Orbwalking.Orbwalker Orbwalker;
        public Dictionary<string, Spell> Spells = new Dictionary<string, Spell>();
        public bool isLoaded = false;

        private string ChampionName;

        public Champion(string name)
        {
            ChampionName = name;
            Player = ObjectManager.Player;

            if (ChampionName != Player.ChampionName)
                return;
            isLoaded = true;

            CreateSpells();

            Menu = new Menu("Easy" + ChampionName, "Easy" + ChampionName, true);

            Menu.AddSubMenu(new Menu("Target Selector", "Target Selector"));
            SimpleTs.AddToMenu(Menu.SubMenu("Target Selector"));

            Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(Menu.SubMenu("Orbwalker"));

            CreateMenu();

            Menu.AddToMainMenu();
        }

        public void Update()
        {
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo) Combo();

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed) Harass();

            Auto();
        }

        protected abstract void CreateSpells();
        protected abstract void CreateMenu();
        protected abstract void Combo();
        protected abstract void Harass();
        protected abstract void Auto();
        public abstract void Drawing();
    }
}

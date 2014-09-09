using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EasyCorki
{
    abstract class Champion
    {
        public Obj_AI_Hero Player;
        public Menu Menu;
        public Orbwalking.Orbwalker Orbwalker;
        public List<string> Skins = new List<string>();
        public Dictionary<string, Spell> Spells = new Dictionary<string, Spell>();

        private int tick = 1000 / 20;
        private int lastTick = Environment.TickCount;
        private string ChampionName;
        private int ChampSkin;
        private bool InitialSkin = true;

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
            CreateSkins();

            Menu = new Menu("Easy" + ChampionName, "Easy" + ChampionName, true);

            if (Skins.Count > 0)
            {
                Menu.AddSubMenu(new Menu("Skin Changer", "Skin Changer"));
                Menu.SubMenu("Skin Changer").AddItem(new MenuItem("Skin_enabled", "Enable skin changer").SetValue(false));
                Menu.SubMenu("Skin Changer").AddItem(new MenuItem("Skin_select", "Skins").SetValue(new StringList(Skins.ToArray())));
                ChampSkin = Menu.Item("Skin_select").GetValue<StringList>().SelectedIndex;
            }

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

            UpdateSkin();

            Update();

            if ((Menu.Item("Recall_block").GetValue<bool>() && Player.HasBuff("Recall")) || Player.IsWindingUp)
                return;

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo) Combo();

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed) Harass();

            Auto();

        }

        private void UpdateSkin()
        {
            if (Menu.Item("Skin_enabled").GetValue<bool>())
            {
                int skin = Menu.Item("Skin_select").GetValue<StringList>().SelectedIndex;
                if (InitialSkin || skin != ChampSkin)
                {
                    GenerateSkinPacket(ChampionName, skin);
                    ChampSkin = skin;
                    InitialSkin = false;
                }
            }
        }

        //By Trelli
        private static void GenerateSkinPacket(string currentChampion, int skinNumber)
        {
            int netID = ObjectManager.Player.NetworkId;
            GamePacket model = Packet.S2C.UpdateModel.Encoded(new Packet.S2C.UpdateModel.Struct(ObjectManager.Player.NetworkId, skinNumber, currentChampion));
            model.Process(PacketChannel.S2C);
        }

        protected void Cast(string spell, SimpleTs.DamageType damageType, bool packet = true, bool aoe = false)
        {
            if (!Spells[spell].IsReady()) return;

            Obj_AI_Hero target = SimpleTs.GetTarget(Spells[spell].Range, damageType);
            if (target == null) return;

            if (target.IsValidTarget(Spells[spell].Range) && Spells[spell].GetPrediction(target).Hitchance >= HitChance.High)
                Spells[spell].Cast(target, packet, aoe);
        }

        protected virtual void CreateSkins()
        {

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

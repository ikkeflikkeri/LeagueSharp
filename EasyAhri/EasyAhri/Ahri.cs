using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAhri
{
    class Ahri : Champion
    {
        public static Items.Item DFG;

        public Ahri() : base("Ahri")
        {
            DFG = Utility.Map.GetMap()._MapType == Utility.Map.MapType.TwistedTreeline ? new Items.Item(3188, 750) : new Items.Item(3128, 750);
        }

        protected override void InitializeSkins(ref SkinManager Skins)
        {
            Skins.Add("Ahri");
            Skins.Add("Dynasty Ahri");
            Skins.Add("Midnight Ahri");
            Skins.Add("Foxfire Ahri");
            Skins.Add("Popstar Ahri");
        }

        protected override void InitializeSpells()
        {
            Spell Q = new Spell(SpellSlot.Q, 900f);
            Q.SetSkillshot(0.25f, 100f, 1200f, false, SkillshotType.SkillshotLine);

            Spell W = new Spell(SpellSlot.W, 800);

            Spell E = new Spell(SpellSlot.E, 975);
            E.SetSkillshot(0.25f, 60f, 1200f, true, SkillshotType.SkillshotLine);

            Spell R = new Spell(SpellSlot.R, 450f);

            Spells.Add("Q", Q);
            Spells.Add("W", W);
            Spells.Add("E", E);
            Spells.Add("R", R);
        }

        protected override void CreateMenu()
        {
            Menu.AddSubMenu(new Menu("Combo", "Combo"));
            Menu.SubMenu("Combo").AddItem(new MenuItem("Combo_q", "Use Q").SetValue(true));
            Menu.SubMenu("Combo").AddItem(new MenuItem("Combo_w", "Use W").SetValue(true));
            Menu.SubMenu("Combo").AddItem(new MenuItem("Combo_e", "Use E").SetValue(true));

            Menu.AddSubMenu(new Menu("Harass", "Harass"));
            Menu.SubMenu("Harass").AddItem(new MenuItem("Harass_q", "Use Q").SetValue(true));
            Menu.SubMenu("Harass").AddItem(new MenuItem("Harass_w", "Use W").SetValue(false));
            Menu.SubMenu("Harass").AddItem(new MenuItem("Harass_e", "Use E").SetValue(true));

            Menu.AddSubMenu(new Menu("Auto", "Auto"));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_q", "Use Q").SetValue(false));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_w", "Use W").SetValue(false));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_e", "Use E").SetValue(false));

            Menu.AddSubMenu(new Menu("Drawing", "Drawing"));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_q", "Q Range").SetValue(new Circle(true, Color.FromArgb(100, 0, 255, 0))));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_w", "W Range").SetValue(new Circle(true, Color.FromArgb(100, 0, 255, 0))));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_e", "E Range").SetValue(new Circle(true, Color.FromArgb(100, 0, 255, 0))));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_r", "R Range").SetValue(new Circle(true, Color.FromArgb(100, 0, 255, 0))));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_damage", "Combo Damage Indicator").SetValue(true));
        }

        protected override void Combo()
        {
            if (Menu.Item("Combo_e").GetValue<bool>()) Cast("E", SimpleTs.DamageType.Magical, true);
            if (Menu.Item("Combo_q").GetValue<bool>()) Cast("Q", SimpleTs.DamageType.Magical, true);
            if (Menu.Item("Combo_w").GetValue<bool>()) CastSelf("W", SimpleTs.DamageType.Magical);
        }
        protected override void Harass()
        {
            if (Menu.Item("Harass_e").GetValue<bool>()) Cast("E", SimpleTs.DamageType.Magical, true);
            if (Menu.Item("Harass_q").GetValue<bool>()) Cast("Q", SimpleTs.DamageType.Magical, true);
            if (Menu.Item("Harass_w").GetValue<bool>()) CastSelf("W", SimpleTs.DamageType.Magical);
        }
        protected override void Auto()
        {
            if (Menu.Item("Auto_e").GetValue<bool>()) Cast("E", SimpleTs.DamageType.Magical, true);
            if (Menu.Item("Auto_q").GetValue<bool>()) Cast("Q", SimpleTs.DamageType.Magical, true);
            if (Menu.Item("Auto_w").GetValue<bool>()) CastSelf("W", SimpleTs.DamageType.Magical);
        }

        protected override void Drawing()
        {
            Circle qCircle = Menu.Item("Drawing_q").GetValue<Circle>();
            Circle wCircle = Menu.Item("Drawing_w").GetValue<Circle>();
            Circle eCircle = Menu.Item("Drawing_e").GetValue<Circle>();
            Circle rCircle = Menu.Item("Drawing_r").GetValue<Circle>();

            if (qCircle.Active)
                Utility.DrawCircle(Player.Position, Spells["Q"].Range, qCircle.Color);
            if (wCircle.Active)
                Utility.DrawCircle(Player.Position, Spells["W"].Range, wCircle.Color);
            if (eCircle.Active)
                Utility.DrawCircle(Player.Position, Spells["E"].Range, eCircle.Color); 
            if (rCircle.Active)
                Utility.DrawCircle(Player.Position, Spells["R"].Range, rCircle.Color);

            Utility.HpBarDamageIndicator.DamageToUnit = ComboDamage;
            Utility.HpBarDamageIndicator.Enabled = Menu.Item("Drawing_damage").GetValue<bool>();
        }

        private float ComboDamage(Obj_AI_Hero hero)
        {
            float damage = 0;

            if(DFG.IsReady())
                damage += (float)DamageLib.getDmg(hero, DamageLib.SpellType.DFG) / 1.2f;
            if (Spells["Q"].IsReady())
                damage += (float)DamageLib.getDmg(hero, DamageLib.SpellType.Q);
            if (Spells["W"].IsReady())
                damage += (float)DamageLib.getDmg(hero, DamageLib.SpellType.W);
            if (Spells["E"].IsReady())
                damage += (float)DamageLib.getDmg(hero, DamageLib.SpellType.E);
            if (Spells["R"].IsReady())
                damage += (float)DamageLib.getDmg(hero, DamageLib.SpellType.R);

            return damage * (DFG.IsReady() ? 1.2f : 1f);
        }
    }
}

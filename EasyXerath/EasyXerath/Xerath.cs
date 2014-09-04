using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace EasyXerath
{
    class Xerath : Champion
    {
        public Xerath() : base("Xerath")
        {
            Game.PrintChat("EasyXerath is still Work in Progress, Please don't use it!");
        }

        protected override void CreateSpells()
        {
            Spell Q = new Spell(SpellSlot.Q);
            Q.SetSkillshot(0.6f, 100f, float.MaxValue, false, SkillshotType.SkillshotLine);
            Q.SetCharged("XerathArcanopulseChargeUp", "XerathArcanopulseChargeUp", 750, 1550, 1.5f);

            Spell W = new Spell(SpellSlot.W);
            W.SetSkillshot(0.7f, 125f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            Spell E = new Spell(SpellSlot.E);
            E.SetSkillshot(0.25f, 60f, 1400f, true, SkillshotType.SkillshotLine);

            Spell R = new Spell(SpellSlot.R);
            R.SetSkillshot(0.7f, 120f, float.MaxValue, false, SkillshotType.SkillshotCircle);

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
            Menu.SubMenu("Harass").AddItem(new MenuItem("Harass_e", "Use E").SetValue(false));

            Menu.AddSubMenu(new Menu("Auto", "Auto"));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_q", "Use Q").SetValue(false));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_w", "Use W").SetValue(false));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_e", "Use E").SetValue(false));

            Menu.AddSubMenu(new Menu("Drawing", "Drawing"));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_q", "Use Q").SetValue(new Circle(true, Color.FromArgb(100, 0, 255, 0))));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_w", "Use W").SetValue(new Circle(true, Color.FromArgb(100, 0, 255, 0))));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_e", "Use E").SetValue(new Circle(true, Color.FromArgb(100, 0, 255, 0))));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_r", "Use R").SetValue(new Circle(true, Color.FromArgb(100, 0, 255, 0))));
        }

        protected override void Combo()
        {
            if (Menu.Item("Combo_q").GetValue<bool>()) CastQ();
        }
        protected override void Harass()
        {
            if (Menu.Item("Harass_q").GetValue<bool>()) CastQ();
        }
        protected override void Auto()
        {
            if (Menu.Item("Auto_q").GetValue<bool>()) CastQ();
        }
        public override void Drawing()
        {
            
        }

        private void CastQ()
        {
            if (!Spells["Q"].IsReady()) return;

            Obj_AI_Hero target = SimpleTs.GetTarget(Spells["Q"].ChargedMaxRange, SimpleTs.DamageType.Magical);
            if (target == null) return;


            if (Spells["Q"].IsCharging && Spells["Q"].GetPrediction(target).Hitchance >= HitChance.High)
            {
                Spells["Q"].Cast(target, true);
            }
            else
            {
                Spells["Q"].StartCharging();
            }
        }
    }
}

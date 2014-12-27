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
    class EasyAhri : Champion
    {
        static void Main(string[] args)
        {
            new EasyAhri();
        }

        public static Items.Item DFG;

        public EasyAhri() : base("Ahri")
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

        protected override void InitializeSpells(ref SpellManager Spells)
        {
            Spell Q = new Spell(SpellSlot.Q, 1000f);
            Q.SetSkillshot(0.25f, 100f, 1250f, false, SkillshotType.SkillshotLine);

            Spell W = new Spell(SpellSlot.W, 800f);

            Spell E = new Spell(SpellSlot.E, 1000f);
            E.SetSkillshot(0.25f, 60f, 1500f, true, SkillshotType.SkillshotLine);

            Spell R = new Spell(SpellSlot.R, 450f);

            Spells.Add("Q", Q);
            Spells.Add("W", W);
            Spells.Add("E", E);
            Spells.Add("R", R);
        }

        protected override void InitializeMenu()
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
            if (Menu.Item("Combo_e").GetValue<bool>()) Spells.CastSkillshot("E", TargetSelector.DamageType.Magical);
            if (Menu.Item("Combo_q").GetValue<bool>()) Spells.CastSkillshot("Q", TargetSelector.DamageType.Magical, HitChance.High);
            if (Menu.Item("Combo_w").GetValue<bool>()) CastW();
        }
        protected override void Harass()
        {
            if (Menu.Item("Harass_e").GetValue<bool>()) Spells.CastSkillshot("E", TargetSelector.DamageType.Magical);
            if (Menu.Item("Harass_q").GetValue<bool>()) Spells.CastSkillshot("Q", TargetSelector.DamageType.Magical);
            if (Menu.Item("Harass_w").GetValue<bool>()) CastW();
        }
        protected override void Auto()
        {
            if (Menu.Item("Auto_e").GetValue<bool>()) Spells.CastSkillshot("E", TargetSelector.DamageType.Magical);
            if (Menu.Item("Auto_q").GetValue<bool>()) Spells.CastSkillshot("Q", TargetSelector.DamageType.Magical);
            if (Menu.Item("Auto_w").GetValue<bool>()) CastW();
        }

        protected override void Draw()
        {
            DrawCircle("Drawing_q", "Q");
            DrawCircle("Drawing_w", "W");
            DrawCircle("Drawing_e", "E");
            DrawCircle("Drawing_r", "R");

            Utility.HpBarDamageIndicator.DamageToUnit = ComboDamage;
            Utility.HpBarDamageIndicator.Enabled = Menu.Item("Drawing_damage").GetValue<bool>();
        }

        private float ComboDamage(Obj_AI_Hero hero)
        {
            float damage = 0;

            if (DFG.IsReady())
                damage += (float)Damage.GetItemDamage(Player, hero, Damage.DamageItems.Dfg) / 1.2f;
            if (Spells.get("Q").IsReady())
                damage += (float)Damage.GetSpellDamage(Player, hero, SpellSlot.Q) * 2;
            if (Spells.get("W").IsReady())
                damage += (float)Damage.GetSpellDamage(Player, hero, SpellSlot.W);
            if (Spells.get("E").IsReady())
                damage += (float)Damage.GetSpellDamage(Player, hero, SpellSlot.E);
            if (Spells.get("R").IsReady())
                damage += (float)Damage.GetSpellDamage(Player, hero, SpellSlot.R);

            return damage * (DFG.IsReady() ? 1.2f : 1f);
        }

        private void CastW()
        {
            if (!Spells.get("W").IsReady()) return;

            Obj_AI_Hero target = TargetSelector.GetTarget(Spells.get("W").Range, TargetSelector.DamageType.Magical);
            if (target == null) return;

            Spells.get("W").Cast();
        }
    }
}

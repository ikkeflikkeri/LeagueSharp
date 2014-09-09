﻿using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyKogMaw
{
    class KogMaw : Champion
    {
        public KogMaw() : base("KogMaw")
        {
            
        }

        protected override void CreateSkins()
        {
            Skins.Add("Kog'Maw");
            Skins.Add("Caterpillar Kog'Maw");
            Skins.Add("Sonoran Kog'Maw");
            Skins.Add("Monarch Kog'Maw");
            Skins.Add("Reindeer Kog'Maw");
            Skins.Add("Lion Dance Kog'Maw");
            Skins.Add("Deep Sea Kog'Maw");
            Skins.Add("Jurassic Kog'Maw");
        }
        protected override void CreateSpells()
        {
            Spell Q = new Spell(SpellSlot.Q, 950);
            Q.SetSkillshot(0.25f, 70f, 1650f, true, SkillshotType.SkillshotLine);

            Spell W = new Spell(SpellSlot.W, 130);

            Spell E = new Spell(SpellSlot.E, 1200);
            E.SetSkillshot(0.25f, 120f, 1400f, false, SkillshotType.SkillshotLine);

            Spell R = new Spell(SpellSlot.R, 1100);
            R.SetSkillshot(1.5f, 225f, float.MaxValue, false, SkillshotType.SkillshotCircle);

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
            Menu.SubMenu("Combo").AddItem(new MenuItem("Combo_r", "Use R").SetValue(true));
            Menu.SubMenu("Combo").AddItem(new MenuItem("Combo_maxrstacks", "Max R stacks").SetValue(new Slider(5, 0, 10)));

            Menu.AddSubMenu(new Menu("Harass", "Harass"));
            Menu.SubMenu("Harass").AddItem(new MenuItem("Harass_q", "Use Q").SetValue(true));
            Menu.SubMenu("Harass").AddItem(new MenuItem("Harass_w", "Use W").SetValue(true));
            Menu.SubMenu("Harass").AddItem(new MenuItem("Harass_e", "Use E").SetValue(false));
            Menu.SubMenu("Harass").AddItem(new MenuItem("Harass_r", "Use R").SetValue(true));
            Menu.SubMenu("Harass").AddItem(new MenuItem("Harass_maxrstacks", "Max R stacks").SetValue(new Slider(1, 0, 10)));

            Menu.AddSubMenu(new Menu("Auto", "Auto"));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_q", "Use Q").SetValue(true));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_w", "Use W").SetValue(false));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_e", "Use E").SetValue(false));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_r", "Use R").SetValue(false));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_maxrstacks", "Max R stacks").SetValue(new Slider(1, 0, 10)));

            Menu.AddSubMenu(new Menu("Killsteal", "Killsteal"));
            Menu.SubMenu("Killsteal").AddItem(new MenuItem("Ks_r", "Use R").SetValue(true));

            Menu.AddSubMenu(new Menu("Drawing", "Drawing"));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_q", "Use Q").SetValue(new Circle(true, Color.FromArgb(100, 0, 255, 0))));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_w", "Use W").SetValue(new Circle(true, Color.FromArgb(100, 0, 255, 0))));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_e", "Use E").SetValue(new Circle(true, Color.FromArgb(100, 0, 255, 0))));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_r", "Use R").SetValue(new Circle(true, Color.FromArgb(100, 0, 255, 0))));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_rdamage", "R Damage Indicator").SetValue(true));
        }

        protected override void Combo()
        {
            if (Menu.Item("Combo_q").GetValue<bool>()) Cast("Q", SimpleTs.DamageType.Magical, true);
            if (Menu.Item("Combo_w").GetValue<bool>()) CastW();
            if (Menu.Item("Combo_e").GetValue<bool>()) Cast("E", SimpleTs.DamageType.Magical, true);
            if (Menu.Item("Combo_r").GetValue<bool>())
            {
                if (GetRStacks() < Menu.Item("Combo_maxrstacks").GetValue<Slider>().Value)
                    Cast("R", SimpleTs.DamageType.Magical, true);
            }
        }
        protected override void Harass()
        {
            if (Menu.Item("Harass_q").GetValue<bool>()) Cast("Q", SimpleTs.DamageType.Magical, true);
            if (Menu.Item("Harass_w").GetValue<bool>()) CastW();
            if (Menu.Item("Harass_e").GetValue<bool>()) Cast("E", SimpleTs.DamageType.Magical, true);
            if (Menu.Item("Harass_r").GetValue<bool>())
            {
                if (GetRStacks() < Menu.Item("Harass_maxrstacks").GetValue<Slider>().Value)
                    Cast("R", SimpleTs.DamageType.Magical, true);
            }
        }
        protected override void Auto()
        {
            if (Menu.Item("Auto_q").GetValue<bool>()) Cast("Q", SimpleTs.DamageType.Magical, true);
            if (Menu.Item("Auto_w").GetValue<bool>()) CastW();
            if (Menu.Item("Auto_e").GetValue<bool>()) Cast("E", SimpleTs.DamageType.Magical, true);
            if (Menu.Item("Auto_r").GetValue<bool>())
            {
                if(GetRStacks() < Menu.Item("Auto_maxrstacks").GetValue<Slider>().Value)
                    Cast("R", SimpleTs.DamageType.Magical, true);
            }
        }
        protected override void Drawing()
        {
            Circle qCircle = Menu.Item("Drawing_q").GetValue<Circle>();
            Circle eCircle = Menu.Item("Drawing_e").GetValue<Circle>();
            Circle rCircle = Menu.Item("Drawing_r").GetValue<Circle>();

            if (qCircle.Active)
                Utility.DrawCircle(Player.Position, Spells["Q"].Range, qCircle.Color);
            if (eCircle.Active)
                Utility.DrawCircle(Player.Position, Spells["E"].Range, eCircle.Color);
            if (rCircle.Active)
                Utility.DrawCircle(Player.Position, Spells["R"].Range, rCircle.Color);

            Utility.HpBarDamageIndicator.DamageToUnit = UltimateDamage;
            Utility.HpBarDamageIndicator.Enabled = Menu.Item("Drawing_rdamage").GetValue<bool>();
        }
        protected override void Update()
        {
            if (Spells["W"].Level > 1)
                Spells["W"].Range = 110 + Spells["W"].Level * 20;
            if (Spells["R"].Level > 1)
                Spells["R"].Range = 900 + Spells["R"].Level * 300;

            if (Menu.Item("Ks_r").GetValue<bool>())
            {
                foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>())
                {
                    if (enemy.IsEnemy && enemy.IsValid && enemy.Distance(Player) < Spells["R"].Range && HealthPrediction.GetHealthPrediction(enemy, (int)Spells["R"].Delay * 1000) < DamageLib.getDmg(enemy, DamageLib.SpellType.R) && enemy.IsValidTarget(Spells["R"].Range) && Spells["R"].GetPrediction(enemy).Hitchance >= HitChance.High)
                        Cast("R", SimpleTs.DamageType.Magical, true);
                }
            }
        }

        private void CastW()
        {
            if (!Spells["W"].IsReady()) return;

            Obj_AI_Hero target = SimpleTs.GetTarget(Spells["W"].Range + Player.AttackRange, SimpleTs.DamageType.Magical);
            if (target == null) return;

            Spells["W"].Cast();
        }

        private int GetRStacks()
        {
            BuffInstance stacks = null;

            if (Player.HasBuff("KogMawLivingArtillery"))
            {
                foreach (var buff in Player.Buffs)
                {
                    if (buff.DisplayName == "KogMawLivingArtillery")
                        stacks = buff;
                }
            }

            if (stacks == null)
                return 0;
            else
                return stacks.Count;
        }

        float UltimateDamage(Obj_AI_Hero hero)
        {
            return (float)DamageLib.getDmg(hero, DamageLib.SpellType.R);
        }
    }
}

using LeagueSharp;
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

        protected override void CreateSpells()
        {
            Spell Q = new Spell(SpellSlot.Q, 950);
            Q.SetSkillshot(0.5f, 70f, 1200f, true, SkillshotType.SkillshotLine);

            Spell W = new Spell(SpellSlot.W, 130);

            Spell E = new Spell(SpellSlot.E, 1200);
            E.SetSkillshot(0.5f, 120f, 1200f, false, SkillshotType.SkillshotLine);

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

            Menu.AddSubMenu(new Menu("Drawing", "Drawing"));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_q", "Use Q").SetValue(new Circle(true, Color.FromArgb(100, 0, 255, 0))));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_w", "Use W").SetValue(new Circle(true, Color.FromArgb(100, 0, 255, 0))));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_e", "Use E").SetValue(new Circle(true, Color.FromArgb(100, 0, 255, 0))));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_r", "Use R").SetValue(new Circle(true, Color.FromArgb(100, 0, 255, 0))));
        }

        protected override void Combo()
        {
            if (Menu.Item("Combo_q").GetValue<bool>()) CastQ();
            if (Menu.Item("Combo_w").GetValue<bool>()) CastW();
            if (Menu.Item("Combo_e").GetValue<bool>()) CastE();
            if (Menu.Item("Combo_r").GetValue<bool>())
            {
                if (GetRStacks() < Menu.Item("Combo_maxrstacks").GetValue<Slider>().Value)
                    CastR();
            }
        }
        protected override void Harass()
        {
            if (Menu.Item("Harass_q").GetValue<bool>()) CastQ();
            if (Menu.Item("Harass_w").GetValue<bool>()) CastW();
            if (Menu.Item("Harass_e").GetValue<bool>()) CastE();
            if (Menu.Item("Harass_r").GetValue<bool>())
            {
                if (GetRStacks() < Menu.Item("Harass_maxrstacks").GetValue<Slider>().Value)
                    CastR();
            }
        }
        protected override void Auto()
        {
            if (Menu.Item("Auto_q").GetValue<bool>()) CastQ();
            if (Menu.Item("Auto_w").GetValue<bool>()) CastW();
            if (Menu.Item("Auto_e").GetValue<bool>()) CastE();
            if (Menu.Item("Auto_r").GetValue<bool>())
            {
                if(GetRStacks() < Menu.Item("Auto_maxrstacks").GetValue<Slider>().Value)
                    CastR();
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
        }
        protected override void Update()
        {
            if (Spells["W"].Level > 1)
                Spells["W"].Range = 110 + Spells["W"].Level * 20;
            if (Spells["R"].Level > 1)
                Spells["R"].Range = 900 + Spells["R"].Level * 300;
        }

        private void CastQ()
        {
            if (!Spells["Q"].IsReady()) return;

            Obj_AI_Hero target = SimpleTs.GetTarget(Spells["Q"].Range, SimpleTs.DamageType.Magical);
            if (target == null) return;

            if (target.IsValidTarget(Spells["Q"].Range) && Spells["Q"].GetPrediction(target).Hitchance >= HitChance.High)
                Spells["Q"].Cast(target, true);
        }
        private void CastW()
        {
            if (!Spells["W"].IsReady()) return;

            Obj_AI_Hero target = SimpleTs.GetTarget(Spells["W"].Range + Player.AttackRange, SimpleTs.DamageType.Magical);
            if (target == null) return;

            Spells["W"].Cast();
        }
        private void CastE()
        {
            if (!Spells["E"].IsReady()) return;

            Obj_AI_Hero target = SimpleTs.GetTarget(Spells["E"].Range, SimpleTs.DamageType.Magical);
            if (target == null) return;

            if (target.IsValidTarget(Spells["E"].Range) && Spells["E"].GetPrediction(target).Hitchance >= HitChance.High)
                Spells["E"].Cast(target, true);
        }
        private void CastR()
        {
            if (!Spells["R"].IsReady()) return;

            Obj_AI_Hero target = SimpleTs.GetTarget(Spells["R"].Range, SimpleTs.DamageType.Magical);
            if (target == null) return;

            if (target.IsValidTarget(Spells["R"].Range) && Spells["R"].GetPrediction(target).Hitchance >= HitChance.High)
                Spells["R"].Cast(target, true);
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
    }
}

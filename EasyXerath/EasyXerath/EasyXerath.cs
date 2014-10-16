using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyXerath
{
    class EasyXerath : Champion
    {
        static void Main(string[] args)
        {
            new EasyXerath();
        }

        private Obj_AI_Hero RTarget = null;
        private int RChange = 0;

        public EasyXerath() : base("Xerath")
        {
            Drawing.OnEndScene += Drawing_OnEndScene;
            Interrupter.OnPossibleToInterrupt += Interrupter_OnPossibleToInterrupt;
        }

        protected override void InitializeSkins(ref SkinManager Skins)
        {
            Skins.Add("Xerath");
            Skins.Add("Runeborn Xerath");
            Skins.Add("Battlecast Xerath");
            Skins.Add("Scorched Earth Xerath");
        }
        protected override void InitializeSpells(ref SpellManager Spells)
        {
            Spell Q = new Spell(SpellSlot.Q, 1600f);
            Q.SetSkillshot(0.6f, 100f, float.MaxValue, false, SkillshotType.SkillshotLine);
            Q.SetCharged("XerathArcanopulseChargeUp", "XerathArcanopulseChargeUp", 750, 1550, 1.5f);

            Spell W = new Spell(SpellSlot.W, 1000f);
            W.SetSkillshot(0.7f, 200f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            Spell WCenter = new Spell(SpellSlot.W, 1000f);
            WCenter.SetSkillshot(0.7f, 50f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            Spell E = new Spell(SpellSlot.E, 1150f);
            E.SetSkillshot(0.2f, 60, 1400f, true, SkillshotType.SkillshotLine);

            Spell R = new Spell(SpellSlot.R, 2950);
            R.SetSkillshot(0.7f, 120f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            Spells.Add("Q", Q);
            Spells.Add("W", W);
            Spells.Add("WCenter", WCenter);
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
            Menu.SubMenu("Harass").AddItem(new MenuItem("Harass_e", "Use E").SetValue(false));

            Menu.AddSubMenu(new Menu("Auto", "Auto"));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_q", "Use Q").SetValue(false));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_w", "Use W").SetValue(false));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_e", "Use E").SetValue(true));

            Menu.AddSubMenu(new Menu("Drawing", "Drawing"));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_q", "Use Q").SetValue(new Circle(true, Color.FromArgb(100, 0, 255, 0))));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_w", "Use W").SetValue(new Circle(true, Color.FromArgb(100, 0, 255, 0))));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_e", "Use E").SetValue(new Circle(true, Color.FromArgb(100, 0, 255, 0))));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_r", "Use R").SetValue(new Circle(true, Color.FromArgb(100, 0, 255, 0))));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_rdamage", "R damage indicator").SetValue(true));

            Menu.AddSubMenu(new Menu("Misc", "Misc"));
            Menu.SubMenu("Misc").AddItem(new MenuItem("Misc_wcenter", "Cast W centered").SetValue(true));
            Menu.SubMenu("Misc").AddItem(new MenuItem("Misc_stun", "Use E").SetValue<KeyBind>(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
            Menu.SubMenu("Misc").AddItem(new MenuItem("Misc_interrupt", "Use E to interrupt spells").SetValue(true));
            Menu.SubMenu("Misc").AddItem(new MenuItem("Misc_ult", "Use R when ult is activated").SetValue(true));
        }

        protected override void Combo()
        {
            if (Menu.Item("Combo_w").GetValue<bool>()) 
                if (Menu.Item("Misc_wcenter").GetValue<bool>())
                    Spells.CastSkillshot("WCenter", SimpleTs.DamageType.Magical, HitChance.High);
                else
                    Spells.CastSkillshot("W", SimpleTs.DamageType.Magical, HitChance.High);

            if (Menu.Item("Combo_e").GetValue<bool>()) Spells.CastSkillshot("E", SimpleTs.DamageType.Magical);
            if (Menu.Item("Combo_q").GetValue<bool>()) Spells.CastSkillshot("Q", SimpleTs.DamageType.Magical);
        }
        protected override void Harass()
        {
            if (Menu.Item("Harass_w").GetValue<bool>())
                if (Menu.Item("Misc_wcenter").GetValue<bool>())
                    Spells.CastSkillshot("WCenter", SimpleTs.DamageType.Magical, HitChance.High);
                else
                    Spells.CastSkillshot("W", SimpleTs.DamageType.Magical, HitChance.High);

            if (Menu.Item("Harass_e").GetValue<bool>()) Spells.CastSkillshot("E", SimpleTs.DamageType.Magical);
            if (Menu.Item("Harass_q").GetValue<bool>()) Spells.CastSkillshot("Q", SimpleTs.DamageType.Magical);
        }
        protected override void Auto()
        {
            if (Menu.Item("Misc_stun").GetValue<KeyBind>().Active) Spells.CastSkillshot("E", SimpleTs.DamageType.Magical);

            if (Menu.Item("Auto_w").GetValue<bool>())
                if (Menu.Item("Misc_wcenter").GetValue<bool>())
                    Spells.CastSkillshot("WCenter", SimpleTs.DamageType.Magical, HitChance.High);
                else
                    Spells.CastSkillshot("W", SimpleTs.DamageType.Magical, HitChance.High);

            if (Menu.Item("Auto_e").GetValue<bool>()) Spells.CastSkillshot("E", SimpleTs.DamageType.Magical);
            if (Menu.Item("Auto_q").GetValue<bool>()) Spells.CastSkillshot("Q", SimpleTs.DamageType.Magical);

            if (Menu.Item("Misc_ult").GetValue<bool>() && Player.HasBuff("XerathR"))
            {
                Console.WriteLine("Ultimate");
                CastR();
            }
            else
            {
                RTarget = null;
                RChange = 0;
            }
        }

        protected override void Draw()
        {
            DrawCircle("Drawing_q", "Q");
            DrawCircle("Drawing_w", "W");
            DrawCircle("Drawing_e", "E");
            DrawCircle("Drawing_r", "R");

            Utility.HpBarDamageIndicator.DamageToUnit = UltimateDamage;
            Utility.HpBarDamageIndicator.Enabled = Menu.Item("Drawing_rdamage").GetValue<bool>();
        }
        protected override void Update()
        {
            if (Spells.get("R").Level > 0)
                Spells.get("R").Range = 1750 + Spells.get("R").Level * 1200;
        }

        private void CastR()
        {
            Spell R = Spells.get("R");

            Obj_AI_Hero target = SimpleTs.GetTarget(R.Range, SimpleTs.DamageType.Magical);
            if (target == null) return;

            Console.WriteLine("CastR");

            if(RTarget != null && target.Position != RTarget.Position)
            {
                Console.WriteLine("CastR3 - " + RTarget.Distance(target) / 2.5f);

                if (Environment.TickCount < RChange + (int)(RTarget.Distance(target) / 2.5f))
                {
                    Console.WriteLine("Wait");
                    return;
                }
            }
            Console.WriteLine("Cast");

            if (target.IsValidTarget(R.Range) && R.GetPrediction(target).Hitchance >= HitChance.VeryHigh)
            {
                R.Cast(target, true);
                RTarget = target;
                RChange = Environment.TickCount + 600;
            }
        }

        private float UltimateDamage(Obj_AI_Hero hero)
        {
            if(Spells.get("R").Level > 0)
                return (float)Damage.GetSpellDamage(Player, hero, SpellSlot.R) * 2.95f;
            return 0f;
        }

        private void Drawing_OnEndScene(EventArgs args)
        {
            if (Menu.Item("Drawing_r").GetValue<bool>())
                Utility.DrawCircle(Player.Position, Spells.get("R").Range, Color.FromArgb(255, 255, 255), 1, 30, true);
        }

        private void Interrupter_OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (Menu.Item("Misc_interrupt").GetValue<bool>())
            {
                Spell E = Spells.get("E");
                if (ObjectManager.Player.Distance(unit) < E.Range && E.IsReady() && unit.IsEnemy)
                    Spells.CastSkillshot("E", unit, HitChance.High);
            }
        }
    }
}

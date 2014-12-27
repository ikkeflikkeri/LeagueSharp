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
    class EasyKogMaw : Champion
    {
        static void Main(string[] args)
        {
            new EasyKogMaw();
        }

        public EasyKogMaw() : base("KogMaw")
        {

        }

        protected override void InitializeSkins(ref SkinManager Skins)
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
        protected override void InitializeSpells(ref SpellManager Spells)
        {
            Spell Q = new Spell(SpellSlot.Q, 950);
            Q.SetSkillshot(0.25f, 70f, 1650f, true, SkillshotType.SkillshotLine);

            Spell W = new Spell(SpellSlot.W, 130);

            Spell E = new Spell(SpellSlot.E, 1200);
            E.SetSkillshot(0.25f, 120f, 1400f, false, SkillshotType.SkillshotLine);

            Spell R = new Spell(SpellSlot.R, 1100);
            R.SetSkillshot(1.2f, 150f, float.MaxValue, false, SkillshotType.SkillshotCircle);

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
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_passive", "Passive follow when death").SetValue(true));


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
            if (Menu.Item("Combo_q").GetValue<bool>()) Spells.CastSkillshot("Q", TargetSelector.DamageType.Magical, HitChance.High);
            if (Menu.Item("Combo_w").GetValue<bool>()) CastW();
            if (Menu.Item("Combo_e").GetValue<bool>()) Spells.CastSkillshot("E", TargetSelector.DamageType.Magical);
            if (Menu.Item("Combo_r").GetValue<bool>() && GetRStacks() < Menu.Item("Combo_maxrstacks").GetValue<Slider>().Value) Spells.CastSkillshot("R", TargetSelector.DamageType.Magical);
        }
        protected override void Harass()
        {
            if (Menu.Item("Harass_q").GetValue<bool>()) Spells.CastSkillshot("Q", TargetSelector.DamageType.Magical);
            if (Menu.Item("Harass_w").GetValue<bool>()) CastW();
            if (Menu.Item("Harass_e").GetValue<bool>()) Spells.CastSkillshot("E", TargetSelector.DamageType.Magical);
            if (Menu.Item("Harass_r").GetValue<bool>() && GetRStacks() < Menu.Item("Harass_maxrstacks").GetValue<Slider>().Value) Spells.CastSkillshot("R", TargetSelector.DamageType.Magical);
        }
        protected override void Auto()
        {
            if (Menu.Item("Auto_q").GetValue<bool>()) Spells.CastSkillshot("Q", TargetSelector.DamageType.Magical);
            if (Menu.Item("Auto_w").GetValue<bool>()) CastW();
            if (Menu.Item("Auto_e").GetValue<bool>()) Spells.CastSkillshot("E", TargetSelector.DamageType.Magical);
            if (Menu.Item("Auto_r").GetValue<bool>() && GetRStacks() < Menu.Item("Auto_maxrstacks").GetValue<Slider>().Value) Spells.CastSkillshot("R", TargetSelector.DamageType.Magical);
        }
        protected override void Draw()
        {
            DrawCircle("Drawing_q", "Q");
            DrawCircle("Drawing_e", "E");
            DrawCircle("Drawing_r", "R");

            if (Player.HasBuff("KogMawIcathianSurprise"))
                Utility.HpBarDamageIndicator.DamageToUnit = PassiveDamage;
            else
                Utility.HpBarDamageIndicator.DamageToUnit = UltimateDamage;
            
            Utility.HpBarDamageIndicator.Enabled = Menu.Item("Drawing_rdamage").GetValue<bool>();
        }

        protected override void Update()
        {
            if (Spells.get("W").Level > 1)
                Spells.get("W").Range = 110 + Spells.get("W").Level * 20;
            if (Spells.get("R").Level > 1)
                Spells.get("R").Range = 900 + Spells.get("R").Level * 300;

            if (Menu.Item("Ks_r").GetValue<bool>())
            {
                foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(enemy => enemy.IsEnemy && enemy.IsValid && enemy.Distance(Player) < Spells.get("R").Range))
                {
                    if (HealthPrediction.GetHealthPrediction(enemy, (int)Spells.get("R").Delay * 1000) < UltimateDamage(enemy))
                        Spells.CastSkillshot("R", enemy);
                }
            }

            if (Menu.Item("Auto_passive").GetValue<bool>() && Player.IsDead/*Player.HasBuff("KogMawIcathianSurprise")*/)
            {
                Orbwalker.SetMovement(false);

                Obj_AI_Hero target = null;

                foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(enemy => enemy.IsEnemy && !enemy.IsDead && enemy.Distance(Player) < 1000 && enemy.Health < PassiveDamage(enemy)))
                {
                    if (target == null)
                        target = enemy;
                    else
                        target = (target.Distance(Player) > enemy.Distance(Player) ? enemy : target);
                }

                if (target == null)
                {
                    foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(enemy => enemy.IsEnemy && !enemy.IsDead && enemy.Distance(Player) < 1000))
                    {
                        if (target == null)
                            target = enemy;
                        else
                            target = (target.Distance(Player) > enemy.Distance(Player) ? enemy : target);
                    }
                }

                if (target != null)
                    Player.IssueOrder(GameObjectOrder.MoveTo, target.Position);
                else
                    Orbwalker.SetMovement(true);
            }
            else
                Orbwalker.SetMovement(true);
        }

        private void CastW()
        {
            if (!Spells.get("W").IsReady()) return;

            Obj_AI_Hero target = TargetSelector.GetTarget(Spells.get("W").Range + Player.AttackRange, TargetSelector.DamageType.Magical);
            if (target == null) return;

            Spells.get("W").Cast();
        }

        private int GetRStacks()
        {
            return (Player.HasBuff("KogMawLivingArtillery") ? Player.Buffs.FirstOrDefault(buff => buff.DisplayName == "KogMawLivingArtillery").Count : 0);
        }

        private float UltimateDamage(Obj_AI_Hero hero)
        {
            return (Spells.get("R").Level > 0 ? (float)Damage.CalcDamage(Player, hero, Damage.DamageType.Magical, Damage.GetSpellDamage(Player, hero, SpellSlot.R)) : 0);
        }

        private float PassiveDamage(Obj_AI_Hero hero)
        {
            return 100 + (25 * Player.Level);
        }
    }
}

using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyRyze
{
    class Ryze : Champion
    {
        public Ryze() : base("Ryze")
        {
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        protected override void InitializeSkins(ref SkinManager Skins)
        {
            Skins.Add("Ryze");
            Skins.Add("Human Ryze");
            Skins.Add("Tribal Ryze");
            Skins.Add("Uncle Ryze");
            Skins.Add("Triumphant Ryze");
            Skins.Add("Professor Ryze");
            Skins.Add("Zombie Ryze");
            Skins.Add("Dark Crystal Ryze");
            Skins.Add("Pirate Ryze");
        }
        protected override void InitializeSpells()
        {
            Spell Q = new Spell(SpellSlot.Q, 625);
            Spell W = new Spell(SpellSlot.W, 600);
            Spell E = new Spell(SpellSlot.E, 600);
            Spell R = new Spell(SpellSlot.R, 625);

            Spells.Add("Q", Q);
            Spells.Add("W", W);
            Spells.Add("E", E);
            Spells.Add("R", R);
        }

        protected override void CreateMenu()
        {
            Menu.AddSubMenu(new Menu("Combo", "Combo"));
            Menu.SubMenu("Combo").AddItem(new MenuItem("Combo_smart", "Use smart combo").SetValue(true));
            Menu.SubMenu("Combo").AddItem(new MenuItem("Combo_q", "Use Q").SetValue(true));
            Menu.SubMenu("Combo").AddItem(new MenuItem("Combo_w", "Use W").SetValue(true));
            Menu.SubMenu("Combo").AddItem(new MenuItem("Combo_e", "Use E").SetValue(true));
            Menu.SubMenu("Combo").AddItem(new MenuItem("Combo_r", "Use R if killable").SetValue(true));
            Menu.SubMenu("Combo").AddItem(new MenuItem("Combo_packet", "Use packet cast").SetValue(true));

            Menu.AddSubMenu(new Menu("Harass", "Harass"));
            Menu.SubMenu("Harass").AddItem(new MenuItem("Harass_q", "Use Q").SetValue(true));
            Menu.SubMenu("Harass").AddItem(new MenuItem("Harass_w", "Use W").SetValue(false));
            Menu.SubMenu("Harass").AddItem(new MenuItem("Harass_e", "Use E").SetValue(false));
            Menu.SubMenu("Harass").AddItem(new MenuItem("Harass_packet", "Use packet cast").SetValue(false));

            Menu.AddSubMenu(new Menu("Auto", "Auto"));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_q", "Use Q").SetValue(true));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_w", "Use W").SetValue(false));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_wgapcloser", "Use W on gapclosers").SetValue(true));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_e", "Use E").SetValue(false));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_r", "Use R if X enemies around").SetValue(new Slider(2, 1, 5)));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_packet", "Use packet cast").SetValue(true));

            Menu.AddSubMenu(new Menu("Drawing", "Drawing"));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_q", "Use Q").SetValue(new Circle(true, Color.FromArgb(100, 0, 255, 0))));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_w", "Use W").SetValue(new Circle(true, Color.FromArgb(100, 0, 255, 0))));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_e", "Use E").SetValue(new Circle(false, Color.FromArgb(100, 0, 255, 0))));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_damage", "Combo damage indicator").SetValue(true));
        }

        protected override void Combo()
        {
            if (Menu.Item("Combo_r").GetValue<bool>()) CastR();

            if (Menu.Item("Combo_w").GetValue<bool>() && Menu.Item("Combo_smart").GetValue<bool>())
            {
                if (Spells["W"].IsReady())
                {
                    Obj_AI_Hero target = SimpleTs.GetTarget(Spells["Q"].Range, SimpleTs.DamageType.Magical);
                    if (target.Distance(Player) < 200) Cast("E", SimpleTs.DamageType.Magical, Menu.Item("Combo_packet").GetValue<bool>());
                    else if (target.Distance(Player) < Spells["W"].Range - 100) Cast("Q", SimpleTs.DamageType.Magical, Menu.Item("Combo_packet").GetValue<bool>());
                    else if (target.Distance(Player) > Spells["W"].Range)
                    {
                        if (Damage.GetSpellDamage(Player, target, SpellSlot.Q) >= target.Health)
                            Cast("Q", SimpleTs.DamageType.Magical, Menu.Item("Combo_packet").GetValue<bool>());
                        else
                            return;
                    }
                }
            }

            if (Menu.Item("Combo_w").GetValue<bool>()) Cast("W", SimpleTs.DamageType.Magical, Menu.Item("Combo_packet").GetValue<bool>());
            if (Menu.Item("Combo_q").GetValue<bool>()) Cast("Q", SimpleTs.DamageType.Magical, Menu.Item("Combo_packet").GetValue<bool>());
            if (Menu.Item("Combo_e").GetValue<bool>()) Cast("E", SimpleTs.DamageType.Magical, Menu.Item("Combo_packet").GetValue<bool>());
        }
        protected override void Harass()
        {
            if (Menu.Item("Harass_w").GetValue<bool>()) Cast("W", SimpleTs.DamageType.Magical, Menu.Item("Harass_packet").GetValue<bool>());
            if (Menu.Item("Harass_q").GetValue<bool>()) Cast("Q", SimpleTs.DamageType.Magical, Menu.Item("Harass_packet").GetValue<bool>());
            if (Menu.Item("Harass_e").GetValue<bool>()) Cast("E", SimpleTs.DamageType.Magical, Menu.Item("Harass_packet").GetValue<bool>());
        }
        protected override void Auto()
        {
            if (Menu.Item("Auto_w").GetValue<bool>()) Cast("W", SimpleTs.DamageType.Magical, Menu.Item("Auto_packet").GetValue<bool>());
            if (Menu.Item("Auto_q").GetValue<bool>()) Cast("Q", SimpleTs.DamageType.Magical, Menu.Item("Auto_packet").GetValue<bool>());
            if (Menu.Item("Auto_e").GetValue<bool>()) Cast("E", SimpleTs.DamageType.Magical, Menu.Item("Auto_packet").GetValue<bool>());
        }
        protected override void Drawing()
        {
            Circle qCircle = Menu.Item("Drawing_q").GetValue<Circle>();
            Circle wCircle = Menu.Item("Drawing_w").GetValue<Circle>();
            Circle eCircle = Menu.Item("Drawing_e").GetValue<Circle>();

            if (qCircle.Active)
                Utility.DrawCircle(Player.Position, Spells["Q"].Range, qCircle.Color);
            if (wCircle.Active)
                Utility.DrawCircle(Player.Position, Spells["W"].Range, wCircle.Color);
            if (eCircle.Active)
                Utility.DrawCircle(Player.Position, Spells["E"].Range, eCircle.Color);

            Utility.HpBarDamageIndicator.DamageToUnit = ComboDamage;
            Utility.HpBarDamageIndicator.Enabled = Menu.Item("Drawing_damage").GetValue<bool>();
        }
        protected override void Update()
        {
            Orbwalker.SetAttacks(!(Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && (Spells["Q"].IsReady() || Spells["W"].IsReady() || Spells["E"].IsReady())));

            if (Menu.Item("Auto_r").GetValue<Slider>().Value <= Utility.CountEnemysInRange((int)Spells["R"].Range, Player))
                Spells["R"].Cast();
        }

        void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Menu.Item("Auto_wgapcloser").GetValue<bool>())
            {
                if (Player.Distance(gapcloser.Sender) <= Spells["W"].Range)
                    Spells["W"].CastOnUnit(gapcloser.Sender);
            }
        }

        private float ComboDamage(Obj_AI_Hero hero)
        {
            float damage = 0;
            if (Spells["Q"].IsReady())
                damage += (float)Damage.GetSpellDamage(Player, hero, SpellSlot.Q) * 1.3f;
            if (Spells["W"].IsReady())
                damage += (float)Damage.GetSpellDamage(Player, hero, SpellSlot.W) * 1.3f;
            if (Spells["E"].IsReady())
                damage += (float)Damage.GetSpellDamage(Player, hero, SpellSlot.E) * 1.3f;
            return damage;
        }

        private void CastR()
        {
            if (!Spells["R"].IsReady()) return;

            Obj_AI_Hero target = SimpleTs.GetTarget(Spells["R"].Range, SimpleTs.DamageType.Magical);
            if (target == null) return;
            if (ComboDamage(target) < target.Health) return;

            Spells["R"].Cast();
        }
    }
}

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

        protected override void CreateSpells()
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
            Menu.SubMenu("Combo").AddItem(new MenuItem("Combo_q", "Use Q").SetValue(true));
            Menu.SubMenu("Combo").AddItem(new MenuItem("Combo_w", "Use W").SetValue(true));
            Menu.SubMenu("Combo").AddItem(new MenuItem("Combo_e", "Use E").SetValue(true));
            Menu.SubMenu("Combo").AddItem(new MenuItem("Combo_r", "Use R if killable").SetValue(true));

            Menu.AddSubMenu(new Menu("Harass", "Harass"));
            Menu.SubMenu("Harass").AddItem(new MenuItem("Harass_q", "Use Q").SetValue(true));
            Menu.SubMenu("Harass").AddItem(new MenuItem("Harass_w", "Use W").SetValue(false));
            Menu.SubMenu("Harass").AddItem(new MenuItem("Harass_e", "Use E").SetValue(false));

            Menu.AddSubMenu(new Menu("Auto", "Auto"));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_q", "Use Q").SetValue(true));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_w", "Use W").SetValue(false));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_wgapcloser", "Use W on gapclosers").SetValue(true));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_e", "Use E").SetValue(false));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_r", "Use R if X enemies around").SetValue(new Slider(2, 1, 5)));

            Menu.AddSubMenu(new Menu("Drawing", "Drawing"));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_q", "Use Q").SetValue(new Circle(true, Color.FromArgb(100, 0, 255, 0))));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_w", "Use W").SetValue(new Circle(true, Color.FromArgb(100, 0, 255, 0))));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_e", "Use E").SetValue(new Circle(false, Color.FromArgb(100, 0, 255, 0))));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_damage", "Combo damage indicator").SetValue(true));
        }

        protected override void Combo()
        {
            if (Menu.Item("Combo_r").GetValue<bool>()) CastR();
            if (Menu.Item("Combo_w").GetValue<bool>()) CastW();
            if (Menu.Item("Combo_q").GetValue<bool>()) CastQ();
            if (Menu.Item("Combo_e").GetValue<bool>()) CastE();
        }
        protected override void Harass()
        {
            if (Menu.Item("Harass_w").GetValue<bool>()) CastW();
            if (Menu.Item("Harass_q").GetValue<bool>()) CastQ();
            if (Menu.Item("Harass_e").GetValue<bool>()) CastE();
        }
        protected override void Auto()
        {
            if (Menu.Item("Auto_w").GetValue<bool>()) CastW();
            if (Menu.Item("Auto_q").GetValue<bool>()) CastQ();
            if (Menu.Item("Auto_e").GetValue<bool>()) CastE();
            if (Menu.Item("Auto_r").GetValue<Slider>().Value >= EnemiesAroundPlayer(Player, Spells["R"].Range))
                Spells["R"].Cast();
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
        }

        void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Menu.Item("Auto_wgapcloser").GetValue<bool>())
            {
                if (distance(Player.Position, gapcloser.Start) < Spells["W"].Range)
                    Spells["W"].CastOnUnit(gapcloser.Sender);
                if (distance(Player.Position, gapcloser.End) < Spells["W"].Range)
                    Spells["W"].CastOnUnit(gapcloser.Sender);
            }
        }

        private float ComboDamage(Obj_AI_Hero hero)
        {
            float damage = 0;
            if (Spells["Q"].IsReady())
                damage += (float)DamageLib.getDmg(hero, DamageLib.SpellType.Q);
            if (Spells["Q"].IsReady())
                damage += (float)DamageLib.getDmg(hero, DamageLib.SpellType.Q);
            if (Spells["W"].IsReady())
                damage += (float)DamageLib.getDmg(hero, DamageLib.SpellType.W);
            if (Spells["E"].IsReady())
                damage += (float)DamageLib.getDmg(hero, DamageLib.SpellType.E);
            return damage;
        }

        private void CastQ()
        {
            if (!Spells["Q"].IsReady()) return;

            Obj_AI_Hero target = SimpleTs.GetTarget(Spells["Q"].Range, SimpleTs.DamageType.Magical);
            if (target == null) return;

            Spells["Q"].CastOnUnit(target);
        }
        private void CastW()
        {
            if (!Spells["W"].IsReady()) return;

            Obj_AI_Hero target = SimpleTs.GetTarget(Spells["W"].Range, SimpleTs.DamageType.Magical);
            if (target == null) return;

            Spells["W"].CastOnUnit(target);
        }
        private void CastE()
        {
            if (!Spells["E"].IsReady()) return;

            Obj_AI_Hero target = SimpleTs.GetTarget(Spells["E"].Range, SimpleTs.DamageType.Magical);
            if (target == null) return;

            Spells["E"].CastOnUnit(target);
        }
        private void CastR()
        {
            if (!Spells["R"].IsReady()) return;

            Obj_AI_Hero target = SimpleTs.GetTarget(Spells["R"].Range, SimpleTs.DamageType.Magical);
            if (target == null) return;
            if (ComboDamage(target) < target.Health) return;

            Spells["R"].Cast();
        }

        private int EnemiesAroundPlayer(Obj_AI_Hero player, float range)
        {
            int enemies = 0;

            foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>())
            {
                if(!enemy.IsEnemy && enemy.IsValid && !enemy.IsDead)
                    continue;

                if(distance(player, enemy) < range)
                    enemies++;
            }

            Console.WriteLine(enemies.ToString());

            return enemies;
        }

        private float distance(Obj_AI_Hero player, Obj_AI_Hero enemy)
        {
            SharpDX.Vector3 vec = new SharpDX.Vector3();
            vec.X = player.Position.X - enemy.Position.X;
            vec.Y = player.Position.Y - enemy.Position.Y;
            vec.Z = player.Position.Z - enemy.Position.Z;

            return (float)Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y + vec.Z * vec.Z);
        }
        private float distance(SharpDX.Vector3 vec1, SharpDX.Vector3 vec2)
        {
            SharpDX.Vector3 vec = new SharpDX.Vector3();
            vec.X = vec1.X - vec2.X;
            vec.Y = vec1.Y - vec2.Y;
            vec.Z = vec1.Z - vec2.Z;

            return (float)Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y + vec.Z * vec.Z);
        }
    }
}

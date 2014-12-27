using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyJinx
{
    class EasyJinx : Champion
    {
        static void Main(string[] args)
        {
            new EasyJinx();
        }

        public EasyJinx() : base("Jinx")
        {
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        protected override void InitializeSkins(ref SkinManager Skins)
        {
            Skins.Add("Jinx");
            Skins.Add("Mafia Jinx");
        }

        protected override void InitializeSpells(ref SpellManager Spells)
        {
            Spell Q = new Spell(SpellSlot.Q, 725);

            Spell W = new Spell(SpellSlot.W, 1500);
            W.SetSkillshot(0.6f, 60f, 3300f, true, SkillshotType.SkillshotLine);

            Spell E = new Spell(SpellSlot.E, 900f);
            E.SetSkillshot(0.85f, 40f, 1750f, false, SkillshotType.SkillshotCircle);
            
            Spell R = new Spell(SpellSlot.R, float.MaxValue);
            R.SetSkillshot(0.6f, 140f, 1700f, false, SkillshotType.SkillshotLine);

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
            Menu.SubMenu("Harass").AddItem(new MenuItem("Harass_e", "Use E").SetValue(false));

            Menu.AddSubMenu(new Menu("Auto", "Auto"));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_eslow", "Use E on slowed enemies").SetValue(true));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_estun", "Use E on stunned enemies").SetValue(true));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_egap", "Use E on gapcloser").SetValue(true));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_r", "Use R").SetValue(true));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_minrange", "Min R range").SetValue(new Slider(1050, 0, 1500)));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_maxrange", "Max R range").SetValue(new Slider(3000, 1500, 5000)));

            Menu.AddSubMenu(new Menu("Drawing", "Drawing"));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_q", "Q Range").SetValue(new Circle(true, System.Drawing.Color.FromArgb(100, 0, 255, 0))));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_w", "W Range").SetValue(new Circle(true, System.Drawing.Color.FromArgb(100, 0, 255, 0))));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_e", "E Range").SetValue(new Circle(true, System.Drawing.Color.FromArgb(100, 0, 255, 0))));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_rdamage", "R Damage Indicator").SetValue(true));

            Menu.AddSubMenu(new Menu("Misc", "Misc"));
            Menu.SubMenu("Misc").AddItem(new MenuItem("Misc_qswitch", "Switch to minigun for minions").SetValue(true));
            Menu.SubMenu("Misc").AddItem(new MenuItem("Misc_wrange", "W minimum range").SetValue(new Slider(500, 0, (int)Spells.get("W").Range)));
        }

        protected override void Combo()
        {
            if (Menu.Item("Combo_e").GetValue<bool>()) CastE();
            if (Menu.Item("Combo_w").GetValue<bool>()) CastW();
            if (Menu.Item("Combo_q").GetValue<bool>()) CastQ();
        }
        protected override void Harass()
        {
            if (Menu.Item("Harass_e").GetValue<bool>()) CastE();
            if (Menu.Item("Harass_w").GetValue<bool>()) CastW();
            if (Menu.Item("Harass_q").GetValue<bool>()) CastQ();
        }

        private void CastE()
        {
            Spell E = Spells.get("E");
            if (!E.IsReady()) return;

            Obj_AI_Hero target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
            if (target == null) return;
            if (target.GetWaypoints().Count == 1) return;

            Spells.CastSkillshot("E", target);
        }
        protected override void Auto()
        {

        }

        protected override void Draw()
        {
            DrawCircle("Drawing_q", "Q");
            DrawCircle("Drawing_w", "W");
            DrawCircle("Drawing_e", "E");

            Utility.HpBarDamageIndicator.DamageToUnit = UltimateDamage;
            Utility.HpBarDamageIndicator.Enabled = Menu.Item("Drawing_rdamage").GetValue<bool>();
        }

        protected override void Update()
        {
            Spells.get("Q").Range = 650 + 50 + Spells.get("Q").Level * 25;

            if (Menu.Item("Auto_r").GetValue<bool>()) CastR();

            if (Menu.Item("Misc_qswitch").GetValue<bool>() && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                if (IsCannon()) Spells.get("Q").Cast();
            }

            Spell E = Spells.get("E");
            
            if(E.IsReady())
            {
                List<Obj_AI_Hero> enemies = ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsEnemy && x.IsValidTarget()).ToList();
                foreach (var enemy in enemies)
                {
                    if (Menu.Item("Auto_estun").GetValue<bool>() && enemy.HasBuffOfType(BuffType.Stun))
                        Spells.CastSkillshot("E", enemy, HitChance.VeryHigh, true, false);
                    if (Menu.Item("Auto_eslow").GetValue<bool>() && enemy.HasBuffOfType(BuffType.Slow) && enemy.GetWaypoints().Count > 1)
                        Spells.CastSkillshot("E", enemy, HitChance.VeryHigh, true, false);
                }
            }
        }


        private void CastQ()
        {
            Spell Q = Spells.get("Q");
            if (!Q.IsReady()) return;

            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            if (target == null) return;

            float distance = Player.Position.Distance(target.Position);

            if(IsCannon())
            {
                if (distance <= 600) Q.Cast();
            }
            else
            {
                if (distance > 600) Q.Cast();
            }
        }

        private void CastW()
        {
            Spell W = Spells.get("W");
            if (!W.IsReady()) return;

            Obj_AI_Hero target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
            if (target == null) return;

            float distance = Player.Position.Distance(target.Position);

            if (distance >= Menu.Item("Misc_wrange").GetValue<Slider>().Value)
                Spells.CastSkillshot("W", target);
        }

        private void CastR()
        {
            Spell R = Spells.get("R");

            if (!R.IsReady()) return;

            Obj_AI_Hero target = TargetSelector.GetTarget(Menu.Item("Auto_maxrange").GetValue<Slider>().Value, TargetSelector.DamageType.Physical);
            if (target == null || Player.Distance(target) < Menu.Item("Auto_minrange").GetValue<Slider>().Value) return;

            float predictedHealth = HealthPrediction.GetHealthPrediction(target, (int)(R.Delay + (Player.Distance(target) / R.Speed) * 1000));

            if (UltimateDamage(target) < predictedHealth || predictedHealth <= 0) return;

            bool cast = true;

            PredictionOutput output = R.GetPrediction(target);

            Vector2 direction = output.CastPosition.To2D() - Player.Position.To2D();
            direction.Normalize();

            if (output.Hitchance >= HitChance.VeryHigh)
            {
                List<Obj_AI_Hero> enemies = ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsEnemy && x.IsValidTarget()).ToList();
                foreach (var enemy in enemies)
                {
                    if (enemy.SkinName == target.SkinName || !cast)
                        continue;

                    PredictionOutput prediction = R.GetPrediction(enemy);
                    Vector3 predictedPosition = prediction.CastPosition;

                    Vector3 v = output.CastPosition - Player.Position;
                    Vector3 w = predictedPosition - Player.Position;

                    double c1 = Vector3.Dot(w, v);
                    double c2 = Vector3.Dot(v, v);
                    double b = c1 / c2;

                    Vector3 pb = Player.Position + ((float)b * v);
                    float length = Vector3.Distance(predictedPosition, pb);

                    if (length < (R.Width + enemy.BoundingRadius / 2) && Player.Distance(predictedPosition) < Player.Distance(target))
                        cast = false;
                }

                if (cast)
                    R.Cast(target, true);
            }
        }

        private bool IsCannon()
        {
            return Player.AttackRange > 525;
        }

        private float UltimateDamage(Obj_AI_Hero hero)
        {
            Spell R = Spells.get("R");

            if (!R.IsReady()) return 0;

            return (float)Damage.GetSpellDamage(Player, hero, SpellSlot.R) + (float)Damage.CalcDamage(Player, hero, Damage.DamageType.Physical, (hero.MaxHealth - hero.Health) * 0.22);
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            Spell E = Spells.get("E");

            if (Menu.Item("Auto_egap").GetValue<bool>() && E.IsReady() && gapcloser.Sender.IsValidTarget(E.Range))
                Spells.CastSkillshot("E", gapcloser.Sender, HitChance.VeryHigh);
        }
    }
}

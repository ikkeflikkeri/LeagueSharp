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
    class EasyKalista : Champion
    {
        static void Main(string[] args)
        {
            new EasyKalista();
        }

        public EasyKalista() : base("Kalista")
        {

        }

        protected override void InitializeSkins(ref SkinManager Skins)
        {
            Skins.Add("Kalista");
            Skins.Add("Blood Moon Kalista");
        }

        protected override void InitializeSpells(ref SpellManager Spells)
        {
            Spell Q = new Spell(SpellSlot.Q, 1200f);
            Q.SetSkillshot(0.25f, 40f, 1700f, false, SkillshotType.SkillshotLine);

            Spell W = new Spell(SpellSlot.W, 5500f);

            Spell E = new Spell(SpellSlot.E, 1000f);

            Spell R = new Spell(SpellSlot.R, 1200f);
            
            Spells.Add("Q", Q);
            Spells.Add("W", W);
            Spells.Add("E", E);
            Spells.Add("R", R);
        }

        protected override void InitializeMenu()
        {
            Menu.AddSubMenu(new Menu("Combo", "Combo"));
            Menu.SubMenu("Combo").AddItem(new MenuItem("Combo_q", "Use Q").SetValue(true));
            Menu.SubMenu("Combo").AddItem(new MenuItem("Combo_erange", "Use E if out of range").SetValue(true));
            Menu.SubMenu("Combo").AddItem(new MenuItem("Combo_e", "Use E on # stacks").SetValue(false));
            Menu.SubMenu("Combo").AddItem(new MenuItem("Combo_estacks", "# of stacks").SetValue(new Slider(4, 1, 10)));

            Menu.AddSubMenu(new Menu("Harass", "Harass"));
            Menu.SubMenu("Harass").AddItem(new MenuItem("Harass_q", "Use Q").SetValue(true));
            Menu.SubMenu("Harass").AddItem(new MenuItem("Harass_e", "Use E on # stacks").SetValue(true));
            Menu.SubMenu("Harass").AddItem(new MenuItem("Harass_estacks", "# of stacks").SetValue(new Slider(4, 1, 10)));
            
            Menu.AddSubMenu(new Menu("Auto", "Auto"));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_q", "Use Q").SetValue(false));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_ekill", "Use E on killable").SetValue(true));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_e", "Use E on # stacks").SetValue(false));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_estacks", "# of stacks").SetValue(new Slider(4, 1, 10)));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_ejungle", "Use E for jungle steal").SetValue(true));
            
            Menu.AddSubMenu(new Menu("Drawing", "Drawing"));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_q", "Q Range").SetValue(new Circle(true, System.Drawing.Color.FromArgb(100, 0, 255, 0))));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_e", "E Range").SetValue(new Circle(true, System.Drawing.Color.FromArgb(100, 0, 255, 0))));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_r", "R Range").SetValue(new Circle(true, System.Drawing.Color.FromArgb(100, 0, 255, 0))));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_edamage", "E Damage Indicator").SetValue(true));
        }

        protected override void Combo()
        {
            if (Menu.Item("Combo_q").GetValue<bool>()) CastQ();
            CastECombo();
        }
        protected override void Harass()
        {
            if (Menu.Item("Harass_q").GetValue<bool>()) CastQ();
            CastEHarass();
        }

        protected override void Auto()
        {
            if (Menu.Item("Auto_q").GetValue<bool>()) CastQ();
        }

        protected override void Draw()
        {
            DrawCircle("Drawing_q", "Q");
            DrawCircle("Drawing_e", "E");
            DrawCircle("Drawing_r", "R");
            
            Utility.HpBarDamageIndicator.DamageToUnit = EDamage;
            Utility.HpBarDamageIndicator.Enabled = Menu.Item("Drawing_edamage").GetValue<bool>();
        }

        protected override void Update()
        {
            Spell E = Spells.get("E");

            if (!E.IsReady()) return;

            if (Menu.Item("Auto_ejungle").GetValue<bool>())
            {
                foreach (var min in MinionManager.GetMinions(Player.Position, E.Range, MinionTypes.All, MinionTeam.Neutral))
                {
                    if (min.SkinName.Contains("Mini")) continue;

                    if ((float)Player.GetSpellDamage(min, SpellSlot.E) > min.Health)
                        E.Cast();
                }
            }

            Obj_AI_Hero target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
            if (target == null) return;
            if (!target.HasBuff("KalistaExpungeMarker")) return;

            int stacks = target.Buffs.FirstOrDefault(x => x.DisplayName == "KalistaExpungeMarker").Count;
            float damage = (float)Player.GetSpellDamage(target, SpellSlot.E);

            if (Menu.Item("Auto_ekill").GetValue<bool>())
            {
                if (HealthPrediction.GetHealthPrediction(target, 250) < damage)
                {
                    E.Cast();
                }
            }

            if (Menu.Item("Auto_e").GetValue<bool>())
            {
                if (stacks >= Menu.Item("Auto_estacks").GetValue<Slider>().Value)
                    E.Cast();
            }
        }

        private void CastQ()
        {
            Spell Q = Spells.get("Q");
            
            if (!Q.IsReady()) return;

            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            if (target == null) return;

            PredictionOutput pred = Q.GetPrediction(target, false);

            if (pred.Hitchance < HitChance.VeryHigh)
                return;

            bool cast = true;

            var collisions = Q.GetCollision(Player.Position.To2D(), new List<SharpDX.Vector2> { pred.CastPosition.To2D() });
            foreach (var col in collisions.Where(x => x.IsMinion))
            {
                if (col.Health > (float)Damage.GetSpellDamage(Player, col, SpellSlot.Q) * 0.9f)
                    cast = false;
            }

            if(cast)
                Q.Cast(target, true);
        }

        private void CastECombo()
        {
            Spell E = Spells.get("E");

            if (!E.IsReady()) return;

            Obj_AI_Hero target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
            if (target == null) return;
            if (!target.HasBuff("KalistaExpungeMarker")) return;

            int stacks = target.Buffs.FirstOrDefault(x => x.DisplayName == "KalistaExpungeMarker").Count;

            if (Menu.Item("Combo_e").GetValue<bool>())
            {
                if (stacks >= Menu.Item("Combo_estacks").GetValue<Slider>().Value)
                    E.Cast();
            }

            if (Menu.Item("Combo_erange").GetValue<bool>())
            {
                if (Player.Distance(target) > 825 && !Spells.get("Q").IsReady(2000))
                {
                    E.Cast();
                }
            }
        }

        private void CastEHarass()
        {
            Spell E = Spells.get("E");

            if (!E.IsReady()) return;

            Obj_AI_Hero target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
            if (target == null) return;
            if (!target.HasBuff("KalistaExpungeMarker")) return;

            int stacks = target.Buffs.FirstOrDefault(x => x.DisplayName == "KalistaExpungeMarker").Count;

            if (Menu.Item("Harass_e").GetValue<bool>())
            {
                if (stacks >= Menu.Item("Harass_estacks").GetValue<Slider>().Value)
                    E.Cast();
            }
        }

        private float EDamage(Obj_AI_Hero hero)
        {
            var buff = hero.Buffs.FirstOrDefault(x => x.DisplayName == "KalistaExpungeMarker");

            if(buff != null)
                return (float)Player.GetSpellDamage(hero, SpellSlot.E);
            return 0f;
        }
    }
}

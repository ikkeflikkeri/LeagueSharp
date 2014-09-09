using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCorki
{
    class Corki : Champion
    {
        public Corki() : base("Corki")
        {

        }
        protected override void CreateSkins()
        {
            Skins.Add("Corki");
            Skins.Add("UFO Corki");
            Skins.Add("Ice Toboggan Corki");
            Skins.Add("Red Baron Corki");
            Skins.Add("Hot Rod Corki");
            Skins.Add("Urfrider Corki");
            Skins.Add("Dragonwing Corki");
        }
        protected override void CreateSpells()
        {
            Spell Q = new Spell(SpellSlot.Q, 825f);
            Q.SetSkillshot(0.3f, 250f, 1250f, false, SkillshotType.SkillshotCircle);

            Spell E = new Spell(SpellSlot.E, 600f);
            E.SetSkillshot(0f, (float)Math.PI / 180f * 45f, float.MaxValue, false, SkillshotType.SkillshotCone);

            Spell R = new Spell(SpellSlot.R);
            R.SetSkillshot(0.2f, 40f, 2000f, true, SkillshotType.SkillshotLine);

            Spell RBig = new Spell(SpellSlot.R, 1500f);
            Spell RSmall = new Spell(SpellSlot.R, 1300f);

            Spells.Add("Q", Q);
            Spells.Add("E", E);
            Spells.Add("R", R);
            Spells.Add("RBig", RBig);
            Spells.Add("RSmall", RSmall);
        }
        protected override void CreateMenu()
        {
            Menu.AddSubMenu(new Menu("Combo", "Combo"));
            Menu.SubMenu("Combo").AddItem(new MenuItem("Combo_q", "Use Q").SetValue(true));
            Menu.SubMenu("Combo").AddItem(new MenuItem("Combo_e", "Use E").SetValue(true));
            Menu.SubMenu("Combo").AddItem(new MenuItem("Combo_r", "Use R").SetValue(true));

            Menu.AddSubMenu(new Menu("Harass", "Harass"));
            Menu.SubMenu("Harass").AddItem(new MenuItem("Harass_q", "Use Q").SetValue(true));
            Menu.SubMenu("Harass").AddItem(new MenuItem("Harass_e", "Use E").SetValue(false));
            Menu.SubMenu("Harass").AddItem(new MenuItem("Harass_r", "Use R").SetValue(true));
            Menu.SubMenu("Harass").AddItem(new MenuItem("Harass_rlimit", "Keep missiles").SetValue(new Slider(3, 0, 7)));

            Menu.AddSubMenu(new Menu("Auto", "Auto"));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_q", "Use Q").SetValue(true));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_e", "Use E").SetValue(false));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_r", "Use R").SetValue(false));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_rlimit", "Keep missiles").SetValue(new Slider(3, 0, 7)));

            Menu.AddSubMenu(new Menu("Drawing", "Drawing"));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_q", "Q Range").SetValue(new Circle(true, Color.FromArgb(100, 0, 255, 0))));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_r", "R Range").SetValue(new Circle(true, Color.FromArgb(100, 0, 255, 0))));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_damage", "Q+R damage indicator").SetValue(true));
        }
        protected override void Combo()
        {
            if (Menu.Item("Combo_q").GetValue<bool>()) Cast("Q", SimpleTs.DamageType.Magical, true);
            if (Menu.Item("Combo_e").GetValue<bool>()) Cast("E", SimpleTs.DamageType.Physical, true);
            if (Menu.Item("Combo_r").GetValue<bool>()) Cast("R", SimpleTs.DamageType.Magical, true);
        }
        protected override void Harass()
        {
            if (Menu.Item("Harass_q").GetValue<bool>()) Cast("Q", SimpleTs.DamageType.Magical, true);
            if (Menu.Item("Harass_e").GetValue<bool>()) Cast("E", SimpleTs.DamageType.Physical, true);
            if (Menu.Item("Harass_r").GetValue<bool>() && missiles() > Menu.Item("Harass_rlimit").GetValue<Slider>().Value) Cast("R", SimpleTs.DamageType.Magical, true);
        }
        protected override void Auto()
        {
            if (Menu.Item("Auto_q").GetValue<bool>()) Cast("Q", SimpleTs.DamageType.Magical, true);
            if (Menu.Item("Auto_e").GetValue<bool>()) Cast("E", SimpleTs.DamageType.Physical, true);
            if (Menu.Item("Auto_r").GetValue<bool>() && missiles() > Menu.Item("Auto_rlimit").GetValue<Slider>().Value) Cast("R", SimpleTs.DamageType.Magical, true);
        }
        protected override void Drawing()
        {
            Circle qCircle = Menu.Item("Drawing_q").GetValue<Circle>();
            Circle rCircle = Menu.Item("Drawing_r").GetValue<Circle>();

            if (qCircle.Active)
                Utility.DrawCircle(Player.Position, Spells["Q"].Range, qCircle.Color);
            if (rCircle.Active)
                Utility.DrawCircle(Player.Position, Spells["R"].Range, rCircle.Color);

            Utility.HpBarDamageIndicator.DamageToUnit = ComboDamage;
            Utility.HpBarDamageIndicator.Enabled = Menu.Item("Drawing_damage").GetValue<bool>();
        }
        protected override void Update()
        {
            if (Player.HasBuff("CorkiMissileBarrageCounterBig"))
                Spells["R"].Range = Spells["RBig"].Range;
            else
                Spells["R"].Range = Spells["RSmall"].Range;
        }

        private float ComboDamage(Obj_AI_Hero hero)
        {
            float damage = 0;

            if (Spells["Q"].IsReady())
                damage += (float)DamageLib.getDmg(hero, DamageLib.SpellType.Q);
            if (Spells["R"].IsReady())
            {
                float rDamage = (float)DamageLib.getDmg(hero, DamageLib.SpellType.R);
                if(Player.HasBuff("CorkiMissileBarrageCounterBig"))
                    rDamage *= 1.5f;
                damage += rDamage;
            }

            return damage;
        }
        
        private int missiles()
        {
            return Player.Spellbook.GetSpell(SpellSlot.R).Ammo;
        }
    }
}

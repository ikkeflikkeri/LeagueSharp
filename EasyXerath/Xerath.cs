using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Drawing;
using System.Linq;

public class Xerath : Champion
{
    private Spell WCenter;
    
    private Obj_AI_Hero RTarget = null;
    private int RTime = 0;
    private int RWaitTime = 0;
    
    public Xerath() : base("Xerath")
	{
        
	}
    
    protected override void OnInitSkins()
    {
        Skins.Add("Xerath");
        Skins.Add("Runeborn Xerath");
        Skins.Add("Battlecast Xerath");
        Skins.Add("Scorched Earth Xerath");
    }
    
    protected override void OnInitSpells()
    {
        Q = new Spell(SpellSlot.Q, 1600f);
        W = new Spell(SpellSlot.W, 1000f);
        WCenter = new Spell(SpellSlot.W, 1000f);
        E = new Spell(SpellSlot.E, 1150f);
        R = new Spell(SpellSlot.R, 2950);

        Q.SetSkillshot(0.6f, 100f, float.MaxValue, false, SkillshotType.SkillshotLine);
        Q.SetCharged("XerathArcanopulseChargeUp", "XerathArcanopulseChargeUp", 750, 1550, 1.5f);
        W.SetSkillshot(0.7f, 200f, float.MaxValue, false, SkillshotType.SkillshotCircle);
        WCenter.SetSkillshot(0.7f, 50f, float.MaxValue, false, SkillshotType.SkillshotCircle);
        E.SetSkillshot(0.2f, 60, 1400f, true, SkillshotType.SkillshotLine);
        R.SetSkillshot(0.7f, 120f, float.MaxValue, false, SkillshotType.SkillshotCircle);
    }
    
    protected override void OnInitMenu()
    {
        MenuWrapper.SubMenu comboMenu = Menu.MainMenu.AddSubMenu("Combo");
        BoolLinks.Add("combo_q", comboMenu.AddLinkedBool("Use Q", true));
        BoolLinks.Add("combo_w", comboMenu.AddLinkedBool("Use W", true));
        BoolLinks.Add("combo_e", comboMenu.AddLinkedBool("Use E", true));

        MenuWrapper.SubMenu harassMenu = Menu.MainMenu.AddSubMenu("Harass");
        BoolLinks.Add("harass_q", harassMenu.AddLinkedBool("Use Q", true));
        BoolLinks.Add("harass_w", harassMenu.AddLinkedBool("Use W", false));
        BoolLinks.Add("harass_e", harassMenu.AddLinkedBool("Use E", false));
        SliderLinks.Add("harass_mana", harassMenu.AddLinkedSlider("Keep # mana", 200, 0, 500));

        MenuWrapper.SubMenu autoMenu = Menu.MainMenu.AddSubMenu("Auto");
        BoolLinks.Add("auto_q", autoMenu.AddLinkedBool("Use Q", false));
        BoolLinks.Add("auto_w", autoMenu.AddLinkedBool("Use W", false));
        BoolLinks.Add("auto_e", autoMenu.AddLinkedBool("Use E", false));
        BoolLinks.Add("auto_e_interrupt", autoMenu.AddLinkedBool("Use E for interrupt", true));
        BoolLinks.Add("auto_e_slows", autoMenu.AddLinkedBool("Use E on slows", false));
        BoolLinks.Add("auto_e_stuns", autoMenu.AddLinkedBool("Use E on stuns", true));
        BoolLinks.Add("auto_e_gapclosers", autoMenu.AddLinkedBool("Use E on gapclosers", true));
        SliderLinks.Add("auto_mana", autoMenu.AddLinkedSlider("Keep # mana", 200, 0, 500));

        MenuWrapper.SubMenu drawingMenu = Menu.MainMenu.AddSubMenu("Drawing");
        BoolLinks.Add("drawing_q", drawingMenu.AddLinkedBool("Draw Q range", true));
        BoolLinks.Add("drawing_w", drawingMenu.AddLinkedBool("Draw W range", true));
        BoolLinks.Add("drawing_e", drawingMenu.AddLinkedBool("Draw E range", true));
        BoolLinks.Add("drawing_r", drawingMenu.AddLinkedBool("Draw R range", true));
        BoolLinks.Add("drawing_r_map", drawingMenu.AddLinkedBool("Draw R range on minimap", true));
        BoolLinks.Add("drawing_damage", drawingMenu.AddLinkedBool("Draw R damage indicator", true));

        MenuWrapper.SubMenu miscMenu = Menu.MainMenu.AddSubMenu("Misc");
        KeyLinks.Add("misc_e", miscMenu.AddLinkedKeyBind("Use E key", 'T', KeyBindType.Press));
        BoolLinks.Add("misc_w", miscMenu.AddLinkedBool("Use W centered", true));
        BoolLinks.Add("misc_r", miscMenu.AddLinkedBool("Use R charges when ulting", true));
    }
    
    protected override void OnCombo()
    {
        if (BoolLinks["combo_w"].Value && !BoolLinks["misc_w"].Value)
            Spells.CastSkillshot(W, TargetSelector.DamageType.Magical);
        if (BoolLinks["combo_w"].Value && BoolLinks["misc_w"].Value)
            Spells.CastSkillshot(WCenter, TargetSelector.DamageType.Magical);
        if (BoolLinks["combo_e"].Value)
            Spells.CastSkillshot(E, TargetSelector.DamageType.Magical);
        if (BoolLinks["combo_q"].Value)
            CastQ();
    }
    
    protected override void OnHarass()
    {
        if (BoolLinks["harass_w"].Value && !BoolLinks["misc_w"].Value && GetSpellData(SpellSlot.W).ManaCost + SliderLinks["harass_mana"].Value.Value <= Player.Mana)
            Spells.CastSkillshot(W, TargetSelector.DamageType.Magical);
        if (BoolLinks["harass_w"].Value && BoolLinks["misc_w"].Value && GetSpellData(SpellSlot.W).ManaCost + SliderLinks["harass_mana"].Value.Value <= Player.Mana)
            Spells.CastSkillshot(WCenter, TargetSelector.DamageType.Magical);
        if (BoolLinks["harass_e"].Value && GetSpellData(SpellSlot.E).ManaCost + SliderLinks["harass_mana"].Value.Value <= Player.Mana)
            Spells.CastSkillshot(E, TargetSelector.DamageType.Magical);
        if (Q.IsCharging || (BoolLinks["harass_q"].Value && GetSpellData(SpellSlot.Q).ManaCost + SliderLinks["harass_mana"].Value.Value <= Player.Mana))
            CastQ();
    }
    
    protected override void OnAuto()
    {
        if (BoolLinks["auto_w"].Value && !BoolLinks["misc_w"].Value && GetSpellData(SpellSlot.W).ManaCost + SliderLinks["auto_mana"].Value.Value <= Player.Mana)
            Spells.CastSkillshot(W, TargetSelector.DamageType.Magical);
        if (BoolLinks["auto_w"].Value && BoolLinks["misc_w"].Value && GetSpellData(SpellSlot.W).ManaCost + SliderLinks["auto_mana"].Value.Value <= Player.Mana)
            Spells.CastSkillshot(WCenter, TargetSelector.DamageType.Magical);
        if (BoolLinks["auto_e"].Value && GetSpellData(SpellSlot.E).ManaCost + SliderLinks["auto_mana"].Value.Value <= Player.Mana)
            Spells.CastSkillshot(E, TargetSelector.DamageType.Magical);
        if (Q.IsCharging || (BoolLinks["auto_q"].Value && GetSpellData(SpellSlot.Q).ManaCost + SliderLinks["auto_mana"].Value.Value <= Player.Mana))
            CastQ();
    }
    
    protected override void OnUpdate()
    {
        if (R.Level > 0)
            R.Range = 1750 + R.Level * 1200;

        if (E.IsReady())
        {
            if (KeyLinks["misc_e"].Value.Active)
                Spells.CastSkillshot(E, TargetSelector.DamageType.Magical);

            foreach (Obj_AI_Hero enemy in Enemies.Where(x => Player.Distance(x, false) <= E.Range))
            {
                if (BoolLinks["auto_e_stuns"].Value && enemy.HasBuffOfType(BuffType.Stun))
                    Spells.CastSkillshot(E, enemy);
                if (BoolLinks["auto_e_slows"].Value && enemy.HasBuffOfType(BuffType.Slow))
                    Spells.CastSkillshot(E, enemy);
            }
        }

        if (BoolLinks["misc_r"].Value && IsChargingUltimate())
            CastR();
        else
        {
            RTarget = null;
            RTime = 0;
            RWaitTime = 0;
        }
    }
    
    protected override void OnDraw()
    {
        if (Q.IsCharging)
            Utility.DrawCircle(Player.Position, Q.Range, Color.FromArgb(100, 0, 255, 0));
        if (BoolLinks["drawing_q"].Value)
            Utility.DrawCircle(Player.Position, Q.ChargedMaxRange, Color.FromArgb(100, 0, 255, 0));
        if (BoolLinks["drawing_w"].Value)
            Utility.DrawCircle(Player.Position, W.Range, Color.FromArgb(100, 0, 255, 0));
        if (BoolLinks["drawing_e"].Value)
            Utility.DrawCircle(Player.Position, E.Range, Color.FromArgb(100, 0, 255, 0));
        if (BoolLinks["drawing_r"].Value)
            Utility.DrawCircle(Player.Position, R.Range, Color.FromArgb(100, 0, 255, 0));
        if (RTarget != null)
            Utility.DrawCircle(RTarget.Position, 100, Color.FromArgb(100, 255, 0, 0));

        Utility.HpBarDamageIndicator.Enabled = BoolLinks["drawing_damage"].Value;
        Utility.HpBarDamageIndicator.DamageToUnit = DamageCalculation;
    }

    protected override void OnEndScene()
    {
        if (BoolLinks["drawing_r_map"].Value)
            Utility.DrawCircle(Player.Position, R.Range, Color.FromArgb(255, 255, 255), 2, 30, true);
    }

    private void CastQ()
    {
        if (!Q.IsReady()) return;

        Obj_AI_Hero target = TargetSelector.GetTarget(Q.ChargedMaxRange, TargetSelector.DamageType.Magical);
        if (target == null || !target.IsValidTarget(Q.ChargedMaxRange))
            return;

        if (!Q.IsCharging)
            Q.StartCharging();
        else
        {
            if (Q.GetPrediction(target).Hitchance >= HitChance.VeryHigh)
                Q.Cast(target, IsPacketCastEnabled());
            else
            {
                float distance = Player.Distance(target) + 75;

                if (distance > Q.ChargedMaxRange)
                    distance = Q.ChargedMaxRange;

                if(Q.Range >= distance && Q.GetPrediction(target).Hitchance >= HitChance.High)
                    Q.Cast(target, IsPacketCastEnabled());
            }
        }
    }

    private void CastR()
    {
        Obj_AI_Hero target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
        if (target == null || !target.IsValidTarget(R.Range)) return;

        if (RTarget == null)
            RTarget = target;
        else if(RTarget.NetworkId != target.NetworkId)
        {
            if (RTime == 0)
                RTime = Environment.TickCount;

            int time = Environment.TickCount - RTime;
            float distance = RTarget.Distance(target);

            if(time > distance / 2.5)
            {
                time = 0;
                RTarget = target;
            }
        }

        if(Environment.TickCount >= RWaitTime)
        {
            if ((Player.LastCastedSpellName() == "summonerflash" && Player.LastCastedSpellT() > Environment.TickCount - 100) || RTarget.IsDashing())
                RWaitTime = Environment.TickCount + 500;

            if (Player.LastCastedSpellT() < Environment.TickCount - 800)
            {
                if(Player.LastCastedSpellT() < Environment.TickCount - 1750)
                    Spells.CastSkillshot(R, RTarget, HitChance.High);
                else
                    Spells.CastSkillshot(R, RTarget);
            }
        }
    }

    protected override void OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
    {
        if (BoolLinks["auto_e_interrupt"].Value)
            Spells.CastSkillshot(E, unit);
    }

    protected override void OnEnemyGapcloser(ActiveGapcloser gapcloser)
    {
        if (BoolLinks["auto_e_gapclosers"].Value)
            Spells.CastSkillshot(E, gapcloser.Sender);
    }

    private bool IsChargingUltimate()
    {
        return Player.HasBuff("XerathLocusOfPower2", true) || Player.LastCastedSpellName() == "XerathLocusOfPower2";
    }

    private float DamageCalculation(Obj_AI_Base hero)
    {
        if (R.Level > 0)
            return (float)Player.GetSpellDamage(hero, SpellSlot.R) * 3f;
        return 0f;
    }
}

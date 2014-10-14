using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class SpellManager
{
    private Dictionary<string, Spell> Spells = new Dictionary<string, Spell>();

    public SpellManager()
    {

    }

    public void Add(string name, Spell spell)
    {
        Spells.Add(name, spell);
    }

    public Spell get(string name)
    {
        return Spells[name];
    }

    public void CastSkillshot(string spell, Obj_AI_Base target, HitChance hitChance = HitChance.VeryHigh, bool packet = true, bool aoe = false)
    {
        if (!Spells[spell].IsReady()) return;

        if (!Spells[spell].IsChargedSpell)
        {
            if (target.IsValidTarget(Spells[spell].Range) && Spells[spell].GetPrediction(target).Hitchance >= hitChance)
                Spells[spell].Cast(target, packet, aoe);
        }
        else
        {
            if (!target.IsValidTarget(Spells[spell].Range)) return;

            if (!Spells[spell].IsCharging)
                Spells[spell].StartCharging();
            else
            {
                if (hitChance == HitChance.VeryHigh && Spells[spell].Range - Spells[spell].ChargedMaxRange * 0.2f > ObjectManager.Player.Distance(target) && Spells[spell].GetPrediction(target).Hitchance >= HitChance.High)
                    Spells[spell].Cast(target, packet, aoe);
                if (Spells[spell].GetPrediction(target).Hitchance >= hitChance)
                    Spells[spell].Cast(target, packet, aoe);
            }
        }
    }
    public void CastSkillshot(string spell, SimpleTs.DamageType damageType, HitChance hitChance = HitChance.VeryHigh, bool packet = true, bool aoe = false)
    {
        if (!Spells[spell].IsReady()) return;

        if (!Spells[spell].IsChargedSpell)
        {
            Obj_AI_Hero target = SimpleTs.GetTarget(Spells[spell].Range, damageType);
            if (target == null) return;

            if (target.IsValidTarget(Spells[spell].Range) && Spells[spell].GetPrediction(target).Hitchance >= hitChance)
                Spells[spell].Cast(target, packet, aoe);
        }
        else
        {
            Obj_AI_Hero target = SimpleTs.GetTarget(Spells[spell].ChargedMaxRange, damageType);
            if (target == null || !target.IsValidTarget(Spells[spell].ChargedMaxRange))
                return;

            if (!Spells[spell].IsCharging)
                Spells[spell].StartCharging();
            else
            {
                if (hitChance == HitChance.VeryHigh && Spells[spell].Range - Spells[spell].ChargedMaxRange * 0.2f > ObjectManager.Player.Distance(target) && Spells[spell].GetPrediction(target).Hitchance >= HitChance.High)
                    Spells[spell].Cast(target, packet, aoe);
                if (Spells[spell].GetPrediction(target).Hitchance >= hitChance)
                    Spells[spell].Cast(target, packet, aoe);
            }
        }
    }
    public void CastOnTarget(string spell, Obj_AI_Base target, bool packet = true)
    {
        if (!Spells[spell].IsReady()) return;

        if (target.IsValidTarget(Spells[spell].Range))
            Spells[spell].CastOnUnit(target, packet);
    }
    public void CastOnTarget(string spell, SimpleTs.DamageType damageType, bool packet = true)
    {
        if (!Spells[spell].IsReady()) return;

        Obj_AI_Hero target = SimpleTs.GetTarget(Spells[spell].Range, damageType);
        if (target == null) return;

        if (target.IsValidTarget(Spells[spell].Range))
            Spells[spell].CastOnUnit(target, packet);
    }
}
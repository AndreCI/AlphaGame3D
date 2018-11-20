using UnityEngine;
using System;

public class EffectFactory {
    private SpellUtils.EffectTypes type;
    private int duration;
    private int amplitude;
    public EffectFactory(SpellUtils.EffectTypes type_, int duration_, int amplitude_ = 0)
    {
        type = type_;
        duration = duration_;
        amplitude = amplitude_;
    }

    public UnitEffect GetEffect(Unit u_)
    {
        switch (type)
        {
            case SpellUtils.EffectTypes.FROST:
                return new FrostEffect(type, u_, duration);
            case SpellUtils.EffectTypes.BURN:
                return new BurnEffect(type, u_, duration);
            case SpellUtils.EffectTypes.ATTACK_MODIFIER:
                return new AttackModifierEffect(type, u_, duration, amplitude);
            case SpellUtils.EffectTypes.MVT_GAIN:
                return new MouvementGainEffect(type, u_, duration, amplitude);
            case SpellUtils.EffectTypes.ARMOR_GAIN:
                return new ArmorGainEffect(type, u_, duration, amplitude);
            case SpellUtils.EffectTypes.NO_FOOD_CONSO:
                return new NoFoodConsoEffect(type, u_, duration);
            case SpellUtils.EffectTypes.REGEN:
                return new RegenerationEffect(type, u_, duration, amplitude);
            default:
                return null;
        }
    }

    public string GetAbsoluteDescriptions()
    {
        switch (type)
        {
            case SpellUtils.EffectTypes.FROST:
                return SpellUtils.effectDescriptionAbsolute[type] + " for " + duration + " turns.";
            case SpellUtils.EffectTypes.BURN:
                return SpellUtils.effectDescriptionAbsolute[type] + " for " + duration + " turns.";
            case SpellUtils.EffectTypes.ATTACK_MODIFIER:
                return SpellUtils.effectDescriptionAbsolute[type] + " by " + amplitude + " for " + duration + " turns.";
            case SpellUtils.EffectTypes.MVT_GAIN:
                return SpellUtils.effectDescriptionAbsolute[type] + " by " + amplitude + " for " + duration + " turns.";
            case SpellUtils.EffectTypes.ARMOR_GAIN:
                return SpellUtils.effectDescriptionAbsolute[type] + " by " + amplitude + " for " + duration + " turns.";
            case SpellUtils.EffectTypes.NO_FOOD_CONSO:
                return SpellUtils.effectDescriptionAbsolute[type] + " by " + amplitude + " for " + duration + " turns.";
            case SpellUtils.EffectTypes.REGEN:
                return SpellUtils.effectDescriptionAbsolute[type] + "  (" + amplitude + " heal) for " + duration + " turns.";

            default:
                return null;
        }
    }
}
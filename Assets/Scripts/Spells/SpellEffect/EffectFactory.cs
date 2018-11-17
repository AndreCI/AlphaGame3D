using UnityEngine;
using System;

public class EffectFactory {
    private SpellUtils.UnitEffect type;
    private int duration;
    private int amplitude;
    public EffectFactory(SpellUtils.UnitEffect type_, int duration_, int amplitude_ = 0)
    {
        type = type_;
        duration = duration_;
        amplitude = amplitude_;
    }

    public UnitEffect GetEffect(Unit u_)
    {
        switch (type)
        {
            case SpellUtils.UnitEffect.FROST:
                return new FrostEffect(u_, duration);
            case SpellUtils.UnitEffect.BURN:
                return new BurnEffect(u_, duration);
            case SpellUtils.UnitEffect.ATTACK_MODIFIER:
                return new AttackModifierEffect(u_, duration, amplitude);
            default:
                return null;
        }
    }
}
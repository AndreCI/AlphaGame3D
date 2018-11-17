public class AttackModifierEffect : UnitEffect
{
    private int modifier;
    public override bool effectEnded
    {
        get
        {
            return duration <= -1;
        }
    }
    public AttackModifierEffect(SpellUtils.EffectTypes type_, Unit u_, int duration_, int modifier_) : base(type_, u_, duration_)
    {
        applyOnTouch = true;
        modifier = modifier_;
    }

    public override void ApplyEffect()
    {
        base.ApplyEffect();
        u.currentAttackModifier += modifier;
    }

    public override string GetDescriptionRelative()
    {
        return SpellUtils.effectDescriptionAbsolute[type] + " by " + modifier +" for " + duration + " turns.";
    }
}
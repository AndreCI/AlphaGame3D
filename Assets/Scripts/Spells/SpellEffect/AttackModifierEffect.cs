public class AttackModifierEffect : UnitEffect
{
    private int modifier;
    public AttackModifierEffect(Unit u_, int duration_, int modifier_) : base(u_, duration_)
    {
        applyOnTouch = true;
        modifier = modifier_;
    }

    public override void ApplyEffect()
    {
        base.ApplyEffect();
        u.currentAttackModifier += modifier;
    }
}
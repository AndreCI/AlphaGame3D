public class FrostEffect : UnitEffect
{
    public FrostEffect(SpellUtils.EffectTypes type_, Unit u_, int duration_) : base(type_, u_, duration_)
    {
        applyOnTouch = true;
    }

    public override void ApplyEffect()
    {
        base.ApplyEffect();
        u.currentMovementPoints -= 1;
    }

    public override string GetDescriptionRelative()
    {
        return SpellUtils.effectDescriptionAbsolute[type] + " for " + duration + " turns.";
    }
}
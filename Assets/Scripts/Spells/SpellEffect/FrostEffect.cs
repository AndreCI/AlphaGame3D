public class FrostEffect : UnitEffect
{
    public FrostEffect(SpellUtils.EffectTypes type_, Unit u_, int duration_) : base(type_, u_, duration_)
    {
        applyOnTouch = true;
    }

    public override System.Object[] ApplyEffect()
    {
        base.ApplyEffect();
        u.currentMovementPoints -= 1;
        System.Object[] e = { Utils.notificationTypes.FREEZE, "-1" };
        return e;
    }

    public override string GetDescriptionRelative()
    {
        return SpellUtils.effectDescriptionAbsolute[type] + " for " + duration + " turns.";
    }
}
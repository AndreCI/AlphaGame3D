public class FrostEffect : UnitEffect
{
    public FrostEffect(SpellUtils.EffectTypes type_, Unit u_, int duration_) : base(type_, u_, duration_)
    {
        applyOnTouch = true;
    }

    public override System.Object[] ApplyEffect()
    {
        base.ApplyEffect();
        u.currentMovementPoints -= u.currentMovementPoints > 2 ? 2 : u.currentMovementPoints;
        System.Object[] e = { Utils.NotificationTypes.DEBUFF_MVT, "-2" };
        return e;
    }

    public override string GetDescriptionRelative()
    {
        return SpellUtils.effectDescriptionAbsolute[type] + " (" + duration + " turns left).";
    }
}
public class BurnEffect : UnitEffect
{
    public BurnEffect(SpellUtils.EffectTypes type_, Unit u_, int duration_) : base(type_, u_, duration_)
    {
        applyOnTouch = false;
    }

    public override System.Object[] ApplyEffect()
    {
        base.ApplyEffect();
        u.TakesDamage(5);
        System.Object[] e = { Utils.notificationTypes.DAMAGE, "-5" };
        return e;
    }

    public override string GetDescriptionRelative()
    {
        return SpellUtils.effectDescriptionAbsolute[type] + " for " + duration + " turns.";
    }

}
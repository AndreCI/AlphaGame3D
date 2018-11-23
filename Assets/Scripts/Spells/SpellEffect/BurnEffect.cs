public class BurnEffect : UnitEffect
{
    public BurnEffect(SpellUtils.EffectTypes type_, Unit u_, int duration_) : base(type_, u_, duration_)
    {
        applyOnTouch = false;
    }

    public override System.Object[] ApplyEffect()
    {
        base.ApplyEffect();
        u.TakesDamage(5, unsafeDeath:true);
        System.Object[] e = { Utils.NotificationTypes.DAMAGE, "-5" };
        return e;
    }

    public override string GetDescriptionRelative()
    {
        return SpellUtils.effectDescriptionAbsolute[type] + " (" + duration + " turns left).";
    }

}
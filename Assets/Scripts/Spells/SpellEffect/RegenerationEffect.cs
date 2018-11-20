public class RegenerationEffect : UnitEffect
{
    public int modifier;
    public RegenerationEffect(SpellUtils.EffectTypes type_, Unit u_, int duration_, int modifier_) : base(type_, u_, duration_)
    {
        applyOnTouch = false;
        modifier = modifier_;
    }

    public override System.Object[] ApplyEffect()
    {
        base.ApplyEffect();
        u.Heal(modifier);
        System.Object[] e = { Utils.NotificationTypes.HEAL, "+"+modifier.ToString() };
        return e;
    }

    public override string GetDescriptionRelative()
    {
        return SpellUtils.effectDescriptionAbsolute[type] + " (" + modifier+" heal) ("+ duration + " turns left).";
    }

}
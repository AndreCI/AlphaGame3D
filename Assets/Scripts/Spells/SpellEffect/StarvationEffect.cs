public class StarvationEffect : UnitEffect
{
    public override bool effectEnded
    {
        get
        {
            return duration <= -1;
        }
    }
    public StarvationEffect(SpellUtils.EffectTypes type_, Unit u_, int duration_)  :base(type_, u_, duration_)
    {

    }

    public override System.Object[] ApplyEffect()
    {
        base.ApplyEffect();
        u.TakesDamage(10);
        System.Object[] e = { Utils.NotificationTypes.DAMAGE, "-10" };
        return e;
    }

    public override string GetDescriptionRelative()
    {
        return SpellUtils.effectDescriptionAbsolute[type];
    }

    public override void End()
    {
        
    }

}
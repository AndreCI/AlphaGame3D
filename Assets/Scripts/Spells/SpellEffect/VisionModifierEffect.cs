public class VisionModifierEffect : UnitEffect
{
    private int modifier;
    public override bool effectEnded
    {
        get
        {
            return duration <= -1;
        }
    }
    public VisionModifierEffect(SpellUtils.EffectTypes type_, Unit u_, int duration_, int modifier_) : base(type_, u_, duration_)
    {
        applyOnTouch = true;
        modifier = modifier_;
    }

    public override System.Object[] ApplyEffect()
    {
        base.ApplyEffect();
        u.currentVisionRangeModifier += modifier;
        string notif = "";
        System.Object[] e = {null, null};
        if (modifier > 0)
        {
            notif += "+";
            e[0] = Utils.NotificationTypes.BUFF_ATCK;
        }
        else
        {
            notif += "-";
            e[0] = Utils.NotificationTypes.DEBUFF_ATCK;
        }
        notif += modifier.ToString();
        e[1] = notif;
        u.owner.UpdateVisibleNodes();
        return e;
    }

    public override string GetDescriptionRelative()
    {
        return SpellUtils.effectDescriptionAbsolute[type] + " by " + modifier +" (" + duration + " turns left).";
    }
}
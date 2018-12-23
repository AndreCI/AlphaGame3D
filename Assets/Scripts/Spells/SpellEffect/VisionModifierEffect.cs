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
        HexGrid.Instance.DecreaseVisibility(u.currentPosition, u.visionRange + u.currentVisionRangeModifier, u.owner);
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
        //TODO: debug vision buffing
        HexGrid.Instance.IncreaseVisibility(u.currentPosition, u.visionRange + u.currentVisionRangeModifier, u.owner);
        return e;
    }

    public override string GetDescriptionRelative()
    {
        return SpellUtils.effectDescriptionAbsolute[type] + " by " + modifier +" (" + duration + " turns left).";
    }
}
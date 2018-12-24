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
        string notif = "";
        System.Object[] e = {null, null};
        if (modifier > 0)
        {
            notif += "+";
            e[0] = Utils.NotificationTypes.BUFF_ATCK;
            HexGrid.Instance.IncreaseVisibilityFromRadius(u.currentPosition, u.visionRange + u.currentVisionRangeModifier, 
                u.visionRange + u.currentVisionRangeModifier + modifier, u.owner);

        }
        else
        {
            notif += "-";
            e[0] = Utils.NotificationTypes.DEBUFF_ATCK;
            HexGrid.Instance.DecreaseVisibilityFromRadius(u.currentPosition, u.visionRange + u.currentVisionRangeModifier + modifier,
                u.visionRange + u.currentVisionRangeModifier, u.owner);

        }
        u.currentVisionRangeModifier += modifier;
        notif += modifier.ToString();
        e[1] = notif;
        //TODO: debug vision buffing
        return e;
    }

    public override string GetDescriptionRelative()
    {
        return SpellUtils.effectDescriptionAbsolute[type] + " by " + modifier +" (" + duration + " turns left).";
    }
}
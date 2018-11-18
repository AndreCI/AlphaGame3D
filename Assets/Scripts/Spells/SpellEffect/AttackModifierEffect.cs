public class AttackModifierEffect : UnitEffect
{
    private int modifier;
    public override bool effectEnded
    {
        get
        {
            return duration <= -1;
        }
    }
    public AttackModifierEffect(SpellUtils.EffectTypes type_, Unit u_, int duration_, int modifier_) : base(type_, u_, duration_)
    {
        applyOnTouch = true;
        modifier = modifier_;
    }

    public override System.Object[] ApplyEffect()
    {
        base.ApplyEffect();
        u.currentAttackModifier += modifier;
        string notif = "";
        System.Object[] e = {null, null};
        if (modifier > 0)
        {
            notif += "+";
            e[0] = Utils.notificationTypes.BUFF_ATCK;
        }
        else
        {
            notif += "-";
            e[0] = Utils.notificationTypes.DEBUFF_ATCK;
        }
        notif += modifier.ToString();
        e[1] = notif;
        return e;
    }

    public override string GetDescriptionRelative()
    {
        return SpellUtils.effectDescriptionAbsolute[type] + " by " + modifier +" for " + duration + " turns.";
    }
}
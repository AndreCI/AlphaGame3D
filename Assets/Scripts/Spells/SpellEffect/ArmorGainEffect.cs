public class ArmorGainEffect : UnitEffect
{
    private int modifier;
    public override bool effectEnded
    {
        get
        {
            return duration <= -1;
        }
    }
    public ArmorGainEffect(SpellUtils.EffectTypes type_, Unit u_, int duration_, int modifier_) : base(type_, u_, duration_)
    {
        applyOnTouch = true;
        modifier = modifier_;
    }

    public override System.Object[] ApplyEffect()
    {
        base.ApplyEffect();
        u.armor += modifier;
        string notif = "";
        System.Object[] e = { null, null };
        if (modifier > 0)
        {
            notif += "+";
            e[0] = Utils.NotificationTypes.BUFF_ARMOR;
        }
        else
        {
            notif += "-";
            throw new System.NotImplementedException("Armor loss has not been implemented");
            e[0] = Utils.NotificationTypes.DEBUFF_MVT;
        }
        notif += modifier.ToString();
        e[1] = notif;
        return e;
    }

    public override string GetDescriptionRelative()
    {
        return SpellUtils.effectDescriptionAbsolute[type] + " by " + modifier + " (" + duration + " turns left)";
    }
}
using System.Collections.Generic;

public abstract class UnitEffect
{
    protected SpellUtils.EffectTypes type;
    protected Unit u;
    public bool applyOnTouch;
    public int duration;
    public virtual bool effectEnded
    {
        get
        {
            return duration <= 0;
        }
    }
    

    public UnitEffect(SpellUtils.EffectTypes type_, Unit u_, int duration_)
    {
        type = type_;
        u = u_;
        duration = duration_;
    }

    public virtual void ApplyEffect()
    {
        duration -= 1;
    }

    public void End()
    {
        duration = 0;
    }

    public abstract string GetDescriptionRelative();
}
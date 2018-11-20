using System.Collections.Generic;

public abstract class UnitEffect
{
    public SpellUtils.EffectTypes type;
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
        u.effectAnimations.StartAnimation(type);
    }

    public virtual System.Object[] ApplyEffect()
    {
        duration -= 1;
        if (effectEnded)
        {
            End();
        }
        return null;
    }

    public virtual void End()
    {
        int counter = 0;
        foreach (UnitEffect ue in u.currentEffect)
        {
            if (ue.type == type)
            {
                counter++;
            }
        }
        if (counter <= 1) //it is still in the list
        {
            u.effectAnimations.StopAnimation(type);
        }
    }
    

    public abstract string GetDescriptionRelative();
}
using UnityEngine;
using UnityEditor;

public abstract class UnitEffect
{
    protected Unit u;
    protected int duration;

    public UnitEffect(Unit u_, int duration_)
    {
        u = u_;
        duration = duration_;
    }

    public virtual void ApplyEffect()
    {
        duration -= 1;
        if (duration <= 0)
        {
            End();
            return;
        }
    }

    public void End()
    {
        duration = 0;
        u.currentEffect.Remove(this);
    }
}
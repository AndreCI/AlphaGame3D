public abstract class UnitEffect
{
    protected Unit u;
    public int duration;

    public UnitEffect(Unit u_, int duration_)
    {
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
        u.currentEffect.Remove(this);
    }
}
public class BurnEffect : UnitEffect
{
    public BurnEffect(Unit u_, int duration_) : base(u_, duration_)
    {
    }

    public override void ApplyEffect()
    {
        base.ApplyEffect();
        u.TakesDamage(5);
    }
}
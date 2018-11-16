public class FrostEffect : UnitEffect
{
    public FrostEffect(Unit u_, int duration_) : base(u_, duration_)
    {
    }

    public override void ApplyEffect()
    {
        base.ApplyEffect();
        u.currentMovementPoints -= 1;
    }
}
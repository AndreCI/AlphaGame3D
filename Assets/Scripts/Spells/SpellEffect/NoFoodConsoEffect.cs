public class NoFoodConsoEffect : UnitEffect
{
    public NoFoodConsoEffect(SpellUtils.EffectTypes type_, Unit u_, int duration_) : base(type_, u_, duration_)
    {
        applyOnTouch = false;
    }

    public override System.Object[] ApplyEffect()
    {
        base.ApplyEffect();
        return null;
    }

    public override string GetDescriptionRelative()
    {
        return SpellUtils.effectDescriptionAbsolute[type] + " (" + duration + " turns left).";
    }

}
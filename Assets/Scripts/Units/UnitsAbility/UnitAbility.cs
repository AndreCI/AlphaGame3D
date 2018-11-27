using UnityEngine;

[System.Serializable]
public abstract class UnitAbility {
    public Unit abilityOwner;
    public UnitAbilityUtils.TYPES type;
}
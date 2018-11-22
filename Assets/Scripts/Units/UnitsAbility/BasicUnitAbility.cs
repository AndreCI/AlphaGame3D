using UnityEngine;
using UnityEditor;


[System.Serializable]
public class BasicUnitAbility : UnitAbility
{
    
    public UnitAbilityUtils.TYPES type;
    public int amplitude;
    public TurnSubject.NOTIFICATION_TYPE trigger;

    public override void Notify(Player player, TurnSubject.NOTIFICATION_TYPE subjectType)
    {
        if(abilityOwner.owner.Equals(player) && subjectType == trigger)
        {
            Trigger();
        }
    }

    public override void Trigger()
    {
        switch (type)
        {
            case UnitAbilityUtils.TYPES.GOLD_MODIFIER:
                Debug.Log("gold added");
                abilityOwner.owner.AddGold(amplitude);
                break;
            case UnitAbilityUtils.TYPES.HEAL:
                abilityOwner.Heal(amplitude);
                break;
        }
    }
}
using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public class TriggeredUnitAbility : UnitAbility, IObserver
{
    
    public int amplitude;
    public TurnSubject.NOTIFICATION_TYPE trigger;

    [Header("Apply Status data, not necessary")]
    public int duration;
    public SpellUtils.EffectTypes status;
    private EffectFactory effectFactory;


    public void Notify(Player player, TurnSubject.NOTIFICATION_TYPE subjectType)
    {
        if(effectFactory == null && type == UnitAbilityUtils.TYPES.APPLY_EFFECT)
        {
            effectFactory = new EffectFactory(status, duration, amplitude); 
        }
        if(abilityOwner.owner.Equals(player) && subjectType == trigger)
        {
            Trigger();
        }
    }

    public void Trigger()
    {
        switch (type)
        {
            case UnitAbilityUtils.TYPES.GOLD_MODIFIER:
                abilityOwner.DisplayNotifications(new Dictionary<Utils.NotificationTypes, int> {
                    { Utils.NotificationTypes.GOLD, amplitude} });
                abilityOwner.owner.AddGold(amplitude);
                break;
            case UnitAbilityUtils.TYPES.HEAL:
                abilityOwner.DisplayNotifications(new Dictionary<Utils.NotificationTypes, int> {
                    { Utils.NotificationTypes.HEAL, amplitude} });
                abilityOwner.Heal(amplitude);
                break;
            case UnitAbilityUtils.TYPES.ARMOR:
                abilityOwner.DisplayNotifications(new Dictionary<Utils.NotificationTypes, int> {
                    { Utils.NotificationTypes.BUFF_ARMOR, amplitude} });
                abilityOwner.armor+=(amplitude);
                break;
            case UnitAbilityUtils.TYPES.APPLY_EFFECT:
                UnitEffect ue = effectFactory.GetEffect(abilityOwner);
                if (ue.applyOnTouch)
                {
                    ue.ApplyEffect();
                }
                if (ue.duration > 0)
                {
                    abilityOwner.currentEffect.Add(ue);
                }
                Debug.Log("vision=" + abilityOwner.GetVisionRange());
                Debug.Log(ue.duration);
                break;
        }
    }
}
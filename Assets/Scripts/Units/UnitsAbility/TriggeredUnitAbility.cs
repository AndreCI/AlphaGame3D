using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class TriggeredUnitAbility : UnitAbility, IObserver
{
    
    public int amplitude;
    public TurnSubject.NOTIFICATION_TYPE trigger;

    [Header("Apply Status data, not necessary")]
    public int duration;
    public SpellUtils.EffectTypes status;
    private EffectFactory effectFactory;

    //Alternative parameters, to be set inside unit
    [HideInInspector]
    public Unit unitCreationPrefab;
    [HideInInspector]
    public HexCell cellTarget;
    [HideInInspector]
    public bool applyEffectOnCellTarget = false;


    public void Notify(Player player, TurnSubject.NOTIFICATION_TYPE subjectType)
    {
        if(type == UnitAbilityUtils.TYPES.APPLY_EFFECT && effectFactory == null)
        {
            effectFactory = new EffectFactory(status, duration, amplitude); 
        }else if(effectFactory!=null && effectFactory.amplitude != amplitude)
        {
            effectFactory.amplitude = amplitude;
        }
        if (abilityOwner.owner.Equals(player) && subjectType == trigger)
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
                if (applyEffectOnCellTarget)
                {
                    UnitEffect ue = effectFactory.GetEffect(cellTarget.unit);
                    if (ue.applyOnTouch)
                    {
                        ue.ApplyEffect();
                    }
                    cellTarget.unit.currentEffect.Add(ue);
                }
                else
                {
                    UnitEffect ue = effectFactory.GetEffect(abilityOwner);
                    if (ue.applyOnTouch)
                    {
                        ue.ApplyEffect();
                    }
                    abilityOwner.currentEffect.Add(ue);
                }
               /* if (ue.duration > 0)
                {
                    abilityOwner.currentEffect.Add(ue);
                }*/
                break;
            case UnitAbilityUtils.TYPES.CREATE_UNIT:
                abilityOwner.StartCoroutine(CreateUnitWithDelay());
                break;
        }
    }

    private IEnumerator CreateUnitWithDelay(float delay = 1.2f)
    {
        yield return new WaitForSeconds(delay);
        ConstructionManager.Instance.SetUnitToBuild(unitCreationPrefab, owner:abilityOwner.owner);
        cellTarget.Construct(true, owner:abilityOwner.owner);
    }
}
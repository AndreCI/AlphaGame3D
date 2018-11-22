using UnityEngine;

[System.Serializable]
public abstract class UnitAbility : IObserver {

    public Unit abilityOwner;

    public abstract void Notify(Player player, TurnSubject.NOTIFICATION_TYPE subjectType);
    public abstract void Trigger();
}
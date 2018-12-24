using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : Unit
{
    [Header("Wizard specific data")]
    public Unit unitCreatedPrefab;
    public TurnSubject OnTargetDeathTurnSubject;
    public TriggeredUnitAbility ability;
    // Use this for initialization
    public override void Setup()
    {

        base.Setup();
        OnTargetDeathTurnSubject = new TurnSubject(TurnSubject.NOTIFICATION_TYPE.TARGET_DEATH);
        OnTargetDeathTurnSubject.AddObserver(ability);
        ability.unitCreationPrefab = unitCreatedPrefab;
    }


    protected override IEnumerator Attack(HexCell target, bool riposte)
    {
        ability.cellTarget = target;
        Unit attacked = target.unit;
        yield return StartCoroutine(base.Attack(target, riposte));

        if (!TurnManager.Instance.inactivePlayer.currentUnits.Contains(attacked))
        {
            OnTargetDeathTurnSubject.NotifyObservers(owner);
        }
    }
    public override Type GetSpawnPoint()
    {
        return typeof(Barracks);
    }
}

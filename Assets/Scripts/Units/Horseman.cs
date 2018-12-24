using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Horseman : Unit
{

    public TurnSubject OnMouvementTurnSubject;
    public TriggeredUnitAbility ability;

    // Use this for initialization
    public override void Setup()
    {

        base.Setup();
        OnMouvementTurnSubject = new TurnSubject(TurnSubject.NOTIFICATION_TYPE.MOVING);
        OnMouvementTurnSubject.AddObserver(ability);
    }

    protected override IEnumerator MoveStep(HexCell previousCell, HexCell nextCell)
    {
        OnMouvementTurnSubject.NotifyObservers(owner);
        return base.MoveStep(previousCell, nextCell);
    }

    public override Type GetSpawnPoint()
    {
        return typeof(Stables);
    }
}

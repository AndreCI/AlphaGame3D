using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : FlyingUnit
{

    public TurnSubject OnAttackTurnSubject;
    public TriggeredUnitAbility ability;

    // Use this for initialization
    public override void Setup()
    {

        base.Setup();
        OnAttackTurnSubject = new TurnSubject(TurnSubject.NOTIFICATION_TYPE.ATTACKING);
        OnAttackTurnSubject.AddObserver(ability);
        ability.applyEffectOnCellTarget = true;
    }


    protected override IEnumerator Attack(HexCell target, bool riposte)
    {
        ability.cellTarget = target;
        OnAttackTurnSubject.NotifyObservers(owner);
        yield return StartCoroutine(base.Attack(target, riposte));
        
    }
    public override Type GetSpawnPoint()
    {
        return typeof(Stables);
    }
}

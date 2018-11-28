using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scout : Unit
{


    public TurnSubject NoMouvementMadeSubject;
    public TriggeredUnitAbility ability;

    // Use this for initialization
    public override void Setup()
    {

        base.Setup();
        TurnManager.Instance.EndTurnSubject.AddObserver(this);
        NoMouvementMadeSubject = new TurnSubject(TurnSubject.NOTIFICATION_TYPE.NO_MOUVEMENT);
        NoMouvementMadeSubject.AddObserver(ability);
    }

    public override void Notify(Player player, TurnSubject.NOTIFICATION_TYPE type)
    {
        Debug.Log("Notified with:" + type.ToString() + " player:" + player.ToString());
        base.Notify(player, type);
        if(type==TurnSubject.NOTIFICATION_TYPE.END_OF_TURN && player.Equals(owner))
        {
            Debug.Log("RIHGT!");
            if(currentMovementPoints == maxMovementPoints)
            {
                Debug.Log("Notifiy abi");
                NoMouvementMadeSubject.NotifyObservers(player);
                Debug.Log(currentEffect.Count);
            }
        }
    }
    public override Type GetSpawnPoint()
    {
        return typeof(HallCenter);
    }
}

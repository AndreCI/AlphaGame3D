using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scout : Unit
{


    public TurnSubject NoMouvementMadeSubject;
    public TriggeredUnitAbility ability;
    private int defaultAmplitude;
    private int lastTurnMvtPoints;

    // Use this for initialization
    public override void Setup()
    {

        base.Setup();
       // TurnManager.Instance.StartTurnSubject.AddObserver(this);
        NoMouvementMadeSubject = new TurnSubject(TurnSubject.NOTIFICATION_TYPE.NO_MOUVEMENT);
        NoMouvementMadeSubject.AddObserver(ability);
        defaultAmplitude = ability.amplitude;
    }

    public override void Notify(Player player, TurnSubject.NOTIFICATION_TYPE type)
    {
        lastTurnMvtPoints = currentMovementPoints;   
        base.Notify(player, type);
        if (type==TurnSubject.NOTIFICATION_TYPE.START_OF_TURN && player.Equals(owner))
        {
            if(lastTurnMvtPoints == maxMovementPoints)
            {
                Debug.Log("One added");
                NoMouvementMadeSubject.NotifyObservers(player);
                ability.amplitude += 1;
            }
            else
            {
                ability.amplitude = defaultAmplitude;
              
            }
        }
    }
    public override Type GetSpawnPoint()
    {
        return typeof(HallCenter);
    }
}

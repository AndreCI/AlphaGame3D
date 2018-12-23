using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scout : Unit
{


    public TurnSubject NoMouvementMadeSubject;
    public TriggeredUnitAbility ability;
    private int defaultAmplitude;

    // Use this for initialization
    public override void Setup()
    {

        base.Setup();
        TurnManager.Instance.EndTurnSubject.AddObserver(this);
        NoMouvementMadeSubject = new TurnSubject(TurnSubject.NOTIFICATION_TYPE.NO_MOUVEMENT);
        NoMouvementMadeSubject.AddObserver(ability);
        defaultAmplitude = ability.amplitude;
    }

    public override void Notify(Player player, TurnSubject.NOTIFICATION_TYPE type)
    {
        base.Notify(player, type);
        if(type==TurnSubject.NOTIFICATION_TYPE.END_OF_TURN && player.Equals(owner))
        {
            if(currentMovementPoints == maxMovementPoints)
            {
                Debug.Log("One added");
                NoMouvementMadeSubject.NotifyObservers(player);
                ability.amplitude += 1;
            }
            else
            {
                ability.amplitude = defaultAmplitude;
                HexGrid.Instance.DecreaseVisibility(currentPosition, visionRange + currentVisionRangeModifier, owner);
                currentVisionRangeModifier = 0;
                HexGrid.Instance.IncreaseVisibility(currentPosition, visionRange + currentVisionRangeModifier, owner);
            }
        }
    }
    public override Type GetSpawnPoint()
    {
        return typeof(HallCenter);
    }
}

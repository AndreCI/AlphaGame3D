using System;
using System.Collections.Generic;
using UnityEngine;

public class CropFreeze : Spell
{
    public static CropFreeze Instance;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            base.AwakeBase();
        }
        else
        {
            throw new System.NotImplementedException();
        }
    }

    public override void PlayAnimation()
    {
        transform.position = playerInfos[TurnManager.Instance.currentPlayer].position.transform.position;
        ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem p in particles)
        {
            p.Play();
        }
    }
    public override void Activate(List<Node> affectedNodes_)
    {
        int waste = TurnManager.Instance.currentPlayer.food > damage ? damage : TurnManager.Instance.currentPlayer.food;
        TurnManager.Instance.currentPlayer.food -= waste;
        //TurnManager.Instance.currentPlayer.foodPrediction -= waste;
        TurnManager.Instance.currentPlayer.manaBank += waste;

        TurnManager.Instance.inactivePlayer.foodPrediction -= waste;
        TurnManager.Instance.inactivePlayer.manaBank += waste;

        base.Activate(affectedNodes_);
    }

}
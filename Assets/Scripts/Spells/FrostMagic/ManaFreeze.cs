using System;
using System.Collections.Generic;
using UnityEngine;

public class ManaFreeze : Spell
{
    public static ManaFreeze Instance;

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
        int save = TurnManager.Instance.currentPlayer.mana;
        TurnManager.Instance.currentPlayer.mana = 0;
        TurnManager.Instance.currentPlayer.manaBank += save;
        TurnManager.Instance.currentPlayer.actionPoints += 1;
        base.Activate(affectedNodes_);
    }

}
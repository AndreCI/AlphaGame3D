using System;
using System.Collections.Generic;
using UnityEngine;

public class Naturewrath : Spell
{
    public static Naturewrath Instance;

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
        GetComponentInChildren<Animation>().Play();
    }
    public override void Activate(List<Node> affectedNodes_)
    {
        foreach (Node node in affectedNodes_)
        {
            node.Damage(damage);
        }
        TurnManager.Instance.currentPlayer.actionPoints += 1;
        base.Activate(affectedNodes_);
    }
}
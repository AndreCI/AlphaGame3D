using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

public class ArcaneIntellect : Spell
{
    public static ArcaneIntellect Instance;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            unlock = new List<Type>();
            base.AwakeBase();
        }
        else
        {
            throw new System.NotImplementedException();
        }
    }
    public override List<Type> GetRequierements()
    {
        return new List<Type> { typeof(MagicCenter) };
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
        TurnManager.Instance.currentPlayer.actionPoints += 2;
        base.Activate(affectedNodes_);
    }
}
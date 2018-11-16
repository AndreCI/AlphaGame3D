﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class Frostblast : Spell
{
    public static Frostblast Instance;
    public int frostDuration;

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
        GetComponentInChildren<Animation>().Play();
    }
    public override void Activate(List<Node> affectedNodes_)
    {
        foreach(Node node in affectedNodes_)
        {
            node.Damage(damage);
            if (node.unit != null) {
                node.unit.currentEffect.Add(new FrostEffect(node.unit, frostDuration));
            }
        }
        base.Activate(affectedNodes_);
    }
}
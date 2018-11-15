﻿using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

public class Fireblast : Spell
{
    public static Fireblast Instance;

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
            if (node == currentPosition)
            {
                node.Damage(damage);
            }
            node.Damage(damage);
        }
        base.Activate(affectedNodes_);
    }
}
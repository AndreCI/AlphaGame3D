﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class ArcaneIntellect : Spell
{
    public static ArcaneIntellect Instance;

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
    public override void Activate(List<HexCell> affectedNodes_)
    {
        TurnManager.Instance.currentPlayer.actionPoints += 2;
        base.Activate(affectedNodes_);
    }
}
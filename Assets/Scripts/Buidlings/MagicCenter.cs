﻿using UnityEngine;
using System.Collections.Generic;
using System;

public class MagicCenter : Building
{

    // Use this for initialization
    void Start()
    {
        TurnManager.Instance.StartTurnSubject.AddObserver(this);
        unlock = new List<Type> { typeof(Fireblast), typeof(Frostblast), typeof(Frostlance), typeof(ArcaneIntellect),typeof(Fireblast), typeof(Shrine) };
    }
    public override List<Type> GetRequierements()
    {
        return new List<Type> { typeof(HallCenter) };
    }



    public override void Notify(Player player)
    {
        base.Notify(player);
        if (player.Equals(owner) && constructionTime <= 0)
        {
            player.mana += 4;
            if (tier2)
            {
                player.mana += 4;
                //unlock.Add();
            }
        }
        
    }
}

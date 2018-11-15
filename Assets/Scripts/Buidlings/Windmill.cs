using UnityEngine.UI;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;

public class Windmill : Building
{
    void Start()
    {
        TurnManager.Instance.StartTurnSubject.AddObserver(this);
        unlock = new List<Type>();
    }

    public override List<Type> GetRequierements()
    {
        return new List<Type> { typeof(HallCenter) };
    }

    void Update()
    {
        
    }

    public override void Notify(Player player)
    {
        base.Notify(player);
        if (player.Equals(owner) && constructionTime <= 0)
        {
            owner.foodPrediction += 3;
            owner.AddGold(3);
            if (tier2)
            {
                owner.AddGold(3);
                owner.foodPrediction += 3;
            }
        }
    }

}
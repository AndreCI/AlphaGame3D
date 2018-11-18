using System;
using System.Collections.Generic;
using UnityEngine;

public class HallCenter : DefensiveBuilding {

	// Use this for initialization
	void Start () {
        TurnManager.Instance.StartTurnSubject.AddObserver(this);
    }

    public override void Death()
    {
        base.Death();
        Debug.Log("Player " + owner.id + " lose!");
    }

    public override void Notify(Player player)
    {
        base.Notify(player);
        if (player.Equals(owner) && constructionTime <= 0)
        {
            owner.AddGold(8);
            //owner.foodPrediction += 4; //Done dirty somewhere else (see Utils.EatFood)
            owner.actionPoints += 1;
            if (isTier2)
            {
                owner.actionPoints += 1;
            }
        }
        
    }
    public override void UpgradeToT2()
    {
        maxHealth += maxHealthT2Upgrade;
        currentHealth += maxHealthT2Upgrade;
        base.UpgradeToT2();
    }
}

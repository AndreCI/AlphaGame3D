using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HallCenter : DefensiveBuilding {
    //public new List<Type> unlock;

	// Use this for initialization
	void Start () {
        TurnManager.Instance.StartTurnSubject.AddObserver(this);
        //owner = Player.getPlayerFromId(playerId);
        unlock = new List<Type>() { typeof(Barracks), typeof(MagicCenter), typeof(Windmill) };
    }
    public override List<Type> GetRequierements()
    {
        return new List<Type>();
    }

    // Update is called once per frame
    void Update () {
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
            if (tier2)
            {
                owner.actionPoints += 1;
            }
        }
        
    }
    public override void UpgradeToT2()
    {
        maxHealth += 10;
        currentHealth += 10;
        base.UpgradeToT2();
    }
}

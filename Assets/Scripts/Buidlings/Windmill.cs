using System;
using System.Collections.Generic;

public class Windmill : Building
{
    void Start()
    {
        TurnManager.Instance.StartTurnSubject.AddObserver(this);
    }
    

    public override void Notify(Player player)
    {
        base.Notify(player);
        if (player.Equals(owner) && constructionTime <= 0)
        {
            owner.foodPrediction += 3;
            owner.AddGold(3);
            if (isTier2)
            {
                owner.AddGold(3);
                owner.foodPrediction += 3;
            }
        }
    }

}
using System;
using System.Collections.Generic;

public class MagicCenter : Building
{

    // Use this for initialization
    void Start()
    {
        TurnManager.Instance.StartTurnSubject.AddObserver(this);
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

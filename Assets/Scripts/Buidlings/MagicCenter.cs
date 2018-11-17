using System;
using System.Collections.Generic;

public class MagicCenter : Building
{

    // Use this for initialization
    void Start()
    {
        TurnManager.Instance.StartTurnSubject.AddObserver(this);
        unlock = new List<Type> { typeof(Fireblast),
            typeof(Frostblast), typeof(Frostlance),
            typeof(ArcaneIntellect), typeof(Arcaneblast), typeof(Fireblast),
            typeof(FireHammer), typeof(FlammingSwords),
            typeof(Shrine) };
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

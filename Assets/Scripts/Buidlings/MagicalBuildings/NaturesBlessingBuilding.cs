using UnityEngine;
using System.Collections.Generic;

public class NaturesBlessingBuilding : DefensiveBuilding
{
    private List<HexCell> affectedNodes;
    public EffectFactory effect;

    private void Start()
    {
        TurnManager.Instance.StartTurnSubject.AddObserver(this, 4); 
        startOfTurnNotificationData = new Dictionary<Utils.NotificationTypes, int> {
            {Utils.NotificationTypes.FOOD, 1 }
        };
       // affectedNodes = currentPosition2.adjacentNodes;
    }

    public override void Death()
    {
        TurnManager.Instance.StartTurnSubject.RemoveObserver(this);
        base.Death();
    }

    public override void Notify(Player player, TurnSubject.NOTIFICATION_TYPE type)
    {
      /*  if (player.Equals(owner) &&type == TurnSubject.NOTIFICATION_TYPE.START_OF_TURN)
        {
            foreach (Node n in affectedNodes)
            {
                if (n.unit != null)
                {
                    UnitEffect ue = effect.GetEffect(n.unit);
                    if (ue.applyOnTouch)
                    {
                        ue.ApplyEffect();
                    }
                    if (ue.duration > 0)
                    {
                        n.unit.currentEffect.Add(ue); //Adding effect during opening phase should be done in 
                        //a better way.
                    }
                }
            }TakeDamage(1);
        }
        base.Notify(player, type);*/
    }
}

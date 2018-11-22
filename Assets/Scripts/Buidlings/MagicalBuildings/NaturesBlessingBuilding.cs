using UnityEngine;
using System.Collections.Generic;

public class NaturesBlessingBuilding : DefensiveBuilding
{
    private NodeUtils.NodeWrapper adjacentNodes;
    public EffectFactory effect;

    private void Start()
    {
        TurnManager.Instance.StartTurnSubject.AddObserver(this, 6);
        startOfTurnNotificationData = new Dictionary<Utils.NotificationTypes, int> {
            {Utils.NotificationTypes.FOOD, 1 }
        };
        adjacentNodes = NodeUtils.GetNeighborsNode(currentPosition, 1);
    }

    public override void Death()
    {
        TurnManager.Instance.StartTurnSubject.RemoveObserver(this);
        base.Death();
    }

    public override void Notify(Player player, TurnSubject.NOTIFICATION_TYPE type)
    {
        if (player.Equals(owner) &&type == TurnSubject.NOTIFICATION_TYPE.START_OF_TURN)
        {
            foreach (NodeUtils.NodeWrapper n in adjacentNodes.GetNodeChildren())
            {
                if (n.root.unit != null)
                {
                    UnitEffect ue = effect.GetEffect(n.root.unit);
                    if (ue.applyOnTouch)
                    {
                        ue.ApplyEffect();
                    }
                    if (ue.duration > 0)
                    {
                        n.root.unit.currentEffect.Add(ue);
                    }
                }
            }TakeDamage(1);
        }
        base.Notify(player, type);
    }
}

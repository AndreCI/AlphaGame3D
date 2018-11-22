using UnityEngine;
using System.Collections.Generic;

public class ArcaneMirageBuilding : DefensiveBuilding
{
    public int damageOverTime;
    private void Start()
    {
        TurnManager.Instance.StartTurnSubject.AddObserver(this, 6);
        startOfTurnNotificationData = new Dictionary<Utils.NotificationTypes, int>();
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
            TakeDamage(damageOverTime);
        }
        base.Notify(player, type);
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

public class HallCenter : DefensiveBuilding {

	// Use this for initialization
	void Start () {
        TurnManager.Instance.StartTurnSubject.AddObserver(this);
        startOfTurnNotificationData = new Dictionary<Utils.NotificationTypes, int> {
            {Utils.NotificationTypes.GOLD, 8 },
            {Utils.NotificationTypes.FOOD, 4 },
            {Utils.NotificationTypes.ACTION_POINT, 2 }
        };
    }

    public override void Death()
    {
        base.Death();
        Debug.Log("Player " + owner.id + " lose!");
    }

    public override void UpgradeToT2()
    {
        maxHealth += maxHealthT2Upgrade;
        currentHealth += maxHealthT2Upgrade;
        startOfTurnNotificationData[Utils.NotificationTypes.ACTION_POINT] += 1;
        base.UpgradeToT2();
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

public class HallCenter : DefensiveBuilding {

	// Use this for initialization
	void Start () {
        TurnManager.Instance.StartTurnSubject.AddObserver(this);
        notificationsData = new Dictionary<Utils.notificationTypes, int> {
            {Utils.notificationTypes.GOLD, 8 },
            {Utils.notificationTypes.FOOD, 4 },
            {Utils.notificationTypes.ACTION_POINT, 1 }
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
        notificationsData[Utils.notificationTypes.ACTION_POINT] += 1;
        base.UpgradeToT2();
    }
}

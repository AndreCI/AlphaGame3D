using System;
using System.Collections.Generic;

public class Windmill : Building
{
    void Start()
    {
        TurnManager.Instance.StartTurnSubject.AddObserver(this);
        startOfTurnNotificationData = new Dictionary<Utils.NotificationTypes, int> {
            {Utils.NotificationTypes.GOLD, 3 },
            {Utils.NotificationTypes.FOOD, 3 }
        };
    }


    public override void UpgradeToT2()
    {
        base.UpgradeToT2();
        startOfTurnNotificationData[Utils.NotificationTypes.GOLD] += 3;
        startOfTurnNotificationData[Utils.NotificationTypes.FOOD] += 3;
    }

}
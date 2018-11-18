using System;
using System.Collections.Generic;

public class Windmill : Building
{
    void Start()
    {
        TurnManager.Instance.StartTurnSubject.AddObserver(this);
        startOfTurnNotificationData = new Dictionary<Utils.notificationTypes, int> {
            {Utils.notificationTypes.GOLD, 3 },
            {Utils.notificationTypes.FOOD, 3 }
        };
    }


    public override void UpgradeToT2()
    {
        base.UpgradeToT2();
        startOfTurnNotificationData[Utils.notificationTypes.GOLD] += 3;
        startOfTurnNotificationData[Utils.notificationTypes.FOOD] += 3;
    }

}
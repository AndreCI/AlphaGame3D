﻿using System;
using System.Collections.Generic;

public class Windmill : Building
{
    void Start()
    {
        TurnManager.Instance.StartTurnSubject.AddObserver(this);
        startOfTurnNotificationData = new Dictionary<Utils.NotificationTypes, int> {
            {Utils.NotificationTypes.FOOD, 2 }
        };
    }


    public override void UpgradeToT2()
    {
        base.UpgradeToT2();
        startOfTurnNotificationData[Utils.NotificationTypes.FOOD] += 3;
    }

}
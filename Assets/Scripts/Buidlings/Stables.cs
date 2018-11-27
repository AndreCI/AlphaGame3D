using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System;

public class Stables : Building
{
    void Start()
    {
        TurnManager.Instance.StartTurnSubject.AddObserver(this);
        startOfTurnNotificationData = new Dictionary<Utils.NotificationTypes, int>();
    }

    public override void UpgradeToT2()
    {
        base.UpgradeToT2();
    }

}
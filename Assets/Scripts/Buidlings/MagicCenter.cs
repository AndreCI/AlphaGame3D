using System;
using System.Collections.Generic;

public class MagicCenter : Building
{

    // Use this for initialization
    void Start()
    {
        TurnManager.Instance.StartTurnSubject.AddObserver(this);
        startOfTurnNotificationData = new Dictionary<Utils.notificationTypes, int> {
            {Utils.notificationTypes.MANA, 4 }
        };
    }

    public override void UpgradeToT2()
    {
        base.UpgradeToT2();
        startOfTurnNotificationData[Utils.notificationTypes.MANA] += 4;
    }

}

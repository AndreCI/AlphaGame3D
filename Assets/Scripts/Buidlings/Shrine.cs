using System;
using System.Collections.Generic;

public class Shrine : Building
{
    public SpellUtils.SchoolOfMagic schoolOfMagic;
    

    // Use this for initialization
    void Start()
    {
        owner.schoolOfMagicLevels[schoolOfMagic] += 1;
        isTier2 = true;
        startOfTurnNotificationData = new Dictionary<Utils.NotificationTypes, int>();
    }
}

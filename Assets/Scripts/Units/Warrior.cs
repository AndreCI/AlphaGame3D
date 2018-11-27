using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior : Unit
{ 

    public override Type GetSpawnPoint()
    {
        return typeof(Barracks);
    }
}

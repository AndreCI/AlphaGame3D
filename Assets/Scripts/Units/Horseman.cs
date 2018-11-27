using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Horseman : Unit
{ 

    public override Type GetSpawnPoint()
    {
        return typeof(Stables);
    }
}

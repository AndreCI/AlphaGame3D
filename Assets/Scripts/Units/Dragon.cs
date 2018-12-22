using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : FlyingUnit
{ 

    public override Type GetSpawnPoint()
    {
        return typeof(Stables);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Unit
{
    public override Type GetSpawnPoint()
    {
        return typeof(Barracks);
    }
}

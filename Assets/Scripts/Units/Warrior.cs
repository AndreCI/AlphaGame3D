using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior : Unit
{
    public int riposteAugmentedDamageAmplitude = 5;

    protected override IEnumerator Attack(HexCell target, bool riposte)
    {
        if (riposte)
        {
            currentAttackModifier += riposteAugmentedDamageAmplitude;
        }
        yield return StartCoroutine(base.Attack(target, riposte));
        if (riposte)
        {
            currentAttackModifier -= riposteAugmentedDamageAmplitude;
        }
        
    }
    public override Type GetSpawnPoint()
    {
        return typeof(Barracks);
    }
}

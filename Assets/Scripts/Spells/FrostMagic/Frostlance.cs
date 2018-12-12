using System;
using System.Collections.Generic;
using UnityEngine;

public class Frostlance : Spell
{
    public static Frostlance Instance;
    public SpellUtils.EffectTypes effect;
    public int frostDuration;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            base.AwakeBase();
            effects.Add(new EffectFactory(effect, frostDuration));
        }
        else
        {
            throw new System.NotImplementedException();
        }
    }


    public override void PlayAnimation()
    {
        transform.position = playerInfos[TurnManager.Instance.currentPlayer].position.transform.position;
        ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem p in particles)
        {
            p.Play();
        }
        GetComponentInChildren<Animation>().Play();
    }
    public override void Activate(List<HexCell> affectedNodes_)
    {
        foreach(HexCell node in affectedNodes_)
        {
            node.Damage(damage);
            if (node.unit != null) {
                ApplyEffectsToUnit(node.unit);
            }
        }
        base.Activate(affectedNodes_);
    }
}
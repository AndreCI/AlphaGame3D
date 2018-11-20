using System;
using System.Collections.Generic;
using UnityEngine;

public class EarthLink : Spell
{
    public static EarthLink Instance;
    public SpellUtils.EffectTypes effect;
    public int buffDuration;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            base.AwakeBase();
            effects.Add(new EffectFactory(effect, buffDuration));
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
    public override void Activate(List<Node> affectedNodes_)
    {
        foreach(Node node in affectedNodes_)
        {
            
            if (node.unit != null)
            {
                node.unit.Heal(damage);
                ApplyEffectsToUnit(node.unit);
            }
        }
        base.Activate(affectedNodes_);
    }
}
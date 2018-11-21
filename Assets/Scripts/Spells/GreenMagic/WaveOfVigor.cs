using System;
using System.Collections.Generic;
using UnityEngine;

public class WaveOfVigor : Spell
{
    public static WaveOfVigor Instance;
    public SpellUtils.EffectTypes effect;
    public int buffDuration;
    public int amplitude;
    public SpellUtils.EffectTypes effect2;
    public int buffDuration2;
    public int amplitude2;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            base.AwakeBase();
            effects.Add(new EffectFactory(effect, buffDuration, amplitude));
            effects.Add(new EffectFactory(effect2, buffDuration2, amplitude2));
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
            
            if (node.unit != null && node.unit.owner.Equals(TurnManager.Instance.currentPlayer))
            {
                node.unit.Heal(damage);
                ApplyEffectsToUnit(node.unit);
            }
        }
        base.Activate(affectedNodes_);
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

public class BerserkerSpirit : Spell
{
    public static BerserkerSpirit Instance;
    public SpellUtils.EffectTypes effect;
    public int buffDuration;
    public int buffPower;
    public SpellUtils.EffectTypes effect2;
    public int buffDuration2;
    public int buffPower2;
    public SpellUtils.EffectTypes effect3;
    public int buffDuration3;
    public int buffPower3;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            base.AwakeBase();
            effects.Add(new EffectFactory(effect, buffDuration, buffPower));
            effects.Add(new EffectFactory(effect2, buffDuration2, buffPower2));
            effects.Add(new EffectFactory(effect3, buffDuration3, buffPower3));

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
            if (node.unit != null)
            {
                if (node.unit.owner.Equals(TurnManager.Instance.currentPlayer))
                {
                    node.Damage(6);
                    ApplyEffectsToUnit(node.unit);
                }
            }
        }
        base.Activate(affectedNodes_);
    }
}
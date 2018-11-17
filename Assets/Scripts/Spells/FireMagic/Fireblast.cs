using System;
using System.Collections.Generic;
using UnityEngine;

public class Fireblast : Spell
{
    public static Fireblast Instance;
    public SpellUtils.EffectTypes effect;
    public int burnDuration;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            unlock = new List<Type>();
            base.AwakeBase();
            effects.Add(new EffectFactory(effect, burnDuration));
        }
        else
        {
            throw new System.NotImplementedException();
        }
    }
    public override List<Type> GetRequierements()
    {
        return new List<Type> { typeof(MagicCenter) };
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
            if (node == currentPosition)
            {
                node.Damage(damage);
            }
            node.Damage(damage);
            if (node.unit != null)
            {
                ApplyEffectsToUnit(node.unit);
            }
        }
        base.Activate(affectedNodes_);
    }
}
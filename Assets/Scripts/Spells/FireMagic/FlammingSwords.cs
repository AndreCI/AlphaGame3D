
using System;
using System.Collections.Generic;
using UnityEngine;

public class FlammingSwords : Spell
{
    public static FlammingSwords Instance;
    public int buffDuration;
    public int buffPower;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            unlock = new List<Type>();
            base.AwakeBase();

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
            if (node.unit != null)
            {
                if (node.unit.owner.Equals(TurnManager.Instance.currentPlayer))
                {
                    UnitEffect ue = new AttackModifierEffect(node.unit, buffDuration, buffPower);
                    ue.ApplyEffect();
                    node.unit.currentEffect.Add(ue);
                    

                }
            }
        }
        base.Activate(affectedNodes_);
    }
}
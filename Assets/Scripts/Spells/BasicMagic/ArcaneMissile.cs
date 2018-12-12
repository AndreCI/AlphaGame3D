using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcaneMissile : Spell
{
    public static ArcaneMissile Instance;
    List<HexCell> targetedNodes;
    public int missileNumber;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            base.AwakeBase();

        }
        else
        {
            throw new System.NotImplementedException();
        }
    }

    public override void PlayAnimation()
    {
        StartCoroutine(playAnimationWithWait());
    }

    private IEnumerator playAnimationWithWait()
    {
        foreach (HexCell node in targetedNodes)
        {
            Debug.Log(node.ToString());
            yield return StartCoroutine(playSingleAnimation(node));
        }
    }

    private IEnumerator playSingleAnimation(HexCell node)
    {
        
        transform.position = node.Position;
        ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem p in particles)
        {
            p.Play();
        }
        GetComponentInChildren<Animation>().Play();
        yield return new WaitForSeconds(1.8f);
    }
    public override void Activate(List<HexCell> affectedNodes_)
    {
        targetedNodes = new List<HexCell>();
        targetedNodes = Utils.GetRandomElements<HexCell>(affectedNodes_, missileNumber);
        foreach(HexCell node in targetedNodes)
        {
            node.Damage(damage);
        }
        base.Activate(affectedNodes_);
    }
}
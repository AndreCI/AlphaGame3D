using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcaneMissile : Spell
{
    public static ArcaneMissile Instance;
    List<Node> targetedNodes;
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
        foreach (Node node in targetedNodes)
        {
            Debug.Log(node.ToString());
            yield return StartCoroutine(playSingleAnimation(node));
        }
    }

    private IEnumerator playSingleAnimation(Node node)
    {

        transform.position = node.position;
        ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem p in particles)
        {
            p.Play();
        }
        GetComponentInChildren<Animation>().Play();
        yield return new WaitForSeconds(1.8f);
    }
    public override void Activate(List<Node> affectedNodes_)
    {
        targetedNodes = new List<Node>();
        targetedNodes = Utils.GetRandomElements<Node>(affectedNodes_, missileNumber);
        foreach(Node node in targetedNodes)
        {
            node.Damage(damage);
        }
        base.Activate(affectedNodes_);
    }
}
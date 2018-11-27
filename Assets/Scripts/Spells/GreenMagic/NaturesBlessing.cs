using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaturesBlessing : Spell
{
    public static NaturesBlessing Instance;
    public NaturesBlessingBuilding prefabBuilding;
    public SpellUtils.EffectTypes effect;
    public int buffDuration;
    public int amplitude;

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
        transform.position = playerInfos[TurnManager.Instance.currentPlayer].position.transform.position;
        ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem p in particles)
        {
            p.Play();
        }
        GetComponentInChildren<Animation>().Play();
    }

    protected override void GetCastableNodes()
    {
        base.GetCastableNodes();
        foreach(Node n in castableNodes)
        {
            if (!n.walkable)
            {
                n.MakeIdle();
            }
        }
        castableNodes.RemoveAll(n => n.state == Node.STATE.IDLE);
    }

    public override void Activate(List<Node> affectedNodes_)
    {
        StartCoroutine(WaitForEffect(affectedNodes_));
    }
    private IEnumerator WaitForEffect(List<Node> affectedNodes_)
    {
        yield return new WaitForSeconds(1.0f);
        Node node = affectedNodes_[0];
        GameObject buildingObject = (GameObject)Instantiate(prefabBuilding.prefab, node.transform.position + node.positionOffset, node.transform.rotation);

        Building building = (Building)buildingObject.GetComponent(prefabBuilding.GetType());
        building.SetVisible(true);
        building.SetCurrentPosition(node);
        building.owner = TurnManager.Instance.currentPlayer;
        ((NaturesBlessingBuilding)building).effect = new EffectFactory(effect, buffDuration, amplitude);
        TurnManager.Instance.currentPlayer.currentBuildings.Add(building);
        TurnManager.Instance.currentPlayer.UpdateVisibleNodes();
        node.building = building;
        node.walkable = false;
        base.Activate(affectedNodes_);
        yield return new WaitForEndOfFrame();
    }
}
﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcaneMirage : Spell
{
    public static ArcaneMirage Instance;
    public ArcaneMirageBuilding prefabBuilding;

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
    protected override void GetCastableNodes()
    {
        base.GetCastableNodes();
        foreach (HexCell n in castableNodes)
        {
            if (!n.IsFree(TurnManager.Instance.currentPlayer))
            {
                n.State = HexCell.STATE.IDLE;
            }
        }
        castableNodes.RemoveAll(n => n.State == HexCell.STATE.IDLE);
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
        StartCoroutine(WaitForEffect(affectedNodes_));
    }
    private IEnumerator WaitForEffect(List<HexCell> affectedNodes_)
    {
       yield return new WaitForSeconds(1.0f);
        HexCell node = affectedNodes_[0];
        GameObject buildingObject = (GameObject)Instantiate(prefabBuilding.prefab, node.Position, node.transform.rotation);

        Building building = (Building)buildingObject.GetComponent(prefabBuilding.GetType());
        building.SetVisible(true);
        building.currentPosition = node;
        building.owner = TurnManager.Instance.currentPlayer;
        TurnManager.Instance.currentPlayer.currentBuildings.Add(building);
        node.building = building;
        base.Activate(affectedNodes_);
        yield return new WaitForEndOfFrame();
    }
}
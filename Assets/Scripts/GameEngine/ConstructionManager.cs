﻿using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConstructionManager : MonoBehaviour, IObserver
{
    public static ConstructionManager Instance;


    // Use this for initialization
    void Awake()
    {
        if (Instance != null)
        {
            return;
        }
        Instance = this;
        buildingToConstruct = null;
        unitToConstruct = null;
        spellToConstruct = null;
        TurnManager.Instance.EndTurnSubject.AddObserver(this);
        TurnManager.Instance.StartTurnSubject.AddObserver(this);
        canConstruct = false;
        availablePositions = new List<Node>();
    }
    [Header("Building prefabs")]
    public Building Barracks;
    public Building HallCenter;
    public Building WindMill;
    //public Building MagicCenter;

    [Header("Unit prefabs")]
    public Unit Warrior;
    public Unit Wizard;
    

    private Building buildingToConstruct;
    private Unit unitToConstruct;
    private Spell spellToConstruct;
    public string mode;
    public bool canConstruct;
    private List<Node> availablePositions;

    public void SetBuildingToBuild(Building building, bool hallCenter=false)
    {
        ResetConstruction();
        Selector.Instance.Unselect();
        mode = "building";
        buildingToConstruct = building;
        canConstruct = true;
        if (!hallCenter)
        {
            Selector.Instance.currentObject = (building);
            building.UpdateCardDisplayInfo();
            foreach (Node n in NodeUtils.BFSNodesAdj(TurnManager.Instance.currentPlayer.currentBuildings[0].currentPosition, 
                                                    TurnManager.Instance.currentPlayer.buildingRange).
                                                    GetChildrens())
            
            {
                if (n.walkable)
                {
                    n.state = Node.STATE.SELECTABLE_CONSTRUCT;
                    availablePositions.Add(n);
                }
            }
        }
    }

    public Building ConstructBuilding(Node node)
    {
        GameObject prefab = buildingToConstruct.prefab;
        GameObject buildingObject = (GameObject)Instantiate(prefab, node.transform.position + node.positionOffset, node.transform.rotation);

        Building building = (Building)GetScript(buildingObject);
        building.SetCurrentPosition(node);
        building.owner = TurnManager.Instance.currentPlayer;
        TurnManager.Instance.currentPlayer.currentBuildings.Add(building);
        TurnManager.Instance.currentPlayer.requirementSystem.AddCopy(building.GetType());
        TurnManager.Instance.currentPlayer.gold -= building.goldCost;
        TurnManager.Instance.currentPlayer.actionPoints -= building.actionPointCost;
        TurnManager.Instance.currentPlayer.UpdateVisibleNodes();
        foreach (Node n in availablePositions)
        {
            n.MakeIdle();
        }
        Selector.Instance.currentObject = null;
        if (CardDisplay.Instance != null) { CardDisplay.Instance.DisableCardDisplay(); } //sanity check because hallcenter spawn is manually made.
        TurnManager.Instance.ButtonUpdateSubject.NotifyObservers(TurnManager.Instance.currentPlayer);
        return building;
    }

    public void SetSpellToConstruct(Spell spell)
    {
        ResetConstruction();
        mode = "spell";
        spellToConstruct = spell;
 
        spell.owner = TurnManager.Instance.currentPlayer;
        Selector.Instance.Select(spellToConstruct);
        canConstruct = true;
    }

    public void SpellNodeSelected(Node node)
    {
        spellToConstruct.GetSpellEffectNodes(node);
    }

    public Spell ConstructSpell(Node node)
    {
        spellToConstruct.playerInfos[TurnManager.Instance.currentPlayer].position = node;
        spellToConstruct.playerInfos[TurnManager.Instance.currentPlayer].owner = TurnManager.Instance.currentPlayer;
        TurnManager.Instance.currentPlayer.mana -= spellToConstruct.manaCost;
        TurnManager.Instance.currentPlayer.actionPoints -= spellToConstruct.actionPointCost;
        spellToConstruct.Activate(spellToConstruct.affectedNodes);
        spellToConstruct.PlayAnimation();
        TurnManager.Instance.ButtonUpdateSubject.NotifyObservers(TurnManager.Instance.currentPlayer);

        return spellToConstruct;
    }

    public void SetUnitToBuild(Unit unit)
    {
        ResetConstruction();
        availablePositions = new List<Node>();
        Selector.Instance.Unselect();
        mode = "unit";
        unitToConstruct = unit;
        canConstruct = true;
        Selector.Instance.currentObject = (unit);
        unit.UpdateCardDisplayInfo();
        Selectable spawnPoint = TurnManager.Instance.currentPlayer.GetSelectableFromType(unit.GetSpawnPoint());
        foreach(Node n in NodeUtils.BFSNodesAdj(spawnPoint.currentPosition, 1).GetChildrens())
        {
            if (n.walkable)
            {
                n.state = Node.STATE.SELECTABLE_CONSTRUCT;
                availablePositions.Add(n);
            }
        }

    }

    public Unit ConstructUnit(Node node)
    {
        GameObject prefab = unitToConstruct.prefab;
        GameObject unitObject = (GameObject)Instantiate(prefab, node.transform.position + node.positionOffset, node.transform.rotation);

        Unit unit = (Unit)ConstructionManager.Instance.GetScript(unitObject);
        unit.Setup();

        unit.SetCurrentPosition(node);
        unit.owner = TurnManager.Instance.currentPlayer;
        TurnManager.Instance.currentPlayer.currentUnits.Add(unit);
        TurnManager.Instance.currentPlayer.gold -= unit.goldCost;
        TurnManager.Instance.currentPlayer.actionPoints -= unit.actionPointCost;
        TurnManager.Instance.currentPlayer.UpdateVisibleNodes();
        foreach(Node n in availablePositions)
        {
            n.MakeIdle();
        }
        Selector.Instance.currentObject = null;
        CardDisplay.Instance.DisableCardDisplay();
        TurnManager.Instance.ButtonUpdateSubject.NotifyObservers(TurnManager.Instance.currentPlayer);

        return unit;
    }

    public void ResetConstruction()
    {
        buildingToConstruct = null;
        unitToConstruct = null;
        spellToConstruct = null;
        canConstruct = false;
        foreach (Node n in availablePositions)
        {
            n.MakeIdle();
        }
    }

    public void Notify(Player player, TurnSubject.NOTIFICATION_TYPE type)
    {
        ResetConstruction();
    }

    public Component GetScript(GameObject currentObject)
    {
        if(buildingToConstruct != null)
        {
            return currentObject.GetComponent(buildingToConstruct.GetType());
        }else if (unitToConstruct != null)
        {
            return currentObject.GetComponent(unitToConstruct.GetType());
        }
        throw new System.AccessViolationException("This should not happend");
    }
}

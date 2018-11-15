using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public class ConstructionManager : Observer
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
        canConstruct = false;
        availablePositions = new List<Node>();
    }
    [Header("Building prefabs")]
    public Building Barracks;
    public Building HallCenter;
    //public Building MagicCenter;

    [Header("Unit prefabs")]
    public Unit Warrior;
    public Unit Wizard;
    

    private Building buildingToConstruct;
    private Unit unitToConstruct;
    private Spell spellToConstruct;
    public string mode;
    private Player Builder;
    public bool canConstruct;
    private List<Node> availablePositions;

    public void SetBuildingToBuild(Building building, bool hallCenter=false)
    {
        ResetConstruction();
        Selector.Instance.Unselect();
        mode = "building";
        buildingToConstruct = building;
        Builder = TurnManager.Instance.currentPlayer;
        canConstruct = true;
        if (!hallCenter)
        {
            NodeUtils.NodeWrapper nodeWrapper = NodeUtils.GetPossibleNodes(TurnManager.Instance.currentPlayer.currentBuildings[0].currentPosition, 3);
            foreach (NodeUtils.NodeWrapper nw in nodeWrapper.GetNodeChildren())
            {
                if (nw.state == NodeUtils.NodeWrapper.STATE.EMPTY)
                {
                    nw.root.state = Node.STATE.SELECTABLE_CONSTRUCT;
                    availablePositions.Add(nw.root);
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
        TurnManager.Instance.currentPlayer.gold -= building.goldCost;
        TurnManager.Instance.currentPlayer.actionPoints -= building.actionPointCost;
        TurnManager.Instance.currentPlayer.UpdateVisibleNodes();
        foreach (Node n in availablePositions)
        {
            n.MakeIdle();
        }
        return building;
    }

    public void SetSpellToConstruct(Spell spell)
    {
        ResetConstruction();
        mode = "spell";
        spellToConstruct = spell;
 
        spell.owner = TurnManager.Instance.currentPlayer;
        Selector.Instance.Select(spellToConstruct);
        Builder = TurnManager.Instance.currentPlayer;
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
        
        return spellToConstruct;
    }

    public void SetUnitToBuild(Unit unit)
    {
        ResetConstruction();
        availablePositions = new List<Node>();
        Selector.Instance.Unselect();
        mode = "unit";
        unitToConstruct = unit;
        Builder = TurnManager.Instance.currentPlayer;
        canConstruct = true;
        Selectable requirements = TurnManager.Instance.currentPlayer.GetSelectableFromType(unit.GetRequierements()[0]);
        NodeUtils.NodeWrapper nodeWrapper = NodeUtils.GetPossibleNodes(requirements.currentPosition, 1);
        foreach(NodeUtils.NodeWrapper nw in nodeWrapper.GetNodeChildren())
        {
            if (nw.state == NodeUtils.NodeWrapper.STATE.EMPTY)
            {
                nw.root.state = Node.STATE.SELECTABLE_CONSTRUCT;
                availablePositions.Add(nw.root);
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
        return unit;
    }

    public void ResetConstruction()
    {
        buildingToConstruct = null;
        unitToConstruct = null;
        spellToConstruct = null;
        Builder = null;
        canConstruct = false;
        foreach (Node n in availablePositions)
        {
            n.MakeIdle();
        }
    }

    public override void Notify(Player player)
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
        if (buildingToConstruct == HallCenter)
        {
            return currentObject.GetComponent<HallCenter>();// (buildingToConstruct.GetType());
        }
        //else if(buildingToConstruct == MagicCenter)
        //{
        //    Debug.Log(buildingToConstruct.GetType().ToString());
        //    return currentObject.GetComponent(buildingToConstruct.GetType());
        //}
        else if(unitToConstruct == Warrior)
        {
            return currentObject.GetComponent<Warrior>();
        }else if(unitToConstruct == Wizard)
        {
            return currentObject.GetComponent<Wizard>();
        }
        else if(buildingToConstruct == Barracks)
        {
            return currentObject.GetComponent<Barracks>();
        }
        else
        {
            throw new System.NotImplementedException();
        }
    }
}

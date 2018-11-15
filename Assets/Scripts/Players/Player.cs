using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public class Player
{

    protected static Player player1;
    protected static Player player2;

    public static Player Player1
    {
        get
        {
            if(player1 == null)
            {
                player1 = new Player(1);
            }
            return player1;
        }
    }
    public static Player Player2
    {
        get
        {
            if (player2 == null)
            {
                player2 = new ArtificialIntelligence(2);
            }
            return player2;
        }
    }

    public int id;
    public int gold;
    public int mana;
    public int actionPoints;
    public int food;
    public int foodPrediction;
    public List<Unit> currentUnits;
    public List<Building> currentBuildings;
    public List<Node> visibleNodes;
    public List<Node> knownBuilding;
    protected List<Node> visibleNodesPrev;
    public Dictionary<SpellUtils.SchoolOfMagic, int> schoolOfMagicLevels;

    public static Player getPlayerFromId(int id)
    {
        if (id == 1)
        {
            return Player1;
        }
        else if (id == 2)
        {
            return Player2;
        }
        else
        {
            throw new System.Exception("id must be 1 or 2");
        }
    }
    public void HideVisibleNodes()
    {
        foreach (Node node in visibleNodes)
        {
            node.SetVisible(false);
        }
    }

    public virtual void UpdateVisibleNodes()
    {
        foreach (Building b in currentBuildings)
        {
            if (!knownBuilding.Contains(b.currentPosition))
            {
                knownBuilding.Add(b.currentPosition);
            }
        }
        visibleNodes = new List<Node>();
        foreach (Building buiding in currentBuildings)
        {
            NodeUtils.NodeWrapper currentPositionWrapped = NodeUtils.GetNeighborsNode(buiding.currentPosition, 3);
            List<NodeUtils.NodeWrapper> castableNodesWrapped = currentPositionWrapped.GetNodeChildren();
            foreach (NodeUtils.NodeWrapper nodeWrapped in castableNodesWrapped)
            {
                if (!visibleNodes.Contains(nodeWrapped.root))
                {
                    visibleNodes.Add(nodeWrapped.root);
                }
            }
        }
        foreach (Unit unit in currentUnits)
        {
            NodeUtils.NodeWrapper currentPositionWrapped = NodeUtils.GetNeighborsNode(unit.currentPosition, unit.visionRange);
            List<NodeUtils.NodeWrapper> castableNodesWrapped = currentPositionWrapped.GetNodeChildren();
            foreach (NodeUtils.NodeWrapper nodeWrapped in castableNodesWrapped)
            {
                if (!visibleNodes.Contains(nodeWrapped.root))
                {
                    visibleNodes.Add(nodeWrapped.root);
                }
            }
        }
        foreach (Node node in visibleNodesPrev)
        {
            if (!visibleNodes.Contains(node))
            {
                node.SetVisible(false);
            }

        }
        foreach (Node node in visibleNodes)
        {
            node.SetVisible(true);
            if (node.building != null && !knownBuilding.Contains(node))
            {
                knownBuilding.Add(node);
            }
        }
        visibleNodesPrev = visibleNodes;
    }
    protected Player(int id_)
    {
        gold = 8;
        mana = 0;
        actionPoints = 6;
        food = 4;
        foodPrediction = 0;
        id = id_;
        currentBuildings = new List<Building>();
        currentUnits = new List<Unit>();
        visibleNodesPrev = new List<Node>();
        visibleNodes = new List<Node>();
        knownBuilding = new List<Node>();
        schoolOfMagicLevels = new Dictionary<SpellUtils.SchoolOfMagic, int>
        {
            { SpellUtils.SchoolOfMagic.BASIC, 100 },
            { SpellUtils.SchoolOfMagic.FIRE, 0 },
            {SpellUtils.SchoolOfMagic.FROST, 0 }
        };
    }

    public void AddGold(int amount)
    {
        gold += amount;
    }
    public override bool Equals(object obj)
    {
        if (!(obj is Player || obj is ArtificialIntelligence))
        {
            return false;
        }
        Player other = (Player)obj;
        return other.id == this.id;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        string returnDebugVal = "PLAYER_DEBUG : ["+id+";"+gold+";"+mana+";"+actionPoints+";  ";
        returnDebugVal += "Buildings: {";
        foreach(Building b in currentBuildings)
        {
            returnDebugVal += b.ToString() + "; ";
        }
        returnDebugVal += "}. Units: {";
        foreach(Unit u in currentUnits)
        {
            returnDebugVal += u.ToString() + "; ";
        }
        returnDebugVal += "]";
        return returnDebugVal;
    }

    public bool CheckIfAvailable(Selectable target)
    {
        int requirementSatisfied = 0;
        foreach (Building building in currentBuildings)
        {
            if (building.unlock.Contains(target.GetType()))
            {
                requirementSatisfied += 1;
                break;
            }
        }
        bool requierementsbool = requirementSatisfied >= 1;
        bool cost = target.goldCost <= gold && target.manaCost <= mana && target.actionPointCost <= actionPoints;
        bool levelbool = true;
        if (typeof(Spell).IsAssignableFrom(target.GetType()))
        {
            levelbool = schoolOfMagicLevels[((Spell)target).schoolOfMagic] >= ((Spell)target).requirementLevel;
        }
        return requierementsbool && cost && levelbool;

    }
    public virtual void StartOfTurn()
    {

    }

    public Selectable GetSelectableFromType(Type type)
    {
        foreach(Building b in currentBuildings)
        {
            if(b.GetType() == type)
            {
                return b;
            }
        }

        return null;
    }

}

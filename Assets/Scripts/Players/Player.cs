using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public bool isAi;
    public int gold;
    public int mana;
    public int manaBank;
    public int actionPoints;
    public int food;
    public int foodPrediction;
    public List<Unit> currentUnits;
    public List<Building> currentBuildings;
    public List<Node> visibleNodes;
    public List<Node> knownBuilding;
    protected List<Node> visibleNodesPrev;
    public Dictionary<SpellUtils.SchoolOfMagic, int> schoolOfMagicLevels;
    public RequirementSystem requirementSystem;
    private int buildingVisiblity;
    public int buildingRange;

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
            NodeUtils.NodeWrapper currentPositionWrapped = NodeUtils.GetNeighborsNode(buiding.currentPosition, buildingVisiblity);
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
        manaBank = 0;
        actionPoints = 6;
        food = 4;
        foodPrediction = 4;
        id = id_;
        isAi = false;
        requirementSystem = new RequirementSystem();
        currentBuildings = new List<Building>();
        currentUnits = new List<Unit>();
        visibleNodesPrev = new List<Node>();
        visibleNodes = new List<Node>();
        knownBuilding = new List<Node>();
        schoolOfMagicLevels = new Dictionary<SpellUtils.SchoolOfMagic, int>
        {
            { SpellUtils.SchoolOfMagic.BASIC, 100 },
            { SpellUtils.SchoolOfMagic.GREEN, 0 },
            { SpellUtils.SchoolOfMagic.FIRE, 0 },
            {SpellUtils.SchoolOfMagic.FROST, 0 }
        };


        buildingVisiblity = 3;
        buildingRange = 3;
    }

    public void AddGold(int amount)
    {
        if(gold + amount < 0) {
            gold = 0;
        }
        else {
            gold += amount;
        }
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

    public string GetUnavailableMessage(Selectable target)
    {
        List<string> messages = new List<String>();
        bool alreadyOwned = requirementSystem.MaxCopyOwned(target.GetType()) && target.GetType()!=typeof(Shrine);
        if(alreadyOwned){
            return "You already own this building.";
        }
        messages.Add("You need");
        bool requierementsbool = requirementSystem.CheckIfRequirementAreSatisfied(target.GetType(), target.isTier2);
        if (!requierementsbool)
        {
            messages.Add("an additional building or an upgrade");
        }
        bool cost = target.goldCost <= gold;
        if (!cost)
        {
            messages.Add("more gold");
        }
        cost = target.manaCost <= mana;
        if (!cost)
        {
            messages.Add("more mana");
        }
        cost= target.actionPointCost <= actionPoints;
        if (!cost)
        {
            messages.Add("more action points");
        }

        bool levelbool = true;
        bool cooldown = true;
        if (typeof(Spell).IsAssignableFrom(target.GetType()))
        {
            levelbool = schoolOfMagicLevels[((Spell)target).schoolOfMagic] >= ((Spell)target).requirementLevel;
            if (((Spell)target).playerInfos[this].currentCooldown > 0)
            {
                cooldown = false;
            }
        }
        if (!levelbool)
        {
            messages.Add("more magic level(s)");
        }
        if (!cooldown)
        {
            messages.Add("to wait for cooldown");
        }
        if(messages.Count == 2)
        {
            return messages[0] + " " + messages[1] + ".";
        }
        string message = messages[0] + " ";
        for(int i = 1; i<messages.Count - 2; i++)
        {
            message += messages[i];
            message += ", ";
        }
        message += messages[messages.Count - 2] + " and " + messages[messages.Count - 1] + ".";
        return message;
    }

    public bool CheckIfAvailable(Selectable target)
    {
        bool alreadyOwned = requirementSystem.MaxCopyOwned(target.GetType()) && target.GetType() != typeof(Shrine);
        bool requierementsbool = requirementSystem.CheckIfRequirementAreSatisfied(target.GetType(), target.isTier2);
        bool cost = target.goldCost <= gold && target.manaCost <= mana && target.actionPointCost <= actionPoints;
        bool levelbool = true;
        bool cooldown = true;
        if (typeof(Spell).IsAssignableFrom(target.GetType()))
        {
            levelbool = schoolOfMagicLevels[((Spell)target).schoolOfMagic] >= ((Spell)target).requirementLevel;
            if(((Spell)target).playerInfos[this].currentCooldown > 0)
            {
                cooldown = false;
            }
        }
        return !alreadyOwned && requierementsbool && cost && levelbool && cooldown;

    }
    public virtual IEnumerator StartOfTurn()
    {
        yield return new WaitForSeconds(0.0f);
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

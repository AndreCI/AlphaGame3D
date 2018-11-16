using System;
using System.Collections.Generic;

public class ArtificialIntelligence : Player
{

    public ArtificialIntelligence(int id_) : base(id_)
    {
    }
    public override void StartOfTurn()
    {
        GetVisibleNodes();
        gold += 10;
        if (currentBuildings.Count == 1){
            PlaceBuilding(ConstructionManager.Instance.Barracks);
        }
        PlaceUnit(ConstructionManager.Instance.Warrior);
        //PlaceUnit(ConstructionManager.Instance.Wizard);
        foreach (Unit u in currentUnits)
        {
            MoveUnit(u);
        }
        
        EndOfTurn();

    }

    private void MoveUnit(Unit u)
    {
        Node target = DecideUnitMovement(u);
        u.ShowPotentialMove(target);
        u.StartAIMove();
        u.HidePotentialMove();
        u.HidePossibleMoves();
    }
    private Node DecideUnitMovement(Unit u)
    {
        Node target = null;
        List<Node> attackables = new List<Node>();
        List<Node> goable = new List<Node>();
        List<NodeUtils.NodeWrapper> possibleMoves = u.ShowPossibleMoves();
        foreach(NodeUtils.NodeWrapper n in possibleMoves)
        {
            if (n.root.Attackable(u.currentPosition))
            {
                attackables.Add(n.root);
            }
            else
            {
                goable.Add(n.root);
            }
        }
        if (attackables.Count != 0) {
            target = DecideUnitMovementAttackableNodes(attackables);
        }
        else {
            target = DecideUnitMovementUnattackableNodes(goable);
        }
            return target;
    }
    private Node DecideUnitMovementAttackableNodes(List<Node> attackables)
    {
        List<int> priority = new List<int>();
        foreach(Node n in attackables)
        {
            if (n.building != null && typeof(DefensiveBuilding).IsAssignableFrom(n.building.GetType())) {
                if (typeof(HallCenter).IsAssignableFrom(n.building.GetType()))
                {
                    priority.Add(1000);
                }
                else
                {
                    priority.Add(((DefensiveBuilding)n.building).currentHealth);
                }
            } else if (n.GetUnit() != null && n.GetUnit().GetType() == typeof(Wizard))
            {
                priority.Add(100);
            }
            else
            {
                priority.Add(n.GetUnit().currentHealth);
            }
        }
        int maxIdx = 0;
        int maxPrio = 0;
        for(int j=0; j<priority.Count; j++)
        {
            int i = priority[j];
            if (i > maxPrio)
            {
                maxIdx = j;
                maxPrio = i;
            }
        }

        return attackables[maxIdx];
    }

    private Node DecideUnitMovementUnattackableNodes(List<Node> goable)
    {
        Node target = null;
        float maxDistanceToHallCenterOnX = 0.0f;
        foreach (Node n in goable)
        {
            float distanceOnX = Math.Abs(n.position.x - currentBuildings[0].currentPosition.position.x);
            if (distanceOnX > maxDistanceToHallCenterOnX)
            {
                target = n;
                maxDistanceToHallCenterOnX = distanceOnX;
            }
        }
        if (target == null)
        {
            target = goable[(new System.Random()).Next(0, goable.Count)];
        }
        return target;
    }

    private void PlaceUnit(Unit u)
    {
        ConstructionManager.Instance.SetUnitToBuild(u);
        int idx = 0;
        List<Node> selectables = GetSelectableNodes();
        int randIdx = 1;// (new System.Random()).Next(0, selectables.Count);
        foreach (Node n in selectables)
        {
            idx += 1;
            if (idx > randIdx)
            {
                n.Construct(true);
                n.GetUnit().SetVisible(false);
                return;
            }
        }
    }
    private void PlaceBuilding(Building b)
    {
        ConstructionManager.Instance.SetBuildingToBuild(b);
        int idx = 0;
        List<Node> selectables = GetSelectableNodes();
        int randIdx = (new System.Random()).Next(0, selectables.Count);
        foreach(Node n in selectables)
        {
                idx += 1;
                if (idx > randIdx)
                {
                    n.Construct(true);
                    n.building.SetVisible(false);
                    return;
                }
                
        }
    }
    
    private List<Node> GetSelectableNodes()
    {
        GetVisibleNodes();
        List<Node> selectables = new List<Node>();
        //Debug.Log("visibles" + visibleNodes.Count.ToString());
        foreach (Node n in visibleNodes)
        {
            if (n.state == Node.STATE.SELECTABLE_CONSTRUCT || n.state == Node.STATE.SELECTABLE_CONSTRUCT_FINAL)
            {
                selectables.Add(n);

            }
        }
        //Debug.Log("selectable" + visibleNodes.Count.ToString());
        return selectables;
    }

    public void EndOfTurn()
    {
        TurnManager.Instance.EndOfAITurn();
    }

    public override void UpdateVisibleNodes()
    {

    }

    private void GetVisibleNodes()
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
        }
}
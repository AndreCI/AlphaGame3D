using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArtificialIntelligence : Player
{
    public bool turnFinished;
    private int turnNumber;
    private List<Unit> deactivatedUnits;
    private MonoBehaviour coroutineStarter;

    public ArtificialIntelligence(int id_) : base(id_)
    {
        turnFinished = true;
        buildingRange = 5;
        isAi = true;
        deactivatedUnits = new List<Unit>();
    }
    public override IEnumerator StartOfTurn()
    {
        if(coroutineStarter == null)
        {
            coroutineStarter = currentBuildings[0];
        }
        Debug.Log("--------------------");
        Debug.Log("AI starts a new Turn");
        turnNumber += 1;
        turnFinished = false;
        GetVisibleNodes();
        gold += 20;
        actionPoints += 5;
        foodPrediction -= 2;
        UpdateUnitEffect();
        
        
            yield return coroutineStarter.StartCoroutine(BasicRush());


        UpdateUnitEffect();

        Debug.Log("AI ends the turn.");
            turnFinished = true;
            EndOfTurn();
        
        yield return new WaitForSeconds(0.0f);
    }
    public IEnumerator BasicRush()
    {
        if (turnNumber == 1)
        {
            yield return coroutineStarter.StartCoroutine(PlaceBuilding(ConstructionManager.Instance.Barracks));
            yield return coroutineStarter.StartCoroutine(PlaceBuilding(ConstructionManager.Instance.Barracks));
            yield return coroutineStarter.StartCoroutine(PlaceBuilding(ConstructionManager.Instance.Barracks));
        }
        else if(turnNumber == 2)// || food < 1)
        {
            yield return coroutineStarter.StartCoroutine(PlaceBuilding(ConstructionManager.Instance.WindMill));
        }else if(turnNumber == 8)
        {
            //yield return UpgradeToT2((Building)GetSelectableFromType(typeof(Barracks)));
        }else if(turnNumber > 8)
        {

        }
        yield return coroutineStarter.StartCoroutine(PlaceUnit(ConstructionManager.Instance.Warrior));
        yield return coroutineStarter.StartCoroutine(PlaceUnit(ConstructionManager.Instance.Wizard));
        yield return coroutineStarter.StartCoroutine(PlaceUnit(ConstructionManager.Instance.Warrior));

        //yield return coroutineStarter.StartCoroutine(PlaceUnit(ConstructionManager.Instance.Wizard));
        Debug.Log("Current units:" + currentUnits.Count);
        if (currentUnits.Count != 0)
        {
            yield return coroutineStarter.StartCoroutine(MoveAllUnits()); //get startcoroutine from unit
        }
        Debug.Log("Basic method ended");
        yield return null;
    }
    
    private void ConstructUnit()
    {
        int warnbr = 0;
        int wiznbr = 0;
        foreach(Unit u in currentUnits)
        {
            if (u.GetType() == typeof(Warrior))
            {
                warnbr += 1;
            }else if(u.GetType() == typeof(Wizard))
            {
                wiznbr += 1;
            }
        }
    }

    private void UpdateUnitEffect()
    {
        Debug.Log("AI starts updating its units");
        int count = 0;
        foreach (Unit u in currentUnits)
        {
            if (u.currentHealth <= 0)
            {
                u.Death(AIcall: true);
                deactivatedUnits.Add(u);
                count += 1;
            }

        }
        Debug.Log("AI finished updating. " + count + " units dead found.");
        currentUnits.RemoveAll(u => u.currentHealth <= 0);
        Debug.Log("   Currents units :" + currentUnits.Count);
    }
    private IEnumerator MoveAllUnits()
    {
        Debug.Log("AI starts moving all units");
        foreach(Unit u in currentUnits)
        {
            if (u.currentHealth > 0) //sanity check
            {
                yield return u.StartCoroutine(MoveUnit(u));
            }
        }
        Debug.Log("AI finished moving units");
        yield return new WaitForSeconds(0.0f) ;
    }

    private IEnumerator MoveUnit(Unit u)
    {
        Debug.Log("AI starts moving a unit");
        Node target = DecideUnitMovement(u);
        Debug.Log("   Unit target decided");
        if (target != null)
        {
            u.ShowPotentialMove(target);
            Debug.Log("   Unit path found");
            CardDisplay.Instance.DisableCardDisplay();
            yield return u.StartCoroutine(u.StartMoving());
            Debug.Log("   Movement finished");
            CardDisplay.Instance.DisableCardDisplay();
            u.HidePotentialMove();
            u.HidePossibleMoves();
        }
        Debug.Log("AI move done.");
        yield return new WaitForSeconds(0.0f);
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
        else if(goable.Count!=0){
            target = DecideUnitMovementUnattackableNodes(goable);
        } 
        return target;
    }
    private Node DecideUnitMovementAttackableNodes(List<Node> attackables)
    {
        Dictionary<Node, int> priority = new Dictionary<Node, int>();
        foreach(Node n in attackables)
        {
            if (n.building != null && typeof(DefensiveBuilding).IsAssignableFrom(n.building.GetType())) {
                if (typeof(HallCenter).IsAssignableFrom(n.building.GetType()))
                {
                    priority.Add(n, 1000);
                }
                else
                {
                    priority.Add(n, ((DefensiveBuilding)n.building).currentHealth);
                }
            } else if (n.GetUnit() != null && n.GetUnit().GetType() == typeof(Wizard))
            {
                priority.Add(n, 100);
            }
            else
            {
                priority.Add(n, n.GetUnit().currentHealth);
            }
        }
        return priority.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;

    }

    private Node DecideUnitMovementUnattackableNodes(List<Node> goable)
    {
        Node target = null;
        float maxDistanceToHallCenterOnX = 0.0f;
        List<Node> goableVisible = new List<Node>();
        foreach (Node n in goable)
        {
            if (Player.player1.visibleNodes.Contains(n))
            {
                goableVisible.Add(n);
            }
        }
        if (goableVisible.Count > 0)
        {
            goable = goableVisible;
        }
        foreach (Node n in goable)
        {
            float distanceOnX = Math.Abs(n.position.x - currentBuildings[0].currentPosition.position.x);
            if (distanceOnX > maxDistanceToHallCenterOnX)
            {
                target = n;
                maxDistanceToHallCenterOnX = distanceOnX;
            }
        }
        if (target == null || (goableVisible.Count>0 && maxDistanceToHallCenterOnX>45 && (new System.Random().Next(0, 100)) <=20))
        {
            target = goable[(new System.Random()).Next(0, goable.Count)];
        }
        return target;
    }

    private IEnumerator PlaceUnit(Unit u)
    {
        Debug.Log("AI place unit");
        ConstructionManager.Instance.SetUnitToBuild(u);
        List<Node> selectables = GetSelectableNodes();
        if (selectables.Count > 0)
        {
            Node node = selectables[(new System.Random()).Next(0, selectables.Count)];
            bool instanceInDeactivatedUnits = false;
            foreach (Unit u_ in deactivatedUnits)
            {
                if (u.GetType() == u_.GetType())
                {
                    instanceInDeactivatedUnits = true;
                    u = u_;
                    Debug.Log("        Found a dead units to respawn");
                }
            }
            if (!instanceInDeactivatedUnits)
            {
                node.Construct(true);
                Debug.Log("   New units using constructor");
            }
            else
            {
                ConstructionManager.Instance.ResetConstruction();
                u.transform.position = node.transform.position + node.positionOffset;
                u.transform.rotation = node.transform.rotation;
                u.Setup();
                u.SetCurrentPosition(node);
                u.owner = this;
                currentUnits.Add(u);
                deactivatedUnits.Remove(u);
                gold -= u.goldCost;
                actionPoints -= u.actionPointCost;
                GetVisibleNodes();
                Debug.Log("   New units using respawn");

            }
            if (!TurnManager.Instance.inactivePlayer.visibleNodes.Contains(node))
            {
                node.SetVisible(false);
            }
        }
        Debug.Log("Unit placed.");
        yield return new WaitForSeconds(0.1f);
    }
    private IEnumerator PlaceBuilding(Building b)
    {
        Debug.Log("AI is placing a building.");
        ConstructionManager.Instance.SetBuildingToBuild(b);
        List<Node> selectables = GetSelectableNodes();
        if (selectables.Count > 0)
        {
            int randIdx = (new System.Random()).Next(0, selectables.Count);
            selectables[randIdx].Construct(true);
            if (!Player.player1.visibleNodes.Contains(selectables[randIdx]))
            {
                selectables[randIdx].SetVisible(false);
            }
        }
        Debug.Log("Building placed.");
        yield return new WaitForSeconds(0.1f);
    }

    private IEnumerator UpgradeToT2(Building b)
    {
        b.UpgradeToT2();
        yield return new WaitForSeconds(0.1f);
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
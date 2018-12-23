using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArtificialIntelligence : Player
{
    public bool turnFinished;
    private bool turnShouldBeFinished_;
    public bool turnShouldBeFinished { get
        {
            return turnShouldBeFinished_;
        }set{
            turnShouldBeFinished_ = value;
            TurnManager.Instance.debugCounter = 0;
        } }
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
        turnShouldBeFinished = false;
     //   GetVisibleHexCells();
        gold += 20;
        actionPoints += 5;
        foodPrediction -= 2;
        // UpdateUnitEffect();


        yield return coroutineStarter.StartCoroutine(BasicRush());
        coroutineStarter.StopCoroutine(BasicRush());


        //    UpdateUnitEffect();
        yield return new WaitForSeconds(1f);
        Debug.Log("AI ends the turn.");
            turnFinished = true;
            EndOfTurn();
        
        yield return new WaitForSeconds(0.0f);
    }
      public IEnumerator BasicRush()
      {
          if (turnNumber == 1)
          {
              PlaceBuilding(ConstructionManager.Instance.Barracks);
            
            //yield return coroutineStarter.StartCoroutine(PlaceBuilding(ConstructionManager.Instance.Barracks));
            //yield return coroutineStarter.StartCoroutine(PlaceBuilding(ConstructionManager.Instance.Barracks));
        }
          else if (turnNumber == 2)// || food < 1)
          {
            PlaceBuilding(ConstructionManager.Instance.WindMill);
          }
        PlaceUnit(ConstructionManager.Instance.SkeletonWarrior);
        PlaceUnit(ConstructionManager.Instance.Wizard);
        //yield return coroutineStarter.StartCoroutine(PlaceUnit(ConstructionManager.Instance.Warrior));

        //yield return coroutineStarter.StartCoroutine(PlaceUnit(ConstructionManager.Instance.Wizard));
        Debug.Log("Current units:" + currentUnits.Count);
          if (currentUnits.Count != 0)
          {
              yield return coroutineStarter.StartCoroutine(MoveAllUnits()); //get startcoroutine from unit
          }
        turnShouldBeFinished = true;
          Debug.Log("Basic method ended");
        //yield return new WaitForSeconds(0.1f);
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

      public void UpdateUnitEffect()
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
          turnShouldBeFinished = true;
          Debug.Log("AI finished moving units");
          yield return new WaitForSeconds(0.0f) ;
      }

      private IEnumerator MoveUnit(Unit u)
      {
          Debug.Log("AI starts moving a unit");
          HexCell target = DecideUnitMovement(u);
          Debug.Log("   Unit target decided");
          if (target != null)
          {
              u.SetPathVisible(target);
              Debug.Log("   Unit path found");
              CardDisplay.Instance.DisableCardDisplay();
              yield return u.StartCoroutine(u.MoveTo(target));
              Debug.Log("   Movement finished");
              CardDisplay.Instance.DisableCardDisplay();
          }
        u.ClearPossibleMoves(target);
        if (u.visible)
        {
            yield return new WaitForSeconds(0.9f);
        }
        Debug.Log("AI move done.");
    }
      private HexCell DecideUnitMovement(Unit u)
      {
          HexCell target = null;
          List<HexCell> attackables = new List<HexCell>();
          List<HexCell> goable = new List<HexCell>();
        List<HexCell> possibleMoves = new List<HexCell>();
        u.SearchAndShowPossibleMoves();
        HexCellPriorityQueue possibleMovesQ = new HexCellPriorityQueue() ;// = u.Search();
        possibleMovesQ.Enqueue(u.currentPosition);
        while (possibleMovesQ.Count > 0)
        {
            HexCell current = possibleMovesQ.Dequeue();
            foreach (HexCell cell in current.PathTo)
            {
                possibleMovesQ.Enqueue(cell);
            }
            possibleMoves.Add(current);
        }
        foreach (HexCell n in possibleMoves)
          {
              if (n.State == HexCell.STATE.UNIT_POSSIBLE_ATTACK)// n.Attackable(u.currentPosition))
              {
                  attackables.Add(n);
              }
              else if(n.State == HexCell.STATE.UNIT_POSSIBLE_PATH)
              {
                  goable.Add(n);
              }
          }
          if (attackables.Count != 0) {
              target = DecideUnitMovementAttackableHexCells(attackables, u);
          }
          else if(goable.Count!=0){
              target = DecideUnitMovementUnattackableHexCells(goable);
          }
        u.mouvementStartCell = u.currentPosition;
          return target;
      }
      private HexCell DecideUnitMovementAttackableHexCells(List<HexCell> attackables, Unit attacker)
      {
          Dictionary<HexCell, int> priority = new Dictionary<HexCell, int>();
          foreach(HexCell n in attackables)
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
              } 
              else
              {
                  //Prefer to kill unit (if possible without wasting damage)
                  //Then to attack ranged units (prioritize high range units)
                  //Then prioritize high damage units
                  int prio = 0;
                  bool killable = n.unit.currentHealth <= attacker.currentAttackModifier + attacker.attack;
                  bool ranged = n.unit.attackRange > 0;
                  if (killable)
                  {
                      prio += 200 + n.unit.currentHealth - attacker.currentAttackModifier - attacker.attack;
                  }
                  if (ranged)
                  {
                      prio += 100 + n.unit.attackRange;
                  }
                  prio += n.unit.attack + n.unit.currentAttackModifier;
                  priority.Add(n, prio);
              }
          }
          return priority.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;

      }

      private HexCell DecideUnitMovementUnattackableHexCells(List<HexCell> goable)
      {
          HexCell target = null;
          float distanceToEnemyHallCenter = 9000;
          
          foreach (HexCell n in goable)
          {
            float distanceOnX = n.coordinates.DistanceTo(TurnManager.Instance.inactivePlayer.currentBuildings[0].currentPosition.coordinates);//(n.Position.y - currentBuildings[0].currentPosition.Position.y);
            
              if (distanceOnX < distanceToEnemyHallCenter)
              {
                  target = n;
                  distanceToEnemyHallCenter = distanceOnX;
              }
          }
          if (target == null || (distanceToEnemyHallCenter>90000 && (new System.Random().Next(0, 100)) <=20))
          {
              target = goable[(new System.Random()).Next(0, goable.Count)];
          }
          return target;
      }

      private void PlaceUnit(Unit u)
      {
          Debug.Log("AI place unit");
          ConstructionManager.Instance.SetUnitToBuild(u);
          List<HexCell> selectables = GetSelectableHexCells();
          if (selectables.Count > 0)
          {
              HexCell node = selectables[(new System.Random()).Next(0, selectables.Count)];
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
                  u.transform.position = node.transform.position;
                  u.transform.rotation = node.transform.rotation;
                  u.Setup();
                //  u.SetCurrentPosition(node);
                  u.owner = this;
                  currentUnits.Add(u);
                  deactivatedUnits.Remove(u);
                  gold -= u.goldCost;
                  actionPoints -= u.actionPointCost;
                  Debug.Log("   New units using respawn");

              }
              if (!TurnManager.Instance.inactivePlayer.visibleNodes.Contains(node))
              {
                  node.SetVisible(false);
              }
          }
          Debug.Log("Unit placed.");
      }
      private void PlaceBuilding(Building b)
      {
          Debug.Log("AI is placing a building.");
          ConstructionManager.Instance.SetBuildingToBuild(b);
          List<HexCell> selectables = GetSelectableHexCells();
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
      }

      private IEnumerator UpgradeToT2(Building b)
      {
          b.UpgradeToT2();
          yield return new WaitForSeconds(0.1f);
      }

      private List<HexCell> GetSelectableHexCells()
      {
          List<HexCell> selectables = new List<HexCell>();
          //Debug.Log("visibles" + visibleHexCells.Count.ToString());
          foreach (HexCell n in visibleNodes)
          {
              if (n.State == HexCell.STATE.CONSTRUCT_SELECTABLE)
              {
                  selectables.Add(n);
              }
          }
          //Debug.Log("selectable" + visibleHexCells.Count.ToString());
          return selectables;
      }

      public void EndOfTurn()
      {
          TurnManager.Instance.EndOfAITurn();
      }
    
    
}
using System.Collections.Generic;
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
        availablePositions = new List<HexCell>();
    }
    [Header("Building prefabs")]
    public Building Barracks;
    public Building HallCenter;
    public Building WindMill;
    //public Building MagicCenter;

    [Header("Unit prefabs for IA")]
    public Unit Warrior;
    public Unit Wizard;
    public Unit SkeletonWarrior;
    

    private Building buildingToConstruct;
    private Unit unitToConstruct;
    private Spell spellToConstruct;
    public string mode;
    public bool canConstruct;
    private List<HexCell> availablePositions;

    public void SetBuildingToBuild(Building building, bool hallCenter=false, Player owner = null)
    {
        if (owner == null)
        {
            owner = TurnManager.Instance.currentPlayer;
        }
        ResetConstruction();
        Selector.Instance.Unselect();
        mode = "building";
        buildingToConstruct = building;
        canConstruct = true;
        if (!hallCenter)
        {
            Selector.Instance.currentObject = (building);
            building.UpdateCardDisplayInfo();
            foreach(Building b in owner.currentBuildings)
            {
                foreach(HexDirection direction in HexDirectionExtensions.AllDirections())
                {
                    HexCell potential = b.currentPosition.GetNeighbor(direction);
                    if (potential != null) {
                        potential =potential.GetNeighbor(direction);
                        if (potential != null && potential.IsFree(owner))
                        {
                            potential.State = HexCell.STATE.CONSTRUCT_SELECTABLE;
                            availablePositions.Add(potential);
                        }
                    }
                }
            }
            
        }
    }

    public Building ConstructBuilding(HexCell target, Player owner = null)
    {
        if (owner == null)
        {
            owner = TurnManager.Instance.currentPlayer;
        }
        GameObject prefab = buildingToConstruct.prefab;
        GameObject buildingObject = (GameObject)Instantiate(prefab, target.Position, target.transform.rotation);

        Building building = (Building)GetScript(buildingObject);
        building.owner = owner;
        building.SetCurrentPosition(target);
        owner.currentBuildings.Add(building);
        owner.requirementSystem.AddCopy(building.GetType());
        owner.gold -= building.goldCost;
        owner.actionPoints -= building.actionPointCost;
        HexGrid.Instance.IncreaseVisibility(building.currentPosition, building.visionRange, building.owner);

        foreach (HexCell cell in availablePositions)
        {
            cell.State = HexCell.STATE.IDLE;
        }
        Selector.Instance.currentObject = null;
        if (!TurnManager.Instance.againstAI || !owner.Equals(Player.Player2))
        {
            if (CardDisplay.Instance != null) { CardDisplay.Instance.DisableCardDisplay(); } //sanity check because hallcenter spawn is manually made.
            TurnManager.Instance.ButtonUpdateSubject.NotifyObservers(owner);
            building.SetVisible(true);
        }
        availablePositions = new List<HexCell>();
        return building;
    }

    public void SetSpellToConstruct(Spell spell, Player owner = null)
    {
        if (owner == null)
        {
            owner = TurnManager.Instance.currentPlayer;
        }
        ResetConstruction();
        mode = "spell";
        spellToConstruct = spell;

        spell.owner = owner;
        Selector.Instance.Select(spellToConstruct);
        canConstruct = true;
    }

    public void SpellNodeSelected(HexCell node)
    {
        spellToConstruct.GetSpellEffectNodes(node);
    }

    public Spell ConstructSpell(HexCell node, Player owner = null)
    {
        if (owner == null)
        {
            owner = TurnManager.Instance.currentPlayer;
        }
        spellToConstruct.playerInfos[owner].position = node;
        spellToConstruct.playerInfos[owner].owner = owner;
        owner.mana -= spellToConstruct.manaCost;
        owner.actionPoints -= spellToConstruct.actionPointCost;
        spellToConstruct.Activate(spellToConstruct.affectedNodes);
        spellToConstruct.PlayAnimation();
        TurnManager.Instance.ButtonUpdateSubject.NotifyObservers(owner);

        return spellToConstruct;
    }

    public void SetUnitToBuild(Unit unit, Player owner = null)
    {
        if (owner == null)
        {
            owner = TurnManager.Instance.currentPlayer;
        }
        ResetConstruction();
        availablePositions = new List<HexCell>();
        Selector.Instance.Unselect();
        mode = "unit";
        unitToConstruct = unit;
        canConstruct = true;
        Selector.Instance.currentObject = (unit);
        unit.UpdateCardDisplayInfo();
        Selectable spawnPoint = owner.GetSelectableFromType(unit.GetSpawnPoint());
        foreach(HexCell possibleSpawn in spawnPoint.currentPosition.GetNeighbors())
        {
            if (possibleSpawn.IsFree(owner))
            {
                possibleSpawn.State = HexCell.STATE.CONSTRUCT_SELECTABLE;
                availablePositions.Add(possibleSpawn);
            }
        }

    }

    public Unit ConstructUnit(HexCell target, Player owner = null)
    {
        if (owner == null)
        {
            owner = TurnManager.Instance.currentPlayer;
        }
        GameObject prefab = unitToConstruct.prefab;
        GameObject unitObject = (GameObject)Instantiate(prefab, target.Position, target.transform.rotation);

        Unit unit = (Unit)ConstructionManager.Instance.GetScript(unitObject);
        unit.Setup();

        unit.owner = owner;
        unit.SetCurrentPosition(target);
        owner.currentUnits.Add(unit);
        owner.gold -= unit.goldCost;
        owner.actionPoints -= unit.actionPointCost;
        HexGrid.Instance.IncreaseVisibility(unit.currentPosition, unit.visionRange + unit.currentVisionRangeModifier, unit.owner);
        foreach(HexCell n in availablePositions)
        {
            n.State = HexCell.STATE.IDLE;
        }
        Selector.Instance.currentObject = null;
        if (!TurnManager.Instance.againstAI || !owner.Equals(Player.Player2))
        {
            CardDisplay.Instance.DisableCardDisplay();
            TurnManager.Instance.ButtonUpdateSubject.NotifyObservers(owner);
        }
        availablePositions = new List<HexCell>();
        return unit;
    }

    public void ResetConstruction()
    {
        buildingToConstruct = null;
        unitToConstruct = null;
        spellToConstruct = null;
        canConstruct = false;
        foreach (HexCell n in availablePositions)
        {
            n.State = HexCell.STATE.IDLE;
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

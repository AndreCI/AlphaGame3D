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
            foreach (HexCell n in TurnManager.Instance.currentPlayer.currentBuildings[0].currentPosition.GetNeighbors())
            
            {
                if (n.IsFree)
                {
                    n.State = HexCell.STATE.CONSTRUCT_SELECTABLE;
                    availablePositions.Add(n);
                }
            }
        }
    }

    public Building ConstructBuilding(HexCell target)
    {
        GameObject prefab = buildingToConstruct.prefab;
        GameObject buildingObject = (GameObject)Instantiate(prefab, target.Position, target.transform.rotation);

        Building building = (Building)GetScript(buildingObject);
        building.owner = TurnManager.Instance.currentPlayer;
        building.SetCurrentPosition(target);
        TurnManager.Instance.currentPlayer.currentBuildings.Add(building);
        TurnManager.Instance.currentPlayer.requirementSystem.AddCopy(building.GetType());
        TurnManager.Instance.currentPlayer.gold -= building.goldCost;
        TurnManager.Instance.currentPlayer.actionPoints -= building.actionPointCost;
        HexGrid.Instance.IncreaseVisibility(building.currentPosition, building.visionRange, building.owner);

        foreach (HexCell cell in availablePositions)
        {
            cell.State = HexCell.STATE.IDLE;
        }
        Selector.Instance.currentObject = null;
        if (!TurnManager.Instance.againstAI || !TurnManager.Instance.currentPlayer.Equals(Player.Player2))
        {
            if (CardDisplay.Instance != null) { CardDisplay.Instance.DisableCardDisplay(); } //sanity check because hallcenter spawn is manually made.
            TurnManager.Instance.ButtonUpdateSubject.NotifyObservers(TurnManager.Instance.currentPlayer);
            building.SetVisible(true);
        }
        availablePositions = new List<HexCell>();
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

    public void SpellNodeSelected(HexCell node)
    {
        spellToConstruct.GetSpellEffectNodes(node);
    }

    public Spell ConstructSpell(HexCell node)
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
        availablePositions = new List<HexCell>();
        Selector.Instance.Unselect();
        mode = "unit";
        unitToConstruct = unit;
        canConstruct = true;
        Selector.Instance.currentObject = (unit);
        unit.UpdateCardDisplayInfo();
        Selectable spawnPoint = TurnManager.Instance.currentPlayer.GetSelectableFromType(unit.GetSpawnPoint());
        foreach(HexCell possibleSpawn in spawnPoint.currentPosition.GetNeighbors())
        {
            if (possibleSpawn.IsFree)
            {
                possibleSpawn.State = HexCell.STATE.CONSTRUCT_SELECTABLE;
                availablePositions.Add(possibleSpawn);
            }
        }

    }

    public Unit ConstructUnit(HexCell target)
    {
        GameObject prefab = unitToConstruct.prefab;
        GameObject unitObject = (GameObject)Instantiate(prefab, target.Position, target.transform.rotation);

        Unit unit = (Unit)ConstructionManager.Instance.GetScript(unitObject);
        unit.Setup();

        unit.owner = TurnManager.Instance.currentPlayer;
        unit.SetCurrentPosition(target);
        TurnManager.Instance.currentPlayer.currentUnits.Add(unit);
        TurnManager.Instance.currentPlayer.gold -= unit.goldCost;
        TurnManager.Instance.currentPlayer.actionPoints -= unit.actionPointCost;
        HexGrid.Instance.IncreaseVisibility(unit.currentPosition, unit.visionRange + unit.currentVisionRangeModifier, unit.owner);
        foreach(HexCell n in availablePositions)
        {
            n.State = HexCell.STATE.IDLE;
        }
        Selector.Instance.currentObject = null;
        if (!TurnManager.Instance.againstAI || !TurnManager.Instance.currentPlayer.Equals(Player.Player2))
        {
            CardDisplay.Instance.DisableCardDisplay();
            TurnManager.Instance.ButtonUpdateSubject.NotifyObservers(TurnManager.Instance.currentPlayer);
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

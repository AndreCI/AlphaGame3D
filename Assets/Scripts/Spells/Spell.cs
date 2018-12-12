using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class Spell : Selectable
{
    public GameObject prefab;
    public Sprite sprite;
    [Header("General spell attributes")]
    public SpellUtils.SchoolOfMagic schoolOfMagic;
    public int requirementLevel;
    public string effectDescription;
    public int cooldown;
    public int currentCooldown;
    public int areaOfEffect;
    public int castableRange;
    public int damage;
    protected List<HexCell> castableNodes;
    public List<HexCell> affectedNodes;
    public abstract void PlayAnimation();
    private SpellButtonScript spellButton;
    protected int searchPhaseFrontier;
    private HexCellPriorityQueue searchFrontier;
    public Dictionary<Player, PlayerInfo> playerInfos;
    public List<EffectFactory> effects;

    public void AwakeBase()
    {
        UpdatePlayerInfos();
        effects = new List<EffectFactory>();
    }
    public void UpdatePlayerInfos()
    {
        if (playerInfos == null)
        {
            playerInfos = new Dictionary<Player, PlayerInfo>
            {
                { Player.Player1, new PlayerInfo() },
                { Player.Player2, new PlayerInfo() }
            };
            TurnManager.Instance.StartTurnSubject.AddObserver(this);
        }
    }

    public override void Notify(Player player, TurnSubject.NOTIFICATION_TYPE type)
    {
        if (playerInfos[player].currentCooldown > 0)
        {
            if (type == TurnSubject.NOTIFICATION_TYPE.START_OF_TURN)
            {
                playerInfos[player].currentCooldown -= 1;
            }
        }
    }

    public virtual void Activate(List<HexCell> affectedNodes_)
    {
        Selector.Instance.Unselect();
        playerInfos[TurnManager.Instance.currentPlayer].currentCooldown = cooldown;
        UpdateCooldown();
    }

    protected virtual void ApplyEffectsToUnit(Unit u)
    {
        foreach(EffectFactory factory in effects)
        {
            UnitEffect ue = factory.GetEffect(u);
            if (ue.applyOnTouch)
            {
                ue.ApplyEffect();
            }
            if (ue.duration > 0)
            {
                u.currentEffect.Add(ue);
            }
        }
    }

    public override void Select()
    {
        UpdateCardDisplayInfo();
        GetCastableNodes();
    }

    public override void Unselect()
    {
        if (affectedNodes != null)
        {
            foreach (HexCell node in affectedNodes)
            {
                if (node != null)
                {
                    node.State = HexCell.STATE.IDLE;
                }
            }
        }
        if (castableNodes != null)
        {
            foreach (HexCell node in castableNodes)
            {

                node.State = HexCell.STATE.IDLE;
            }
        }
        CardDisplay.Instance.DisableCardDisplay();
    }

    public List<string> GetKeywordsDescription()
    {
        List<string> descriptions = new List<string>();
        {
            foreach(EffectFactory factory in effects)
            {
                descriptions.Add(factory.GetAbsoluteDescriptions());
            }
        }
        return descriptions;
    }

    public override void UpdateCardDisplayInfo()
    {
        string keywordsDescription = "";
        foreach(string effect in GetKeywordsDescription())
        {
            keywordsDescription += effect + "\n";
        }
        TextMeshProUGUI[] elem = CardDisplay.Instance.EnableSpellCardDisplay(sprite, schoolOfMagic, keywordsDescription);
        foreach (TextMeshProUGUI e in elem)
        {
            switch (e.name)
            {
                case "CardNameText":
                    e.text = cardName;
                    break;
                case "CardCostText":
                    e.text = manaCost.ToString();
                    break;
                case "CardEffectText":
                    e.text = effectDescription;
                    break;
                case "CardCooldownText":
                    e.text = cooldown.ToString();
                    break;
                case "CardRequirementText":
                    e.text = requirementLevel.ToString();
                    break;
                case "CardCastablerangeText":
                    e.text = castableRange.ToString();
                    break;

            }
        }
    }
    public void RegisterButton(SpellButtonScript b)
    {
        spellButton = b;
    }
    private void UpdateCooldown()
    {
        if(spellButton != null)
        {
            spellButton.UpdateInfo();
        }
    }

    public void GetSpellEffectNodes(HexCell source)
    {
        
        affectedNodes = new List<HexCell>();
        foreach (HexCell node in castableNodes)
        {
            node.State = HexCell.STATE.IDLE;
        }
        source.State = HexCell.STATE.SPELL_CURRENT_CAST;
        affectedNodes.Add(source);
        foreach (HexCell node in Search(source, areaOfEffect))
        {
            node.State = HexCell.STATE.SPELL_AFFECTED;
            affectedNodes.Add(node);
        }
        source.State = HexCell.STATE.SPELL_CURRENT_CAST;
        castableNodes = new List<HexCell>();
    }

    protected virtual void GetCastableNodes()
    {
        castableNodes = new List<HexCell>();
        List<Unit> currentUnits = TurnManager.Instance.currentPlayer.currentUnits;
        List<Building> currentBuildings = TurnManager.Instance.currentPlayer.currentBuildings;
        
        foreach (Unit unit in currentUnits)
        {
            castableNodes.AddRange(Search(unit.currentPosition, castableRange));
        }
        foreach (Building buiding in currentBuildings)
        {
            castableNodes.AddRange(Search(buiding.currentPosition, castableRange));
        }
        foreach (HexCell n in castableNodes)
        {
            n.State = HexCell.STATE.SPELL_POSSIBLE_CAST;
        }
    }

    private List<HexCell> Search(HexCell start, int range)
    //Should search using efficient algo (A* and stuff) and display all possible moves for the current unit.
    //Update the state of the node and unpdate searchFrontier in order to have all possible moves.
    {
        List<HexCell> castable = new List<HexCell>();
        HexGrid.Instance.searchFrontierPhase += 2; //Making it +2 allows us to not have to reset this property.
        if (searchFrontier == null)
        {
            searchFrontier = new HexCellPriorityQueue();
        }
        else
        {
            searchFrontier.Clear();
        }

        start.SearchPhase = HexGrid.Instance.searchFrontierPhase;
        start.Distance = 0;
        searchFrontier.Enqueue(start);
        while (searchFrontier.Count > 0)
        {
            HexCell current = searchFrontier.Dequeue();
            current.SearchPhase += 1;
            if (current.Distance < range)
            {
                for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
                {
                    HexCell neighbor = current.GetNeighbor(d);
                    if (
                        neighbor == null ||
                        neighbor.SearchPhase > HexGrid.Instance.searchFrontierPhase
                    )
                    {
                        continue;
                    }
                    if (!IsValidCell(neighbor))
                    {
                        continue;
                    }
                    int moveCost = GetRangeCost(current, neighbor, d);
                    if (moveCost < 0)
                    {
                        continue;
                    }

                    int distance = current.Distance + moveCost;
                    if (neighbor.SearchPhase < HexGrid.Instance.searchFrontierPhase)
                    {
                        neighbor.SearchPhase = HexGrid.Instance.searchFrontierPhase;
                        neighbor.Distance = distance;
                        neighbor.PathFrom = current;
                        current.PathTo.Add(neighbor);
                        neighbor.SearchHeuristic = 0;
                        castable.Add(neighbor);
                        // neighbor.coordinates.DistanceTo(toCell.coordinates);
                        searchFrontier.Enqueue(neighbor);
                    }
                    else if (distance < neighbor.Distance)
                    {
                        int oldPriority = neighbor.SearchPriority;
                        neighbor.Distance = distance;
                        neighbor.PathFrom.PathTo.Remove(neighbor);
                        neighbor.PathFrom = current;
                        current.PathTo.Add(neighbor);
                        searchFrontier.Change(neighbor, oldPriority);
                    }

                }
            }
        }
        return castable;
    }

    public bool IsValidCell(HexCell cell)
    {
        return cell.IsExplored && !cell.IsUnderwater;
    }
    public int GetRangeCost(
        HexCell fromCell, HexCell toCell, HexDirection direction)
    {
        if (!IsValidCell(toCell))
        {
            return -1;
        }
        HexEdgeType edgeType = fromCell.GetEdgeType(toCell);
        if (edgeType == HexEdgeType.Cliff && fromCell.Elevation < toCell.Elevation)
        {
            return -1;
        }
        int moveCost;
        if (fromCell.Walled != toCell.Walled)
        {
            return -1;
        }
        else
        {
            //	moveCost = edgeType == HexEdgeType.Flat ? 5 : 10;
            //	moveCost +=
            //		toCell.UrbanLevel + toCell.FarmLevel + toCell.PlantLevel;
            moveCost = 1;
        }
        return moveCost;
    }

    public class PlayerInfo
    {
        public int currentCooldown;
        public Player owner;
        public HexCell position;

        public PlayerInfo()
        {
            currentCooldown = 0;
            owner = null;
            position = null;
        }
    }
}

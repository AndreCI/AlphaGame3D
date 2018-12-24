using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

public class HexCell : MonoBehaviour, IObserver {
    /* IDLE: default, nothing happening
 * UNIT_POSSIBLE_PATH: this hex can be walked on by the currently selected unit
 * UNIT_CURRENT_PATH: this hex is on the current path of the currently selected unit
 * UNIT_POSSIBLE_ATTACK: this hex contains a unit which can be attacked by the currently selected unit
 * UNIT_CURRENT_ATTACK: this hex contains a unit which can be attacked by the currently selected unit and
 *                      this hex is on the current path
 * CONSTRUCT_SELECTABLE: this hex can be constructed upon
 * SPELL_POSSIBLE_CAST: a spell can be casted on this hex
 * SPELL_AFFECTED: a spell will affect this hex if cast
 * SPELL_CURRENT_CAST: a spell will be cast on this hex if revalidate
 */
    public enum STATE
    {
        IDLE,
        UNIT_POSSIBLE_PATH,
        UNIT_CURRENT_PATH,
        UNIT_POSSIBLE_ATTACK,
        UNIT_CURRENT_ATTACK,
        UNIT_RANGE_ATTACK,
        CONSTRUCT_SELECTABLE,
        SPELL_POSSIBLE_CAST,//: a spell can be casted on this hex
        SPELL_AFFECTED,//: a spell will affect this hex if cast
        SPELL_CURRENT_CAST,//: a spell will be cast on this hex if revalidate
    }


    public HexCoordinates coordinates;
    float clicked = 0;
    float clicktime = 0;
    float clickdelay = 0.5f;
    public RectTransform uiRect;

	public HexGridChunk chunk;

    private STATE state;
    public STATE State {
        get{
            return state;
        }
        set {
            switch (value)
            {
                case STATE.IDLE:
                    PathTo = null;
                    PathFrom = null;
                    SearchRange = false;
                    OnAttackPath = false;
                    RangeDistance = 0;
                    DisableHighlight();
                    break;
                case STATE.UNIT_CURRENT_PATH:
                    EnableHighlight(Color.green);
                    break;
                case STATE.UNIT_POSSIBLE_PATH:
                    EnableHighlight(Color.white);
                    break;
                case STATE.UNIT_CURRENT_ATTACK:
                    EnableHighlight(Color.red);
                    break;
                case STATE.UNIT_POSSIBLE_ATTACK:
                    EnableHighlight(Color.gray);
                    break;
                case STATE.UNIT_RANGE_ATTACK:
                    break;
                case STATE.CONSTRUCT_SELECTABLE:
                    EnableHighlight(Color.white);
                    break;
                case STATE.SPELL_AFFECTED:
                    EnableHighlight(Color.cyan);
                    break;
                case STATE.SPELL_CURRENT_CAST:
                    EnableHighlight(Color.red);
                    break;
                case STATE.SPELL_POSSIBLE_CAST:
                    EnableHighlight(Color.white);
                    break;
            }
            state = value;
        }
    }

	public int Index { get; set; }

	public int ColumnIndex { get; set; }

	public int Elevation {
		get {
			return elevation;
		}
		set {
			if (elevation == value) {
				return;
			}
			int originalViewElevation = ViewElevation;
			elevation = value;
			if (ViewElevation != originalViewElevation) {
				ShaderData.ViewElevationChanged();
			}
			RefreshPosition();
			ValidateRivers();

			

			Refresh();
		}
	}

	public int WaterLevel {
		get {
			return waterLevel;
		}
		set {
			if (waterLevel == value) {
				return;
			}
			int originalViewElevation = ViewElevation;
			waterLevel = value;
			if (ViewElevation != originalViewElevation) {
				ShaderData.ViewElevationChanged();
			}
			ValidateRivers();
			Refresh();
		}
	}

	public int ViewElevation {
		get {
			return elevation >= waterLevel ? elevation : waterLevel;
		}
	}

	public bool IsUnderwater {
		get {
			return waterLevel > elevation;
		}
	}

	public bool HasIncomingRiver {
		get {
			return hasIncomingRiver;
		}
	}

	public bool HasOutgoingRiver {
		get {
			return hasOutgoingRiver;
		}
	}

	public bool HasRiver {
		get {
			return hasIncomingRiver || hasOutgoingRiver;
		}
	}

	public bool HasRiverBeginOrEnd {
		get {
			return hasIncomingRiver != hasOutgoingRiver;
		}
	}

	public HexDirection RiverBeginOrEndDirection {
		get {
			return hasIncomingRiver ? incomingRiver : outgoingRiver;
		}
	}

	

	public HexDirection IncomingRiver {
		get {
			return incomingRiver;
		}
	}

	public HexDirection OutgoingRiver {
		get {
			return outgoingRiver;
		}
	}

	public Vector3 Position {
		get {
			return transform.localPosition;
		}
	}


	public float StreamBedY {
		get {
			return
				(elevation + HexMetrics.streamBedElevationOffset) *
				HexMetrics.elevationStep;
		}
	}

	public float RiverSurfaceY {
		get {
			return
				(elevation + HexMetrics.waterElevationOffset) *
				HexMetrics.elevationStep;
		}
	}

	public float WaterSurfaceY {
		get {
			return
				(waterLevel + HexMetrics.waterElevationOffset) *
				HexMetrics.elevationStep;
		}
	}

	public int UrbanLevel {
		get {
			return urbanLevel;
		}
		set {
			if (urbanLevel != value) {
				urbanLevel = value;
				RefreshSelfOnly();
			}
		}
	}

	public int FarmLevel {
		get {
			return farmLevel;
		}
		set {
			if (farmLevel != value) {
				farmLevel = value;
				RefreshSelfOnly();
			}
		}
	}

	public int PlantLevel {
		get {
			return plantLevel;
		}
		set {
			if (plantLevel != value) {
				plantLevel = value;
				RefreshSelfOnly();
			}
		}
	}

	public int SpecialIndex {
		get {
			return specialIndex;
		}
		set {
			if (specialIndex != value && !HasRiver) {
				specialIndex = value;
				RefreshSelfOnly();
			}
		}
	}

	public bool IsSpecial {
		get {
			return specialIndex > 0;
		}
	}

    public bool IsFree(Player player)
    {
            return !unit && !building && !IsUnderwater && IsExplored(player);
        
    }

	public bool Walled {
		get {
			return walled;
		}
		set {
			if (walled != value) {
				walled = value;
				Refresh();
			}
		}
	}

	public int TerrainTypeIndex {
		get {
			return terrainTypeIndex;
		}
		set {
			if (terrainTypeIndex != value) {
				terrainTypeIndex = value;
				ShaderData.RefreshTerrain(this);
			}
		}
	}

	public bool IsVisible(Player player) {
            if (player.Equals(Player.Player1)) {
                return visibility_p1 > 0 && Explorable;
            } else {
                return visibility_p2 > 0 && Explorable;
            }
		
	}

    public bool IsExplored(Player player) {

        if (player.Equals(Player.Player1))
        {
            return explored_p1 && Explorable;
        }
        else
        {
            return explored_p2 && Explorable;
        }
    }
    private void SetExplored(Player player, bool value) {
            if (player.Equals(Player.Player1))
            {
                explored_p1 = value;
            }
            else
            {
                explored_p2 = value;
            }
        
	}

	public bool Explorable { get; set; }

    public int visionDistance;

	public int Distance {
		get {
			return distance;
		}
		set {
			distance = value;
		}
	}
    public void Construct(bool makeUnwalkable = true, Player owner=null)
    {
        if (ConstructionManager.Instance.mode == "building")
        {
            building = ConstructionManager.Instance.ConstructBuilding(this, owner);
        }
        else if (ConstructionManager.Instance.mode == "unit")
        {
            unit = ConstructionManager.Instance.ConstructUnit(this, owner);
        }
        else if (ConstructionManager.Instance.mode == "spell")
        {
            ConstructionManager.Instance.ConstructSpell(this, owner);
        }
        ConstructionManager.Instance.ResetConstruction();
    }
    private Unit unit_;
    public Unit unit { get {
            return unit_;
        } set {
            if (value != null)
            {
                if (TurnManager.Instance.againstAI)
                {
                    value.SetVisible(visibility_p1 > 0);
                }
            }
            unit_ = value;
        } }
    private Building building_;
    public Building building { get; set; }
     /*       return building_;
        }
        set {
            if (value != null)
            {
                HexGrid.Instance.IncreaseVisibility(this, value.visionRange, building.owner);// + value.currentVisionRangeModifier);
            }
            else {
                HexGrid.Instance.DecreaseVisibility(this, building_.visionRange, building.owner);
            }
            building_ = value;
            
        } }*/

    public HexCell VisionPathFrom { get; set; }
	public HexCell PathFrom { get; set; }
    public HexCell PathFromRanged { get; set; }

    private List<HexCell> pathToRange;
    public List<HexCell> PathToRange
    {
        get
        {
            if (pathToRange == null)
            {
                pathToRange = new List<HexCell>();
            }
            return pathToRange;
        }
        set
        {
            pathToRange = value;
        }
    }
    private List<HexCell> pathTo;
    public List<HexCell> PathTo { get
        {
            if (pathTo == null)
            {
                pathTo = new List<HexCell>();
            }return pathTo;
        }
        set
        {
            pathTo = value;
        }
    }

	public int SearchHeuristic { get; set; }

	public int SearchPriority {
		get {
			return distance + SearchHeuristic;
		}
	}

    public int VisionSearchPhase;

	public int SearchPhase { get; set; }
    public bool SearchRange { get; set; }
    public bool OnAttackPath { get; set; }
    public int RangeDistance { get; set; }

	public HexCell NextWithSamePriority { get; set; }

	public HexCellShaderData ShaderData { get; set; }

	int terrainTypeIndex;

	int elevation = int.MinValue;
	int waterLevel;

	int urbanLevel, farmLevel, plantLevel;

	int specialIndex;

	int distance;

    public int visibility_p1;
    int visibility_p2;

    bool explored_p1;
    bool explored_p2;

	bool walled;


	bool hasIncomingRiver, hasOutgoingRiver;
	HexDirection incomingRiver, outgoingRiver;

	[SerializeField]
	HexCell[] neighbors;
    
    public void SetVisible(bool v)
    {
        if (building)
        {
            building.SetVisible(v);
        }
        if (unit)
        {
            unit.SetVisible(v);
        }
    }

	public void IncreaseVisibility (Player player)
    {
        if (player.Equals(Player.Player1))
        {
            visibility_p1 += 1;
            if (visibility_p1 == 1)
            {
                Player.Player1.visibleNodes.Add(this);
                SetExplored(player, true);
                ShaderData.RefreshVisibility(this, player);
                if (TurnManager.Instance.againstAI)
                {
                    if (unit)
                    {
                        unit.SetVisible(true);
                    }
                    if (building)
                    {
                        building.SetVisible(true);
                    }
                }
            }
        }
        else
        {
            visibility_p2 += 1;
            if (visibility_p2 == 1)
            {
                Player.Player2.visibleNodes.Add(this);
                SetExplored(player, true);
                if (!TurnManager.Instance.againstAI)
                {
                    ShaderData.RefreshVisibility(this, player);
                }
            }

        }
	}
    
	public void DecreaseVisibility (Player player) {
        if (player.Equals(Player.Player1))
        {
            visibility_p1 -= 1;
          //  SetLabel(visibility_p1.ToString());
            if (visibility_p1 == 0)
            {
                Player.Player1.visibleNodes.Remove(this);
                ShaderData.RefreshVisibility(this, player);
                if (unit)
                {
                    unit.SetVisible(false);
                }
            }
        }
        else
        {
            visibility_p2 -= 1;
            if (visibility_p2 == 0)
            {
                Player.Player2.visibleNodes.Remove(this);
                if (!TurnManager.Instance.againstAI)
                {
                    ShaderData.RefreshVisibility(this, player);
                }
            }
        }
	}

	public void ResetVisibility (Player player) {

        if (player.Equals(Player.Player1))
        {
            if (visibility_p1 > 0)
            {
                Player.Player1.visibleNodes.Remove(this);
                visibility_p1 = 0;
                ShaderData.RefreshVisibility(this, player);
            }
        }
        else
        {
            if (visibility_p2 > 0)
            {
                Player.Player2.visibleNodes.Remove(this);
                visibility_p2 = 0;
                if (!TurnManager.Instance.againstAI)
                {
                    ShaderData.RefreshVisibility(this, player);
                }
            }
        }
       // SetLabel(visibility_p1.ToString());
    }

    public void Damage(int amount)
    {
        if (unit != null)
        {
            unit.TakesDamage(amount);
        }
        else if (building != null && typeof(DefensiveBuilding).IsAssignableFrom(building.GetType()))
        {
            ((DefensiveBuilding)building).TakeDamage(amount);
        }
    }

    public List<HexCell> GetNeighbors()
    {
        List<HexCell> n = new List<HexCell>();
        n.AddRange(neighbors);
        return n;
    }

	public HexCell GetNeighbor (HexDirection direction) {
        if (neighbors.Length >= (int)direction)
        {
            return neighbors[(int)direction];
        }
        else
        {
            return null;
        }
	}

	public void SetNeighbor (HexDirection direction, HexCell cell) {
		neighbors[(int)direction] = cell;
		cell.neighbors[(int)direction.Opposite()] = this;
	}

	public HexEdgeType GetEdgeType (HexDirection direction) {
		return HexMetrics.GetEdgeType(
			elevation, neighbors[(int)direction].elevation
		);
	}

	public HexEdgeType GetEdgeType (HexCell otherCell) {
		return HexMetrics.GetEdgeType(
			elevation, otherCell.elevation
		);
	}

	public bool HasRiverThroughEdge (HexDirection direction) {
		return
			hasIncomingRiver && incomingRiver == direction ||
			hasOutgoingRiver && outgoingRiver == direction;
	}

	public void RemoveIncomingRiver () {
		if (!hasIncomingRiver) {
			return;
		}
		hasIncomingRiver = false;
		RefreshSelfOnly();

		HexCell neighbor = GetNeighbor(incomingRiver);
		neighbor.hasOutgoingRiver = false;
		neighbor.RefreshSelfOnly();
	}

	public void RemoveOutgoingRiver () {
		if (!hasOutgoingRiver) {
			return;
		}
		hasOutgoingRiver = false;
		RefreshSelfOnly();

		HexCell neighbor = GetNeighbor(outgoingRiver);
		neighbor.hasIncomingRiver = false;
		neighbor.RefreshSelfOnly();
	}

	public void RemoveRiver () {
		RemoveOutgoingRiver();
		RemoveIncomingRiver();
	}

	public void SetOutgoingRiver (HexDirection direction) {
		if (hasOutgoingRiver && outgoingRiver == direction) {
			return;
		}

		HexCell neighbor = GetNeighbor(direction);
		if (!IsValidRiverDestination(neighbor)) {
			return;
		}

		RemoveOutgoingRiver();
		if (hasIncomingRiver && incomingRiver == direction) {
			RemoveIncomingRiver();
		}
		hasOutgoingRiver = true;
		outgoingRiver = direction;
		specialIndex = 0;

		neighbor.RemoveIncomingRiver();
		neighbor.hasIncomingRiver = true;
		neighbor.incomingRiver = direction.Opposite();
		neighbor.specialIndex = 0;

	}

    

	public int GetElevationDifference (HexDirection direction) {
		int difference = elevation - GetNeighbor(direction).elevation;
		return difference >= 0 ? difference : -difference;
	}

	bool IsValidRiverDestination (HexCell neighbor) {
		return neighbor && (
			elevation >= neighbor.elevation || waterLevel == neighbor.elevation
		);
	}

	void ValidateRivers () {
		if (
			hasOutgoingRiver &&
			!IsValidRiverDestination(GetNeighbor(outgoingRiver))
		) {
			RemoveOutgoingRiver();
		}
		if (
			hasIncomingRiver &&
			!GetNeighbor(incomingRiver).IsValidRiverDestination(this)
		) {
			RemoveIncomingRiver();
		}
	}



	void RefreshPosition () {
		Vector3 position = transform.localPosition;
		position.y = elevation * HexMetrics.elevationStep;
		position.y +=
			(HexMetrics.SampleNoise(position).y * 2f - 1f) *
			HexMetrics.elevationPerturbStrength;
		transform.localPosition = position;

		Vector3 uiPosition = uiRect.localPosition;
		uiPosition.z = -position.y;
		uiRect.localPosition = uiPosition;
	}

	void Refresh () {
		if (chunk) {
			chunk.Refresh();
			for (int i = 0; i < neighbors.Length; i++) {
				HexCell neighbor = neighbors[i];
				if (neighbor != null && neighbor.chunk != chunk) {
					neighbor.chunk.Refresh();
				}
			}
			if (unit) {
				unit.ValidateLocation();
			}
		}
	}

	void RefreshSelfOnly () {
		chunk.Refresh();
		if (unit) {
			unit.ValidateLocation();
		}
	}

	public void Save (BinaryWriter writer) {
		writer.Write((byte)terrainTypeIndex);
		writer.Write((byte)(elevation + 127));
		writer.Write((byte)waterLevel);
		writer.Write((byte)urbanLevel);
		writer.Write((byte)farmLevel);
		writer.Write((byte)plantLevel);
		writer.Write((byte)specialIndex);
		writer.Write(walled);

		if (hasIncomingRiver) {
			writer.Write((byte)(incomingRiver + 128));
		}
		else {
			writer.Write((byte)0);
		}

		if (hasOutgoingRiver) {
			writer.Write((byte)(outgoingRiver + 128));
		}
		else {
			writer.Write((byte)0);
		}

		
		writer.Write(IsExplored(TurnManager.Instance.currentPlayer));
	}

	public void Load (BinaryReader reader, int header) {
		terrainTypeIndex = reader.ReadByte();
		ShaderData.RefreshTerrain(this);
		elevation = reader.ReadByte();
		if (header >= 4) {
			elevation -= 127;
		}
		RefreshPosition();
		waterLevel = reader.ReadByte();
		urbanLevel = reader.ReadByte();
		farmLevel = reader.ReadByte();
		plantLevel = reader.ReadByte();
		specialIndex = reader.ReadByte();
		walled = reader.ReadBoolean();

		byte riverData = reader.ReadByte();
		if (riverData >= 128) {
			hasIncomingRiver = true;
			incomingRiver = (HexDirection)(riverData - 128);
		}
		else {
			hasIncomingRiver = false;
		}

		riverData = reader.ReadByte();
		if (riverData >= 128) {
			hasOutgoingRiver = true;
			outgoingRiver = (HexDirection)(riverData - 128);
		}
		else {
			hasOutgoingRiver = false;
		}

		
	//	IsExplored = header >= 3 ? reader.ReadBoolean() : false;
		ShaderData.RefreshVisibility(this, TurnManager.Instance.currentPlayer);
	}

	public void SetLabel (string text) {
		UnityEngine.UI.Text label = uiRect.GetComponent<Text>();
		label.text = text;
	}

	public void DisableHighlight () {
		Image highlight = uiRect.GetChild(0).GetComponent<Image>();
		highlight.enabled = false;
	}

	public void EnableHighlight (Color color, bool forceColor=false) {
        if (!TurnManager.Instance.againstAI || TurnManager.Instance.currentPlayer.Equals(Player.Player1) || forceColor)
        {
            Image highlight = uiRect.GetChild(0).GetComponent<Image>();
            highlight.color = color;
            highlight.enabled = true;
        }
	}

	public void SetMapData (float data) {
		ShaderData.SetMapData(this, data);
	}
    private void Select()
    {
        if (unit != null)
        {
            Selector.Instance.Select(unit);
        }
        else if (building != null)
        {
            Selector.Instance.Select(building);
        }
    }
    public void OnMouseDown()
    {
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() || TurnManager.Instance.currentPlayer.GetType() == typeof(ArtificialIntelligence))
        {
            return;
        }
        clicked++;
        if (clicked == 1) clicktime = Time.time;

        if (clicked > 1 && Time.time - clicktime < clickdelay)
        {
            clicked = 0;
            clicktime = 0;
            HexMapCamera.instance.SetPosition(Position.x, Position.z);
        }
        else if (clicked > 2 || Time.time - clicktime > 1) clicked = 0;
        if (state == STATE.UNIT_CURRENT_PATH || state == STATE.UNIT_CURRENT_ATTACK)
        {
            StartCoroutine(((Unit)Selector.Instance.currentObject).MoveTo(this));
        }
        else if (ConstructionManager.Instance.canConstruct && ConstructionManager.Instance.mode == "spell" && state == STATE.SPELL_POSSIBLE_CAST)
        {
            ConstructionManager.Instance.SpellNodeSelected(this);
        }
        else if (state == STATE.SPELL_CURRENT_CAST)
        {
            Construct(false);
        }
        else if (unit || building)
        {
            Select();
            return;
        }
        else if (ConstructionManager.Instance.canConstruct)// || ConstructionManager.Instance.mode != "spell" && state == STATE.SELECTABLE_CONSTRUCT_FINAL)
        {
            Construct(true);
        }
    }
    public void OnMouseOver()
    {
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() || TurnManager.Instance.currentPlayer.GetType() == typeof(ArtificialIntelligence))
        {
            return;
        }
        if (state == STATE.UNIT_POSSIBLE_PATH || state == STATE.UNIT_POSSIBLE_ATTACK)
        {
            ((Unit)Selector.Instance.currentObject).SetPathVisible(this, true);
        }
        else if (state == STATE.SPELL_POSSIBLE_CAST)
        {
            EnableHighlight(Color.blue); //TODO: change
        }
        else if ((unit || building) && CardDisplay.Instance.mode == CardDisplay.MODE.NON_DISPLAY)
        {
            if (unit != null && unit.visible)
            {
                unit.UpdateCardDisplayInfo();
            }
            else if (building != null && building.visible)
            {
                building.UpdateCardDisplayInfo();
            }
        }
    }
    public void OnMouseExit()
    {
       
        if ((state == STATE.UNIT_CURRENT_PATH || state == STATE.UNIT_CURRENT_ATTACK) &&(
            !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() &&
                !(TurnManager.Instance.currentPlayer.GetType() == typeof(ArtificialIntelligence))))
      
        {
            ((Unit)Selector.Instance.currentObject).SetPathVisible(this, false);
        }
        else if (state == STATE.SPELL_POSSIBLE_CAST)
        {
            EnableHighlight(Color.white); //TODO: change
        }
        else if ((unit || building) && Selector.Instance.currentObject == null)
        {
            CardDisplay.Instance.DisableCardDisplay();
        }
    }

    public void Notify(Player player, TurnSubject.NOTIFICATION_TYPE subjectType)
    {
        if (!TurnManager.Instance.againstAI || player.Equals(Player.Player1))
        {
            if (subjectType == TurnSubject.NOTIFICATION_TYPE.START_OF_TURN)
            {
          //      ShaderData.RefreshVisibility(this, player);
                
            }
        }
    }

    public override string ToString()
    {
        string pathToStr = "";
        foreach(HexCell cell in PathTo)
        {
            pathToStr += cell.Index + ",";
        }
        string pathFromstr = PathFrom != null ? PathFrom.Index.ToString() : "null";
        return "HexCell ("+Index+") at position " + Position.ToString() + "; STATE=" + state + "; pathTo={" + pathToStr + "}; pathFrom=" + pathFromstr;
    }
}
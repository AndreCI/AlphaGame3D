using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node : MonoBehaviour
{
    public enum STATE
    {
        IDLE,
        SELECTABLE_CONSTRUCT,
        SELECTABLE_CONSTRUCT_FINAL,
        SELECTABLE,
        SELECTABLE_FINAL,
        ON_UNIT_PATH,
        ON_UNIT_PATH_FINAL,
        ATTACKABLE,
        ATTACKALBE_FINAL,
        ATTACKABLE_HIDDEN,
        ATTACKABLE_HIDDEN_FINAL,
        SPELL_SELECTABLE,
        SPELL_SELECTABLE_FINAL,
        SPELL_EFFECT,
        SPELL_EFFECT_FINAL,
        SPELL_SELECTED,
        SPELL_SELECTED_FINAL
    };
    private enum COLORS
    {
        IDLE_COLOR,
        SELECTABLE_COLOR,
        ATTACKABLE_COLOR,
        VALIDATION_COLOR
    }

    private Dictionary<COLORS, Color> colorDictionary;
   
    public Vector3 positionOffset;

    public bool walkable;
    public GameObject FogOfWar;
    public Vector3 position;
    public Building building;
    public Unit unit;
    private Spell spell;
    private Renderer nodeRenderer;
    private List<Renderer> infoRenderers;
    public STATE state;

    // Use this for initialization
    void Awake()
    {
        colorDictionary = new Dictionary<COLORS, Color>
        {
            { COLORS.IDLE_COLOR, Color.black },
            { COLORS.ATTACKABLE_COLOR, Color.red },
            { COLORS.SELECTABLE_COLOR, Color.white },
            { COLORS.VALIDATION_COLOR, Color.green }
        };
        position = transform.position;
        state = STATE.IDLE;
        building = null;
        unit = null;
        walkable = true;
        Renderer[] infoRenderersTemp = GetComponentsInChildren<Renderer>();
        infoRenderers = new List<Renderer>();
        foreach (Renderer rend in infoRenderersTemp)
        {
            if (rend.name.Contains("InfoDisplay"))
            {
                infoRenderers.Add(rend);
            }
            else if (rend.name.Contains("Node"))
            {
                nodeRenderer = rend;
            }
        }
        //nodeRenderer.
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case STATE.SELECTABLE_CONSTRUCT:
                state = STATE.SELECTABLE_CONSTRUCT_FINAL;
                SetSelectedColor(COLORS.SELECTABLE_COLOR);
                break;
            case STATE.SELECTABLE:
                state = STATE.SELECTABLE_FINAL;
                SetSelectedColor(COLORS.SELECTABLE_COLOR);
                break;
            case STATE.ON_UNIT_PATH:
                state = STATE.ON_UNIT_PATH_FINAL;
                SetSelectedColor(COLORS.VALIDATION_COLOR);
                break;
            case STATE.ATTACKABLE:
                state = STATE.ATTACKALBE_FINAL;
                SetSelectedColor(COLORS.ATTACKABLE_COLOR);
                break;
            case STATE.ATTACKABLE_HIDDEN:
                state = STATE.ATTACKABLE_HIDDEN_FINAL;
                SetSelectedColor(COLORS.SELECTABLE_COLOR);
                break;
            case STATE.SPELL_EFFECT:
                state = STATE.SPELL_EFFECT_FINAL;
                SetSelectedColor(COLORS.SELECTABLE_COLOR);
                break;
            case STATE.SPELL_SELECTABLE:
                state = STATE.SPELL_SELECTABLE_FINAL;
                SetSelectedColor(COLORS.SELECTABLE_COLOR);
                break ;
            case STATE.SPELL_SELECTED:
                state = STATE.SPELL_SELECTED_FINAL;
                SetSelectedColor(COLORS.ATTACKABLE_COLOR);
                break;
        }

    }
    public void SetVisible(bool v)
    {
        if (v)
        {
            FogOfWar.GetComponent<ParticleSystem>().Stop();
        }
        else
        {
            FogOfWar.GetComponent<ParticleSystem>().Play();
        }
        if (building != null)
        {
            if (TurnManager.Instance.currentPlayer.knownBuilding.Contains(this))
            {
                building.SetVisible(true);
            }
            else
            {
                building.SetVisible(v);
            }
        }if (unit != null)
        {
            unit.SetVisible(v);
        }
    }

    private void OnMouseDown()
    {
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if (state == STATE.ON_UNIT_PATH_FINAL || state == STATE.ATTACKALBE_FINAL)
        {
            StartCoroutine(((Unit)Selector.Instance.currentObject).StartMoving());
        }
        else if (!walkable && ConstructionManager.Instance.canConstruct && ConstructionManager.Instance.mode == "spell" && state==STATE.SPELL_SELECTABLE_FINAL)
        {
            ConstructionManager.Instance.SpellNodeSelected(this);
        }else if(state == STATE.SPELL_SELECTED_FINAL)
        {
            Construct(false);
        }
        else if (!walkable)
        {
            Select();
            return;
        }
        else if (ConstructionManager.Instance.canConstruct && ConstructionManager.Instance.mode != "spell" && state==STATE.SELECTABLE_CONSTRUCT_FINAL)
        {
            Construct(true);
        }
    }
    public void Construct(bool makeUnwalkable=true)
    {
        if (ConstructionManager.Instance.mode == "building")
        {
            building = ConstructionManager.Instance.ConstructBuilding(this);
        }
        else if (ConstructionManager.Instance.mode == "unit")
        {
            unit = ConstructionManager.Instance.ConstructUnit(this);
        }else if (ConstructionManager.Instance.mode == "spell")
        {
            spell = ConstructionManager.Instance.ConstructSpell(this);
        }
        ConstructionManager.Instance.ResetConstruction();
        if (makeUnwalkable)
        {
            walkable = false;
        }
    }

    private void Select()
    {
        if (unit != null)
        {
            Selector.Instance.Select(unit);
        }else if(building != null)
        {
            Selector.Instance.Select(building);
        }
    }
    private void OnMouseOver()
    {
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if (state == STATE.SELECTABLE_FINAL || state == STATE.ATTACKABLE_HIDDEN_FINAL)
        {
            ((Unit)Selector.Instance.currentObject).ShowPotentialMove(this);
        }else if(state == STATE.SPELL_SELECTABLE_FINAL)
        {
            SetSelectedColor(COLORS.ATTACKABLE_COLOR);
        }else if (!walkable && CardDisplay.Instance.mode == CardDisplay.MODE.NON_DISPLAY)
        {
            if (unit != null)
            {
                unit.UpdateCardDisplayInfo();
            }else if(building != null)
            {
                building.UpdateCardDisplayInfo();
            }
        }
    }
    private void OnMouseExit()
    {
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if (state == STATE.SELECTABLE_FINAL || state == STATE.ON_UNIT_PATH_FINAL || state == STATE.ATTACKALBE_FINAL)
        {
            ((Unit)Selector.Instance.currentObject).HidePotentialMove();
        }else if (state == STATE.SPELL_SELECTABLE_FINAL)
        {
            SetSelectedColor(COLORS.SELECTABLE_COLOR);
        }else if (!walkable && Selector.Instance.currentObject == null)
        {
            CardDisplay.Instance.DisableCardDisplay();
        }
    }

    private void SetSelectedColor(COLORS color)
    {
        foreach(Renderer inforend in infoRenderers)
        {
            inforend.material.color = colorDictionary[color];
        }
    }
    public void MakeIdle()
    {
        SetSelectedColor(COLORS.IDLE_COLOR);
        state = STATE.IDLE;
    }

    public void ResetNode()
    {
        unit = null;
        building = null;
        walkable = true;
    }

    public void UpdateUnit(Unit unit_)
    {
        unit = unit_;
        walkable = false;
    }

    public void Damage(int amount)
    {
        if (unit != null)
        {
            unit.TakesDamage(amount);
        }else if(building !=null && typeof(DefensiveBuilding).IsAssignableFrom(building.GetType()))
        {
            ((DefensiveBuilding)building).TakeDamage(amount);
        }
    }

    public Unit GetUnit()
    {
        return unit;
    }

    public bool Attackable(Node source)
    {
        if ((unit == null && (building==null || !typeof(DefensiveBuilding).IsAssignableFrom(building.GetType()))) || source.unit == null)
        {
            return false;
        } else if ((unit!=null && unit.owner == source.unit.owner) || (building != null && building.owner == source.unit.owner)) {
            return false;
        } return true;
    }

    public bool SpellInteractable(Node source)
    {
        if (source.state == STATE.SPELL_SELECTED_FINAL || source.state == STATE.SPELL_SELECTED)
        {
            return true;
        }
        return false;
    }

    
}

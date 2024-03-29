﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
public class HexCell : MonoBehaviour
{
}
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
        VALIDATION_COLOR,
        PLAYER1_COLOR,
        PLAYER2_COLOR
    }

    private Dictionary<COLORS, Color> colorDictionary;
   
    public Vector3 positionOffset;
   
    public bool walkable;
    public Vector3 position;
    public Building building;
    public Unit unit;
    public STATE state;
    
    public List<Node> adjacentNodes;
     void Awake()
    {
        colorDictionary = new Dictionary<COLORS, Color>
        {
            { COLORS.IDLE_COLOR, Color.black },
            { COLORS.ATTACKABLE_COLOR, Color.red },
            { COLORS.SELECTABLE_COLOR, Color.white },
            { COLORS.VALIDATION_COLOR, Color.green },
            {COLORS.PLAYER1_COLOR, Color.green },
            {COLORS.PLAYER2_COLOR, Color.red }
        };
        position = transform.position;
        state = STATE.IDLE;
        building = null;
        unit = null;
        walkable = true;
        Renderer[] infoRenderersTemp = GetComponentsInChildren<Renderer>();
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

        }
        else
        {

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

    public void OnMouseDown()
    {
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() && TurnManager.Instance.currentPlayer.GetType() == typeof(ArtificialIntelligence))
        {
            return;
        }
        if (state == STATE.ON_UNIT_PATH_FINAL || state == STATE.ATTACKALBE_FINAL)
        {
            StartCoroutine(((Unit)Selector.Instance.currentObject).StartMoving(this));
        }
        else if (ConstructionManager.Instance.canConstruct && ConstructionManager.Instance.mode == "spell" && state==STATE.SPELL_SELECTABLE_FINAL)
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
        else if (ConstructionManager.Instance.canConstruct || ConstructionManager.Instance.mode != "spell" && state==STATE.SELECTABLE_CONSTRUCT_FINAL)
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
            ConstructionManager.Instance.ConstructSpell(this);
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
    public void OnMouseOver()
    {
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() || TurnManager.Instance.currentPlayer.GetType() == typeof(ArtificialIntelligence))
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
            if (unit != null && unit.visible)
            {
                unit.UpdateCardDisplayInfo();
            }else if(building != null && building.visible)
            {
                building.UpdateCardDisplayInfo();
            }
        }
    }
    public void OnMouseExit()
    {
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() || TurnManager.Instance.currentPlayer.GetType() == typeof(ArtificialIntelligence))
        {
            return;
        }
        if (state == STATE.SELECTABLE_FINAL || state == STATE.ON_UNIT_PATH_FINAL || state == STATE.ATTACKALBE_FINAL)
        {
            ((Unit)Selector.Instance.currentObject).HidePotentialMove(this);
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
        if (unit != null && unit.visible)
        {
            if(unit.owner == Player.Player1)
            {
                SetSelectedColor(COLORS.PLAYER1_COLOR);
            }else if (unit.owner == Player.Player2)
            {
                SetSelectedColor(COLORS.PLAYER2_COLOR);
            }
        }
        state = STATE.IDLE;
    }

    public void ResetNode()
    {
        unit = null;
        building = null;
        walkable = true;
        if (state == STATE.IDLE)
        {
            SetSelectedColor(COLORS.IDLE_COLOR);
        }
        SetSelectedColor(COLORS.IDLE_COLOR);
    }

    public void UpdateUnit(Unit unit_)
    {
        unit = unit_;
        if (unit != null && unit.visible)
        {
            if (unit.owner == Player.Player1)
            {
                SetSelectedColor(COLORS.PLAYER1_COLOR);
            }
            else if (unit.owner == Player.Player2)
            {
                SetSelectedColor(COLORS.PLAYER2_COLOR);
            }
        }
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

    public override bool Equals(object other)
    {
        if(other== null)
        {
            return false;
        }
        if (other.GetType()==(typeof(Node)))
        {
            return ((Node)other).position == position;
        }
        return false;
    }
}
*/
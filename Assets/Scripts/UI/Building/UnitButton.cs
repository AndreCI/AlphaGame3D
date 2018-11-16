using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class UnitButton : ConstructButtonScript
{
    public Unit unit;


    public void OnEnable()
    {
        UpdateInfo();
    }

    public override void UpdateInfo()
    {
        base.UpdateInfo();
        if (TurnManager.Instance.currentPlayer.CheckIfAvailable(unit))
        {
            GetComponent<Button>().interactable = true;
        }
        else
        {
            GetComponent<Button>().interactable = false;
        }
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        CardDisplay.Instance.DisableCardDisplay();
        unit.UpdateCardDisplayInfo();
        if (!TurnManager.Instance.currentPlayer.CheckIfAvailable(unit))
        {
            DisplayMessage(TurnManager.Instance.currentPlayer.GetUnavailableMessage(unit));
        }
        base.OnPointerEnter(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (!TurnManager.Instance.currentPlayer.CheckIfAvailable(unit))
        {
            RemoveDisplayMessage();
        }
        base.OnPointerExit(eventData);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (TurnManager.Instance.currentPlayer.CheckIfAvailable(unit))
        {
            ConstructionManager.Instance.SetUnitToBuild(unit);
        }
        base.OnPointerClick(eventData);
    }
}

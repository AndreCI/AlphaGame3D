using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class BuildingButtonScript : ConstructButtonScript
{
    public Building building;

    public void OnEnable()
    {
        UpdateInfo();
    }

    public override void UpdateInfo()
    {
        base.UpdateInfo();
        if (TurnManager.Instance.currentPlayer.CheckIfAvailable(building))
        {
            GetComponent<UnityEngine.UI.Button>().interactable = true;
        }
        else
        {
            GetComponent<UnityEngine.UI.Button>().interactable = false;
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        CardDisplay.Instance.DisableCardDisplay();
        building.UpdateCardDisplayInfo();
        if (!TurnManager.Instance.currentPlayer.CheckIfAvailable(building))
        {
            DisplayMessage(TurnManager.Instance.currentPlayer.GetUnavailableMessage(building));
        }
        base.OnPointerEnter(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (!TurnManager.Instance.currentPlayer.CheckIfAvailable(building))
        {
            RemoveDisplayMessage();
        }
        base.OnPointerExit(eventData);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (TurnManager.Instance.currentPlayer.CheckIfAvailable(building))
        {
            ConstructionManager.Instance.SetBuildingToBuild(building);
        }
        base.OnPointerClick(eventData);
    }
    
}

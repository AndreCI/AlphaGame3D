using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingButtonScript : Observer, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Building building;

    public void OnEnable()
    {
        UpdateInfo();
        TurnManager.Instance.StartTurnSubject.AddObserver(this);
    }

    private void UpdateInfo()
    {
        if (TurnManager.Instance.currentPlayer.CheckIfAvailable(building))
        {
            GetComponent<Button>().interactable = true;
        }
        else
        {
            GetComponent<Button>().interactable = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        CardDisplay.Instance.DisableCardDisplay();
        building.UpdateCardDisplayInfo();
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CardDisplay.Instance.DisableCardDisplay();
        if (Selector.Instance.currentObject != null)
        {
            Selector.Instance.currentObject.UpdateCardDisplayInfo();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (TurnManager.Instance.currentPlayer.CheckIfAvailable(building))
        {
            ConstructionManager.Instance.SetBuildingToBuild(building);
        }
    }

    public override void Notify(Player player)
    {
        UpdateInfo();
    }
}

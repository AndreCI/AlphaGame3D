using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitButton : Observer, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Unit unit;


    public void OnEnable()
    {
        UpdateInfo();
        TurnManager.Instance.StartTurnSubject.AddObserver(this);
    }

    private void UpdateInfo()
    {
        if (TurnManager.Instance.currentPlayer.CheckIfAvailable(unit))
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
        unit.UpdateCardDisplayInfo();

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
        if (TurnManager.Instance.currentPlayer.CheckIfAvailable(unit))
        {
            ConstructionManager.Instance.SetUnitToBuild(unit);
        }
    }

    public override void Notify(Player player)
    {
        UpdateInfo();
    }
}

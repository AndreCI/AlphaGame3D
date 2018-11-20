using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CancelUpgradeButton : MonoBehaviour, IPointerClickHandler
{

    private Building currentBuilding;

    public void OnPointerClick(PointerEventData eventData)
    {
        currentBuilding = (Building)Selector.Instance.currentObject;
        currentBuilding.UpdateCardDisplayInfo();
    }
}

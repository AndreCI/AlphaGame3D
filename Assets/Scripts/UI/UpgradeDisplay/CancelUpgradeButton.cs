using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CancelUpgradeButton : MonoBehaviour, IPointerClickHandler
{

    private Building currentBuilding;
    private Button button;

    public void OnPointerClick(PointerEventData eventData)
    {
        currentBuilding = (Building)Selector.Instance.currentObject;
        currentBuilding.UpdateCardDisplayInfo();
    }

    void Start()
    {
        button = GetComponent<Button>();
    }
    // Update is called once per frame
}

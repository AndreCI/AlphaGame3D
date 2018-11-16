using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!currentBuilding.tier2 && currentBuilding.goldCostTier2 <= TurnManager.Instance.currentPlayer.gold && TurnManager.Instance.currentPlayer.actionPoints > 0)
        {
            currentBuilding.UpgradeToT2();
            TurnManager.Instance.currentPlayer.gold -= currentBuilding.goldCostTier2;
            TurnManager.Instance.currentPlayer.actionPoints -= 1;
        }

    }

    private Building currentBuilding;
    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
    }



    void Update()
    {
        currentBuilding = (Building)Selector.Instance.currentObject;
        if (currentBuilding != null)
        {
            if (currentBuilding.tier2 || currentBuilding.goldCostTier2 > TurnManager.Instance.currentPlayer.gold || TurnManager.Instance.currentPlayer.actionPoints < 0)
            {
                button.interactable = false;
            }
            else
            {
                button.interactable = true;
            }
        }
    }
    // Update is called once per frame
}

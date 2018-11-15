using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    private List<Button> buttons;
    void Start()
    {

    }
    void Update()
    {
        
    }

    public void SelectHallCenter()
    {
        ConstructionManager.Instance.SetBuildingToBuild(ConstructionManager.Instance.HallCenter);
    }
    public void SelectBarracks()
    {
        if (TurnManager.Instance.currentPlayer.gold >= ConstructionManager.Instance.Barracks.goldCost)
        {
            ConstructionManager.Instance.SetBuildingToBuild(ConstructionManager.Instance.Barracks);
        }
    }
    public void SelectWarrior()
    {
        if (TurnManager.Instance.currentPlayer.gold >= ConstructionManager.Instance.Warrior.goldCost)
        {
            ConstructionManager.Instance.SetUnitToBuild(ConstructionManager.Instance.Warrior);
        }
    }
    public void SelectWizard()
    {
        if (TurnManager.Instance.currentPlayer.gold >= ConstructionManager.Instance.Wizard.goldCost)
        {
            ConstructionManager.Instance.SetUnitToBuild(ConstructionManager.Instance.Wizard);
        }
    }
}

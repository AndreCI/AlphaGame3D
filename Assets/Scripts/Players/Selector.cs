using UnityEngine;
using UnityEngine.UI;

public class Selector : IObserver
{
    private static Selector instance = null;
    public Image selectionInfoPanel;

    public static Selector Instance 
    {
        get
        {
            if (instance == null)
            {
                instance = new Selector();
            }
            return instance;
        }
    }

    // Use this for initialization
    private Selector()
    {
        TurnManager.Instance.EndTurnSubject.AddObserver(this);
        currentObject = null;
    }
    [Header("Other (todo)")]
    public Selectable currentObject;

    public void Select(Selectable newSelection)
    {
        if (newSelection.owner == TurnManager.Instance.currentPlayer)
        {
            Unselect();
            currentObject = newSelection;
            currentObject.Select();
        }
    }

    public void Unselect()
    {
        if (ConstructionManager.Instance.mode != "spell") { ConstructionManager.Instance.ResetConstruction(); }
        if (currentObject != null)
        {
            CardDisplay.Instance.DisableCardDisplay();
            currentObject.Unselect();
        }
        currentObject = null;
    }

    public void Notify(Player player, TurnSubject.NOTIFICATION_TYPE type)
    {
        if (currentObject != null)
        {
            if (type == TurnSubject.NOTIFICATION_TYPE.END_OF_TURN)
            {
                Unselect();
            }
        }
    }
}
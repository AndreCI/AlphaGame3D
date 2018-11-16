using UnityEngine;
using UnityEngine.UI;

public class Selector : Observer
{
    public static Selector Instance;
    public Image selectionInfoPanel;


    // Use this for initialization
    void Awake()
    {
        if (Instance != null)
        {
            return;
        }
        TurnManager.Instance.EndTurnSubject.AddObserver(this);
        currentObject = null;
        Instance = this;

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

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Unselect();
        }
    }

    public override void Notify(Player player)
    {
        if (currentObject != null)
        {
            Unselect();
        }
    }
}
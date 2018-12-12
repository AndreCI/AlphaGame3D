using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("General starting objects")]
    private TurnManager turnManager;
    public GameObject menu;
    public GameObject selectionPanel;
    public GameObject ressourcePanel;
    public GameObject notificationPanel;
    public HexGrid hexGrid;

    public static GameManager Instance;
    
    // Use this for initialization
    void Start()
    {
       // selectionPanel.SetActive(true);
        turnManager = TurnManager.Instance;
        turnManager.currentPlayer = Player.Player1;
        turnManager.StartGame(SelectRandomCell(), SelectRandomCell());
        Instance = this;

        //CardDisplay.Instance.DisableCardDisplay();
        
    }

    private HexCell SelectRandomCell()
    {
        HexCell start = null;
        hexGrid.GetPlayer1StartZone();
        while (start == null)
        {
            start = hexGrid.GetCell(Random.Range(0, hexGrid.cellCountX), Random.Range(0, hexGrid.cellCountZ));
            if (start.IsUnderwater)
            {
                start = null;
            }
            foreach(HexCell cell in start.GetNeighbors())
            {
                if(cell==null || cell.IsUnderwater)
                {
                    start = null;
                }
            }
        }
        return start;
    }
    public void ManualEndTurn()
    {
        turnManager.EndTurn();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            menu.SetActive(true);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            Selector.Instance.Unselect();
        }
        else if (Input.GetKeyDown("space"))
        {
            turnManager.TryEndTurn();
        }/*else if (turnManager.currentPlayer.isAi && turnManager.debugCounter > 1 && ((ArtificialIntelligence)Player.Player2).turnShouldBeFinished)
        {
          //  ((ArtificialIntelligence)Player.Player2).UpdateUnitEffect();
            ((ArtificialIntelligence)Player.Player2).turnFinished = true;
            Debug.Log("AI did not end turn. Finishing it now.");

            turnManager.EndTurn();
            turnManager.debugCounter = -1;
        }*/
        if (turnManager.debugCounter >= 0)
        {
            turnManager.debugCounter += Time.deltaTime;
        }
    }

    public void SetUIVisible(bool v)
    {
        selectionPanel.SetActive(v);
        ressourcePanel.SetActive(v);
    }

    public IEnumerator DisplayStartOfTurn()
    {
        notificationPanel.SetActive(true);
        notificationPanel.GetComponentInChildren<TextMeshProUGUI>().text = "New Turn!";
        yield return new WaitForSeconds(1.0f);
        notificationPanel.SetActive(false);
        yield return null;
    }
}

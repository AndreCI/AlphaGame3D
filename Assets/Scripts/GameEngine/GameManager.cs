using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("General starting objects")]
    private TurnManager turnManager;
    public GameObject tutorialStart;
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
        turnManager.StartGame(SelectRandomCell(Player.Player1), SelectRandomCell(Player.Player2));
        Instance = this;

        //CardDisplay.Instance.DisableCardDisplay();
        
    }

    private HexCell SelectRandomCell(Player player)
    {
        HexCell start = null;
        List<HexCell> spawnZone = hexGrid.GetPlayerStartZone(player);
        bool found = true;
        do
        {
            found = true;
            start = spawnZone[Random.Range(0, spawnZone.Count)];
            if (start.IsUnderwater)
            {
                found = false;
            }
           
            foreach (HexCell cell in start.GetNeighbors())
                {
                if (cell == null || cell.IsUnderwater)
                {
                    found = false;
                }
                else
                {
                    HexEdgeType edgeType = start.GetEdgeType(cell);
                    if (edgeType == HexEdgeType.Cliff)
                    {
                        found = false;
                    }
                }
            }
            
        } while (!found);
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
        }else if (turnManager.currentPlayer.isAi && turnManager.debugCounter > 3 && ((ArtificialIntelligence)Player.Player2).turnShouldBeFinished)
        {
          //  ((ArtificialIntelligence)Player.Player2).UpdateUnitEffect();
            ((ArtificialIntelligence)Player.Player2).turnFinished = true;
            Debug.Log("AI did not end turn. Finishing it now.");

            turnManager.EndTurn();
            turnManager.debugCounter = -1;
        }
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

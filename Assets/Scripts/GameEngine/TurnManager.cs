using System.Collections.Generic;
using UnityEngine;

public class TurnManager{

    private static TurnManager instance = null;
    private int TurnNumber;
    private List<Player> playerActive;
    public Player currentPlayer;
    private int playerActiveIndex;
    public TurnSubject StartTurnSubject;
    public TurnSubject EndTurnSubject;
    public TurnSubject ButtonUpdateSubject;
    public bool againstAI;
    public float debugCounter;

    public Player inactivePlayer
    {
        get
        {
            if (Player.Player1.Equals(currentPlayer))
            {
                return Player.Player2;
            }
            else
            {
                return Player.Player1;
            }
        }
    }

    public static TurnManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new TurnManager();
            }
            return instance;
        }
    }
    private TurnManager()
    {
        againstAI = true;// typeof(ArtificialIntelligence).IsAssignableFrom(Player.Player2.GetType());
        TurnNumber = 0;
        StartTurnSubject = new TurnSubject(TurnSubject.NOTIFICATION_TYPE.START_OF_TURN);
        EndTurnSubject = new TurnSubject(TurnSubject.NOTIFICATION_TYPE.END_OF_TURN);
        ButtonUpdateSubject = new TurnSubject(TurnSubject.NOTIFICATION_TYPE.UI_BUTTON_PRESSED);
        playerActiveIndex = 1;
        playerActive = new List<Player>
        {
            Player.Player1,
            Player.Player2
        };
    }
	
	// Update is called once per frame
	public void TryEndTurn () {
        if (againstAI && ((ArtificialIntelligence)Player.Player2).turnFinished)
        {
            EndTurn();
        }
	}
    

    public void EndTurn()
    {
        EndTurnSubject.NotifyObservers(currentPlayer);
        currentPlayer.mana = 0;
        NewTurn();
    }

    void StartTurn()
    {
        currentPlayer.mana += currentPlayer.manaBank;
        currentPlayer.manaBank = 0;
        Utils.EatFood(currentPlayer);
        StartTurnSubject.NotifyObservers(currentPlayer);
        GameObject.FindObjectOfType<MonoBehaviour>().StartCoroutine(currentPlayer.StartOfTurn());
    }

    void NewTurn()
    {
        TurnNumber += 1;
        if (playerActiveIndex == 1)
        {
            playerActiveIndex = 0;
        }
        else {
            playerActiveIndex = 1;
        }
        if (!againstAI)
        {
         //   currentPlayer.HideVisibleNodes();
        }
        currentPlayer = playerActive[playerActiveIndex];
        if (GameManager.Instance != null)
        {
            if (againstAI && currentPlayer.Equals(Player.Player2))
            {
                GameManager.Instance.SetUIVisible(false);
            }
            else
            {
                GameManager.Instance.SetUIVisible(true);
                GameManager.Instance.StartCoroutine(GameManager.Instance.DisplayStartOfTurn());

            }
        }
        StartTurn();
        
    }

    public void StartGame(HexCell startNode, HexCell startNode2)
    {
        currentPlayer = playerActive[0];
        HexCell baseNode = startNode;
        ConstructionManager.Instance.SetBuildingToBuild(ConstructionManager.Instance.HallCenter, true);
        baseNode.Construct(true);
        
        HexMapCamera.instance.SetPosition(baseNode.Position.x, baseNode.Position.z - 15, speed:0.5f);
      
        currentPlayer = playerActive[1];
        baseNode = startNode2;
        ConstructionManager.Instance.SetBuildingToBuild(ConstructionManager.Instance.HallCenter, true);
        baseNode.Construct(true);

        if (againstAI)
        {
            Player.Player2.currentBuildings[0].SetVisible(false);
        }
        NewTurn();

        NotificationsList.Instance.AddNotification("Left click here to start tutorial, right click to dismiss",
            Notification.NOTIFICATION_TYPE.START_GAME);
    }

    public void EndOfAITurn()
    {
        EndTurn();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("General starting objects")]
    public Node startNode;
    public static List<Node> allNodes;
    private TurnManager turnManager;
    public GameObject menu;
    public GameObject selectionPanel;
    public GameObject ressourcePanel;
    public GameObject notificationPanel;

    public static GameManager Instance;
    
    // Use this for initialization
    void Start()
    {
        selectionPanel.SetActive(true);
        turnManager = TurnManager.Instance;
        allNodes = new List<Node>();
        foreach (NodeUtils.NodeWrapper node in NodeUtils.GetNeighborsNode(startNode, 20).GetNodeChildren())
        {
            allNodes.Add(node.root);
        }
        turnManager.StartGame(startNode);
        Instance = this;

        //CardDisplay.Instance.DisableCardDisplay();
        
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
        turnManager.Update();
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

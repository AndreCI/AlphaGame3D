using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public Node startNode;
    public static List<Node> allNodes;
    private TurnManager turnManager;
    
    // Use this for initialization
    void Start()
    {

        turnManager = TurnManager.Instance;
        allNodes = new List<Node>();
        foreach (NodeUtils.NodeWrapper node in NodeUtils.GetNeighborsNode(startNode, 20).GetNodeChildren())
        {
            allNodes.Add(node.root);
        }
        turnManager.StartGame();

        //CardDisplay.Instance.DisableCardDisplay();
        
    }
    public void ManualEndTurn()
    {
        turnManager.EndTurn();
    }

    // Update is called once per frame
    void Update()
    {
        turnManager.Update();
    }
}

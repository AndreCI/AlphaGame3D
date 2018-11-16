using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class Unit : Selectable
{
    [Header("Prefab, animations, and unity stuff")]
    public GameObject prefab;
    public Transform movementSphere;
    public Canvas healthCanvas;
    public Image healthDisplay;
    public Sprite sprite;

    [Header("General Info")]
    public int maxHealth;
    public int attack;
    public int maxMovementPoints;
    public int range;
    public int visionRange;
    public string effectDescription;

    public int currentHealth;
    public int currentMovementPoints;
    protected NodeUtils.NodeWrapper currentPositionWrapped; //NodeWrapper containing the root of the movement tree
    public List<NodeUtils.NodeWrapper> possibleMoves; //List of all the nodes where the unit can go
    protected List<NodeUtils.NodeWrapper> potentialMove; //Current path for the target node
    protected List<Node> path; //selected path to node
    protected bool moving;
    public List<UnitEffect> currentEffect;

    public virtual void Setup()
    {

        currentHealth = maxHealth;
        currentMovementPoints = maxMovementPoints;
        TurnManager.Instance.StartTurnSubject.AddObserver(this);
        moving = false;
        currentEffect = new List<UnitEffect>();
    }

    public override void Notify(Player player)
    {
        if (player.Equals(TurnManager.Instance.currentPlayer))
        {
            currentMovementPoints = maxMovementPoints;
            foreach (UnitEffect ue in currentEffect)
            {
                ue.ApplyEffect();
            }
        }

    }

    private void LateUpdate()
    {
        healthCanvas.transform.rotation = Camera.main.transform.rotation;
    }
    public void SetVisible(bool v)
    {
        foreach(Renderer r in movementSphere.GetComponentsInChildren<Renderer>())
        {
            if (r.name != "Selector")
            {
                r.enabled = v;
            }
        }
        //prefab.SetActive(v);
        healthCanvas.enabled = v;
    }

    public void TakesDamage(int amount)
    {
        currentHealth = currentHealth - amount;
        if (currentHealth <= 0)
        {
            Death();
        }
        
        healthDisplay.fillAmount = (float)currentHealth/(float)(maxHealth);
    }

    private void Death()
    {
        foreach(UnitEffect ue in currentEffect)
        {
            ue.End();
        }
        owner.currentUnits.Remove(this);
        TurnManager.Instance.currentPlayer.UpdateVisibleNodes();
        currentPosition.ResetNode();
        Destroy(prefab);
    }
    public override void Select()
    {
        UpdateCardDisplayInfo();
        Renderer[] rends = GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in rends)
        {
            if (rend.name == "Selector")
            {
                rend.enabled = true;
            }
        }
        ShowPossibleMoves();
    }

    public List<NodeUtils.NodeWrapper> ShowPossibleMoves()
    {
        currentPositionWrapped = NodeUtils.GetPossibleNodes(currentPosition, currentMovementPoints);
        possibleMoves = currentPositionWrapped.GetNodeChildren();
        foreach (NodeUtils.NodeWrapper node in possibleMoves)
        {
            if (node.root == currentPosition)
            {
                node.root.state = Node.STATE.IDLE;
            }
            else if (node.state == NodeUtils.NodeWrapper.STATE.EMPTY)
            {
                node.root.state = Node.STATE.SELECTABLE;
            }
            else
            {
                node.root.state = Node.STATE.ATTACKABLE_HIDDEN;
            }
        }
        return possibleMoves;
    }

    public override void Unselect()
    {
        Renderer[] rends = GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in rends)
        {
            if (rend.name == "Selector")
            {
                rend.enabled = false;
            }
        }
        HidePossibleMoves();
    }

    public void HidePossibleMoves()
    {
        foreach (NodeUtils.NodeWrapper node in possibleMoves)
        {
            node.root.MakeIdle();
        }
        possibleMoves = null;
    }

    public void ShowPotentialMove(Node target)
    {
        potentialMove = currentPositionWrapped.Search(target);
        foreach (NodeUtils.NodeWrapper node in potentialMove)
        {
            if (node.root == currentPosition)
            {
                node.root.state = Node.STATE.IDLE;
            }
            else if (node.state == NodeUtils.NodeWrapper.STATE.EMPTY)
            {
                node.root.state = Node.STATE.ON_UNIT_PATH;
            }
            else
            {
                node.root.state = Node.STATE.ATTACKABLE;
            }
        }
    }

    public void HidePotentialMove()
    {
        foreach (NodeUtils.NodeWrapper node in possibleMoves)
        {
            if (node.state == NodeUtils.NodeWrapper.STATE.EMPTY)
            {
                node.root.state = Node.STATE.SELECTABLE;
            }
            else
            {
                node.root.state = Node.STATE.ATTACKABLE_HIDDEN;
            }
        }
        potentialMove = null;
    }
    protected void FaceNextNode(Node nextNode)
    {
        Vector3 targetDir = nextNode.position - movementSphere.position;
        targetDir.y = 0;
        movementSphere.localRotation = Quaternion.LookRotation(targetDir);
    }

    protected virtual void Attack(Node target)
    {
        target.Damage(attack);
        FinishMove();
        path = new List<Node>();
        currentMovementPoints = 0;
    }

    protected virtual void FinishMove()
    {
        moving = false;
        path = new List<Node>();
        Selector.Instance.Unselect(); //Unselect this, and thus MakeIdle all nodes
    }

    protected virtual void MoveStep()
    {
        //Update currentPosition
        currentPosition.ResetNode();
        currentPosition = path[0];
        currentPosition.UpdateUnit(this);
        currentMovementPoints -= 1;
        UpdateCardDisplayInfo();
        TurnManager.Instance.currentPlayer.UpdateVisibleNodes();
    }

    public virtual IEnumerator StartMoving()
    {
        moving = true;
        path = new List<Node>();
        foreach (NodeUtils.NodeWrapper node in potentialMove)
        {
            path.Add(node.root);
        }
        path.Reverse();
        path.Remove(path[0]);
        FaceNextNode(path[0]);
        yield return new WaitForSeconds(3.0f);

    }
    public void AIMove()
    {
        bool visible = false;
        while(path.Count > 0 && !visible)
        {
            if (Player.Player1.visibleNodes.Contains(path[0]))
            {
                SetVisible(true);
                StartCoroutine(StartMoving());
                visible = true;
            }
            else
            {
                FaceNextNode(path[0]);
                movementSphere.localPosition = new Vector3(path[0].position.x - currentPosition.position.x, movementSphere.localPosition.y, path[0].position.z - currentPosition.position.z);
                currentPosition.ResetNode();
                currentPosition = path[0];
                currentPosition.UpdateUnit(this);
                currentMovementPoints -= 1;
                path.Remove(path[0]);
            }
        }
    }

    public void StartAIMove()
    {
        if (Player.Player1.visibleNodes.Contains(currentPosition))
        {
            StartCoroutine(StartMoving());
        }
        else
        {
            path = new List<Node>();
            foreach (NodeUtils.NodeWrapper node in potentialMove)
            {
                path.Add(node.root);
            }
            path.Reverse();
            path.Remove(path[0]);
            FaceNextNode(path[0]);
            AIMove();
        }
    }

    public override void UpdateCardDisplayInfo()
    {
        TextMeshProUGUI[] elem = CardDisplay.Instance.EnableUnitCardDisplay(currentHealth, maxHealth, sprite);

        foreach (TextMeshProUGUI e in elem)
        {
            switch (e.name)
            {
                case "CardNameText":
                    e.text = cardName;
                    break;
                case "CardCostText":
                    e.text = goldCost.ToString();
                    break;
                case "CardAttackText":
                    e.text = attack.ToString();
                    break;
                case "CardHealthText":
                    e.text = currentHealth + "/" + maxHealth;
                    break;
                case "CardMovementpointsText":
                    e.text = currentMovementPoints + "/" + maxMovementPoints;
                    break;
                case "CardEffectText":
                    e.text = effectDescription;
                    break;
            }
        }
    }

}

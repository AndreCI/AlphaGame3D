using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class Spell : Selectable
{
    public GameObject prefab;
    public Sprite sprite;
    [Header("General spell attributes")]
    public SpellUtils.SchoolOfMagic schoolOfMagic;
    public int requirementLevel;
    public string effectDescription;
    public int cooldown;
    public int currentCooldown;
    public int areaOfEffect;
    public int castableRange;
    public int damage;
    protected List<Node> castableNodes;
    public List<Node> affectedNodes;
    public abstract void PlayAnimation();
    private SpellButtonScript spellButton;

    public Dictionary<Player, PlayerInfo> playerInfos;

    public void AwakeBase()
    {
        UpdatePlayerInfos();
    }
    public void UpdatePlayerInfos()
    {
        if (playerInfos == null)
        {
            playerInfos = new Dictionary<Player, PlayerInfo>
            {
                { Player.Player1, new PlayerInfo() },
                { Player.Player2, new PlayerInfo() }
            };
            TurnManager.Instance.StartTurnSubject.AddObserver(this);
        }
    }

    public override void Notify(Player player)
    {
        if (playerInfos[player].currentCooldown > 0)
        {
            playerInfos[player].currentCooldown -= 1;
        }
    }

    public virtual void Activate(List<Node> affectedNodes_)
    {
        //GameObject attackAnim = (GameObject)Instantiate(animationSpell, currentPosition.position, new Quaternion(0, 0, 0, 0));

        Selector.Instance.Unselect();
        playerInfos[TurnManager.Instance.currentPlayer].currentCooldown = cooldown;
        UpdateCooldown();
        //affectedNodes = new List<Node>();
        //Destroy(this);
    }

    public override void Select()
    {
        UpdateCardDisplayInfo();
        GetCastableNodes();
    }

    public override void Unselect()
    {
        if (affectedNodes != null)
        {
            foreach (Node node in affectedNodes)
            {
                if (node != null)
                {
                    node.MakeIdle();
                }
            }
        }
        if (castableNodes != null)
        {
            foreach (Node node in castableNodes)
            {
                node.MakeIdle();
            }
        }
        CardDisplay.Instance.DisableCardDisplay();
    }

    public override void UpdateCardDisplayInfo()
    {
        TextMeshProUGUI[] elem = CardDisplay.Instance.EnableSpellCardDisplay(sprite, schoolOfMagic);
        foreach (TextMeshProUGUI e in elem)
        {
            switch (e.name)
            {
                case "CardNameText":
                    e.text = cardName;
                    break;
                case "CardCostText":
                    e.text = manaCost.ToString();
                    break;
                case "CardEffectText":
                    e.text = effectDescription;
                    break;
                case "CardCooldownText":
                    e.text = cooldown.ToString();
                    break;
                case "CardRequirementText":
                    e.text = requirementLevel.ToString();
                    break;
                case "CardCastablerangeText":
                    e.text = castableRange.ToString();
                    break;

            }
        }
    }
    public void RegisterButton(SpellButtonScript b)
    {
        spellButton = b;
    }
    private void UpdateCooldown()
    {
        if(spellButton != null)
        {
            spellButton.UpdateInfo();
        }
    }

    public void GetSpellEffectNodes(Node source)
    {
        
        affectedNodes = new List<Node>();
        foreach (Node node in castableNodes)
        {
            node.MakeIdle();
        }
        source.state = Node.STATE.SPELL_SELECTED;
        NodeUtils.NodeWrapper currentPositionWrapped = NodeUtils.GetNeighborsNode(source, areaOfEffect);
        List<NodeUtils.NodeWrapper> castableNodesWrapped = currentPositionWrapped.GetNodeChildren();
        foreach (NodeUtils.NodeWrapper nodeWrapped in castableNodesWrapped)
        {
            nodeWrapped.root.state = Node.STATE.SPELL_EFFECT;
            affectedNodes.Add(nodeWrapped.root);
        }
        source.state = Node.STATE.SPELL_SELECTED;
        castableNodes = new List<Node>();
    }

    protected void GetCastableNodes()
    {
        castableNodes = new List<Node>();
        List<Unit> currentUnits = TurnManager.Instance.currentPlayer.currentUnits;
        List<Building> currentBuildings = TurnManager.Instance.currentPlayer.currentBuildings;
        
        foreach (Unit unit in currentUnits)
        {
            NodeUtils.NodeWrapper currentPositionWrapped = NodeUtils.GetNeighborsNode(unit.currentPosition, castableRange);
            List<NodeUtils.NodeWrapper> castableNodesWrapped = currentPositionWrapped.GetNodeChildren();
            foreach(NodeUtils.NodeWrapper nodeWrapped in castableNodesWrapped)
            {
                if (!castableNodes.Contains(nodeWrapped.root))
                {
                    castableNodes.Add(nodeWrapped.root);
                    nodeWrapped.root.state = Node.STATE.SPELL_SELECTABLE;
                }
            }
        }
        foreach (Building buiding in currentBuildings)
        {
            NodeUtils.NodeWrapper currentPositionWrapped = NodeUtils.GetNeighborsNode(buiding.currentPosition, castableRange + 2);
            List<NodeUtils.NodeWrapper> castableNodesWrapped = currentPositionWrapped.GetNodeChildren();
            foreach (NodeUtils.NodeWrapper nodeWrapped in castableNodesWrapped)
            {
                if (!castableNodes.Contains(nodeWrapped.root))
                {
                    castableNodes.Add(nodeWrapped.root);
                    nodeWrapped.root.state = Node.STATE.SPELL_SELECTABLE;
                }
            }
        }
    }

    public class PlayerInfo
    {
        public int currentCooldown;
        public Player owner;
        public Node position;

        public PlayerInfo()
        {
            currentCooldown = 0;
            owner = null;
            position = null;
        }
    }
}

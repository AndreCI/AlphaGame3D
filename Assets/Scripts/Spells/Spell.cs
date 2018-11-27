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
    public List<EffectFactory> effects;

    public void AwakeBase()
    {
        UpdatePlayerInfos();
        effects = new List<EffectFactory>();
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

    public override void Notify(Player player, TurnSubject.NOTIFICATION_TYPE type)
    {
        if (playerInfos[player].currentCooldown > 0)
        {
            if (type == TurnSubject.NOTIFICATION_TYPE.START_OF_TURN)
            {
                playerInfos[player].currentCooldown -= 1;
            }
        }
    }

    public virtual void Activate(List<Node> affectedNodes_)
    {
        Selector.Instance.Unselect();
        playerInfos[TurnManager.Instance.currentPlayer].currentCooldown = cooldown;
        UpdateCooldown();
    }

    protected virtual void ApplyEffectsToUnit(Unit u)
    {
        foreach(EffectFactory factory in effects)
        {
            UnitEffect ue = factory.GetEffect(u);
            if (ue.applyOnTouch)
            {
                ue.ApplyEffect();
            }
            if (ue.duration > 0)
            {
                u.currentEffect.Add(ue);
            }
        }
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

    public List<string> GetKeywordsDescription()
    {
        List<string> descriptions = new List<string>();
        {
            foreach(EffectFactory factory in effects)
            {
                descriptions.Add(factory.GetAbsoluteDescriptions());
            }
        }
        return descriptions;
    }

    public override void UpdateCardDisplayInfo()
    {
        string keywordsDescription = "";
        foreach(string effect in GetKeywordsDescription())
        {
            keywordsDescription += effect + "\n";
        }
        TextMeshProUGUI[] elem = CardDisplay.Instance.EnableSpellCardDisplay(sprite, schoolOfMagic, keywordsDescription);
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
        foreach (Node node in NodeUtils.BFSNodesAdj(source, areaOfEffect).GetChildrens())
        {
            node.state = Node.STATE.SPELL_EFFECT;
            affectedNodes.Add(node);
        }
        source.state = Node.STATE.SPELL_SELECTED;
        castableNodes = new List<Node>();
    }

    protected virtual void GetCastableNodes()
    {
        castableNodes = new List<Node>();
        List<Unit> currentUnits = TurnManager.Instance.currentPlayer.currentUnits;
        List<Building> currentBuildings = TurnManager.Instance.currentPlayer.currentBuildings;
        
        foreach (Unit unit in currentUnits)
        {
            foreach(Node node in NodeUtils.BFSNodesAdj(unit.currentPosition, castableRange).GetChildrens()){
                if (!castableNodes.Contains(node))
                {
                    castableNodes.Add(node);
                }
            }
            
        }
        foreach (Building buiding in currentBuildings)
        {
            foreach (Node node in NodeUtils.BFSNodesAdj(buiding.currentPosition, castableRange).GetChildrens())
            {
                if (!castableNodes.Contains(node))
                {
                    castableNodes.Add(node);
                }
            }
        }
        foreach (Node n in castableNodes)
        {
            n.state = Node.STATE.SPELL_SELECTABLE;
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

using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class SpellButtonScript : ConstructButtonScript
{

    public string spellname;
    private Spell spell;

    public void Start()
    {
        spell = Utils.stringToSpell[spellname];
        spell.RegisterButton(this);
    }

    public void Update()
    {
        UpdateInfo();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        spell = Utils.stringToSpell[spellname];
        CardDisplay.Instance.DisableCardDisplay();
        spell.UpdateCardDisplayInfo();
        if (!TurnManager.Instance.currentPlayer.CheckIfAvailable(spell))
        {
            DisplayMessage(TurnManager.Instance.currentPlayer.GetUnavailableMessage(spell));
        }
        base.OnPointerEnter(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (!TurnManager.Instance.currentPlayer.CheckIfAvailable(spell))
        {
            RemoveDisplayMessage();
        }
        base.OnPointerExit(eventData);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        spell = Utils.stringToSpell[spellname];
        if (TurnManager.Instance.currentPlayer.CheckIfAvailable(spell))
        {
            ConstructionManager.Instance.SetSpellToConstruct(spell);
        }
        base.OnPointerClick(eventData);
    }

    void OnEnable()
    {
        UpdateInfo();
    }

    public override void UpdateInfo()
    {
        base.UpdateInfo();
        spell = Utils.stringToSpell[spellname];
        if (TurnManager.Instance.currentPlayer.CheckIfAvailable(spell))
        {
            spell = Utils.stringToSpell[spellname];
            spell.UpdatePlayerInfos();
            float readyScore = 0;
            if (spell.cooldown == 0)
            {
                readyScore = 1;
            }
            else
            {
                readyScore = ((float)(spell.cooldown) - (float)spell.playerInfos[TurnManager.Instance.currentPlayer].currentCooldown) / (float)(spell.cooldown);
            }
            if (readyScore < 1)
            {
                GetComponent<Button>().interactable = false;
            }
            else
            {
                GetComponent<Button>().interactable = true;
            }
            GetComponent<Image>().fillAmount = readyScore;
        }
        else
        {
            float readyScore = 0;
            if (spell.cooldown == 0)
            {
                readyScore = 1;
            }
            else
            {
                readyScore = ((float)(spell.cooldown) - (float)spell.playerInfos[TurnManager.Instance.currentPlayer].currentCooldown) / (float)(spell.cooldown);
            }
            if (readyScore == 1)
            {
                readyScore = 0;
            }
            GetComponent<Image>().fillAmount = readyScore;
            GetComponent<Button>().interactable = false;
        }
    }
}

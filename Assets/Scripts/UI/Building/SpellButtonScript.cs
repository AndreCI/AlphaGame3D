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

    private void Update()
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
            float readyScore = ((float)(spell.cooldown) - (float)spell.playerInfos[TurnManager.Instance.currentPlayer].currentCooldown) / (float)(spell.cooldown);
            GetComponent<Image>().fillAmount = readyScore;
            if (readyScore < 1)
            {
                GetComponent<Button>().interactable = false;
            }
            else
            {
                GetComponent<Button>().interactable = true;
            }
        }
        else
        {
            GetComponent<Image>().fillAmount = 0;
            GetComponent<Button>().interactable = false;
        }
    }
}

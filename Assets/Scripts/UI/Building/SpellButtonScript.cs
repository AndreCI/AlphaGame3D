using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpellButtonScript : Observer, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    public string spellname;
    private Spell spell;

    public void Start()
    {
        spell = Utils.stringToSpell[spellname];
        spell.RegisterButton(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        spell = Utils.stringToSpell[spellname];
        CardDisplay.Instance.DisableCardDisplay();
        spell.UpdateCardDisplayInfo();

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CardDisplay.Instance.DisableCardDisplay();
        if (Selector.Instance.currentObject != null)
        {
            Selector.Instance.currentObject.UpdateCardDisplayInfo();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        spell = Utils.stringToSpell[spellname];
        if (TurnManager.Instance.currentPlayer.CheckIfAvailable(spell))
        {
            ConstructionManager.Instance.SetSpellToConstruct(spell);
        }
    }

    void OnEnable()
    {
        UpdateInfo();
    }

    public void UpdateInfo()
    {
        spell = Utils.stringToSpell[spellname];
        TurnManager.Instance.StartTurnSubject.AddObserver(this); //TODO: seems buggy
        if (TurnManager.Instance.currentPlayer.CheckIfAvailable(spell))
        {
            //GetComponent<Button>().interactable = true;
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

    public override void Notify(Player player)
    {
        UpdateInfo();
    }
}

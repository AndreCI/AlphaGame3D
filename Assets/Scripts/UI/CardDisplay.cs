using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CardDisplay : MonoBehaviour
{

    public static CardDisplay Instance;
    [Header("Game object containing UI elements")]
    public GameObject unitCard;
    public GameObject buildingCardNormal;
    public GameObject buildingCardDefensive;
    public GameObject spellCard;
    public GameObject buildingCardT2;
    public GameObject buildingCardDefensiveT2;
    public GameObject spellEffectHelp;
    public TextMeshProUGUI spellEffectText;
    public GameObject unitEffectHelp;
    public TextMeshProUGUI unitEffectText;
    public Button upgradeButton;

    

    public enum MODE
    {
        NON_DISPLAY,
        UNIT_DISPLAY,
        BUILDING_DISPLAY_NORMAL,
        BUILDING_DISPLAY_DEFENSIVE,
        SPELL_DISPLAY
    }
    public MODE mode; //0 non display, 1 unit display, 2 building display

    // Use this for initialization
    void Start()
    {
        if (Instance != null)
        {
            return;
        }
        DisableCardDisplay();
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public TextMeshProUGUI[] EnableUnitCardDisplay(float currentHealth, float maxHealth, Sprite sprite, string effectDescriptor)
    {
        DisableCardDisplay();
        mode = MODE.UNIT_DISPLAY;
        unitCard.SetActive(true);
        unitCard.GetComponent<Image>().enabled = true;

        unitEffectHelp.SetActive(true);
        unitEffectText.text = effectDescriptor;
        Vector2 newSize = new Vector2(0, 0);
        if (effectDescriptor != "")
        {
            newSize = unitEffectText.GetPreferredValues();
            newSize += newSize / 5;

        }
        unitEffectHelp.GetComponent<Image>().rectTransform.sizeDelta = newSize;
        if (effectDescriptor == "")
        {
            unitEffectHelp.SetActive(true);
        }

        TextMeshProUGUI[] elements = unitCard.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI t in elements)
        {
            t.enabled = true;
        }
        Image[] images = unitCard.GetComponentsInChildren<Image>();
        foreach (Image t in images)
        {
            t.enabled = true;
            if (t.name == "HealthBar")
            {
                t.fillAmount = (float)currentHealth / (float)(maxHealth);
            }else if (t.name == "CardImage"){
                t.sprite = sprite;
            }
        }
        
        return elements;
    }

    public TextMeshProUGUI[] EnableNormalBuildigCardDisplay(Sprite sprite, bool t2)
    {
        DisableCardDisplay();
        mode = MODE.BUILDING_DISPLAY_NORMAL;
        GameObject buildingCard;
        if (t2)
        {
            buildingCard = buildingCardT2;
        }
        else
        {
            buildingCard = buildingCardNormal;
        }
        buildingCard.SetActive(true);
        buildingCard.GetComponent<Image>().enabled = true;

        TextMeshProUGUI[] elements = buildingCard.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI t in elements)
        {
            t.enabled = true;
        }
        Image[] images = buildingCard.GetComponentsInChildren<Image>();
        foreach (Image t in images)
        {
            t.enabled = true;
            if (t.name == "CardImage")
            {
                t.sprite = sprite;
            }
        }
        if (!t2)
        {
            upgradeButton.gameObject.SetActive(true);
        }
        return elements;
    }

    public TextMeshProUGUI[] EnableDefensiveBuildingCardDisplay(int currentHealth, int maxHealth, Sprite sprite, bool t2)
    {
        DisableCardDisplay();
        mode = MODE.BUILDING_DISPLAY_DEFENSIVE;
        GameObject buildingCard;
        if (t2)
        {
            buildingCard = buildingCardDefensiveT2;
        }
        else
        {
            buildingCard = buildingCardDefensive;
        }
        buildingCard.SetActive(true);
        buildingCard.GetComponent<Image>().enabled = true;

        TextMeshProUGUI[] elements = buildingCard.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI t in elements)
        {
            t.enabled = true;
        }
        Image[] images = buildingCard.GetComponentsInChildren<Image>();
        foreach (Image t in images)
        {
            t.enabled = true;
            if (t.name == "HealthBar")
            {
                t.fillAmount = (float)currentHealth / (float)(maxHealth);
            }
            else if (t.name == "CardImage")
            {
                t.sprite = sprite;
            }
        }
        if (!t2)
        {
            upgradeButton.gameObject.SetActive(true);
        }
        return elements;
    }

    public TextMeshProUGUI[] EnableSpellCardDisplay(Sprite sprite, SpellUtils.SchoolOfMagic schoolOfMagic, string effectDescriptor)
    {
        DisableCardDisplay();
        mode = MODE.SPELL_DISPLAY;
        spellCard.SetActive(true);
        spellCard.GetComponent<Image>().enabled = true;
        Sprite requirementSprite = SpellUtils.Instance.GetSpriteFromSchoolOfMagic(schoolOfMagic);

        TextMeshProUGUI[] elements = spellCard.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI t in elements)
        {
            t.enabled = true;
        }
        Image[] images = spellCard.GetComponentsInChildren<Image>();
        foreach (Image t in images)
        {
            t.enabled = true;
            if (t.name == "CardImage")
            {
                t.sprite = sprite;
            }else if(t.name == "RequirementImage")
            {
                t.sprite = requirementSprite;
            }
        }
        spellEffectHelp.SetActive(true);
        Vector2 newSize = new Vector2(0, 0); ;
        spellEffectText.text = effectDescriptor;
        if (effectDescriptor != "")
        {
            newSize = spellEffectText.GetPreferredValues();
            newSize += newSize / 5;
            
        }
        spellEffectHelp.GetComponent<Image>().rectTransform.sizeDelta = newSize;
        spellEffectHelp.SetActive(false);
        return elements;
    }



    public void DisableCardDisplay()
    {
        mode = MODE.NON_DISPLAY;
        DisableSpecificCardDisplay(unitCard);
        DisableSpecificCardDisplay(buildingCardDefensive);
        DisableSpecificCardDisplay(buildingCardNormal);
        DisableSpecificCardDisplay(buildingCardDefensiveT2);
        DisableSpecificCardDisplay(buildingCardT2);
        DisableSpecificCardDisplay(spellCard);
    }
    private void DisableSpecificCardDisplay(GameObject card)
    {
        card.GetComponent<Image>().enabled = false;

        TextMeshProUGUI[] elements = card.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI t in elements)
        {
            t.enabled = false;
        }
        Image[] images = card.GetComponentsInChildren<Image>();
        foreach (Image t in images)
        {
            t.enabled = false;

        }
        upgradeButton.gameObject.SetActive(false);
        card.SetActive(false);
        spellEffectHelp.SetActive(false);
        unitEffectHelp.SetActive(false);
    }
}

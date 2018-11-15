using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

[System.Serializable]
public abstract class Building : Selectable
{
    [Header("Prefab")]
    public GameObject prefab;
    public Sprite sprite;
    [Header("General Info")]
    public string effectDescription;
    public string tier2EffectDescription;
    public int constructionTime;

    public bool tier2;
    public int goldCostTier2;

    private void Start()
    {
        tier2 = false;
    }
    public void SetVisible(bool v)
    {
        foreach (Renderer r in prefab.GetComponentsInChildren<Renderer>())
        {
            if (r.name != "Selector")
            {
                r.enabled = v;
            }
        }
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
    }
    public override void UpdateCardDisplayInfo()
    {
        TextMeshProUGUI[] elem = CardDisplay.Instance.EnableNormalBuildigCardDisplay(sprite, tier2);
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
                case "CardEffectText":
                    string temp = effectDescription;
                    e.text = effectDescription; 
                    break;
            }
        }
    }
    public override void Notify(Player player)
    {
        if (player.Equals(owner)) 
        {
            if (constructionTime > 0)
            {
                constructionTime -= 1;
                return;
            }
        }
        else
        {
            return;
        }

    }

    public virtual void UpgradeToT2()
    {
        tier2 = true;
        cardName += " T2";
        effectDescription += "\n" + tier2EffectDescription;
        goldCost += goldCostTier2;
        UpdateCardDisplayInfo();
    }
}
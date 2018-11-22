using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

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

    public int goldCostTier2;
    protected Dictionary<Utils.NotificationTypes, int> startOfTurnNotificationData;

    private void Start()
    {
        isTier2 = false;
        visible = true;
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
        visible = v;
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
        TextMeshProUGUI[] elem = CardDisplay.Instance.EnableNormalBuildigCardDisplay(sprite, isTier2);
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
                    e.text = effectDescription; 
                    break;
            }
        }
    }

    public virtual void MockCardDisplayT2Info()
    {
        TextMeshProUGUI[] elem = CardDisplay.Instance.EnableNormalBuildigCardDisplay(sprite, true);
        CardDisplay.Instance.upgradeToT2Preview.SetActive(true);
        foreach (TextMeshProUGUI e in elem)
        {
            switch (e.name)
            {
                case "CardNameText":
                    e.text = cardName + " T2";
                    break;
                case "CardCostText":
                    e.text = goldCostTier2.ToString();
                    break;
                case "CardEffectText":
                    e.text = "Once upgraded, this building will have:" + "\n" + tier2EffectDescription;
                    break;
            }
        }
    }
    public override void Notify(Player player, TurnSubject.NOTIFICATION_TYPE type)
    {
        if (player.Equals(owner)) 
        {
            if (type == TurnSubject.NOTIFICATION_TYPE.START_OF_TURN)
            {
                if (constructionTime > 0)
                {
                    constructionTime -= 1;
                    StartCoroutine(DisplayAndApplyNotification(owner, new Dictionary<Utils.NotificationTypes, int> {
                    {Utils.NotificationTypes.BUILDING, constructionTime }
                }));
                    return;
                }
                else
                {
                    StartCoroutine(DisplayAndApplyNotification(owner, startOfTurnNotificationData));
                }
            }
        }
        else
        {
            return;
        }

    }
    public IEnumerator DisplayAndApplyNotification(Player currentPlayer, Dictionary<Utils.NotificationTypes, int> notificationData)
    {
        notificationPanel.SetActive(true);
        notificationPanel.transform.rotation = Camera.main.transform.rotation;
        foreach (Utils.NotificationTypes type in notificationData.Keys)
        {
            string data = "";
            if (notificationData[type] > 0)
            {
                data += "+";
            }
            else
            {
                data += "-";
            }
                
            data+=notificationData[type].ToString();
            Utils.ApplyNotification(type, notificationData[type], currentPlayer);
            if (visible)
            {
                yield return StartCoroutine(FadeNotification(data, type));
            }
        }
        notificationPanel.SetActive(false);
        yield return null;
    }

    public virtual void UpgradeToT2()
    {
        isTier2 = true;
        cardName += " T2";
        effectDescription += "\n" + tier2EffectDescription;
        goldCost += goldCostTier2;
        TurnManager.Instance.currentPlayer.requirementSystem.SetTier2(GetType());
        UpdateCardDisplayInfo();
    }
}
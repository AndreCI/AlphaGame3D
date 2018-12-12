using TMPro;

public abstract class DefensiveBuilding : Building
{
    public int maxHealth;
    public int currentHealth;
    public int maxHealthT2Upgrade;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        StartCoroutine(FadeNotification("-"+amount.ToString(), Utils.NotificationTypes.DAMAGE));
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Death();
        }
    }
    public virtual void Death()
    {
        owner.currentBuildings.Remove(this);
        TurnManager.Instance.currentPlayer.UpdateVisibleNodes();
        currentPosition.building = null;
        Destroy(prefab);
    }
    public override void UpdateCardDisplayInfo()
    {
        TextMeshProUGUI[] elem = CardDisplay.Instance.EnableDefensiveBuildingCardDisplay(currentHealth, maxHealth, sprite, isTier2);
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
                case "CardHealthText":
                    e.text = currentHealth.ToString() +"/"+maxHealth.ToString();
                    break;
            }
        }
    }
    public override void MockCardDisplayT2Info()
    {
        TextMeshProUGUI[] elem = CardDisplay.Instance.EnableDefensiveBuildingCardDisplay(currentHealth + maxHealthT2Upgrade, maxHealth + maxHealthT2Upgrade, sprite, true);
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
                case "CardHealthText":
                    e.text = (currentHealth+maxHealthT2Upgrade).ToString() + "/" + (maxHealth+maxHealthT2Upgrade).ToString();
                    break;
            }
        }
    }
}
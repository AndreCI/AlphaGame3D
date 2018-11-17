using TMPro;

public abstract class DefensiveBuilding : Building
{
    public int maxHealth;
    public int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
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
        currentPosition.ResetNode();
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
}
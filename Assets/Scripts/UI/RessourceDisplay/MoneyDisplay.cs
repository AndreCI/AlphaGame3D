using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MoneyDisplay : MonoBehaviour
{

    public Text money;

    // Update is called once per frame
    void Update()
    {
        money.text = TurnManager.Instance.currentPlayer.gold.ToString();
    }
}

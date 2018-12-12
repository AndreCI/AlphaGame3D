using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class MoneyDisplay : MonoBehaviour
{

    public TextMeshProUGUI money;

    // Update is called once per frame
    void LateUpdate()
    {
        money.text = TurnManager.Instance.currentPlayer.gold.ToString();
    }
}

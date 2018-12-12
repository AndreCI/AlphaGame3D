using TMPro;
using UnityEngine;

public class ManaDisplay : MonoBehaviour
{

    public TextMeshProUGUI mana;

    // Update is called once per frame
    void LateUpdate()
    {
        string manabank = TurnManager.Instance.currentPlayer.manaBank>0? " (" + TurnManager.Instance.currentPlayer.manaBank.ToString() + ")" : "";
        mana.text = TurnManager.Instance.currentPlayer.mana.ToString() + manabank;
    }
}

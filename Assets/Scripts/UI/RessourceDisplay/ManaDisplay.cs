using TMPro;
using UnityEngine;

public class ManaDisplay : MonoBehaviour
{

    public TextMeshProUGUI mana;

    // Update is called once per frame
    void Update()
    {
        mana.text = TurnManager.Instance.currentPlayer.mana.ToString();
    }
}

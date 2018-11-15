using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ManaDisplay : MonoBehaviour
{

    public Text mana;

    // Update is called once per frame
    void Update()
    {
        mana.text = TurnManager.Instance.currentPlayer.mana.ToString();
    }
}

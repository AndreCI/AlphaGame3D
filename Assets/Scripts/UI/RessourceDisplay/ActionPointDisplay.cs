using TMPro;
using UnityEngine;


public class ActionPointDisplay : MonoBehaviour
{

    public TextMeshProUGUI ap;

    // Update is called once per frame
    void LateUpdate()
    {
        ap.text = TurnManager.Instance.currentPlayer.actionPoints.ToString();
    }
}

using UnityEngine;
using UnityEngine.UI;


public class ActionPointDisplay : MonoBehaviour
{

    public Text ap;

    // Update is called once per frame
    void Update()
    {
        ap.text = TurnManager.Instance.currentPlayer.actionPoints.ToString();
    }
}

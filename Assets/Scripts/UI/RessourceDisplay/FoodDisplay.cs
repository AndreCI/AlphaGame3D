using TMPro;
using UnityEngine;

public class FoodDisplay : MonoBehaviour
{

    public TextMeshProUGUI food;

    // Update is called once per frame
    void LateUpdate()
    {
        food.text = Player.Player1.food.ToString() + " ("+ Player.Player1.foodPrediction.ToString() + ")";
    }
}

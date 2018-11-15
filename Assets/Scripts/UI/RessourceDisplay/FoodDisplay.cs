using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FoodDisplay : MonoBehaviour
{

    public Text food;

    // Update is called once per frame
    void Update()
    {
        food.text = Player.Player1.food.ToString() + " ("+ Player.Player1.foodPrediction.ToString() + ")";
    }
}

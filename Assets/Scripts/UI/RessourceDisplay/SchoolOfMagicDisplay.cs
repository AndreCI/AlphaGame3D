using TMPro;
using UnityEngine;

public class SchoolOfMagicDisplay : MonoBehaviour
{

    public TextMeshProUGUI level;
    public SpellUtils.SchoolOfMagic schoolOfMagic;

    // Update is called once per frame
    void LateUpdate()
    {
        level.text = TurnManager.Instance.currentPlayer.schoolOfMagicLevels[schoolOfMagic].ToString();
    }
}

﻿using UnityEngine;
using System.Collections;
using TMPro;

public class SchoolOfMagicDisplay : MonoBehaviour
{

    public TextMeshProUGUI level;
    public SpellUtils.SchoolOfMagic schoolOfMagic;

    // Update is called once per frame
    void Update()
    {
        level.text = TurnManager.Instance.currentPlayer.schoolOfMagicLevels[schoolOfMagic].ToString();
    }
}

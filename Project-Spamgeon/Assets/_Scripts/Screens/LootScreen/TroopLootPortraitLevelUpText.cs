using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TroopLootPortraitLevelUpText : MonoBehaviour {

    [SerializeField] private Text statText;

	public void SetStatText(string statName)
    {
        statText.text = statName + "+";
    }
}

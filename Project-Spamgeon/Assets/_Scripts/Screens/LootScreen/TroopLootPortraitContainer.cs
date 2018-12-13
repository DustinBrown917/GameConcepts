using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopLootPortraitContainer : MonoBehaviour {

    [SerializeField] private TroopLootPortrait[] portraits;

    private void OnEnable()
    {
        for(int i = 0; i < portraits.Length; i++)
        {
            if(i < GameManager.GetLeftPlayer().ActiveTroopCount)
            {
                portraits[i].gameObject.SetActive(true);
                portraits[i].AssignTroop(GameManager.GetLeftPlayer().GetActiveTroop(i));
            } else
            {
                portraits[i].gameObject.SetActive(false);
            }
        }
    }
}

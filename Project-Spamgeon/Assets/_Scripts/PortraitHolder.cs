using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortraitHolder : MonoBehaviour {

    [SerializeField] private TroopPortrait[] portraits;

    // Use this for initialization
    void Start() {

    }

    public void AssignTroop(Troop t)
    {
        if(t == null) { return; }

        for(int i = 0; i <= portraits.Length; i++)
        {
            if (!portraits[i].gameObject.activeSelf) {
                portraits[i].gameObject.SetActive(true);
                portraits[i].AssignTroop(t);
                break;
            }
        }
    }

}

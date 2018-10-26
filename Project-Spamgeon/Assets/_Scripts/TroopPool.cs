using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Troop Pool", menuName = "Troop Pool")]
public class TroopPool : ScriptableObject {

    public List<GameObject> troops;

    public GameObject GetRandomTroop()
    {
        return troops[UnityEngine.Random.Range(0, troops.Count)];
    }
}

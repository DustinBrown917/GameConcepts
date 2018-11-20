using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopPoolManager : MonoBehaviour {

    [SerializeField] private TroopPool[] pools;

    private void Start()
    {
        Debug.Log(pools[0].name);
    }

    public TroopPool GetPool(string poolName)
    {
        for(int i = 0; i < pools.Length; i++) {
            if(pools[i].name == poolName) {
                return pools[i];
            }
        }

        Debug.LogWarning("TroopPool " + poolName + " not found.");
        return null;
    }
    
}

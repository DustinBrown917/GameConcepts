using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopPoolManager : MonoBehaviour {

    private static TroopPool[] pools;

    [SerializeField] private TroopPool[] localPools;

    private void Awake()
    {
        pools = localPools;
    }

    public static TroopPool GetPool(string poolName)
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

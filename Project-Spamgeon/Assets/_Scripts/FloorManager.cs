using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour {

    private static FloorManager instance_ = null;
    public static FloorManager Instance { get { return instance_; } }

    [SerializeField] private MinMax[] monstersPerFloor;
    [SerializeField] private MinMax monsterCountFactor;

    private void Awake()
    {
        if(instance_ == null) {
            instance_ = this;
        } else if(instance_ != this)
        {
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        instance_ = null;
    }

    public int GetMonsterCount()
    {
        int count = 1;
        int forFloor = GameManager.CurrentDungeonDepth;
        if(forFloor > 0 && forFloor < monstersPerFloor.Length)
        {
            count = UnityEngine.Random.Range((int)monstersPerFloor[forFloor - 1].Min, (int)monstersPerFloor[forFloor - 1].Max);
        } else
        {
            count = (int)UnityEngine.Random.Range(monsterCountFactor.Min * forFloor, monsterCountFactor.Max * forFloor);
        }

        count = Mathf.Clamp(count, 1, 5);

        return count;
    }
}

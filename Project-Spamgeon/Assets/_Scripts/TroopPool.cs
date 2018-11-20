using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Troop Pool", menuName = "Troop Pool")]
public class TroopPool : ScriptableObject {

    [SerializeField] private List<GameObject> troops;
    public int Count { get { return troops.Count; } }

    public Troop this[int index]
    {
        get
        {
            return troops[index].GetComponent<Troop>();
        }
    }

    public GameObject GetRandomTroop()
    {
        return troops[UnityEngine.Random.Range(0, troops.Count)];
    }

    public Troop GetTroopAt(int index)
    {
        if(index < 0 || index >= troops.Count) {
            Debug.LogError("Attempting to access invalid TroopPool index. Accessing: " + index.ToString() + " of " + Count + ".");
            return null;
        }

        return troops[index].GetComponent<Troop>();
    }

    public GameObject GetInstanceOfTroopAt(int index, int initialLevel)
    {
        Troop t = GetTroopAt(index);
        GameObject go = Instantiate(t.gameObject);
        t = go.GetComponent<Troop>();
        t.PushToLevel(initialLevel);

        return go;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    private List<Node> nodes;

    public int Size { get { return nodes.Count; } }

    public Room()
    {
        nodes = new List<Node>();
    }

    public void AddNode(Node n)
    {
        nodes.Add(n);
    }

    public Node GetNode(int index)
    {
        if(index < 0 || index >= nodes.Count)
        {
            UnityEngine.Debug.LogError("Tried to access invalid Room index of " + index.ToString() + " vs max index of " + nodes.Count.ToString());
        }
        return nodes[index];
    }
}


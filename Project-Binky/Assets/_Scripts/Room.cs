using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    private List<Node> nodes;

    public int Size { get { return nodes.Count; } }

    public Room() : this(new List<Node>()) { }

    public Room(List<Node> nodes_)
    {
        nodes = nodes_;
    }

    public void AddNode(Node n)
    {
        nodes.Add(n);
        n.SetRoom(this);
    }

    public void RemoveNode(Node n)
    {
        n.SetRoom(null);
        nodes.Remove(n);
    }

    public void RemoveNode(int index)
    {
        if (index < 0 || index >= nodes.Count)
        {
            UnityEngine.Debug.LogError("Tried to access invalid Room index of " + index.ToString() + " vs max index of " + nodes.Count.ToString());
        }
        Node n = nodes[index];
        n.SetRoom(null);
        nodes.Remove(n);
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


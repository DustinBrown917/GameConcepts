using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {
    private Point address;
    public Point Address { get { return address; } }
    private float value;
    public float Value { get { return value; } }

    public Node() : this(new Point()) { }

    public Node(Point address_) : this(address_, 0.0f) { }

    public Node(Point address_, float value_)
    {
        address = address_;
        value = value_;
    }

    public void SetAddress(int x_, int y_)
    {
        address = new Point(x_, y_);
    }

    public void SetAddress(Point p) { SetAddress(p.x, p.y); }

    public void SetValue(float val)
    {
        value = val;
    }
}


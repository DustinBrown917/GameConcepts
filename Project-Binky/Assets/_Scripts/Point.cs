using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point  {
    public int x;
    public int y;

    public Point(int x_, int y_)
    {
        x = x_;
        y = y_;
    }

    public Point(Point p) : this(p.x, p.y) { }

    public Point() : this(0, 0) { }
}

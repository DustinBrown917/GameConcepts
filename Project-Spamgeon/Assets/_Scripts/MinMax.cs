using System;
using UnityEngine;

[Serializable]
public class MinMax{

    [SerializeField] private float min_;
    public float Min { get { return min_; } }

    [SerializeField] private float max_;
    public float Max { get { return max_; } }

    public float Difference { get { return max_ - min_; } }

    public MinMax(float value1, float value2)
    {
        if(value1 > value2)
        {
            max_ = value1;
            min_ = value2;
        }
        else
        {
            max_ = value2;
            min_ = value1;
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Roads
{
    public Points StartingPoint;
    public Points EndingPoint;

    public GameObject GameObject;
    public Color RoadColor;

    public float Length;

    public string GetLengthAsString()
    {
        return Length.ToString("F2");
    }
}

public class RoadComparer : IComparer<Roads>
{
    public int Compare(Roads road1, Roads road2)
    {
        return road1.Length.CompareTo(road2.Length);
    }
}



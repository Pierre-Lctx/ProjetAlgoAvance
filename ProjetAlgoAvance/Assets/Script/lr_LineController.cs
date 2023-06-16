using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class lr_LineController : MonoBehaviour
{
    private LineRenderer lr;
    public List<Transform> points;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    public void SetUpLine(List<Points> points)
    {
        lr.positionCount = points.Count;
        foreach(Points point in points)
        {
            this.points.Add(point.gameObject.transform);
        }
    }

    private void Update()
    {
        foreach(Transform point in points)
        {
            int indexInList = points.IndexOf(point);
            for (int i = 0; i < points.Count; i++)
            {
                if (i != indexInList)
                {
                    Debug.Log("Chemin :" + i);
                    lr.SetPosition(i, point.position);
                }
            }
        }
    }
}

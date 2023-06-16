using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projet_Truck : MonoBehaviour
{
    public int TourneeID;
    public GameObject prefab;
    public Color Color;

    public float speed;

    public Projet_Truck()
    {

    }

    /// <summary>
    /// Move truck to end point
    /// </summary>
    /// <param name="endPoint">Vector3 of the point</param>
    public void MoveTruck(Transform startPoint, Transform endPoint)
    {
        Vector3 dir = (endPoint.position - startPoint.position).normalized;

        prefab.transform.position += dir * speed * Time.deltaTime;
    }
}

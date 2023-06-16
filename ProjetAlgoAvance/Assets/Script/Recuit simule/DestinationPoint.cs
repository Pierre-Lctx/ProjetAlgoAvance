using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestinationPoint : MonoBehaviour
{
    public int pointID; // Identifiant unique du point de destination
    public Vector3 position; // Position du point de destination

    public float CalculateDistance(DestinationPoint otherPoint)
    {
        return Vector3.Distance(position, otherPoint.position);
    }
}

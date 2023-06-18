using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trucks : MonoBehaviour
{
    public List<int> truckTourneeIndex;
    public List<Roads> roads;
    public int id;

    public List<float> speeds = new List<float>();

    public Trucks()
    {
        truckTourneeIndex = new List<int>();
    }
}

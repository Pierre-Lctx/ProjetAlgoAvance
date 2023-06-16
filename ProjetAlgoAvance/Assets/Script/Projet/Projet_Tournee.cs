using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projet_Tournee : MonoBehaviour
{
    public List<GameObject> Tournee = new List<GameObject>();

    public void GenerateTournee()
    {
        Tournee.Clear();

        Projet_GeneratePoint map = new Projet_GeneratePoint();

        int tourneeIndex = 0;


        foreach(Projet_Point point in map.pointsOnMap)
        {
            Tournee.Add(point.gameObject);
            if (tourneeIndex > map.numberOfTruck - 1)
            {
                tourneeIndex = 0;
            }
            else
            {
                tourneeIndex++;
            }
        }
    }
}

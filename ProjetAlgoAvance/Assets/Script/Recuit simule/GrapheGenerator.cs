using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GrapheGenerator : MonoBehaviour
{
    [Header("Parameters"), Space]
    public int NumberOfPoints = 10;

    public int MapSize = 25;


    [Header("Game Objects"), Space]
    public List<GameObject> PointsOnMap;
    public List<GameObject> paths = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        GenerateMap();
        GeneratePaths();
    }

    public void GeneratePaths()
    {
        int xIndex = 0;
        int yIndex = 0;
        foreach (GameObject originPoint in PointsOnMap)
        {
            foreach (GameObject destinationPoint in PointsOnMap)
            {
                if (originPoint != destinationPoint)
                {
                    DestinationPoint point1 = new DestinationPoint();
                    DestinationPoint point2 = new DestinationPoint();

                    point1.pointID = xIndex;
                    point2.pointID = yIndex;

                    point1.position = originPoint.transform.position;
                    point2.position = destinationPoint.transform.position;
                    CreatePhysicalRoad(point1, point2, Color.black);
                }
                yIndex++;
            }
            xIndex++;
            yIndex = 0;
        }
    }

    public void CreatePhysicalRoad(DestinationPoint originPoints, DestinationPoint endPoint, Color color)
    {
        // Créer un cube pour représenter la ligne
        float cubeLength = Vector3.Distance(originPoints.position, endPoint.position);
        Vector3 cubeScale = new Vector3(.25f, .1f, cubeLength);
        Vector3 cubePosition = (originPoints.position + endPoint.position) / 2f;
        Quaternion cubeRotation = Quaternion.LookRotation(endPoint.position - originPoints.position);
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.localScale = cubeScale;
        cube.transform.position = cubePosition;
        cube.transform.rotation = cubeRotation;

        cube.name = "Point " + originPoints.pointID + " - Point " + endPoint.pointID;

        // Appliquer la couleur noire au cube
        Renderer cubeRenderer = cube.GetComponent<Renderer>();
        cubeRenderer.material.color = color;

        paths.Add(cube);
    }

    public void GenerateMap()
    {
        for (int i = 0; i < NumberOfPoints; i++)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            go.transform.position = new Vector3(Random.Range(-MapSize, MapSize),0, Random.Range(-MapSize, MapSize));
            go.name = "Point " + (i + 1).ToString();

            Renderer renderer = go.GetComponent<Renderer>();
            renderer.material.color = Color.yellow;

            PointsOnMap.Add(go);
        }
    }
}

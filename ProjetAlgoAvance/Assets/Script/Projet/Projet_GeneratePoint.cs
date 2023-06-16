using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Projet_GeneratePoint : MonoBehaviour
{
    [Header("Game objects")]
    public GameObject prefab;
    public Camera cam;
    public Projet_Tournee tournee;

    [Header("Lists")]
    public List<GameObject> pointsGameObject = new List<GameObject>();
    public List<Projet_Point> pointsOnMap = new List<Projet_Point>();
    public List<GameObject> paths = new List<GameObject>();

    public List<Projet_Tournee> tourneeList = new List<Projet_Tournee>();

    [Header("Parameters")]
    public int sizeOfMap;
    public int numberOfPoints;

    public int numberOfTruck;

    public void Start()
    {
        cam = Camera.main;
        cam.transform.position = new Vector3(0, 2 * sizeOfMap, 0);

        GenerateMap();

        tournee.GenerateTournee();
        GenerateTourneeGameObjects();
    }

    public void GenerateMap()
    {
        for (int i = 0; i < numberOfPoints; i++)
        {
            GameObject gameObject = Instantiate(prefab, new Vector3(Random.Range(-sizeOfMap, sizeOfMap), 0, Random.Range(-sizeOfMap, sizeOfMap)), Quaternion.identity);
            gameObject.transform.localScale = new Vector3(1, 1, 1);
            gameObject.name = $"Point {i + 1}";

            Renderer renderer = gameObject.GetComponent<Renderer>();
            
            Projet_Point newPoint = gameObject.GetComponent<Projet_Point>();
            
            newPoint.gameObject = gameObject;
            newPoint.position = gameObject.transform;

            if (i == 0)
            {
                newPoint.isStartAndEnd = true;
            }
            else
            {
                newPoint.isStartAndEnd = false;
            }

            newPoint.SetColor();
            pointsGameObject.Add(newPoint.gameObject);
            pointsOnMap.Add(newPoint);
        }

        
    }

    public void GenerateTourneeGameObjects()
    {
        List<Color> colors = new List<Color>();

        for (int i = 0; i < numberOfTruck; i++)
        {
            colors.Add(new Color(Random.Range(0, 255), Random.Range(0, 255), Random.Range(0, 255)));
        }

        int colorIndex = 0;

        for (int i = 0; i < numberOfTruck; i++)
        {
            foreach (GameObject originPoint in tournee.Tournee[i])
            {
                foreach (GameObject destinationPoint in tournee.Tournee[i])
                {
                    if (originPoint != destinationPoint)
                    {
                        CreatePhysicalRoad(originPoint, destinationPoint, colors[i]);
                    }
                }
            }
        }
    }

    void CreatePhysicalRoad(GameObject originPoints, GameObject endPoint, Color color)
    {
        // Créer un cube pour représenter la ligne
        float cubeLength = Vector3.Distance(originPoints.transform.position, endPoint.transform.position);
        Vector3 cubeScale = new Vector3(.25f, .1f, cubeLength);
        Vector3 cubePosition = (originPoints.transform.position + endPoint.transform.position) / 2f;
        Quaternion cubeRotation = Quaternion.LookRotation(endPoint.transform.position - originPoints.transform.position);
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.localScale = cubeScale;
        cube.transform.position = cubePosition;
        cube.transform.rotation = cubeRotation;

        cube.name = originPoints.gameObject.name + " - " + endPoint.gameObject.name;

        // Appliquer la couleur noire au cube
        Renderer cubeRenderer = cube.GetComponent<Renderer>();
        cubeRenderer.material.color = color;

        paths.Add(cube);
    }
}

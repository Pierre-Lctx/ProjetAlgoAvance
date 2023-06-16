using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Threading;

public class MapGenerator : MonoBehaviour
{
    [Range(1, 100), Header("Parameters")]
    public int mapSize;
    public List<GameObject> gameObjectsPoints;
    public List<Points> points = new List<Points>();
    public List<Points> pointsOrdered = new List<Points>();
    [Range(1, 22)]
    public int numberOfRoads = 1;
    int lastNumberOfRoads = 1;

    bool roadsGenerated = false;

    List<Points> shortestPath = new List<Points>();
    List<GameObject> paths = new List<GameObject>();

    [Header("Roads - Color")]

    public List<GameObject> GreenRoads;
    public List<GameObject> BlueRoads;
    public List<GameObject> RedRoads;
    public List<GameObject> BlackRoads;

    bool greenIsDisplay = true;
    bool blueIsDisplay = true;
    bool redIsDisplay = true;
    bool blackIsDisplay = true;

    List<Roads> Roads = new List<Roads>();

    Points startPoint;
    Points endPoint;

    // Start is called before the first frame update
    void Start()
    {
        //GenerateMap();
        GenerateExistingPoints();
    }

    private void Update()
    {
        if (lastNumberOfRoads != numberOfRoads)
        {
            Debug.Log("Change");
            lastNumberOfRoads = numberOfRoads;
            roadsGenerated = false;
            ClearMap();
        }
        if (roadsGenerated == false)
        {
            GenerateRoads();
            FindShortestWay();
            //StartCoroutine(GenerateShortestPathWithDelay());
        }
    }

    void GenerateMap()
    {
        int numberOfPoint = 1;
        for (int i = 0; i < mapSize; i++)
        {
            Vector3 co = new Vector3(Random.Range(-25, 25), 0, Random.Range(-25, 25));

            Points pointToAdd = new Points();

            pointToAdd.position = co;
            pointToAdd.gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            if (i == 0)
            {
                pointToAdd.gameObject.name = "Start Point";
                Renderer renderer = pointToAdd.gameObject.GetComponent<Renderer>();
                renderer.material.color = Color.green;

                pointToAdd.gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            }
            else if (i == mapSize- 1)
            {
                pointToAdd.gameObject.name = "End point";
                Renderer renderer = pointToAdd.gameObject.GetComponent<Renderer>();
                renderer.material.color = Color.red;

                pointToAdd.gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            }
            else
            {
                pointToAdd.gameObject.name = "Point " + numberOfPoint;
            }

            pointToAdd.gameObject.transform.position = co;
            points.Add(pointToAdd);

            numberOfPoint++;
        }
    }

    private void GenerateShortestPaths()
    {
        float distance = 0;

        List<Points> unvisitedTown = new List<Points>(points);
        unvisitedTown.Remove(startPoint);


    }

    public void GenerateExistingPoints()
    {
        foreach(GameObject gameObject in gameObjectsPoints)
        {
            Points newPoint = new Points();

            newPoint.gameObject = gameObject;
            newPoint.position = gameObject.transform.position;

            newPoint.Name = gameObject.name;
            if (newPoint.Name == "Rouen")
            {
                newPoint.IsStart = true;

                Renderer cubeRenderer = gameObject.GetComponent<Renderer>();
                cubeRenderer.material.color = Color.green;
            }
            else if (newPoint.Name == "Bordeaux")
            {
                newPoint.IsEnd = true;

                Renderer cubeRenderer = gameObject.GetComponent<Renderer>();
                cubeRenderer.material.color = Color.red;
            }

            points.Add(newPoint);
        }
    }

    public void GenerateRoads()
    {
        int pointCount = points.Count;

        foreach(Points originPoint in points)
        {
            foreach(Points destinationPoint in points)
            {
                if (originPoint != destinationPoint)
                {
                    Roads newRoads = new Roads();
                    newRoads.StartingPoint = originPoint;
                    newRoads.EndingPoint = destinationPoint;
                    newRoads.Length = CalculateLength(originPoint, destinationPoint);

                    Roads.Add(newRoads);

                    //CreatePhysicalRoad(originPoint, destinationPoint);
                }
            }
        }
    }

    private IEnumerator GenerateShortestPathWithDelay()
    {
        List<Points> unvisitedPoints = new List<Points>(points);
        
        foreach (Points point in points)
        {
            if (point.IsStart)
            {
                startPoint = point;
                unvisitedPoints.Remove(startPoint);
                shortestPath.Clear();
                shortestPath.Add(startPoint);
            }
            else if (point.IsEnd)
            {
                endPoint = point;
                unvisitedPoints.Remove(endPoint);
            }
        }

        while (unvisitedPoints.Count > 0)
        {
            float shortestDistance = Mathf.Infinity;
            Points closestPoint = null;

            foreach (Points currentPoint in unvisitedPoints)
            {
                float distance = CalculateLength(shortestPath[shortestPath.Count - 1], currentPoint);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    closestPoint = currentPoint;
                }
            }
            shortestPath.Add(closestPoint);
            unvisitedPoints.Remove(closestPoint);
        }

        shortestPath.Add(endPoint);

        for (int i = 0; i < shortestPath.Count - 1; i++)
        {
            yield return new WaitForSeconds(1); // Pause d'une seconde
            CreatePhysicalRoad(shortestPath[i], shortestPath[i + 1], Color.black);
        }
    }

    public void ClearMap()
    {
        foreach(GameObject gameObject in paths)
        {
            Destroy(gameObject);
            paths.Remove(gameObject);
        }
    }

    private float CalculateLength(Points point1, Points point2)
    {
        return Vector3.Distance(point1.position, point2.position);
    }

    private List<Roads> OrderRoads(Points originPoint)
    {
        List<Roads> roads = new List<Roads>();

        foreach(Roads road in Roads)
        {
            if (road.StartingPoint == originPoint)
            {
                roads.Add(road);
            }
        }

        RoadComparer comparer = new RoadComparer();
        roads.Sort(comparer);

        return roads;
    }

    void CreatePhysicalRoad(Points originPoints, Points endPoint, Color color)
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

        cube.name = originPoints.Name + " - " + endPoint.Name;

        // Appliquer la couleur noire au cube
        Renderer cubeRenderer = cube.GetComponent<Renderer>();
        cubeRenderer.material.color = color;

        paths.Add(cube);

        //Ajout des roads aux listes
        if (color == Color.green)
        {
            GreenRoads.Add(cube);
        }
        else if (color == Color.blue)
        {
            BlueRoads.Add(cube);
        }
        else if (color == Color.red)
        {
            RedRoads.Add(cube);
        }
        else if (color == Color.black)
        {
            BlackRoads.Add(cube);
        }
    }
    
    public void FindShortestWay()
    {
        ClearMap();
        foreach(Points point in points)
        {
            List<Roads> roads = OrderRoads(point);
            //Debug.Log(roads.Count);

            if (roads.Count > 0)
            {
                for (int i = 0; i < numberOfRoads; i++)
                {
                    Vector3 startPos = roads[i].StartingPoint.position;
                    Vector3 endPos = roads[i].EndingPoint.position;

                    Color color = Color.white;
                    if (i == 0)
                    {
                        color = Color.green;
                    }
                    else if (i == 1)
                    {
                        color = Color.blue;
                    }
                    else if (i == 2)
                    {
                        color = Color.red;
                    }
                    else
                    {
                        color = Color.black;
                    }

                    Debug.DrawLine(startPos, endPos, color, 9999);

                    CreatePhysicalRoad(roads[i].StartingPoint, roads[i].EndingPoint, color);
                }
            }
        }

        roadsGenerated = true;
    }

    #region

    public void DisplayGreenRoads()
    {
        greenIsDisplay = !greenIsDisplay;
        
        foreach(GameObject gameObject in GreenRoads)
        {
            gameObject.SetActive(greenIsDisplay);
        }
    }

    public void DisplayBlueRoads()
    {
        blueIsDisplay = !blueIsDisplay;

        foreach (GameObject gameObject in BlueRoads)
        {
            gameObject.SetActive(blueIsDisplay);
        }
    }

    public void DisplayRedRoads()
    {
        redIsDisplay = !redIsDisplay;

        foreach (GameObject gameObject in RedRoads)
        {
            gameObject.SetActive(redIsDisplay);
        }
    }

    public void DisplayBlackRoads()
    {
        blackIsDisplay = !blackIsDisplay;

        foreach (GameObject gameObject in BlackRoads)
        {
            gameObject.SetActive(blackIsDisplay);
        }
    }

    #endregion
}

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("GameObjects"), Space]
    public Camera cam;
    public GameObject pointPrefab;
    public List<Points> pointsOnMap = new List<Points>();
    public List<GameObject> gameObjectsOnMap = new List<GameObject>();

    public List<GameObject> roadsGameObject = new List<GameObject>();
    public List<Roads> roadsList = new List<Roads>();

    public RecuitSimule recuitSimule;

    public List<Trucks> trucksLists;

    public GameObject truckPrefab;
    public List<GameObject> trucks = new List<GameObject>();

    public List<GameObject> trucksObjects;
    public GameObject truckGameObjectPrefab;

    [Header("Inputs"), Space]
    public TMP_Text mapSizeText;
    public TMP_Text numberOfPointsText;

    [Header("Texts"), Space]

    public TMP_Text PointsGenerationText;
    public TMP_Text TourneeGenerationText;
    public TMP_Text RecuitSimuleText;

    [Header("Parameters"), Space]
    public int mapSize;
    public int numberOfPoints;

    public Points OriginPoint;

    public float timeOfAction;

    public int numberOftrucks;

    public float vMoyen;
    public float vBouchon;

    public void GeneratePointsOnMap()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        ResetMap();

        for (int i = 0; i < numberOfPoints; i++)
        {
            GameObject gameObject = Instantiate(pointPrefab, new Vector3(Random.Range(-mapSize, mapSize), 0, Random.Range(-mapSize, mapSize)), Quaternion.identity);

            gameObject.name = $"Point {i + 1}";

            gameObject.transform.localScale = new Vector3(mapSize * .01f, mapSize * .01f, mapSize * .01f);

            Points point = new Points();

            point.gameObject = gameObject;
            point.transform = gameObject.transform;
            point.id = i;

            if (i == 0)
            {
                point.isStartAndEnd = true;
                OriginPoint = point;
            }
            else
            {
                point.isStartAndEnd = false;
            }

            pointsOnMap.Add(point);
            gameObjectsOnMap.Add(gameObject);
        }

        stopwatch.Stop();
        double duration = stopwatch.Elapsed.TotalSeconds;

        PointsGenerationText.text = "Points generation duration : " + duration + " sec";
    }

    public void ResetMap()
    {
        foreach (GameObject gameObject in gameObjectsOnMap)
        {
            Destroy(gameObject);
        }

        foreach (GameObject gameObject in roadsGameObject)
        {
            Destroy(gameObject);
        }

        pointsOnMap.Clear();
        roadsList.Clear();

        gameObjectsOnMap.Clear();
        roadsGameObject.Clear();
    }

    public void GenerateRoads()
    {
        foreach (Points originPoints in pointsOnMap)
        {
            foreach (Points destinationPoints in pointsOnMap)
            {
                if (originPoints != destinationPoints)
                {
                    Roads newRoads = new Roads();

                    newRoads.originPoint = originPoints;
                    newRoads.destinationPoints = destinationPoints;

                    roadsList.Add(newRoads);
                    roadsGameObject.Add(newRoads.GenerateRoad(mapSize));
                }
            }
        }
    }

    public void GenerateTournee()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        // Assurez-vous que numberOfPoints et numberOftrucks sont correctement initialisés
        numberOfPoints = pointsOnMap.Count;

        // Créez une liste contenant les indices des points (à l'exception du point d'origine)
        List<int> pointIndices = new List<int>();
        for (int i = 1; i < numberOfPoints; i++)
        {
            pointIndices.Add(i);
        }

        // Mélangez les indices de manière aléatoire
        Shuffle(pointIndices);

        // Réinitialisez les tournées
        trucksLists.Clear();

        //Création des camions de la tournée
        for (int i = 0; i < numberOftrucks; i++)
        {
            GameObject gameObject = Instantiate(truckGameObjectPrefab);
            Trucks newTruck = gameObject.GetComponent<Trucks>();
            newTruck.truckTourneeIndex = new List<int>();
            newTruck.id = i;

            newTruck.gameObject.transform.localScale = new Vector3(mapSize, mapSize, mapSize);

            trucksLists.Add(newTruck);
        }

        int currentTruckIndex = 0;
        for (int i = 1; i < pointsOnMap.Count; i++)
        {
            trucksLists[currentTruckIndex].truckTourneeIndex.Add(pointsOnMap[i].id);

            if (currentTruckIndex + 1 >= numberOftrucks)
            {
                currentTruckIndex = 0;
            }
            else
            {
                currentTruckIndex++;
            }

            //Debug.Log($"Taille de la liste de tournée du camion : {trucksLists[currentTruckIndex].truckTourneeIndex.Count}");

            /*truck.truckTourneeIndex.Insert(0, OriginPoint);
            truck.truckTourneeIndex.Add(OriginPoint);*/
        }

        stopwatch.Stop();
        double duration = stopwatch.Elapsed.TotalSeconds;

        TourneeGenerationText.text = "Tour generation duration : " + timeOfAction + " sec";
    }

    private void Shuffle<T>(List<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }


    public void GenerateShortestWay()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        recuitSimule.points = pointsOnMap;
        for (int i = 0; i < numberOftrucks; i++)
        {
            List<Transform> bestSolutionPoints = new List<Transform>();

            bestSolutionPoints.Add(pointsOnMap[0].transform);

            List<int> meilleureSolution = recuitSimule.SimulatedAnnealing(trucksLists[i].truckTourneeIndex, recuitSimule.temperature_initiale, recuitSimule.temperature_finale, recuitSimule.max_iterations, recuitSimule.alpha);

            foreach (int index in meilleureSolution)
            {
                bestSolutionPoints.Add(pointsOnMap[index].gameObject.transform);
            }

            trucksLists[i].roads = new List<Roads>();

            for (int k = 0; k < bestSolutionPoints.Count - 1; k++)
            {
                Transform originPoint = bestSolutionPoints[k];
                Transform destinationPoint = bestSolutionPoints[k + 1];

                Roads newRoad = new Roads();

                int index1 = meilleureSolution[k];
                int index2;
                if (k + 1 >= bestSolutionPoints.Count - 1)
                {
                    index2 = meilleureSolution[0];
                }
                else
                {
                    index2 = meilleureSolution[k + 1];
                }

                newRoad.originPoint = pointsOnMap[index1];
                newRoad.destinationPoints = pointsOnMap[index2];

                roadsList.Add(newRoad);
                roadsGameObject.Add(newRoad.GenerateRoad(mapSize));

                trucksLists[i].roads.Add(newRoad);
            }

            GameObject newTruck = Instantiate(truckPrefab, OriginPoint.gameObject.transform.position, Quaternion.identity);

            Color randomColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

            ChangeColorScript colorScript = newTruck.GetComponentInChildren<ChangeColorScript>();
            colorScript.color = randomColor;

            colorScript.ChangeColor();

            trucksLists[i].speeds = recuitSimule.speeds;

            trucks.Add(newTruck);
        }

        stopwatch.Stop();
        double duration = stopwatch.Elapsed.TotalSeconds;

        RecuitSimuleText.text = "Shortest way generation duration " + duration + " sec";
    }

    public void MoveAllTrucks()
    {
        for (int i = 0; i < numberOftrucks; i++)
        {
            List<Roads> truckRoads = GetTruckRoads(i);
            List<GameObject> roads = new List<GameObject>();

            foreach (Roads road in trucksLists[i].roads)
            {
                roads.Add(road.roadGameObject);
            }

            StartCoroutine(MoveTruckAlongRoads(trucksLists[i].roads, trucks[i], trucks[i].GetComponentInChildren<ChangeColorScript>().color, trucksLists[i]));
        }
    }

    public List<Roads> GetTruckRoads(int truckIndex)
    {
        List<Roads> truckRoads = new List<Roads>();

        foreach (int index in trucksLists[truckIndex].truckTourneeIndex)
        {
            if (index < roadsList.Count)
            {
                truckRoads.Add(roadsList[index]);
            }
        }

        return truckRoads;
    }

    public IEnumerator MoveTruckAlongRoads(List<Roads> roads, GameObject truck, Color OriginalColor, Trucks truckObject)
    {
        float movementSpeed = 25f;

        Vector3 initialPosition = truck.transform.position;

        //int index = 0;

        foreach (Roads road in roads)
        {
            road.ChangeColor(OriginalColor);

            //movementSpeed = truckObject.speeds[index];

            //UnityEngine.Debug.Log("Vitesse du camion : " + movementSpeed);

            Vector3 originScale = new Vector3(mapSize * 0.01f, mapSize * 0.01f, mapSize * 0.01f);
            Vector3 newScale = new Vector3(mapSize * 0.03f, mapSize * 0.03f, mapSize * 0.03f);
            road.originPoint.gameObject.transform.localScale = newScale;
            road.destinationPoints.gameObject.transform.localScale = newScale;

            float journeyLength = road.Distance;
            float journeyDuration = journeyLength / movementSpeed;
            float elapsedTime = 0.0f;

            Vector3 direction = road.destinationPoints.transform.position - road.originPoint.transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            targetRotation *= Quaternion.Euler(0, -90, 0); // Rotation de 90 degrés autour de l'axe Y

            while (elapsedTime < journeyDuration)
            {
                float t = elapsedTime / journeyDuration;
                Vector3 intermediatePosition = Vector3.Lerp(road.originPoint.transform.position, road.destinationPoints.transform.position, t);

                truck.transform.position = intermediatePosition;
                truck.transform.rotation = Quaternion.Slerp(truck.transform.rotation, targetRotation, t);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            road.originPoint.gameObject.transform.localScale = originScale;
            road.destinationPoints.gameObject.transform.localScale = originScale;

            truck.transform.position = road.destinationPoints.transform.position;
            initialPosition = truck.transform.position;

            //index++;
        }

        Destroy(truck);
    }



    private void Start()
    {
        cam = Camera.main;

        //Place cam
        cam.transform.position = new Vector3(0, mapSize * 2 + mapSize * .2f, 0);

        mapSizeText.text = mapSize.ToString();
        numberOfPointsText.text = numberOfPoints.ToString();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TSPSolver : MonoBehaviour
{
    public List<DestinationPoint> visitedPoints = new List<DestinationPoint>();
    public float maxDistance = 1000f;

    void Start()
    {
        DestinationPoint[] allPoints = FindObjectsOfType<DestinationPoint>();
        visitedPoints.AddRange(allPoints);
    }

    float EvaluateSolution()
    {
        float totalDistance = 0f;

        for (int i = 0; i < visitedPoints.Count - 1; i++)
        {
            totalDistance += visitedPoints[i].CalculateDistance(visitedPoints[i + 1]);
        }

        totalDistance += visitedPoints[visitedPoints.Count - 1].CalculateDistance(visitedPoints[0]);

        return totalDistance;
    }

    void PerturbSolution()
    {
        // Échangez deux points de destination aléatoires
        int randomIndex1 = Random.Range(1, visitedPoints.Count - 1);
        int randomIndex2 = Random.Range(1, visitedPoints.Count - 1);

        DestinationPoint temp = visitedPoints[randomIndex1];
        visitedPoints[randomIndex1] = visitedPoints[randomIndex2];
        visitedPoints[randomIndex2] = temp;
    }

    void SimulatedAnnealing()
    {
        float initialTemperature = 100f;
        float coolingRate = 0.95f;
        int maxIterations = 1000;

        List<DestinationPoint> bestSolution = new List<DestinationPoint>(visitedPoints);
        float bestDistance = EvaluateSolution();

        List<DestinationPoint> currentSolution = new List<DestinationPoint>(visitedPoints);
        float currentDistance = bestDistance;

        for (int iteration = 0; iteration < maxIterations; iteration++)
        {
            PerturbSolution();

            float newDistance = EvaluateSolution();

            if (newDistance < currentDistance)
            {
                currentDistance = newDistance;

                if (newDistance < bestDistance)
                {
                    bestDistance = newDistance;
                    bestSolution = new List<DestinationPoint>(currentSolution);
                }
            }

            if (currentDistance <= maxDistance)
            {
                break;
            }

            initialTemperature *= coolingRate;
        }

        visitedPoints = new List<DestinationPoint>(bestSolution);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SimulatedAnnealing();
        }
    }
}

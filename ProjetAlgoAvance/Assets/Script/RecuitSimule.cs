using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecuitSimule : MonoBehaviour
{
    public List<Points> points; // Modifier le type de la liste en Points
    public float temperature_initiale;
    public float temperature_finale;
    public int max_iterations;
    public float alpha;
    public float probaBouchon;
    public float vBouchon;
    public float vMoyen;
    public List<float> t;

    public List<float> speeds = new List<float>();

    public MapGenerator mapGenerator;

    private float GetLength(Transform firstPoint, Transform secondPoint)
    {
        return Mathf.Pow(secondPoint.position.x - firstPoint.position.x, 2) +
            Mathf.Pow(secondPoint.position.y - firstPoint.position.y, 2);
    }

    public List<int> SimulatedAnnealing(List<int> points, float temperature_initiale, float temperature_finale, int max_iterations, float alpha)
    {
        int num_points = points.Count;
        List<int> solution_courante = new List<int>();
        for (int i = 0; i < points.Count; i++)
        {
            solution_courante.Add(points[i]);
        }
        Shuffle(solution_courante);  // Mélanger la solution initiale
        solution_courante.Insert(0, mapGenerator.OriginPoint.id);

        /*string solutionBase = "[";
        foreach (int index in solution_courante)
        {
            solutionBase += $" {index} ";
        }
        solutionBase += "]";

        Debug.Log($"Meilleure solution de base recuit : {solutionBase}");*/

        float distance_courante = CalculateTotalDistance(solution_courante);

        List<int> meilleure_solution = new List<int>(solution_courante);
        float meilleure_distance = distance_courante;

        float temperature = temperature_initiale;

        int iteration = 0;
        while (temperature > temperature_finale && iteration < max_iterations)
        {
            speeds.Clear();
            speeds.Add(vMoyen);

            List<int> nouvelle_solution = GenerateNeighborPath(solution_courante);
            float nouvelle_distance = CalculateTotalDistance(nouvelle_solution);
            float difference_distance = nouvelle_distance - distance_courante;

            if (difference_distance < 0 || Random.value < Mathf.Exp(-difference_distance / temperature))
            {
                // Accepter la nouvelle solution
                solution_courante = nouvelle_solution;
                distance_courante = nouvelle_distance;

                // Mettre à jour la meilleure solution
                if (distance_courante < meilleure_distance)
                {
                    meilleure_solution = new List<int>(solution_courante);
                    meilleure_distance = distance_courante;
                }
            }

            temperature *= alpha;  // Réduire la température
            iteration += 1;
        }

        meilleure_solution.Add(mapGenerator.OriginPoint.id);

        /*string meilleureSolution = "[";
        foreach(int index in meilleure_solution)
        {
            meilleureSolution += $" {index} ";
        }
        meilleureSolution += "]";

        Debug.Log($"Meilleure solution du camion actuel : {meilleureSolution}");*/

        speeds.Add(vMoyen);


        return meilleure_solution;
    }

    private float CalculateTotalDistance(List<int> solution)
    {
        float total_distance = 0;
        float duree = 0;
        for (int i = 0; i < solution.Count - 1; i++)
        {
            // Calcul des index des points à récupérer
            int index1 = solution[i];
            int index2;
            if (i + 1 >= solution.Count)
            {
                index2 = solution[0];
            }
            else
            {
                index2 = solution[i + 1];
            }
            

            //Debug.Log($"Index 1 : {index1}\nIndex 2 : {index2}");

            // Récupération des points
            Transform point1 = points[index1].gameObject.transform;
            Transform point2 = points[index2].gameObject.transform;

            // Variation du trafic
            if (UnityEngine.Random.value < probaBouchon)
            {
                // Il y a des bouchons
                float tempDistance = GetLength(point1, point2) + (GetLength(point1, point2) * probaBouchon);
                duree += tempDistance / vBouchon;
                total_distance += tempDistance;

                speeds.Add(vBouchon);
            }
            else
            {
                // Il n'y a pas de bouchons
                float tempDistance = GetLength(point1, point2);
                duree += tempDistance / vMoyen;
                total_distance += tempDistance;

                speeds.Add(vMoyen);
            }

            t.Add(duree);
        }

        return total_distance;
    }

    private List<int> GenerateNeighborPath(List<int> solution)
    {
        // Fonction pour générer un voisin de la solution courante
        List<int> neighbor = new List<int>(solution);
        int index1 = UnityEngine.Random.Range(1, solution.Count - 1);
        int index2 = UnityEngine.Random.Range(1, solution.Count - 1);
        int temp = neighbor[index1];
        neighbor[index1] = neighbor[index2];
        neighbor[index2] = temp;
        return neighbor;
    }

    private void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}

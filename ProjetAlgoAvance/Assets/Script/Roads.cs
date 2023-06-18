using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class Roads
{
    public Points originPoint;
    public Points destinationPoints;

    public Color color;

    public GameObject roadGameObject;

    public float roadSpeed;

    public Roads()
    {

    }

    public GameObject GenerateRoad(float mapSize)
    {
        // Créer un cube pour représenter la ligne
        Vector3 cubeScale = new Vector3(mapSize * .0025f, mapSize * .001f, Distance);
        Vector3 cubePosition = (originPoint.transform.position + destinationPoints.transform.position) / 2f;
        Quaternion cubeRotation = Quaternion.LookRotation(destinationPoints.transform.position - originPoint.transform.position);
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.localScale = cubeScale;
        cube.transform.position = cubePosition;
        cube.transform.rotation = cubeRotation;

        cube.name = originPoint.gameObject.name + " - " + destinationPoints.gameObject.name;

        cube.AddComponent<Renderer>();

        // Appliquer la couleur noire au cube
        Renderer cubeRenderer = cube.GetComponent<Renderer>();
        cubeRenderer.material.color = color;

        roadGameObject = cube;

        return cube;
    }

    public void ChangeColor(Color color)
    {
        Renderer renderer = roadGameObject.GetComponent<Renderer>();
        renderer.material.color = color;
    }

    public float Distance { get { return GetLength(); } }

    private float GetLength()
    {
        return Vector3.Distance(originPoint.gameObject.transform.position, destinationPoints.gameObject.transform.position);
    }
}

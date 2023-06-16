using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Projet_Point : MonoBehaviour
{
    public Transform position;
    public GameObject gameObject;

    public bool isStartAndEnd = false;

    public Projet_Point(Transform position, GameObject gameObject)
    {
        this.position = position;
        this.gameObject = gameObject;
    }

    public void SetColor()
    {
        if (isStartAndEnd)
        {
            Renderer renderer = gameObject.GetComponent<Renderer>();
            renderer.material.color = Color.yellow;
        }
        else
        {
            Renderer renderer = gameObject.GetComponent<Renderer>();
            renderer.material.color = Color.white;
        }
    }
}

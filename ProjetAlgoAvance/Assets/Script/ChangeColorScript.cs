using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColorScript : MonoBehaviour
{
    public Color color;
    private void Update()
    {
        ChangeColor();
    }

    public void ChangeColor()
    {
        Renderer renderer = gameObject.GetComponent<Renderer>();
        renderer.material.color = color;
    }
}

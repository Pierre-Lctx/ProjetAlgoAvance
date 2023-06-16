using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projet_PointObject : MonoBehaviour
{
    public Vector3 startScale;
    void Awake()
    {
        startScale = gameObject.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                GameObject selectedObject = hit.collider.gameObject;
                // Faites quelque chose avec l'objet sélectionné
                Debug.Log("Objet sélectionné : " + selectedObject.name);

                selectedObject.transform.localScale = new Vector3(10,10,startScale.z);
            }
            else
            {
                gameObject.transform.localScale = startScale;
            }
        }
    }
}

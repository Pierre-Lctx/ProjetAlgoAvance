using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class UIController : MonoBehaviour
{
    public MapGenerator mapGenerator;

    public TMP_InputField numberOfPointsInput;
    public TMP_InputField mapSizeInput;
    public TMP_InputField numberOfTrucks;

    // Start is called before the first frame update
    void Start()
    {
        numberOfPointsInput.text = mapGenerator.numberOfPoints.ToString();
        mapSizeInput.text = mapGenerator.mapSize.ToString();
        numberOfTrucks.text = mapGenerator.numberOftrucks.ToString();
    }

    private void Update()
    {
        if (mapGenerator.mapSize != int.Parse(mapSizeInput.text) && int.Parse(mapSizeInput.text) > 0)
            ChangeMapSize();
        if (mapGenerator.numberOfPoints != int.Parse(numberOfPointsInput.text) && int.Parse(numberOfPointsInput.text) > 0)
            ChangeNumberOfPoints();
        if (mapGenerator.numberOftrucks != int.Parse(numberOfTrucks.text) && int.Parse(numberOfTrucks.text) > 0)
            ChangeNumberOfTrucks();
    }

    public void ChangeMapSize()
    {
        mapGenerator.mapSize = int.Parse(mapSizeInput.text);
    }

    public void ChangeNumberOfPoints()
    {
        mapGenerator.numberOfPoints = int.Parse(numberOfPointsInput.text);
    }

    public void ChangeNumberOfTrucks()
    {
        mapGenerator.numberOftrucks = int.Parse(numberOfTrucks.text);
    }
}

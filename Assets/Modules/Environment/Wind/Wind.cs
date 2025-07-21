using System;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    [Header("Terrain Sizing")]
    // This will be helpful for accessing the grid size and what not
    public PerlinNoiseTerrainGeneration TerrainGenerator;
    [HideInInspector]
    public float Width;
    [HideInInspector]
    public float Height;

    [Header("Wind Array")]
    // * Wind Array
    [Tooltip("Number of cells per square meter. Make sure that this goes into the grid size evenly")]
    public float WindCellDensity;
    public float MaximumY;
    public Vector3 VectorSeed;
    public Vector3[,] WindArr;
    public List<Vector3> WindArrElements;
    public float WindVectorHeight;

    [Header("Prefabs")]
    public GameObject WindArrow;
    public GameObject WindCellObj;

    // * Private Variables
    private Color[] Gradient;

    void Awake()
    {
        // * Wind Vector
        MaximumY = .15f;
        VectorSeed = new Vector3(
            UnityEngine.Random.Range(-1f, 1f),
            UnityEngine.Random.Range(-MaximumY, MaximumY),
            UnityEngine.Random.Range(-1f, 1f)
        );


        //* Gradient
        Gradient = new Color[100];
        List<Color> gradientColors = new List<Color> { Color.blue, Color.green, Color.red, Color.yellow, Color.white };
        // ? What's a better name for this?
        float[] gradientColorValues = { 0, .25f, .5f, .75f, 1f };

        Gradient = CustomGradient.GenerateCustomGradient(gradientColors, gradientColorValues, Gradient.Length);
    }


    void Start()
    {

        // * Grid Size
        Width = TerrainGenerator.Width;
        Height = TerrainGenerator.Height;
        WindArr = new Vector3[(int)(Height * WindCellDensity), (int)(Width * WindCellDensity)];
        WindArr[0, 0] = VectorSeed;

        // * Grid Generation
        IterateOverWindArray(WindArr, GenerateWindVector);

        // * Grid Debugging
        WindArrElements = new List<Vector3>();
        for (int i = 0; i < WindArr.GetLength(0); i++)
        {
            for (int j = 0; j < WindArr.GetLength(1); j++)
            {
                WindArrElements.Add(WindArr[i, j]);
            }
        }

        // * Wind Cell Generation
        IterateOverWindArray(WindArr, GenerateWindCellObject);

        // * Wind Vector visualization
        IterateOverWindArray(WindArr, VisualizeWindCurrentVector);
    }

    private void GenerateWindCellObject(Vector3[,] windArr, int i, int j)
    {
        // Instantiate the object at a specific location; note that we are using WindVectorheight / 2 to define the center of the cell
        GameObject windCellObj = Instantiate(WindCellObj, new Vector3(i - Width / 2, WindVectorHeight / 2, j - Height / 2), Quaternion.identity);

        // Resize the cell apprpriately: We want it to touch the ground, and go up to where the arrow would be
        windCellObj.transform.localScale = new Vector3(1f, WindVectorHeight, 1f);

        // Gain access to the script
        WindCell windCell = windCellObj.GetComponent<WindCell>();

        // Assign the direction and (later the) magnitude
        windCell.WindVector = windArr[i, j];
    }

    private void VisualizeWindCurrentVector(Vector3[,] windArr, int i, int j)
    {
        GameObject windArrow = Instantiate(WindArrow, new Vector3(i - Width / 2, WindVectorHeight, j - Height / 2), Quaternion.identity);
        windArrow.transform.forward = windArr[i, j];
        // Arbitrarily setting the numerical value to get a particular color
        // ? Let's try using (0, 0, 1) as the north vector
        float gradientIndexer = .5f * Vector3.Dot(Vector3.forward, windArrow.transform.forward) + .5f;
        // Debug.Log($"The arrow at ({i},{j}) is has a value of {gradientIndexer}");
        windArrow.GetComponent<Renderer>().material.color = CustomGradient.GetColorFromGradient(Gradient, gradientIndexer);
    }

    private void GenerateWindVector(Vector3[,] windArr, int i, int j)
    {
        // Debug.Log($"(Wihin GenerateWindVector) Starting to execute for element at ({i},{j})");
        // The seed vector starts at 0,0, so we can skip it
        if (i == 0 && j == 0)
        {
            // Nothing
        }

        // Top row
        else if (i == 0)
        {
            // Debug.Log($"Top row element: {(i, j)}");
            // Adjacent wind vectors on the edges could have more variation
            windArr[i, j] = GenerateWindVector(windArr[0, j - 1], .35f);
            // Debug.Log($"windArr[0, j - 1] is: {windArr[0, j - 1]}, deviation is {.25f}, and resulting vector is {windArr[i,j]}");
        }

        else if (j == 0)
        {
            // Debug.Log($"Left column element: {(i, j)}");
            // Adjacent wind vectors on the edges could have more variation
            windArr[i, j] = GenerateWindVector(windArr[i - 1, 0], .35f);
            // Debug.Log($"windArr[0, j - 1] is: {windArr[i - 1, 0]}, deviation is {.25f}, and resulting vector is {windArr[i,j]}");
        }

        else
        {
            // Generate an array of left, top, and previous diagonal vectors
            Vector3[] neighboringVectors = { windArr[i - 1, j - 1], windArr[i - 1, j - 1], windArr[i - 1, j - 1] };

            // Obtain the average vector of the neighbors
            Vector3 avgNeighborVector = AverageWindVector(neighboringVectors);

            // Here, we are using more neighbors to decide the next wind vector
            windArr[i, j] = GenerateWindVector(avgNeighborVector);
        }
    }

    private void IterateOverWindArray(Vector3[,] windArr, Action<Vector3[,], int, int> operation)
    {
        // Get dimensions of array
        int numRows = windArr.GetLength(0);
        int numCols = windArr.GetLength(1);

        // Set stride length
        int strideAcrossRows = (int)(Width / numCols);
        int strideDownCols = (int)(Height / numRows);

        Debug.Log($"numRows = {numRows}, numCols = {numCols}, strideAcrossRows = {strideAcrossRows}, strideDownCols = {strideDownCols}, width = {Width}, height = {Height}");

        // Iterate over rows
        for (int i = 0; i < numRows; i += strideDownCols)
        {
            // Iterate over columns
            for (int j = 0; j < numCols; j += strideAcrossRows)
            {
                // Execute the operation
                operation(windArr, i, j);
            }
        }
    }


    private Vector3 GenerateWindVector(Vector3 prevVector, float deviation = .2f)
    {
        Vector3 newVector = new Vector3(
            ClampedRandomRange(prevVector.x, deviation, -1f, 1f),
            ClampedRandomRange(prevVector.y, deviation, -MaximumY, MaximumY),
            ClampedRandomRange(prevVector.z, deviation, -1f, 1f)
        );

        return newVector.normalized;
    }

    private Vector3 AverageWindVector(Vector3[] windVectors)
    {
        // Create a placeholder for values
        Vector3 avgWindVector = Vector3.zero;

        // Sum up the vectors
        foreach (Vector3 windVector in windVectors)
        {
            avgWindVector += windVector;
        }

        // Find the average vector
        avgWindVector /= windVectors.Length;

        // ! For now, we ware normalizing the vector
        return avgWindVector.normalized;
    }

    private float ClampedRandomRange(float centralValue, float deviation, float leftClamp, float rightClamp)
    {
        float randomValue = UnityEngine.Random.Range(centralValue - deviation, centralValue + deviation);

        // Clamping to bottom of acceptable range
        randomValue = Mathf.Max(leftClamp, randomValue);

        // Clamping to top of acceptable range
        randomValue = Mathf.Min(rightClamp, randomValue);

        return randomValue;
    }



}

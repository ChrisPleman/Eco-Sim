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
    public float MaximumY;
    public Vector3 VectorSeed;
    public Vector3[,] WindArr;
    public List<Vector3> WindArrElements;

    [Header("Prefabs")]
    public GameObject WindArrow;

    void Awake()
    {
        MaximumY = .15f;
        VectorSeed = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-MaximumY, MaximumY),
            Random.Range(-1f, 1f)
        );
    }


    void Start()
    {
        Width = TerrainGenerator.Width;
        Height = TerrainGenerator.Height;
        // WindArr = new float[(int)Height, (int)Width];
        WindArr = new Vector3[3, 3];
        WindArr[0, 0] = VectorSeed;

        // ! This is too much hardcoding. In the end, I will use either the global variables, or just access the n-rows and n-cols
        GenerateWindArray(3, 3);

        WindArrElements = new List<Vector3>();
        for (int i = 0; i < WindArr.GetLength(0); i++)
        {
            for (int j = 0; j < WindArr.GetLength(1); j++)
            {
                WindArrElements.Add(WindArr[i, j]);
            }
        }

        VisualizeWindCurrents();
    }

    private void VisualizeWindCurrents()
    {
        for (int i = 0; i < WindArr.GetLength(0); i++)
        {
            for (int j = 0; j < WindArr.GetLength(1); j++)
            {
                GameObject windArrow = Instantiate(WindArrow, new Vector3(i, 3, j), Quaternion.identity);
                windArrow.transform.forward = WindArr[i, j];
            }
        }
    }

    private void GenerateWindArray(int numRows, int numCols)
    {
        for (int i = 0; i < numRows; i++)
        {
            // The seed vector starts at 0,0, so we can skip
            for (int j = 1; j < numCols; j++)
            {
                // Top row
                if (i == 0)
                {
                    // Adjacent wind vectors on the edges could have more variation
                    WindArr[i, j] = GenerateWindVector(WindArr[0, j - 1], .25f);
                    continue;
                }

                if (j == 0)
                {
                    // Adjacent wind vectors on the edges could have more variation
                    WindArr[i, j] = GenerateWindVector(WindArr[i - 1, 0], .25f);
                    continue;
                }

                // For now, we will only use the previous diagonals
                WindArr[i, j] = GenerateWindVector(WindArr[i - 1, j - 1]);
            }
        }
    }


    private Vector3 GenerateWindVector(Vector3 prevVector, float deviation = .1f)
    {
        Vector3 newVector = new Vector3(
            ClampedRandomRange(prevVector.x, deviation, -1f, 1f),
            ClampedRandomRange(prevVector.y, deviation, -MaximumY, MaximumY),
            ClampedRandomRange(prevVector.z, deviation, -1f, 1f)
        );

        return newVector.normalized;
    }

    private float ClampedRandomRange(float centralValue, float deviation, float leftClamp, float rightClamp)
    {
        float randomValue = Random.Range(centralValue - deviation, centralValue + deviation);

        // Clamping to bottom of acceptable range
        randomValue = Mathf.Max(leftClamp, randomValue);

        // Clamping to top of acceptable range
        randomValue = Mathf.Min(rightClamp, randomValue);

        return randomValue;
    }



}

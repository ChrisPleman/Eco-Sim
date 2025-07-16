using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{

    public List<GameObject> TerrainTypes;

    public List<float> GridSize;

    private List<GameObject> GeneratedTiles = new List<GameObject>();
    private List<int> BaseTilePool = new List<int>();

    void Awake()
    {
        StartCoroutine(TerrainGeneration());
    }

    IEnumerator TerrainGeneration()
    {
        Debug.Log(BaseTilePool);
        // Create a newly generated index list
        for (int i = 0; i < TerrainTypes.Count; i++)
        {
            BaseTilePool.Add(i);
        }

        float tileSize = TerrainTypes[0].transform.localScale.x;

        // int[,] grid = new int[(int)GridSize[0],(int)GridSize[1]];

        for (float i = 0; i < GridSize[0] + tileSize; i += tileSize)
        {
            for (float j = 0; j < GridSize[1] + tileSize; j += tileSize)
            {
                int randomTileIndex = 0;
                float x_coord = i - GridSize[0] / 2;
                float z_coord = j - GridSize[1] / 2;
                Vector3 coord = new Vector3(x_coord, 0f, z_coord);

                // Copy list
                List<int> CurrentTilePool = new List<int>(BaseTilePool);

                if (GeneratedTiles.Count > 0)
                {
                    foreach (GameObject generatedTile in GeneratedTiles)
                    {
                        float generatedTileToNextTileDistance = Vector3.Distance(generatedTile.transform.position, coord);
                        if (generatedTileToNextTileDistance <= Mathf.Sqrt(2) * tileSize)
                        {
                            // Should yield 2 for tiles on the diagonal and 1 for adjacent
                            float distanceFactor = Mathf.Ceil(generatedTileToNextTileDistance / tileSize);

                            // I want to add 1 if on diagonal, or 2 if on adjacent
                            distanceFactor = 4 * (1 / distanceFactor);

                            // Get the index associated with the particular tile
                            int indexToAdd = TerrainTypes.IndexOf(generatedTile);

                            for (int z = 0; z < (int)distanceFactor; z++)
                            {
                                CurrentTilePool.Add(indexToAdd);
                            }
                        }
                    }

                    randomTileIndex = Random.Range(0, CurrentTilePool.Count);
                    randomTileIndex = CurrentTilePool[randomTileIndex];
                }
                else
                {
                    randomTileIndex = Random.Range(0, TerrainTypes.Count);
                }
                Debug.Log($"Placing a tile index {randomTileIndex} at {x_coord} , {z_coord}");
                Instantiate(TerrainTypes[randomTileIndex], new Vector3(x_coord, 0, z_coord), Quaternion.identity);

                yield return new WaitForSeconds(.1f);
            }
        }
    }
}

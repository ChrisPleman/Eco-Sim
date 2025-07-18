using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class PerlinNoiseTerrainGeneration : MonoBehaviour
{
    [Header("Grid Dimensions")]
    public float Width;
    public float Height;

    [Header("Tile & Tile Settings")]

    public GameObject Tile;

    [Range(.1f, 1f)]
    public float TileSize;

    // public float SeaLevel;

    [Header("Texture Settings")]

    [Range(.1f, 5f)]
    public float Zoom;

    [Header("Terrain Coloration")]
    public Color DebugColor;
    [Tooltip("The higher the granularity the more smooth the coloration will be")]
    [Range(1, 5)]
    public int TextureGranularity;

    public List<Color> TerrainColors;
    public float[] TerrainColorHeights;


    // * Private variables

    private float TextureScale;
    private List<Color> currTerrainColors;
    private float lastTimeColorChanged;
    private float[] currTerrainHeights;
    private float lastTimeHeightChanged;

    private List<GameObject> Tiles;

    void Awake()
    {
        Tiles = new List<GameObject>();

        // * Adjust the TileSize to ensure it can evenly fit into 1
        List<float> validTileSizes = new List<float> { .1f, .125f, .25f, .5f, 1f };
        float minDistance = 1f;
        float closestTileSize = 0;
        for (int i = 0; i < validTileSizes.Count; i++)
        {
            float currDistance = Mathf.Abs(TileSize - validTileSizes[i]);
            if (currDistance < minDistance)
            {
                minDistance = currDistance;
                closestTileSize = validTileSizes[i];
            }
        }

        TileSize = closestTileSize;

        // ! Doing this here permanently changes the scale of the prefab. If I want to change each individual one, I'd have to change it after instantiating (Probably..)
        Tile.transform.localScale = new Vector3(TileSize, .1f, TileSize);

        // I want round to the thousands place
        float precision = Mathf.Pow(10f, 3f);
        // We are dividing by Zoom, because we want to invert the behavior such that larger Zoom values "zoom in" on the texture, and vice versa
        TextureScale = Mathf.Round(precision / Zoom) / precision;

        currTerrainColors = CopyColorsList();
        currTerrainHeights = CopyHeightArray();

        GenerateTerrain();

        Debug.Log($"{Tiles.Count}");
    }

    void Start()
    {
        lastTimeColorChanged = Time.time;
    }

    void Update()
    {
        // If the terrain colors have changed, then we need to update the colors
        if (!ListElementsAreEqual(TerrainColors, currTerrainColors) && Time.time - lastTimeColorChanged > 1f)
        {
            UpdateTerrainColor();

            lastTimeColorChanged = Time.time;
        }

        currTerrainColors = CopyColorsList();

        // If the terrain color heights have changed, then we need to update the colors
        if (!ArrayElementsAreEqual(TerrainColorHeights, currTerrainHeights) && Time.time - lastTimeHeightChanged > 1f)
        {
            UpdateTerrainColor();

            lastTimeHeightChanged = Time.time;
        }

        float[] currTerrainColorHeights = CopyHeightArray();
    }

    private void GenerateTerrain()
    {
        IterateOverTiles(GenerateTile);
    }

    private void UpdateTerrainColor()
    {
        foreach (GameObject tile in Tiles)
        {
            // Get the tile's height
            float tileHeight = tile.GetComponent<PerlinNoiseTile>().Height;

            // Update the tile color
            tile.GetComponent<Renderer>().material.color = CustomGradient(tileHeight);            
        }
    }

    private void IterateOverTiles(Action<float, float> operation)
    {
        int counter = 0;
        // Since our tiles will be squares that we know were originally set to be 1 x 1 x 1 cubes, we can use the TileSize variable 
        // to increment through the loop
        for (float x = 0; x < Width; x += TileSize)
        {
            for (float z = 0; z < Height; z += TileSize)
            {
                operation(x, z);
                counter++;
            }
        }

        Debug.Log($"Number of operations completed: {counter}");
    }

    private List<Color> CopyColorsList()
    {
        // Debug.Log("Copying Colors list!");
        // Copy the rgb values of the terrain colors
        List<Color> copiedColors = new List<Color>(TerrainColors.Count);
        for (int i = 0; i < TerrainColors.Count; i++)
        {
            // Debug.Log($"The color being copied is {TerrainColors[i]}");
            // Get the rgb colors of the current terrain color
            copiedColors.Add(TerrainColors[i]);
            // new float[] { TerrainColors[i].r, TerrainColors[i].g, TerrainColors[i].b };
            // Debug.Log($"The new color at index {i} is {copiedColors[i]}");
        }

        return copiedColors;
    }

    private bool ListElementsAreEqual(List<Color> list1, List<Color> list2)
    {
        for (int i = 0; i < list1.Count; i++)
        {
            if (list1[i] != list2[i])
            {
                return false;
            }
        }

        return true;
    }

    private float[] CopyHeightArray()
    {
        // Debug.Log("Copying Colors list!");
        // Copy the rgb values of the terrain colors
        float[] copiedColorHeights = new float[TerrainColorHeights.Length];
        for (int i = 0; i < TerrainColors.Count; i++)
        {
            copiedColorHeights[i] = TerrainColorHeights[i];
        }

        return copiedColorHeights;
    }

    private bool ArrayElementsAreEqual(float[] arr1, float[] arr2)
    {
        for (int i = 0; i < arr1.Length; i++)
        {
            if (arr1[i] != arr2[i])
            {
                return false;
            }
        }

        return true;
    }

    private void GenerateTile(float x, float z)
    {
        // Debug.Log($"({x - Width / 2f}, 0, {z - Height / 2f})");
        // Instantiate and gain access to the current tile
        GameObject newTile = Instantiate(Tile, new Vector3(x - Width / 2f, 0, z - Height / 2f), Quaternion.identity);

        // Get the material on the current tile
        Material newTileMaterial = newTile.GetComponent<Renderer>().material;

        // Generate a random color for now, so that we can see the different tiles generated
        // The input needs to be float values, since the PerlinNoise function repeats at integer values
        float randomHeightValue = Mathf.PerlinNoise(TextureScale * x / Width, TextureScale * z / Height);

        // Clamp to max of 1
        randomHeightValue = Mathf.Min(1f, randomHeightValue);

        // Update the height value of the tile
        newTile.GetComponent<PerlinNoiseTile>().Height = randomHeightValue;

        // // Setting everything below sea level to 0:
        // if (randomHeightValue <= SeaLevel)
        // {
        //     randomHeightValue = 0f;
        // }
        // Debug.Log($"With Zoom set as {TextureScale} and input to the noise function as ({x * TextureScale},{z * TextureScale}), the color at pixel ({x},{z}) is {randomHeightValue}");
        newTileMaterial.color = CustomGradient(randomHeightValue);

        // Save the tile
        Tiles.Add(newTile);
    }


    // private void UpdateTileColor()
    // {
    //     // int index = (int)(x * Height / TileSize * TileSize + z / TileSize);
    //     // Debug.Log($"x is {x}");
    //     // Debug.Log($"z is {z}");
    //     // Debug.Log($"Changing index: {index}");

    //     foreach (GameObject tile in Tiles)
    //     {
    //         // Get the tile's height
    //         float tileHeight = tile.GetComponent<PerlinNoiseTile>().Height;

    //         // Update the tile color
    //         tile.GetComponent<Renderer>().material.color = CustomGradient(tileHeight);            
    //     }
    // }





    private Color CustomGradient(float heightValue)
    {
        // Control how smoothly colors change
        heightValue = Mathf.Round(Mathf.Pow(10, TextureGranularity) * heightValue) / Mathf.Pow(10, TextureGranularity);

        // Start from the second color, and if in right band, compare to previous
        for (int i = 1; i < TerrainColors.Count; i++)
        {
            Color terrainColor = TerrainColors[i];
            float terrainColorHeight = TerrainColorHeights[i];

            if (heightValue <= terrainColorHeight)
            {
                return Color.Lerp(
                    TerrainColors[i - 1],
                    terrainColor,
                    // We are finding how far along we are WITHIN the current color range
                    (heightValue - TerrainColorHeights[i - 1]) / (terrainColorHeight - TerrainColorHeights[i - 1])
                );
            }
        }

        Debug.Log(heightValue);
        // Can help to debug
        return DebugColor;
    }
    



    // private Color CustomGradient(float heightValue)
    // {
    //     // Get the next color index
    //     int rightColorIndex = (int)Math.Ceiling(heightValue * Colors.Count);

    //     if (rightColorIndex == Colors.Count)
    //     {
    //         rightColorIndex -= 1;
    //     }

    //     int leftColorIndex = rightColorIndex - 1;
    //     try
    //     {
    //         return Color.Lerp(Colors[leftColorIndex], Colors[rightColorIndex], heightValue);
    //     }
    //     catch (System.Exception e)
    //     {
    //         Debug.Log(heightValue);
    //         Debug.Log(Math.Ceiling(heightValue * Colors.Count));
    //         Debug.Log((int)Math.Ceiling(heightValue * Colors.Count));
    //         Debug.Log($"{leftColorIndex}, {rightColorIndex}");
    //         throw e;
    //     }
    // }
}

using System;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoiseTerrainGeneration : MonoBehaviour
{
    [Header("Grid Dimensions")]
    public float Width;
    public float Height;

    [Header("Tile & Tile Settings")]

    public GameObject Tile;

    public enum TilesResolution
    {
        one = 1,
        two = 2,
        four = 4,
        five = 5,
        eight = 8,
        ten = 10
    }
    public TilesResolution TilesPerMeter;
    private float TileSize;

    // public float SeaLevel;

    [Header("Texture Settings")]

    [Range(.1f, 5f)]
    public float Zoom;
    [Range(1, 5)]
    public int ZoomPrecision;
    public enum NoiseLayers
    {
        zero = 0,
        one = 1,
        two = 2,
        three = 3,
        four = 4,
        five = 5,
        six = 6,
        seven = 7,
        eight = 8,
        nine = 9,
        ten = 10
    }

    public NoiseLayers AdditionalNoiseLayers;

    [Header("Terrain Coloration")]
    public Color DebugColor;
    [Tooltip("The higher the granularity the more smooth the coloration will be")]
    [Range(1, 5)]
    public int TextureGranularity;

    public List<Color> TerrainColors;
    public float[] TerrainColorHeights;
    private Color[] Gradient;
    private int NumColorsInGradient;


    // * Private variables

    private float TextureScale;
    private List<Color> currTerrainColors;
    private float lastTimeColorChanged;
    private float[] currTerrainHeights;
    private float lastTimeHeightChanged;

    private List<GameObject> Tiles;

    void Awake()
    {
        // * Tile Size
        TileSize = 1 / (float)TilesPerMeter;

        // * Creating an empty list for the tiles to be generated
        Tiles = new List<GameObject>();

        // ! Doing this here permanently changes the scale of the prefab. If I want to change each individual one, I'd have to change it after instantiating (Probably..)
        Tile.transform.localScale = new Vector3(TileSize, .1f, TileSize);

        // * Texture Scale
        SetTextureZoom(ZoomPrecision);

        // * Custom Color Gradient
        NumColorsInGradient = 100;
        Gradient = CustomGradient.GenerateCustomGradient(TerrainColors, TerrainColorHeights, NumColorsInGradient);

        // foreach (Color color in Gradient)
        // {
        //     Debug.Log(color);
        // }

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
            Gradient = CustomGradient.GenerateCustomGradient(TerrainColors, TerrainColorHeights, NumColorsInGradient);

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
            tile.GetComponent<Renderer>().material.color = CustomGradient.GetColorFromGradient(Gradient, tileHeight);
        }
    }

    private void IterateOverTiles(Action<float, float> operation)
    {
        int counter = 0;
        // Since our tiles will be squares that we know were originally set to be 1 x 1 x 1 cubes, we can use the TileSize variable 
        // to increment through the loop
        for (float x = 0; x < Height; x += TileSize)
        {
            for (float z = 0; z < Width; z += TileSize)
            {
                operation(x, z);
                counter++;
            }
        }

        Debug.Log($"Number of tile operations completed: {counter}");
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

        for (int i = 0; i < (int)AdditionalNoiseLayers; i++)
        {
            float textureScaleFactor = Mathf.Pow(2f * (i+1) + 1f, -2f - i);
            float factor = Mathf.Pow(.5f, i + 2);
            float noiseValue = Mathf.PerlinNoise(
                textureScaleFactor * TextureScale * x,
                textureScaleFactor * TextureScale * z
            ) - .5f;
            // Debug.Log($"2f * i + 1f = {2f * i + 1f}, -1f - i = {-1f - i}, and Mathf.Pow(2f * i + 1f, -1f - i) = {Mathf.Pow(2f * i + 1f, -1f - i)}");
            // Debug.Log($"x is {x}, textureScaleFactor is {textureScaleFactor}, TextureScale is {TextureScale}, and z is {z}");
            // Debug.Log($"x arg is set to {textureScaleFactor * TextureScale * x}. z arg is set to {textureScaleFactor * TextureScale * z}.");
            // Debug.Log($"The textureScaleFactor is {textureScaleFactor}, the factor is {factor}, and the noiseValue is {noiseValue}");
            randomHeightValue += noiseValue * factor;

        }

        // * Clamping range
        // ? The more appropriate approach would likely be to see the texture ahead of time, and then normalizing all of the values
        // Clamp to max of 1
        randomHeightValue = Mathf.Min(1f, randomHeightValue);
        // Clamp to a min of 0
        randomHeightValue = Mathf.Max(0f, randomHeightValue);

        // Update the height value of the tile
        PerlinNoiseTile newTileClass = newTile.GetComponent<PerlinNoiseTile>();
        newTileClass.Height = randomHeightValue;

        newTileMaterial.color = CustomGradient.GetColorFromGradient(Gradient, randomHeightValue);

        // Assign a type to the tile; start at second element to be able to compare previous values
        for (int i = 1; i < TerrainColorHeights.Length; i++)
        {
            // Between arr[i-1] and arr[i]
            if (randomHeightValue <= TerrainColorHeights[i])
            {
                float heightBand = TerrainColorHeights[i] - TerrainColorHeights[i - 1];
                // If below halfway point between curr and previous color, then it's the previous terrain type
                if (randomHeightValue < TerrainColorHeights[i] - heightBand / 2f)
                {
                    newTileClass.TileType = (TypesOfTile)(i - 1);
                    break;
                }
                else
                {
                    newTileClass.TileType = (TypesOfTile)i;
                    break;
                }

            }
        }

        // Save the tile
        Tiles.Add(newTile);
    }

    // Adjust the scale of the perlin noise texture and its precision
    private void SetTextureZoom(int precision)
    {
        // I want round to the thousands place
        float ZoomPrecision = Mathf.Pow(10f, precision);
        // We are dividing by Zoom, because we want to invert the behavior such that larger Zoom values "zoom in" on the texture, and vice versa
        TextureScale = Mathf.Round(ZoomPrecision / Zoom) / ZoomPrecision;
    }

    private void TBD()
    {
        
    }
}

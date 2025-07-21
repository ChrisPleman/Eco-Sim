using System.Collections.Generic;
using UnityEngine;

public static class CustomGradient
{

    public static Color GetColorFromGradient(Color[] gradient, float heightValue)
    {
        // This will help us to convert the heightValue (float) to an index (int)
        int precision = gradient.Length;

        int colorIndex = (int)(heightValue * precision);

        // Ensure that we don't trigger an IndexOutOfRange error
        if (colorIndex == precision)
        {
            colorIndex -= 1;
        }

        return gradient[colorIndex];
    }

    public static Color[] GenerateCustomGradient(List<Color> colors, float[] colorHeights, int numColors)
    {
        // This will house the color gradient
        Color[] gradient = new Color[numColors];

        for (int i = 0; i < numColors; i++)
        {
            // The height value is the inverse of the current index
            float heightValue = (float)i / numColors;

            // We need to see which colors to choose
            for (int j = 1; j < colorHeights.Length; j++)
            {
                // We are within the two color values to create a sub-gradient
                if (heightValue <= colorHeights[j])
                {
                    try
                    {
                        gradient[i] = Color.Lerp(
                        colors[j - 1],
                        colors[j],
                        // Normalized distance along the desired sub-gradient
                        (heightValue - colorHeights[j - 1]) / (colorHeights[j] - colorHeights[j - 1])
                    );
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"The current value of 'i' is {i}");
                        throw e;
                    }

                    // We don't need to keep checking
                    break;
                }
            }
        }

        return gradient;
    }


    // ! TBD: I don't know if this is redundant, since I have a method that can create a gradient, and another that can access a color from gradient array

    // private Color CustomGradient(float heightValue)
    // {
    //     // Control how smoothly colors change
    //     heightValue = Mathf.Round(Mathf.Pow(10, TextureGranularity) * heightValue) / Mathf.Pow(10, TextureGranularity);

    //     // Start from the second color, and if in right band, compare to previous
    //     for (int i = 1; i < TerrainColors.Count; i++)
    //     {
    //         Color terrainColor = TerrainColors[i];
    //         float terrainColorHeight = TerrainColorHeights[i];

    //         if (heightValue <= terrainColorHeight)
    //         {
    //             return Color.Lerp(
    //                 TerrainColors[i - 1],
    //                 terrainColor,
    //                 // We are finding how far along we are WITHIN the current color range
    //                 (heightValue - TerrainColorHeights[i - 1]) / (terrainColorHeight - TerrainColorHeights[i - 1])
    //             );
    //         }
    //     }

    //     Debug.Log(heightValue);
    //     // Can help to debug
    //     return DebugColor;
    // }
}

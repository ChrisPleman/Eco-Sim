using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
public enum TypesOfTile
{
    DeepWater,

    Water,
    Sand,
    // Dirt,
    Grass,
    Stone
}

public class PerlinNoiseTile : MonoBehaviour
{
    public float Height;
    // public TileType TileType;

    public TypesOfTile TileType;
}

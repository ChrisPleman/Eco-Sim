using UnityEngine;

public class TileType : MonoBehaviour
{
    public enum TileTypes
    {
        Water,
        Dirt,
        Grass,
        Stone
    }

    public TileTypes TypeOfTile;

    public string Characteristics;

    void Awake()
    {
        Characteristics = $"I am of type {TypeOfTile} and that comes with some characteristics like: can a plant grow on me, resources, water, food, nitrogen, softness, etc.";
    }
}

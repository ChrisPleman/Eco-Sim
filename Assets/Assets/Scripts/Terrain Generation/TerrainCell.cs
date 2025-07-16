using UnityEngine;

public class TerrainCell : MonoBehaviour
{
    public bool Collapsed;
    public TerrainTile[] TileOptions;

    public void CreateTerrainCell(bool collapseState, TerrainTile[] terrainTileOptions)
    {
        Collapsed = collapseState;
        TileOptions = terrainTileOptions;
    }

    public void RecreateCell(TerrainTile[] terrainTiles)
    {
        TileOptions = terrainTiles;
    }
}

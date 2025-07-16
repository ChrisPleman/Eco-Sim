using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UIElements;

public class WaveFunction : MonoBehaviour
{
    // Dimensions of the map
    public int Dimensions;
    // Stores all created tiles
    public TerrainTile[] TileObjects;
    // Grid of the cells
    public List<TerrainCell> GridComponents;
    // Reference to the individual cell that will be added to Grid
    public TerrainCell TerrainCell;

    private int Iterations = 0;
    private List<TerrainTile> TerrainOptionsAsList;


    void Awake()
    {
        GridComponents = new List<TerrainCell>();
        InitializeGrid();

        // Instead of looping everytime in CellGeneration() we can just copy it
        TerrainOptionsAsList = new List<TerrainTile>();

        foreach (TerrainTile terrainTile in TileObjects)
        {
            TerrainOptionsAsList.Add(terrainTile);
        }
    }

    private void InitializeGrid()
    {
        // The terrain is pretty generic, so I want to randomly add a seed of complexity somewhere on the map
        int randIndex = UnityEngine.Random.Range(0, Dimensions * Dimensions);
        Debug.Log($"Complex tile placed at {randIndex}");

        for (int x = 0; x < Dimensions; x++)
        {
            for (int z = 0; z < Dimensions; z++)
            {
                TerrainCell newTerrainCell = Instantiate(TerrainCell, new Vector3(x, 0, z), Quaternion.identity);

                if (x * Dimensions + z == randIndex)
                {
                    newTerrainCell.CreateTerrainCell(false, new TerrainTile[] { TileObjects[1] });//, TileObjects[2], TileObjects[5], TileObjects[6], TileObjects[7] });
                }
                else
                {
                    newTerrainCell.CreateTerrainCell(false, TileObjects);
                }
                GridComponents.Add(newTerrainCell);
            }
        }

        StartCoroutine(CheckEntropy());
    }

    // Return the cell in the grid with the fewest options
    private IEnumerator CheckEntropy()
    {
        List<TerrainCell> tempGrid = new List<TerrainCell>(GridComponents);

        tempGrid.RemoveAll(c => c.Collapsed);

        tempGrid.Sort((a, b) => { return a.TileOptions.Length - b.TileOptions.Length; });

        int arrLength = tempGrid[0].TileOptions.Length;
        int stopIndex = default;

        for (int i = 0; i < tempGrid.Count; i++)
        {
            if (tempGrid[i].TileOptions.Length > arrLength)
            {
                stopIndex = i;
                break;
            }
        }

        if (stopIndex > 0)
        {
            tempGrid.RemoveRange(stopIndex, tempGrid.Count - stopIndex);
        }

        yield return new WaitForSeconds(.01f);

        CollapseTerrainCell(tempGrid);
    }

    private void CollapseTerrainCell(List<TerrainCell> tempGrid)
    {
        // Get index of one of the cells with the fewest possible states
        int randIndex = UnityEngine.Random.Range(0, tempGrid.Count);

        TerrainCell terrainCellToCollapse = tempGrid[randIndex];
        

        // Set the collapsed state to true, so that it doesn't get picked up in the CheckEntropy method
        terrainCellToCollapse.Collapsed = true;
        // Get the terrainCellToCollapse from the list of terrain cells

        int rIndex = UnityEngine.Random.Range(0, terrainCellToCollapse.TileOptions.Length);
        // Debug.Log($"Total num options: {terrainCellToCollapse.TileOptions.Length} v random index generated: {rIndex}");

        try
        {

            TerrainTile selectedTile = terrainCellToCollapse.TileOptions[rIndex];
            // Collapse the state down to a single state
            terrainCellToCollapse.TileOptions = new TerrainTile[] { selectedTile };

            // Obtain the actual tile type
            TerrainTile foundTile = terrainCellToCollapse.TileOptions[0];
            // Instantiate the tile type at the position of that particular cell
            Instantiate(foundTile, terrainCellToCollapse.transform.position, Quaternion.identity);

            UpdateGeneration();
        }
        catch (System.Exception e)
        {
            Debug.Log($"We are currently trying to access the following cell: {terrainCellToCollapse.GetInstanceID()}");
            Debug.Log($"We are currently trying to access the cell at {terrainCellToCollapse.transform.position}");
            Debug.Log($"The total number of options at this cell are: {terrainCellToCollapse.TileOptions.Length}");
            Debug.Log("We have reached an error:");
            Debug.Log($"{e}");
            Debug.Log("We are currently trying to randomly index the TileOptions of the current cell targeted for collapse");
            Debug.Log($"We are trying to access the value at index {rIndex}");
            Debug.Log("However, there doesn't seem to be a value at the index.");
            Debug.Log("Currently the TileOptions at this index are the following:");
            foreach (TerrainTile terrainTile in terrainCellToCollapse.TileOptions)
            {
                Debug.Log(terrainTile);
            }

            throw new Exception("Ending execution");
        }
    }

    // ? I'm wondering if I can speed up performance by passing on indices that are more than two tiles away from any processed cell
    // ? Would this save the time required at all?
    private void UpdateGeneration()
    {
        // Debug.Log("Starting UpdateGeneration!");
        // Copy the grid
        List<TerrainCell> newGenerationCell = new List<TerrainCell>(GridComponents);

        for (int x = 0; x < Dimensions; x++)
        {
            for (int z = 0; z < Dimensions; z++)
            {
                // Mapping nested indices to a flattened index
                var index = z + x * Dimensions;
                // Don't recreate collapsed cells
                if (GridComponents[index].Collapsed)
                {
                    // Debug.Log("Called");
                    newGenerationCell[index] = GridComponents[index];
                }
                else
                {
                    // Debug.Log($"We are currently at ({x},{z}).\nUp neighbor is currently set to ({x-1},{z})\nLeft neighbor is currently set to ({x},{z-1})\nRight neighbor is currently set to ({x},{z+1})\nDown neighbor is currently set to ({x+1},{z})");
                    // List<TerrainTile> options = new List<TerrainTile>(TerrainOptionsAsList);
                    List<TerrainTile> options = new List<TerrainTile>();
                    foreach (TerrainTile terrainTile in TileObjects)
                    {
                        //? Are we basically just converting from an array to a list? --> Yes, it's easier to work with
                        options.Add(terrainTile);
                    }

                    // Update the 'Up' states
                    if (x > 0)
                    {
                        // ! For whatever reason the tutorial does not have the 'up' line including the .GetComponent<>() like the other three options, I need to better understand why
                        // Accessing the cell above the current cell in the grid
                        TerrainCell up = GridComponents[z + (x - 1) * Dimensions];
                        List<TerrainTile> validOptions = new List<TerrainTile>();

                        foreach (TerrainTile terrainOption in up.TileOptions)
                        {
                            // We don't need to add duplicates
                            if (validOptions.Contains(terrainOption))
                            {
                                continue;
                            }
                            // Break early if we hit all of the possible tiles
                            if (validOptions.Count >= TileObjects.Length)
                            {
                                break;
                            }

                            var validOption = Array.FindIndex(TileObjects, obj => obj == terrainOption);
                            var valid = TileObjects[validOption].DownNeighbors;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        CheckValidity(options, validOptions.Distinct().ToList(), x, z, "up", GridComponents[index].GetInstanceID());
                    }

                    // Update the 'Left' states
                    if (z > 0)
                    {
                        TerrainCell left = GridComponents[z - 1 + x * Dimensions];
                        List<TerrainTile> validOptions = new List<TerrainTile>();

                        foreach (TerrainTile terrainOption in left.TileOptions)
                        {
                            // We don't need to add duplicates
                            if (validOptions.Contains(terrainOption))
                            {
                                continue;
                            }
                            // Break early if we hit all of the possible tiles
                            if (validOptions.Count >= TileObjects.Length)
                            {
                                break;
                            }
                            
                            var validOption = Array.FindIndex(TileObjects, obj => obj == terrainOption);
                            var valid = TileObjects[validOption].RightNeighbors;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        CheckValidity(options, validOptions.Distinct().ToList(), x, z, "left", GridComponents[index].GetInstanceID());
                    }

                    // Update the 'Right' states
                    if (z < Dimensions - 1)
                    {
                        TerrainCell right = GridComponents[z + 1 + x * Dimensions];
                        List<TerrainTile> validOptions = new List<TerrainTile>();


                        // Debug.Log($"Number of components at index {z + 1 + x * Dimensions}: {right.TileOptions.Length}");

                        foreach (TerrainTile terrainOption in right.TileOptions)
                        {
                            // We don't need to add duplicates
                            if (validOptions.Contains(terrainOption))
                            {
                                continue;
                            }
                            // Break early if we hit all of the possible tiles
                            if (validOptions.Count >= TileObjects.Length)
                            {
                                break;
                            }
                            
                            // Debug.Log($"The current terrainOption is {terrainOption}");
                            var validOption = Array.FindIndex(TileObjects, obj => obj == terrainOption);
                            // Debug.Log($"The current terrainOption is at index {validOption} in TileObjects");
                            var valid = TileObjects[validOption].LeftNeighbors;
                            // Debug.Log($"The current contents of 'valid' is:");

                            // foreach (TerrainTile validTerrainTile in valid)
                            // {
                            //     Debug.Log(validTerrainTile);
                            // }

                            // Debug.Log($"The current length of validOptions is {validOptions.Count}");
                            validOptions = validOptions.Concat(valid).ToList();
                            // Debug.Log($"The current length of validOptions is {validOptions.Count}");

                            // Debug.Log($"The current contents of 'valid' is:");
                            // foreach (TerrainTile validTerrainTileOption in validOptions)
                            // {
                            //     Debug.Log(validTerrainTileOption);
                            // }


                        }


                        // throw new Exception();

                        CheckValidity(options, validOptions.Distinct().ToList(), x, z, "right", GridComponents[index].GetInstanceID());
                    }

                    // Update the 'Down' states
                    if (x < Dimensions - 1)
                    {
                        TerrainCell down = GridComponents[z + (x + 1) * Dimensions];
                        List<TerrainTile> validOptions = new List<TerrainTile>();

                        foreach (TerrainTile terrainOption in down.TileOptions)
                        {
                            // We don't need to add duplicates
                            if (validOptions.Contains(terrainOption))
                            {
                                continue;
                            }
                            // Break early if we hit all of the possible tiles
                            if (validOptions.Count >= TileObjects.Length)
                            {
                                break;
                            }
                            
                            var validOption = Array.FindIndex(TileObjects, obj => obj == terrainOption);
                            var valid = TileObjects[validOption].UpNeighbors;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        CheckValidity(options, validOptions.Distinct().ToList(), x, z, "down", GridComponents[index].GetInstanceID());
                    }

                    // Convert back from list to array
                    TerrainTile[] newTileList = new TerrainTile[options.Count];

                    for (int i = 0; i < options.Count; i++)
                    {
                        newTileList[i] = options[i];
                    }

                    // Update the grid cell with the updated list of possible options
                    newGenerationCell[index].RecreateCell(newTileList);
                }
            }
        }

        GridComponents = newGenerationCell;
        Iterations++;
        // Debug.Log("Completed UpdateGeneration!");

        // This is a recursive process: CheckEntropy() calls CollapseTerrainCell() which calls UpdateGeneration() which calls CheckEntropy()
        // The recursive process should only stop once all of the cells are complete collpased: i.e., when Iterations == Dimensions * Dimensions (or total number of cells)
        if (Iterations < Dimensions * Dimensions)
        {
            StartCoroutine(CheckEntropy());
        }

    }

    private void CheckValidity(List<TerrainTile> optionList, List<TerrainTile> validOptions, int xCoord, int zCoord, string neighborCell, int instanceId)
    {
        // Debug.Log($"We are currently working on cell ({instanceId}) at coord ({xCoord}, {zCoord}) while referencing its {neighborCell} neighbor");

        // Debug.Log($"We are currently working with the following data:\n'validOptions' (size - {validOptions.Count}):");
        // int currIndex = 0;
        // foreach (TerrainTile validOption in validOptions)
        // {
        //     Debug.Log($"{currIndex}: {validOption}");
        //     currIndex++;
        // }

        //* Decrement from end to beginning
        for (int i = optionList.Count - 1; i >= 0; i--)
        {
            var element = optionList[i];

            if (!validOptions.Contains(element))
            {
                optionList.RemoveAt(i);
            }
            // Debug.Log($"The first index we are starting at is {i}, and there is an element at this index with a value of {element}");
            // throw new IndexOutOfRangeException("Stopping here to prevent the code from clogging the console with print statemnts");
        }

        // Debug.Log($"We are currently working with the following data:\n'optionList' (size - {optionList.Count}):");
        // currIndex = 0;
        // foreach (TerrainTile option in optionList)
        // {
        //     Debug.Log($"{currIndex}: {option}");
        //     currIndex++;
        // }


    }
}

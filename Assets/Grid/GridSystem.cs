using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteAlways]
public class GridSystem : MonoBehaviour
{
    Pathfinder pathfinder = null;
    Tilemap tilemap = null;

    Dictionary<GridCoordinates, GridBlock> gridDictionary = new Dictionary<GridCoordinates, GridBlock>();

    private void Awake()
    {
        pathfinder = GetComponent<Pathfinder>();
        tilemap = GetComponentInChildren<Tilemap>();
    }

    private void Start()
    {
        SetupGrid();
    }

    public void SetupGrid()
    {
        gridDictionary.Clear();

        foreach(GridBlock gridBlock in GetComponentsInChildren<GridBlock>())
        {
            Vector3 coordinates = gridBlock.transform.localPosition;
            int x = Mathf.RoundToInt(coordinates.x);
            int z = Mathf.RoundToInt(coordinates.z);

            gridBlock.SetupGridBlock(x, z);

            GridCoordinates gridCoordinates = gridBlock.gridCoordinates;
            gridDictionary.Add(gridCoordinates,gridBlock);
        }

        pathfinder.InitalizePathfinder(gridDictionary);
    }
    
    public void DeleteGrid()
    {
        foreach (GridBlock gridBlock in GetComponentsInChildren<GridBlock>())
        {
            if (gridBlock == null) continue;
            DestroyImmediate(gridBlock.gameObject);
        }

        gridDictionary.Clear();
    }
}

[Serializable]
public struct GridCoordinates
{
    public int x;
    public int z;

    public GridCoordinates(int _x, int _z)
    {
        x = _x;
        z = _z;
    }
}

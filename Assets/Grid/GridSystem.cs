using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteAlways]
public class GridSystem : MonoBehaviour
{
    [SerializeField] Material lightMaterial = null;
    [SerializeField] Material darkMaterial = null;
    [SerializeField] Material redMaterial = null;
    [SerializeField] Material blueMaterial = null;

    [SerializeField] Material highlightMaterial = null;
    [SerializeField] Material unworthyMaterial = null;

    public GridCoordinates playerZeroCoordinates;
    public GridCoordinates enemyZeroCoordinates;
    
    Pathfinder pathfinder = null;
    Tilemap tilemap = null;

    Dictionary<GridCoordinates, GridBlock> gridDictionary = new Dictionary<GridCoordinates, GridBlock>();

    private void Awake()
    {
        pathfinder = GetComponent<Pathfinder>();
        tilemap = GetComponentInChildren<Tilemap>();
        SetupGrid();
    }

    public void SetupGrid()
    {
        gridDictionary.Clear();

        foreach(GridBlock gridBlock in GetComponentsInChildren<GridBlock>())
        {
            Vector3 localPosition = gridBlock.transform.localPosition;
            int x = Mathf.RoundToInt(localPosition.x);
            int z = Mathf.RoundToInt(localPosition.z);

            GridCoordinates gridCoordinates = new GridCoordinates(x, z);

            SetupGridBlock(gridBlock, gridCoordinates);

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

    public void HighlightPath(List<GridBlock> _path, int _furthestBlockIndex)
    {
        GridBlock goalBlock = _path[_path.Count - 1];

        foreach(GridBlock gridBlock in _path)
        { 
            int currentIndex = _path.IndexOf(gridBlock);

            Material currentHighlightMaterial = highlightMaterial;
            if (currentIndex > _furthestBlockIndex) currentHighlightMaterial = unworthyMaterial;

            gridBlock.SetColors(currentHighlightMaterial, Color.white);
        }

        if (goalBlock.contestedFighter != null) goalBlock.SetColors(redMaterial, Color.white);
    }

    public void UnhighlightPath(List<GridBlock> _path)
    {
        foreach (GridBlock gridBlock in _path)
        {
            Material newMaterial = GetGridBlockMaterial(gridBlock.gridCoordinates);
            Color textColor = GetTextColor(newMaterial);
            gridBlock.SetColors(newMaterial, textColor);
        }
    }

    private void SetupGridBlock(GridBlock _gridBlock, GridCoordinates _gridCoordinates)
    {
        _gridBlock.gridCoordinates = _gridCoordinates;

        Material newMaterial = GetGridBlockMaterial(_gridCoordinates);
        Color textColor = Color.white;

        if (newMaterial == lightMaterial) textColor = Color.black;

        _gridBlock.SetupGridBlock(newMaterial, textColor);     
    }

    public GridBlock GetGridBlock(int _x, int _z)
    {
        GridBlock gridBlock = pathfinder.GetGridBlock(_x, _z);
        return gridBlock;
    }

    private Material GetGridBlockMaterial(GridCoordinates _gridCoordinates)
    {
        if (DoCoordinatesMatch(_gridCoordinates, playerZeroCoordinates)) return blueMaterial;
        else if (DoCoordinatesMatch(_gridCoordinates, enemyZeroCoordinates)) return redMaterial;
        else
        {
            if (IsLightBlock(_gridCoordinates)) return lightMaterial;
            else return darkMaterial;
        }
    }

    private Color GetTextColor(Material _material)
    {
        if (_material == lightMaterial) return Color.black;
        else return Color.white;
    }

    private bool DoCoordinatesMatch(GridCoordinates _coordinates0, GridCoordinates _coordinates1)
    {
        return _coordinates0.x == _coordinates1.x && _coordinates0.z == _coordinates1.z;
    }

    private bool IsLightBlock(GridCoordinates _gridCoordinates)
    {
        bool isXEven = _gridCoordinates.x % 2 == 0;
        bool isZEven = _gridCoordinates.z % 2 == 0;

        if (isXEven == isZEven) return true;
        else return false;
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

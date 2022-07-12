using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteAlways]
public class GridSystem : MonoBehaviour
{
    Pathfinder pathfinder = null;
    Tilemap tilemap = null;

    //Dictionary<GridCoordinates, GridPiece> gridBlocks = new Dictionary<GridCoordinates, GridPiece>();
    List<GridPiece> gridPieces = new List<GridPiece>();

    private void Awake()
    {
        pathfinder = GetComponent<Pathfinder>();
        tilemap = GetComponentInChildren<Tilemap>();
    }

    public void SetupGrid()
    {
        gridPieces.Clear();

        foreach(GridPiece gridPiece in GetComponentsInChildren<GridPiece>())
        {
            Vector3 coordinates = gridPiece.transform.localPosition;
            int x = Mathf.RoundToInt(coordinates.x);
            int z = Mathf.RoundToInt(coordinates.z);

            gridPiece.SetupGridPiece(x, z);

            gridPieces.Add(gridPiece);
        }
    }
    
    public void DeleteGrid()
    {
        foreach (GridPiece gridPiece in GetComponentsInChildren<GridPiece>())
        {
            if (gridPiece == null) continue;
            DestroyImmediate(gridPiece.gameObject);
        }

        gridPieces.Clear();
    }
}

[Serializable]
public class GridCoordinates
{
    int xCoordinate = 0;
    int zCoordinate = 0;

    public GridCoordinates(int _x, int _z)
    {

    }

    public int GetX()
    {
        return xCoordinate;
    }

    public int GetZ()
    {
        return zCoordinate;
    }
}

//[SerializeField] GridPiece gridPiecePrefab = null;

////[SerializeField] Dictionary<GridPiece, GridCoordinates> grid = new Dictionary<GridPiece, GridCoordinates>();
//[SerializeField] List<GridPiece> gridPieces = new List<GridPiece>();

////Error here with switching rows
//int currentHighestX = -1;
//int currentHighestZ = -1;

//public int GetNextAvailableRow()
//{
//    return 0;
//}

//public void AddGridPiece()
//{
//    GridPiece gridInstance = Instantiate(gridPiecePrefab, transform);

//    string formattedName = gridInstance.name.Replace("(Clone)", "");
//    gridInstance.name = formattedName;

//    gridInstance.UpdateCoordinatesText(0, 0);
//}

//public void AddRowPiece(int _columnNumber)
//{
//    GridPiece gridInstance = Instantiate(gridPiecePrefab, transform);

//    string formattedName = gridInstance.name.Replace("(Clone)", "");
//    gridInstance.name = formattedName;

//    currentHighestX += 1;

//    Vector3 newPosition = new Vector3(currentHighestX, 0, _columnNumber);
//    gridInstance.transform.localPosition = newPosition;

//    gridInstance.UpdateCoordinatesText((int)gridInstance.transform.localPosition.x, (int)gridInstance.transform.localPosition.z);

//    gridPieces.Add(gridInstance);
//}

//public void AddColumnPiece(int _rowNumber)
//{
//    GridPiece gridInstance = Instantiate(gridPiecePrefab, transform);

//    string formattedName = gridInstance.name.Replace("(Clone)", "");
//    gridInstance.name = formattedName;

//    currentHighestZ += 1;

//    Vector3 newPosition = new Vector3(_rowNumber, 0, currentHighestZ);
//    gridInstance.transform.localPosition = newPosition;

//    gridInstance.UpdateCoordinatesText((int)gridInstance.transform.localPosition.x, (int)gridInstance.transform.localPosition.z);


//    gridPieces.Add(gridInstance);
//}




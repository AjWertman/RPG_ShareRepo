using System;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    Dictionary<GridCoordinates, GridPiece> gridBlocks = new Dictionary<GridCoordinates, GridPiece>();
    List<GridPiece> gridPieces = new List<GridPiece>();

    int maxXCoordinate = -1;
    int maxZCoordinate = -1;

    GridPiece currentBlock = null;
    GridPiece previousBlock = null;

    public void InitalizePathfinder(List<GridPiece> _gridPieces)
    {
        gridPieces = _gridPieces;
        CalculateMaxCoordinates(gridPieces);
    }

    public List<GridPiece> CalculatePath(GridCoordinates _start, GridCoordinates _end)
    {
        List<GridPiece> path = new List<GridPiece>();



        return path;
    }

    private IEnumerable<GridPiece> GetNeighbors(GridCoordinates _gridCoordinates)
    {
        int x = _gridCoordinates.GetX();
        int z = _gridCoordinates.GetZ();

        //Right
        yield return GetGridPiece(x + 1, z);

        //Left
        yield return GetGridPiece(x - 1, z);

        //Up
        yield return GetGridPiece(x, z + 1);

        //Down
        yield return GetGridPiece(x, z - 1);

        //Right and Up
        yield return GetGridPiece(x + 1, z + 1);

        //Left and Up 
        yield return GetGridPiece(x -1 , z + 1);

        //Right and Down
        yield return GetGridPiece(x + 1, z - 1);

        //Left and Down
        yield return GetGridPiece(x -1, z -1);

    }

    private GridPiece GetGridPiece(int _x, int _z)
    {
        foreach (GridCoordinates coordinates in gridBlocks.Keys)
        {
            int x = coordinates.GetX();
            int z = coordinates.GetZ();

            if (x == _x && z == _z) return gridBlocks[coordinates];
        }

        return null;
    }

    private bool DoCoordinatesMatch(GridCoordinates _start, GridCoordinates _end)
    {
        return _start.GetX() == _end.GetX() && _start.GetZ() == _end.GetZ();
    }

    private void CalculateMaxCoordinates(List<GridPiece> _gridPieces)
    {
        foreach(GridPiece gridPiece in _gridPieces)
        {
            GridCoordinates gridCoordinates = gridPiece.GetGridCoordinates();
            int x = gridCoordinates.GetX();
            int z = gridCoordinates.GetZ();

            maxXCoordinate = Mathf.Max(maxXCoordinate, x);
            maxZCoordinate = Mathf.Max(maxZCoordinate, z);
        }
    }
}

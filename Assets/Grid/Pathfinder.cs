using System;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    Dictionary<GridCoordinates, GridBlock> gridDictionary = new Dictionary<GridCoordinates, GridBlock>();

    List<GridBlock> openList = new List<GridBlock>();
    List<GridBlock> closedList = new List<GridBlock>();

    int maxXCoordinate = -1;
    int maxZCoordinate = -1;

    const int straightCost = 10;
    const int diagonalCost = 14;

    GridBlock currentBlock = null;
    GridBlock previousBlock = null;

    public void InitalizePathfinder(Dictionary<GridCoordinates, GridBlock> _gridDictionary)
    {
        gridDictionary = _gridDictionary;
    }

    public List<GridBlock> FindPath(GridCoordinates _start, GridCoordinates _end)
    {
        openList.Clear();
        closedList.Clear();

        GridBlock startBlock = gridDictionary[_start];
        GridBlock endBlock = gridDictionary[_end];

        ResetPathfindingValues();

        startBlock.pathfindingCostValues.gCost = 0;
        startBlock.pathfindingCostValues.hCost = CalculateDistance(_start, _end);
        startBlock.pathfindingCostValues.CalculateFCost();
        openList.Add(startBlock);

        while (openList.Count > 0)
        {
            GridBlock currentBlock = GetLowestFCostBlock(openList);

            if(currentBlock == endBlock)
            {
                return CalculatePath(endBlock);
            }

            openList.Remove(currentBlock);
            closedList.Add(currentBlock);

            foreach (GridBlock neighborBlock in GetNeighbors(currentBlock.gridCoordinates))
            {
                if (neighborBlock == null) continue;
                if (closedList.Contains(neighborBlock)) continue;

                int distanceToNeighbor = CalculateDistance(currentBlock.gridCoordinates, neighborBlock.gridCoordinates);
                int tenativeGCost = currentBlock.pathfindingCostValues.gCost + distanceToNeighbor;
                if (tenativeGCost < neighborBlock.pathfindingCostValues.gCost)
                {
                    GridCoordinates gridCoordinates = neighborBlock.gridCoordinates;

                    neighborBlock.pathfindingCostValues.cameFromBlock = currentBlock;
                    neighborBlock.pathfindingCostValues.gCost = tenativeGCost;
                    neighborBlock.pathfindingCostValues.hCost = CalculateDistance(gridCoordinates, endBlock.gridCoordinates);
                    neighborBlock.pathfindingCostValues.CalculateFCost();

                    if(!openList.Contains(neighborBlock))
                    {
                        openList.Add(neighborBlock);
                    }
                }
            }
        }

        return null;
    }

    private List<GridBlock> CalculatePath(GridBlock _endBlock)
    {
        List<GridBlock> calculatedPath = new List<GridBlock>();

        calculatedPath.Add(_endBlock);

        GridBlock currentBlock = _endBlock;

        while(currentBlock.pathfindingCostValues.cameFromBlock != null)
        {
            GridBlock cameFromBlock = currentBlock.pathfindingCostValues.cameFromBlock;
            calculatedPath.Add(cameFromBlock);

            currentBlock = cameFromBlock;
        }

        calculatedPath.Reverse();

        return calculatedPath;
    }

    private GridBlock GetLowestFCostBlock(List<GridBlock> blockList)
    {
        GridBlock lowestFCostBlock = blockList[0];

        foreach(GridBlock gridBlock in blockList)
        {
            if(gridBlock.pathfindingCostValues.fCost < lowestFCostBlock.pathfindingCostValues.fCost)
            {
                lowestFCostBlock = gridBlock;
            }
        }

        return lowestFCostBlock;
    }

    private void ResetPathfindingValues()
    {
        foreach (GridBlock gridBlock in gridDictionary.Values)
        {
            gridBlock.pathfindingCostValues.ResetValues();
        }
    }

    public IEnumerable<GridBlock> GetNeighbors(GridCoordinates _gridCoordinates)
    {
        int x = _gridCoordinates.x;
        int z = _gridCoordinates.z;

        //Right
        yield return GetGridBlock(x + 1, z);

        //Left
        yield return GetGridBlock(x - 1, z);

        //Up
        yield return GetGridBlock(x, z + 1);

        //Down
        yield return GetGridBlock(x, z - 1);

        //Right and Up
        yield return GetGridBlock(x + 1, z + 1);

        //Left and Up 
        yield return GetGridBlock(x -1 , z + 1);

        //Right and Down
        yield return GetGridBlock(x + 1, z - 1);

        //Left and Down
        yield return GetGridBlock(x -1, z -1);
    }

    public GridBlock GetGridBlock(int _x, int _z)
    {
        GridCoordinates gridCoordinates = new GridCoordinates(_x, _z);

        if (gridDictionary.ContainsKey(gridCoordinates)) return gridDictionary[gridCoordinates];
        else return null;
    }

    private int CalculateDistance(GridCoordinates _start, GridCoordinates _end)
    {
        int xDistance = Mathf.Abs( _start.x - _end.x);
        int yDistance = Mathf.Abs(_start.z - _end.z);
        int remainingDistance = Mathf.Abs(xDistance - yDistance);

        int distanceCost = (diagonalCost * Mathf.Min(xDistance, yDistance)) + (straightCost * remainingDistance);

        return distanceCost;
    }
}

[Serializable]
public struct PathfindingCostValues
{
    public GridBlock cameFromBlock;
    public int gCost;
    public int hCost;
    public int fCost;

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public void ResetValues()
    {
        cameFromBlock = null;
        gCost = int.MaxValue;
        CalculateFCost();
    }
}

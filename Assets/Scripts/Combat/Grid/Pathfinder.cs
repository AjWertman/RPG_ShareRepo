using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Combat.Grid
{
    public class Pathfinder : MonoBehaviour
    {
        const int straightCost = 10;
        const int diagonalCost = 14;

        int maxXCoordinate = -1;
        int maxZCoordinate = -1;

        GridBlock currentBlock = null;
        GridBlock previousBlock = null;

        Dictionary<GridCoordinates, GridBlock> gridDictionary = new Dictionary<GridCoordinates, GridBlock>();

        List<GridBlock> openList = new List<GridBlock>();
        List<GridBlock> closedList = new List<GridBlock>();

        public void InitalizePathfinder(Dictionary<GridCoordinates, GridBlock> _gridDictionary)
        {
            gridDictionary.Clear();
            gridDictionary = _gridDictionary;
        }

        public List<GridBlock> FindPath(GridBlock _startBlock, GridBlock _endBlock)
        {
            openList.Clear();
            closedList.Clear();

            ResetPathfindingValues();

            _startBlock.pathfindingCostValues.gCost = 0;
            _startBlock.pathfindingCostValues.hCost = CalculateDistance(_startBlock, _endBlock);
            _startBlock.pathfindingCostValues.CalculateFCost();
            openList.Add(_startBlock);

            while (openList.Count > 0)
            { 
                GridBlock currentBlock = GetLowestFCostBlock(openList);

                if (currentBlock == _endBlock) return CalculatePath(_endBlock);

                openList.Remove(currentBlock);
                closedList.Add(currentBlock);

                foreach (GridBlock neighborBlock in GetNeighbors(currentBlock))
                {
                    if (neighborBlock == null) continue;
                    if (closedList.Contains(neighborBlock)) continue;

                    if (!neighborBlock.IsMovable(_startBlock, _endBlock))
                    {
                        closedList.Add(neighborBlock);
                        continue;
                    }

                    int distanceToNeighbor = CalculateDistance(currentBlock, neighborBlock);
                    int tenativeGCost = currentBlock.pathfindingCostValues.gCost + distanceToNeighbor;
                    if (tenativeGCost < neighborBlock.pathfindingCostValues.gCost)
                    {
                        neighborBlock.pathfindingCostValues.cameFromBlock = currentBlock;
                        neighborBlock.pathfindingCostValues.gCost = tenativeGCost;
                        neighborBlock.pathfindingCostValues.hCost = CalculateDistance(neighborBlock, _endBlock);
                        neighborBlock.pathfindingCostValues.CalculateFCost();

                        if (!openList.Contains(neighborBlock))
                        {
                            openList.Add(neighborBlock);
                        }
                    }
                }
            }

            return new List<GridBlock>();
        }

        public GridBlock GetGridBlock(int _x, int _z)
        {
            GridCoordinates gridCoordinates = new GridCoordinates(_x, _z);

            if (gridDictionary.ContainsKey(gridCoordinates)) return gridDictionary[gridCoordinates];
            else return null;
        }

        public static int CalculateDistance(GridBlock _startBlock, GridBlock _endBlock)
        {
            GridCoordinates startCoordinates = _startBlock.gridCoordinates;
            GridCoordinates endCoordinates = _endBlock.gridCoordinates;

            int xDistance = Mathf.Abs(startCoordinates.x - endCoordinates.x);
            int yDistance = Mathf.Abs(startCoordinates.z - endCoordinates.z);
            int remainingDistance = Mathf.Abs(xDistance - yDistance);

            int distanceCost = (diagonalCost * Mathf.Min(xDistance, yDistance)) + (straightCost * remainingDistance);

            return distanceCost;
        }

        public bool IsNeighborBlock(GridBlock _targetBlock, GridBlock _possibleNeighbor)
        {
            foreach(GridBlock gridBlock in GetNeighbors(_targetBlock))
            {
                if (gridBlock == _possibleNeighbor) return true;
            }

            return false;
        }

        private List<GridBlock> CalculatePath(GridBlock _endBlock)
        {
            List<GridBlock> calculatedPath = new List<GridBlock>();

            calculatedPath.Add(_endBlock);

            GridBlock currentBlock = _endBlock;

            while (currentBlock.pathfindingCostValues.cameFromBlock != null)
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

            foreach (GridBlock gridBlock in blockList)
            {
                if (gridBlock.pathfindingCostValues.fCost < lowestFCostBlock.pathfindingCostValues.fCost)
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

        private IEnumerable<GridBlock> GetNeighbors(GridBlock _gridBlock)
        {
            int x = _gridBlock.gridCoordinates.x;
            int z = _gridBlock.gridCoordinates.z;

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
            yield return GetGridBlock(x - 1, z + 1);

            //Right and Down
            yield return GetGridBlock(x + 1, z - 1);

            //Left and Down
            yield return GetGridBlock(x - 1, z - 1);
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
            hCost = int.MaxValue;
            CalculateFCost();
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Combat.Grid
{
    /// <summary>
    /// Calculates paths and differnet gridblock formations.
    /// </summary>
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
      
       /// <summary>
       /// Calculates the best possible path based on A* pathfinding.
       /// </summary>
        public List<GridBlock> FindOptimalPath(GridBlock _startBlock, GridBlock _endBlock)
        {
            //Refactor to take into account affected blocks and end position evalution.
            //also add GetBlocksInRange(GridBlock _targetBlock, float range);

            if (_startBlock == _endBlock) return null;

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

                foreach (GridBlock neighborBlock in GetNeighbors(currentBlock,1))
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


        /// <summary>
        /// Returns the G-Cost to reach a specific block.
        /// </summary>
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

        public GridBlock GetGridBlock(int _x, int _z)
        {
            GridCoordinates gridCoordinates = new GridCoordinates(_x, _z);

            if (gridDictionary.ContainsKey(gridCoordinates)) return gridDictionary[gridCoordinates];
            else return null;
        }

        public bool IsNeighborBlock(GridBlock _targetBlock, GridBlock _possibleNeighbor)
        {
            foreach (GridBlock gridBlock in GetNeighbors(_targetBlock, 1))
            {
                if (gridBlock == _possibleNeighbor) return true;
            }

            return false;
        }

        public List<GridBlock> GetNeighbors(GridBlock _centerBlock, int _amount)
        {
            List<GridBlock> neighborBlocks = new List<GridBlock>();
            foreach(GridBlock neighbor in CalculateNeighborBlocks(_centerBlock, _amount))
            {
                if (neighbor != null) neighborBlocks.Add(neighbor);
            }
            return neighborBlocks;
        }

        public bool ArePathsEqual(List<GridBlock> _pathA, List<GridBlock> _pathB)
        {
            if (_pathA == null || _pathB == null) return false;

            int aCount = _pathA.Count;
            int bCount = _pathB.Count;
            if (aCount == 0 || bCount == 0) return false;
            if (aCount != bCount) return false;

            foreach(GridBlock gridBlock in _pathA)
            {
                if (!_pathB.Contains(gridBlock)) return false;
            }

            return true;
        }

        /// <summary>
        /// Retraces the path found in "FindOptimalPath()" and reverses the order to be usable by the combat system.
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// Returns all the neighbor blocks by the specified amount of neighbors
        /// </summary>
        private IEnumerable<GridBlock> CalculateNeighborBlocks(GridBlock _gridBlock, int _amount)
        {
            int myX = _gridBlock.gridCoordinates.x;
            int myZ = _gridBlock.gridCoordinates.z;

            for (int newX = -_amount; newX < _amount + 1; newX++)
            {
                int x = myX - newX;
                for (int newZ = -_amount; newZ < _amount + 1; newZ++)
                {
                    int z = myZ - newZ; ;
                    GridBlock newBlock = GetGridBlock(x, z);
                    if(newBlock != null) yield return newBlock;
                }
            }

            //Queen(Chess) movement
            //if(_amount >= 1)
            //{
            //    for (int i = 1; i < _amount + 1; i++)
            //    {
            //        yield return GetGridBlock(x + i, z);
            //        yield return GetGridBlock(x - i, z);
            //        yield return GetGridBlock(x, z + i);
            //        yield return GetGridBlock(x, z - i);
            //        yield return GetGridBlock(x + i, z + i);
            //        yield return GetGridBlock(x - i, z + i);
            //        yield return GetGridBlock(x + i, z - i);
            //        yield return GetGridBlock(x - i, z - i);
            //    }
            //}
            
        }
    }

    /// <summary>
    /// Struct containing necessary information for pathfinding.
    /// </summary>
    [Serializable]
    public struct PathfindingCostValues
    {
        public GridBlock cameFromBlock;
        public int gCost; //Cost to move to a block (straight = 10, diagonal = 14)
        public int hCost; //Ideal G-Cost to move to a specific point
        public int fCost; //G-Cost + H-Cost

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

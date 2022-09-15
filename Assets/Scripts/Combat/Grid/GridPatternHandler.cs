using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Combat.Grid
{
    public enum GridPattern { None, Neighbors, DirectionSelection, Queen, Rook, Bishop, StraightLine}

    public class GridPatternHandler : MonoBehaviour
    {
        Dictionary<GridCoordinates, GridBlock> gridDictionary = new Dictionary<GridCoordinates, GridBlock>();

        public void InitalizePatternHandler(Dictionary<GridCoordinates, GridBlock> _gridDictionary)
        {
            gridDictionary = _gridDictionary;
        }

        public List<GridBlock> GetPattern(GridBlock _centerBlock, GridBlock _endBlock, GridPattern _gridPattern, int _radius)
        {
            if (IsChessPattern(_gridPattern)) return GetChessPattern(_centerBlock, _gridPattern, _radius);
            else if (IsNeighborsPattern(_gridPattern)) return GetNeighbors(_centerBlock, _radius);
            else if (_gridPattern == GridPattern.StraightLine) return GetStraightLine(_centerBlock, _endBlock);

            return new List<GridBlock>();
        }

        private List<GridBlock> GetNeighbors(GridBlock _centerBlock, int _amount)
        {
            List<GridBlock> neighborBlocks = new List<GridBlock>();
            foreach (GridBlock neighbor in CalculateNeighborBlocks(_centerBlock, _amount))
            {
                if (neighbor != null && neighbor != _centerBlock) neighborBlocks.Add(neighbor);
            }
            return neighborBlocks;
        }

        private List<GridBlock> GetChessPattern(GridBlock _centerBlock, GridPattern _gridPattern, int _radius)
        {
            if (_radius <= 0) return null;

            List<GridBlock> chessPattern = new List<GridBlock>();

            GridCoordinates centerCoordinates = _centerBlock.gridCoordinates;
            int x = centerCoordinates.x;
            int z = centerCoordinates.z;

            bool isQueen = _gridPattern == GridPattern.Queen;

            if (isQueen || _gridPattern == GridPattern.Rook)
            {
                for (int i = 1; i < _radius + 1; i++)
                {
                    chessPattern.Add(GetGridBlock(x + i, z));
                    chessPattern.Add(GetGridBlock(x - i, z));
                    chessPattern.Add(GetGridBlock(x, z + i));
                    chessPattern.Add(GetGridBlock(x, z - i));
                }
            }

            if (isQueen || _gridPattern == GridPattern.Bishop)
            {
                for (int i = 1; i < _radius + 1; i++)
                {
                    chessPattern.Add(GetGridBlock(x + i, z + i));
                    chessPattern.Add(GetGridBlock(x - i, z + i));
                    chessPattern.Add(GetGridBlock(x + i, z - i));
                    chessPattern.Add(GetGridBlock(x - i, z - i));
                }
            }

            return chessPattern;
        }

        private List<GridBlock> GetStraightLine(GridBlock _startBlock, GridBlock _targetBlock)
        {
            List<GridBlock> straightLineBlocks = new List<GridBlock>();

            int startX = _startBlock.gridCoordinates.x;
            int startZ = _startBlock.gridCoordinates.z;
            int targetX = _targetBlock.gridCoordinates.x;
            int targetZ = _targetBlock.gridCoordinates.z;

            int xDistance = Mathf.Abs(targetX - startX);
            int zDistance = Mathf.Abs(targetZ - startZ);

            int distance = 0;
            Vector2 direction = new Vector2();

            //Straight Diagonal
            if(xDistance == zDistance)
            {
                if (startX > targetX) direction = new Vector2(-1, direction.y);
                else direction = new Vector2(1, direction.y);
                if (startZ > targetZ) direction = new Vector2(direction.x, -1);
                else direction = new Vector2(direction.x, 1);

                distance = xDistance;
            }

            //Straight Up or Down
            else if(startX == targetX && startZ != targetZ)
            {
                if(startZ > targetZ) direction = new Vector2(0, -1);
                else direction = new Vector2(0, 1);
                distance = zDistance;
            }

            //Straight Left or Right
            else if (startX != targetX && startZ == targetZ)
            {
                if (startX > targetX) direction = new Vector2(-1, direction.y);
                else direction = new Vector2(1, direction.y);
                distance = xDistance;
            }

            for (int i = 1; i < distance + 1; i++)
            {
                int x = startX + (i * (int)direction.x);
                int z = startZ + (i * (int)direction.y);

                GridBlock nextBlock = GetGridBlock(x, z);
                if(nextBlock != null) straightLineBlocks.Add(nextBlock);
            }

            return straightLineBlocks;
        }

        /// <summary>
        /// Returns all the neighbor blocks by the specified amount of neighbors
        /// </summary>
        private IEnumerable<GridBlock> CalculateNeighborBlocks(GridBlock _gridBlock, int _amount)
        {
            if (_gridBlock == null) yield break;

            int myX = _gridBlock.gridCoordinates.x;
            int myZ = _gridBlock.gridCoordinates.z;

            for (int newX = -_amount; newX < _amount + 1; newX++)
            {
                int x = myX - newX;
                for (int newZ = -_amount; newZ < _amount + 1; newZ++)
                {
                    int z = myZ - newZ; ;
                    GridBlock newBlock = GetGridBlock(x, z);
                    if (newBlock != null) yield return newBlock;
                }
            }
        }

        private GridBlock GetGridBlock(int _x, int _z)
        {
            GridCoordinates gridCoordinates = new GridCoordinates(_x, _z);

            if (gridDictionary.ContainsKey(gridCoordinates)) return gridDictionary[gridCoordinates];
            else return null;
        }

        private bool IsChessPattern(GridPattern _gridPattern)
        {
            if (_gridPattern == GridPattern.Bishop) return true;
            if (_gridPattern == GridPattern.Rook) return true;
            if (_gridPattern == GridPattern.Queen) return true;

            return false;
        }

        private bool IsNeighborsPattern(GridPattern _gridPattern)
        {
            if (_gridPattern == GridPattern.Neighbors) return true;
            if (_gridPattern == GridPattern.DirectionSelection) return true;

            return false;
        }
    }
}

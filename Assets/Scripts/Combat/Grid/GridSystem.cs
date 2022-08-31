using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Combat.Grid
{
    /// <summary>
    /// Controls the basic necessary information for the combat grid.
    /// </summary>
    [ExecuteAlways]
    public class GridSystem : MonoBehaviour
    {
        [SerializeField] Material lightMaterial = null;
        [SerializeField] Material darkMaterial = null;
        [SerializeField] Material redMaterial = null;
        [SerializeField] Material blueMaterial = null;

        [SerializeField] Material highlightMaterial = null;
        [SerializeField] Material unworthyMaterial = null;
        [SerializeField] Material neutralMaterial = null;

        public GridBlock[] gridBlocks = null;

        public GridCoordinates playerZeroCoordinates;
        public GridCoordinates enemyZeroCoordinates;

        public GridCoordinates minCoordinates;
        public GridCoordinates maxCoordinates;

        public bool isPathHighlighted = false;

        Pathfinder pathfinder = null;

        Dictionary<GridCoordinates, GridBlock> gridDictionary = new Dictionary<GridCoordinates, GridBlock>();

        bool isNormalized = true;

        private void Awake()
        {
            pathfinder = GetComponent<Pathfinder>();
            SetupGrid();
        }

        public void SetupGrid()
        {
            gridDictionary.Clear();
            gridBlocks = GetComponentsInChildren<GridBlock>();

            foreach (GridBlock gridBlock in gridBlocks)
            {
                Vector3 localPosition = gridBlock.transform.localPosition;
                int x = Mathf.RoundToInt(localPosition.x);
                int z = Mathf.RoundToInt(localPosition.z);

                GridCoordinates gridCoordinates = new GridCoordinates(x, z);

                BoundaryCheck(gridCoordinates);
                SetupGridBlock(gridBlock, gridCoordinates);
                gridBlock.ActivateMeshRenderer(false);
                gridDictionary.Add(gridCoordinates, gridBlock);
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

        /// <summary>
        /// Highlights the path green to the furtherst block a character can move based on their energy.
        /// If they do not have enough energy to reach the goal, the remaining path is highlighted red.
        /// </summary>
        public void HighlightPath(List<GridBlock> _path, int _furthestBlockIndex)
        {
            GridBlock startBlock = _path[0];
            GridBlock goalBlock = _path[_path.Count - 1];
            Fighter goalBlockFighter = goalBlock.contestedFighter;

            foreach (GridBlock gridBlock in _path)
            {
                int currentIndex = _path.IndexOf(gridBlock);

                Material currentHighlightMaterial = highlightMaterial;
                if (currentIndex > _furthestBlockIndex) currentHighlightMaterial = unworthyMaterial;
                if (gridBlock == goalBlock && goalBlockFighter != null) currentHighlightMaterial = redMaterial;

                if (gridBlock == startBlock) continue;

                gridBlock.HighlightBlock(currentHighlightMaterial, GridBlockMeshKey.Path);
            }

            if (goalBlockFighter!= null) goalBlockFighter.HighlightFighter(true);

            isPathHighlighted = true;
        }

        public List<GridBlock> HighlightNeighbors(GridBlock _centerBlock, int _radius, bool _isSelectingDirection)
        {
            List<GridBlock>neighborBlocks = pathfinder.GetNeighbors(_centerBlock, _radius);
            GridBlockMeshKey meshKey = GridBlockMeshKey.None;
            if (_isSelectingDirection) meshKey = GridBlockMeshKey.Arrow;

            HighlightBlocks(neighborBlocks,meshKey, neutralMaterial);

            if (_isSelectingDirection)
            {
                foreach(GridBlock gridBlock in neighborBlocks)
                {
                    gridBlock.RotateGridArrowMesh(_centerBlock);
                }
            }

            return neighborBlocks;
        }

        private void HighlightBlocks(List<GridBlock> _gridBlocks, GridBlockMeshKey _meshKey, Material _highlightMaterial)
        {
            foreach (GridBlock gridBlock in _gridBlocks)
            {
                gridBlock.HighlightBlock(_highlightMaterial, _meshKey);
            }
        }

        public void UnhighlightBlocks(List<GridBlock> _gridBlocks)
        {
            foreach (GridBlock gridBlock in _gridBlocks)
            {
                if (gridBlock == _gridBlocks[_gridBlocks.Count - 1])
                {
                    Fighter tempFighter = gridBlock.contestedFighter;
                    if (tempFighter != null) tempFighter.HighlightFighter(false);
                }

                if (!gridBlock.IsMeshActive()) continue;

                gridBlock.UnhighlightBlock();
            }
            isPathHighlighted = false;
        }

        public GridBlock GetGridBlock(int _x, int _z)
        {
            return pathfinder.GetGridBlock(_x, _z);
        }

        public static List<Transform> GetTravelDestinations(List<GridBlock> _gridBlocks)
        {
            List<Transform> travelDestinations = new List<Transform>();

            foreach (GridBlock gridBlock in _gridBlocks)
            {
                travelDestinations.Add(gridBlock.travelDestination);
            }

            return travelDestinations;
        }

        private void SetupGridBlock(GridBlock _gridBlock, GridCoordinates _gridCoordinates)
        {
            _gridBlock.gridCoordinates = _gridCoordinates;

            Material newMaterial = GetGridBlockMaterial(_gridCoordinates);
            Color textColor = Color.white;

            if (newMaterial == lightMaterial) textColor = Color.black;

            _gridBlock.SetupGridBlock(newMaterial, textColor);
        }

        //Sets the boundaries of the grid system to the lowest and highest x and z values.
        private void BoundaryCheck(GridCoordinates _gridCoordinates)
        {
            int x = _gridCoordinates.x;
            int z = _gridCoordinates.z;

            if (x < minCoordinates.x) minCoordinates.x = x;
            if (x > maxCoordinates.x) maxCoordinates.x = x;

            if (z < minCoordinates.z) minCoordinates.z = z;
            if (z > maxCoordinates.z) maxCoordinates.z = z;
        }

        private Material GetGridBlockMaterial(GridCoordinates _gridCoordinates)
        {
            if (IsLightBlock(_gridCoordinates)) return lightMaterial;
            else return darkMaterial;
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
}
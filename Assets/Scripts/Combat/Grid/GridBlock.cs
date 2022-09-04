using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPGProject.Combat.Grid
{
    /// <summary>
    /// The component of a gridblock in the combat grid.
    /// </summary>
    public class GridBlock : MonoBehaviour, CombatTarget
    {
        [SerializeField] GridBlockMesh[] gridBlockMeshes;
        [SerializeField] GridBlockEffect[] gridBlockEffects; 

        [SerializeField] Image highlightImage = null;
        [SerializeField] TextMeshProUGUI coordinatesText = null;

        public GridCoordinates gridCoordinates;
        public PathfindingCostValues pathfindingCostValues;

        public Transform travelDestination = null;

        public Fighter contestedFighter = null;
        public AbilityBehavior activeAbility = null;

        public bool isMovable = true;

        MeshRenderer meshRenderer = null;
        MeshRenderer highlightMesh = null;

        /// <summary>
        /// Called whenever the grid block updates its current fighter
        /// </summary>
        public event Action<Fighter, GridBlock> onContestedFighterUpdate;

        /// <summary>
        /// Called whenever the grid block updates its current ability
        /// </summary>
        public event Action<AbilityBehavior, GridBlock> onAffectedBlockUpdate;

        public void SetupGridBlock(Material _newMaterial, Color _textColor)
        {
            meshRenderer = GetComponentInChildren<MeshRenderer>();
            SetColors(_newMaterial, _textColor);
            UpdateCoordinatesText(gridCoordinates.x, gridCoordinates.z);
            UnhighlightBlock();
        }

        public void SetContestedFighter(Fighter _fighter)
        {
            contestedFighter = _fighter;
            if (_fighter == null) return;

            onContestedFighterUpdate(contestedFighter, this);
        }

        public void SetActiveAbility(AbilityBehavior _abilityBehavior)
        { 
            activeAbility = _abilityBehavior;
            ActivateGridAbility(_abilityBehavior.gridBlockKey);
            onAffectedBlockUpdate(activeAbility, this);
        }

        public void HighlightBlock(Material _highlightColor, GridBlockMeshKey _gridBlockMeshKey)
        {
            highlightMesh = GetGridBlockMesh(_gridBlockMeshKey);

            if (highlightMesh != null && contestedFighter == null)
            {
                highlightMesh.material = _highlightColor;
                highlightMesh.gameObject.SetActive(true);
            }

            Color32 color = _highlightColor.color;
            color.a = 255;

            highlightImage.color = color;
        }

        public void RotateGridArrowMesh(GridBlock _centerBlock)
        {
            int x = gridCoordinates.x - _centerBlock.gridCoordinates.x;
            int z = gridCoordinates.z - _centerBlock.gridCoordinates.z;

            float yEuler = GetArrowYEuler(x, z);

            Vector3 newEulers = new Vector3(0, yEuler, 0);
            highlightMesh.transform.parent.localEulerAngles = newEulers;
        }

        private float GetArrowYEuler(int _x, int _z)
        {
            if(_x == -1)
            {
                if (_z == -1) return 225f;
                else if (_z == 0) return 270f;
                else if (_z == 1) return 315f;
            }
            else if(_x == 0)
            {
                if (_z == -1) return 180f;
                else if (_z == 1) return 0f;
            }
            else if (_x == 1)
            {
                if (_z == -1) return 135f;
                else if (_z == 0) return 90f;
                else if (_z == 1) return 45f;
            }

            return 0f;
        }

        public void UnhighlightBlock()
        {
            DeactivateGridBlockMeshes();
            Color32 color = Color.white;
            color.a = 65;

            highlightImage.color = color;
        }

        public void SetColors(Material _newMaterial, Color _textColor)
        {
            meshRenderer.material = _newMaterial;
            coordinatesText.color = Color.white;
            //coordinatesText.color = _textColor;
        }

        public bool IsMovable(GridBlock _currentBlock, GridBlock _targetBlock)
        {
            if (!isMovable) return false;
            if (contestedFighter != null)
            {
                Fighter currentFighter = _currentBlock.contestedFighter;
                Fighter targetBlockFighter = _targetBlock.contestedFighter;
                bool isCurrentPlayer = currentFighter.unitInfo.isPlayer;
                bool isMyFighterPlayer = contestedFighter.unitInfo.isPlayer;

                bool isSameTeam = isCurrentPlayer == isMyFighterPlayer;

                if (isSameTeam) return false;

                if (isCurrentPlayer)
                {
                    if (contestedFighter != targetBlockFighter) return false;
                }
                else
                {
                    if (currentFighter.selectedTarget != (CombatTarget)contestedFighter) return false;
                }
            }

            return true;
        }

        public bool IsContested(Fighter _currentFighter)
        {
            if (contestedFighter != null && contestedFighter != _currentFighter) return true;
            else if (activeAbility != null && activeAbility.GetType() == typeof(BattleTeleporter)) return true;
            return false;
        }

        public void ActivateMeshRenderer(bool _shouldActivate)
        {
            meshRenderer.enabled = _shouldActivate;
        }

        public static string CoordsToString(GridCoordinates _gridCoordinates)
        {
            return (_gridCoordinates.x.ToString() + "," + _gridCoordinates.z.ToString());
        }

        public bool IsMeshActive()
        {
            if (highlightMesh == null) return false;
            return highlightMesh.enabled;
        }

        public Transform GetAimTransform()
        {
            return travelDestination;
        }

        private void UpdateCoordinatesText(int _x, int _z)
        {
            if (!coordinatesText.gameObject.activeSelf) return;
            string coordinates = "(" + _x.ToString() + "," + _z.ToString() + ")";
            coordinatesText.text = coordinates;
        }

        private MeshRenderer GetGridBlockMesh(GridBlockMeshKey _gridBlockMeshKey)
        {
            DeactivateGridBlockMeshes();

            foreach(GridBlockMesh gridBlockMesh in gridBlockMeshes)
            {
                if (gridBlockMesh.meshKey == _gridBlockMeshKey) return gridBlockMesh.mesh;
            }

            return null;
        }

        private void ActivateGridAbility(GridBlockAbiltyKey _gridBlockAbiltyKey)
        {
            DeactiveGridAbilities();

            foreach (GridBlockEffect gridBlockEffect in gridBlockEffects)
            {
                if (gridBlockEffect.abiltyKey == GridBlockAbiltyKey.None) continue;
                if(gridBlockEffect.abiltyKey == _gridBlockAbiltyKey)
                {
                    gridBlockEffect.abilityInstance.SetActive(true);
                }
            }
        }

        private void DeactivateGridBlockMeshes()
        {
            foreach(GridBlockMesh gridBlockMesh in gridBlockMeshes)
            {
                gridBlockMesh.mesh.gameObject.SetActive(false);
            }
        }

        private void DeactiveGridAbilities()
        {
            foreach(GridBlockEffect gridBlockEffect in gridBlockEffects)
            {
                gridBlockEffect.abilityInstance.SetActive(false);
            }
        }

        /// <summary>
        /// A serializable dictionary of the highlight meshes and their respective keys.
        /// </summary>
        [Serializable]
        private struct GridBlockMesh
        {
            public GridBlockMeshKey meshKey;
            public MeshRenderer mesh;
        }
        [Serializable]
        private struct GridBlockEffect
        {
            public GridBlockAbiltyKey abiltyKey;
            public GameObject abilityInstance;
        }
    }

    /// <summary>
    /// Current status of the gridblock.
    /// </summary>
    [Serializable]
    public struct GridBlockStatus
    {
        public Fighter contestedFighter;
        public Ability currentEffect;
        //public GridItem currentItem; 
        ///Barrels/Crystals for explosion?
    }

    /// <summary>
    /// This grid's version of a Vector2
    /// </summary>
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

    /// <summary>
    /// Key to differentiate the meshes a grid block can show.
    /// </summary>
    public enum GridBlockMeshKey { None, Arrow, Path }
}
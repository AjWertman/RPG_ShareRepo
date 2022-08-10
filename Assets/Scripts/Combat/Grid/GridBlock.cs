using System;
using TMPro;
using UnityEngine;

namespace RPGProject.Combat.Grid
{
    public class GridBlock : MonoBehaviour, CombatTarget
    {
        [SerializeField] TextMeshProUGUI coordinatesText = null;
        [SerializeField] bool isMovable = true;

        public GridCoordinates gridCoordinates;
        public PathfindingCostValues pathfindingCostValues;

        public Transform travelDestination = null;

        public Fighter contestedFighter = null;
        public Ability activeAbility = null;

        MeshRenderer meshRenderer = null;

        public event Action<Fighter, GridBlock> onContestedFighterUpdate;

        public void InitializeBlock()
        {
            meshRenderer = GetComponentInChildren<MeshRenderer>();
        }

        public void SetupGridBlock(Material _newMaterial, Color _textColor)
        {
            InitializeBlock();
            SetColors(_newMaterial, _textColor);
            UpdateCoordinatesText(gridCoordinates.x, gridCoordinates.z);
        }

        public void SetContestedFighter(Fighter _fighter)
        {
            contestedFighter = _fighter;

            if (contestedFighter == null) return;
            onContestedFighterUpdate(contestedFighter, this);
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
                    return true;
                    //if (currentFighter.selectedTarget != (CombatTarget)contestedFighter) return false;
                }
            }

            return true;
        }

        public void ActivateMeshRenderer(bool _shouldActivate)
        {
            meshRenderer.enabled = _shouldActivate;
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
    }

    [Serializable]
    public struct GridBlockStatus
    {
        public Fighter contestedFighter;
        public Ability currentEffect;
        //public GridItem currentItem; 
        ///Barrels/Crystals for explosion?
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
}
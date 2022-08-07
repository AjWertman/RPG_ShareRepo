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
            coordinatesText.color = _textColor;
        }

        public bool IsMovable(Fighter _currentFighter)
        {
            if (!isMovable) return false;
            if (contestedFighter != null)
            {
                bool isCurrentPlayer = _currentFighter.unitInfo.isPlayer;
                if (isCurrentPlayer == contestedFighter.unitInfo.isPlayer) return false;

                if (!isCurrentPlayer)
                {
                    if (_currentFighter.selectedTarget != (CombatTarget)contestedFighter) return false;
                }
            }

            return true;
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
using System;
using TMPro;
using UnityEngine;

namespace RPGProject.Combat.Grid
{
    public class GridBlock : MonoBehaviour, CombatTarget
    {
        [SerializeField] TextMeshProUGUI coordinatesText = null;

        [SerializeField] GameObject pathfindingTextContainer = null;
        [SerializeField] TextMeshProUGUI fValueText = null;
        [SerializeField] TextMeshProUGUI gValueText = null;
        [SerializeField] TextMeshProUGUI hValueText = null;

        [SerializeField] bool isMovable = true;

        public GridCoordinates gridCoordinates;
        public PathfindingCostValues pathfindingCostValues;

        public Transform travelDestination = null;

        public Fighter contestedFighter = null;
        public Ability activeAbility = null;

        MeshRenderer meshRenderer = null;

        public event Action<Fighter, GridBlock> onContestedFighterUpdate;

        public void InitializePiece()
        {
            meshRenderer = GetComponentInChildren<MeshRenderer>();

            pathfindingTextContainer.SetActive(false);
        }

        public void SetupGridBlock(Material _newMaterial, Color _textColor)
        {
            InitializePiece();
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

        public void UpdateCoordinatesText(int _x, int _z)
        {
            if (!coordinatesText.gameObject.activeSelf) return;
            string coordinates = "(" + _x.ToString() + "," + _z.ToString() + ")";
            coordinatesText.text = coordinates;
        }

        public void UpdatePathfindingValueTexts(int _f, int _g, int _h)
        {
            if (!pathfindingTextContainer.activeSelf)
            {
                pathfindingTextContainer.SetActive(true);
            }

            fValueText.text = _f.ToString();
            gValueText.text = _g.ToString();
            hValueText.text = _h.ToString();
        }

        public bool IsMovable(Fighter _currentFighter, GridBlock _targetBlock)
        {
            if (!isMovable) return false;
            if (contestedFighter != null)
            {
                bool isCurrentPlayer = _currentFighter.GetUnitInfo().IsPlayer();
                if (isCurrentPlayer == contestedFighter.GetUnitInfo().IsPlayer()) return false;

                if (isCurrentPlayer)
                {
                    if (contestedFighter != _targetBlock.contestedFighter) return false;
                }
                else
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
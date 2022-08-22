using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPGProject.Combat.Grid
{
    public class GridBlock : MonoBehaviour, CombatTarget
    {
        [SerializeField] MeshRenderer highlightMesh = null;
        [SerializeField] Image highlightImage = null;
        [SerializeField] TextMeshProUGUI coordinatesText = null;

        public GridCoordinates gridCoordinates;
        public PathfindingCostValues pathfindingCostValues;

        public Transform travelDestination = null;

        public Fighter contestedFighter = null;
        public AbilityBehavior activeAbility = null;

        public bool isMovable = true;

        MeshRenderer meshRenderer = null;

        public event Action<Fighter, GridBlock> onContestedFighterUpdate;
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
            if (_fighter == null) return;
            contestedFighter = _fighter;
            onContestedFighterUpdate(contestedFighter, this);
        }

        public void SetActiveAbility(AbilityBehavior _abilityBehavior)
        { 
            activeAbility = _abilityBehavior;
            onAffectedBlockUpdate(activeAbility, this);
        }

        public void HighlightBlock(Material _highlightColor)
        {
            highlightMesh.material = _highlightColor;
            highlightMesh.gameObject.SetActive(true);

            Color32 color = highlightImage.color;
            color.a = 255;

            highlightImage.color = color;
        }

        public void UnhighlightBlock()
        {
            highlightMesh.gameObject.SetActive(false);
            Color32 color = highlightImage.color;
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

        public bool IsMeshActive()
        {
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
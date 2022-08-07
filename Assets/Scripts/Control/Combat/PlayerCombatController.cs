using RPGProject.Combat;
using RPGProject.Combat.Grid;
using RPGProject.Core;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Control.Combat
{
    public class PlayerCombatController : MonoBehaviour
    {
        BattleGridManager battleGridManager = null;
        BattleHandler battleHandler = null;
        GridSystem gridSystem = null;
        Pathfinder pathfinder = null;

        CombatCamera combatCamera = null;
        Raycaster raycaster = null;

        List<GridBlock> tempPath = new List<GridBlock>();
        List<GridBlock> path = new List<GridBlock>();
        int furthestBlockIndex = 0;

        UnitController currentUnitTurn = null;
        Ability selectedAbility = null;

        private void Awake()
        {
            battleHandler = GetComponent<BattleHandler>();
            raycaster = GetComponent<Raycaster>();

            battleGridManager = GetComponentInChildren<BattleGridManager>();
            gridSystem = GetComponentInChildren<GridSystem>();
            pathfinder = GetComponentInChildren<Pathfinder>();

            combatCamera = FindObjectOfType<CombatCamera>();

            battleHandler.onUnitTurnUpdate += UpdateCurrentUnitTurn;
        }

        private void Start()
        {
            battleHandler.GetBattleUIManager().onAbilitySelect += SetSelectedAbility;
        }

        private void Update()
        {
            //Testing
            if (Input.GetKeyDown(KeyCode.K))
            {
                raycaster.isRaycasting = true;
            }
            //

            HandleCameraControl();

            if (!raycaster.isRaycasting) return;

            HandleRaycasting();
        }

        private void HandleCameraControl()
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                combatCamera.RotateFreeLook(true);
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                combatCamera.RotateFreeLook(false);
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                combatCamera.RecenterCamera();
            }
        }

        private void HandleRaycasting()
        {
            RaycastHit hit = raycaster.GetRaycastHit();
            if (hit.collider == null) return;

            CombatTarget combatTarget = hit.collider.GetComponent<CombatTarget>();

            if (combatTarget != null)
            {
                GridBlock targetBlock = GetTargetBlock(combatTarget);
                HandlePathfinding(targetBlock);

                if (Input.GetMouseButtonDown(0))
                {
                    if (tempPath == null) return;
                    path = GetFurthestPath(tempPath);
                    OnPlayerClick(targetBlock);
                }
            }
        }

        private void OnPlayerClick(GridBlock _targetBlock)
        {
            raycaster.isRaycasting = false;
            gridSystem.UnhighlightPath(tempPath);

            if (selectedAbility != null)
            {
                CombatTarget target = _targetBlock;
                Fighter targetBlockFighter = _targetBlock.contestedFighter;

                if (targetBlockFighter != null) target = targetBlockFighter;
                if (target == null) return;

                if (selectedAbility.requiresTarget && targetBlockFighter == null) return;

                battleHandler.OnPlayerMove(target, selectedAbility);
                selectedAbility = null;
            }
            else
            {
                StartCoroutine(currentUnitTurn.PathExecution(path));
            }
        }

        private void HandlePathfinding(GridBlock _targetBlock)
        {
            if (_targetBlock == null) return;

            GridBlock currentBlock = null;
            if (currentUnitTurn != null) currentBlock = currentUnitTurn.currentBlock;

            if (currentBlock == null) return;
            if (_targetBlock == currentBlock) return;

            if (!_targetBlock.IsMovable(currentBlock.contestedFighter)) return;

            gridSystem.UnhighlightPath(tempPath);
            tempPath = pathfinder.FindPath(currentBlock, _targetBlock);

            furthestBlockIndex = GetFurthestBlockIndex(tempPath);

            if (selectedAbility != null) return;
            gridSystem.HighlightPath(tempPath, furthestBlockIndex);
        }

        private void UpdateCurrentUnitTurn(UnitController _unitController)
        {
            currentUnitTurn = _unitController;
            if (currentUnitTurn.GetUnitInfo().isPlayer) raycaster.isRaycasting = true;
            else raycaster.isRaycasting = false;
        }

        private void SetSelectedAbility(Ability _selectedAbility)
        {
            selectedAbility = _selectedAbility;
        }
         
        private GridBlock GetTargetBlock(CombatTarget _combatTarget)
        {
            GridBlock targetBlock = null;
            Fighter fighter = _combatTarget.GetComponent<Fighter>();
            if (fighter != null)
            {
                targetBlock = battleGridManager.GetGridBlockByFighter(fighter);
            }
            else
            {
                targetBlock = _combatTarget.GetComponent<GridBlock>();
            }
            return targetBlock;
        }

        private List<GridBlock> GetFurthestPath(List<GridBlock> _path)
        {
            List<GridBlock> furthestPath = new List<GridBlock>();

            for (int i = 0; i < _path.Count; i++)
            {
                furthestPath.Add(_path[i]);

                if (i == furthestBlockIndex) break;
            }

            return furthestPath;
        }

        private int GetFurthestBlockIndex(List<GridBlock> _path)
        {
            float totalGCostAllowance = currentUnitTurn.GetTotalPossibleGCostAllowance();

            foreach (GridBlock gridBlock in _path)
            {
                if (totalGCostAllowance >= gridBlock.pathfindingCostValues.gCost) continue;

                return _path.IndexOf(gridBlock) - 1;
            }

            return _path.Count - 1;
        }
    }
}
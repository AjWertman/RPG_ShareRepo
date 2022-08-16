using RPGProject.Combat;
using RPGProject.Combat.Grid;
using RPGProject.Core;
using System;
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

        BattleCamera battleCamera = null;
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

            battleCamera = FindObjectOfType<BattleCamera>();

            battleHandler.onUnitTurnUpdate += UpdateCurrentUnitTurn;
        }

        private void Start()
        {
            battleHandler.GetBattleUIManager().onAbilitySelect += SetSelectedAbility;
            SetupBattleCamera();
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
            float vertical = Input.GetAxisRaw("Vertical");
            float horizontal = Input.GetAxisRaw("Horizontal");

            Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;
            
            if(inputDirection.magnitude > .01f)
            {
                battleCamera.MoveFollowTransform(inputDirection);
            }

            if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow)) battleCamera.RotateFreeLook(true);
            else if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.RightArrow)) battleCamera.RotateFreeLook(false);


            if(Input.GetAxisRaw("Mouse ScrollWheel") > 0) battleCamera.Zoom(true);
            else if(Input.GetAxisRaw("Mouse ScrollWheel") < 0) battleCamera.Zoom(false);

            //R
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                battleCamera.RecenterCamera();
                //battleCamera.SetFollowTarget(currentUnitTurn.transform);
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
                else if (Input.GetMouseButtonDown(1))
                {
                    Fighter fighter = (Fighter)combatTarget;

                    if (fighter == null) return;
                    battleCamera.SetFollowTarget(fighter.GetAimTransform());
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

            if (_targetBlock.IsMovable(currentBlock, _targetBlock) == false) return;

            gridSystem.UnhighlightPath(tempPath);
            tempPath = pathfinder.FindOptimalPath(currentBlock, _targetBlock);

            furthestBlockIndex = GetFurthestBlockIndex(tempPath);

            if (selectedAbility != null) return;
            gridSystem.HighlightPath(tempPath, furthestBlockIndex);
        }

        private void SetupBattleCamera()
        {
            GridCoordinates minCoordinates = gridSystem.minCoordinates;
            GridCoordinates maxCoordinates = gridSystem.maxCoordinates;

            battleCamera.InitalizeBattleCamera(minCoordinates.x, minCoordinates.z, maxCoordinates.x, maxCoordinates.z);
        }

        private void UpdateCurrentUnitTurn(UnitController _unitController)
        {
            currentUnitTurn = _unitController;
            if (currentUnitTurn.unitInfo.isPlayer) raycaster.isRaycasting = true;
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
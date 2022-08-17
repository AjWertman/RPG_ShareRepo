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
        List<CombatTarget> selectedTargets = new List<CombatTarget>();
        Ability selectedAbility = null;

        bool isPathfinding = true;
        bool isSelecting = false;

        private void Awake()
        {
            battleHandler = GetComponent<BattleHandler>();
            raycaster = GetComponent<Raycaster>();

            battleGridManager = GetComponentInChildren<BattleGridManager>();
            gridSystem = GetComponentInChildren<GridSystem>();
            pathfinder = GetComponentInChildren<Pathfinder>();

            battleCamera = FindObjectOfType<BattleCamera>();

            battleHandler.onUnitTurnUpdate += UpdateCurrentUnitTurn;
            battleHandler.onPlayerMoveCompletion += ActivateRaycaster;
        }

        private void Start()
        {
            battleHandler.GetBattleUIManager().onAbilitySelect += SetSelectedAbility;
            SetupBattleCamera();
        }

        private void Update()
        {
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
                if (isPathfinding) HandlePathfinding(targetBlock);

                if (Input.GetMouseButtonDown(0))
                {
                    if (tempPath == null) return;
                    path = GetFurthestPath(tempPath);
                    OnPlayerClick(combatTarget);
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    Fighter fighter = (Fighter)combatTarget;

                    if (fighter == null) return;
                    battleCamera.SetFollowTarget(fighter.GetAimTransform());
                }
            }
        }

        private void OnPlayerClick(CombatTarget _combatTarget)
        { 
            if (_combatTarget == null) return;

            CombatTarget trueTarget = _combatTarget;

            Type targetType = _combatTarget.GetType();

            GridBlock targetBlock = null;
            Fighter targetFighter = null;

            if (selectedAbility != null)
            {
                if (targetType == typeof(GridBlock))
                {
                    targetBlock = (GridBlock)_combatTarget;
                    Fighter contestedFighter = targetBlock.contestedFighter;
                    if (contestedFighter != null)
                    {
                        targetFighter = contestedFighter;
                        trueTarget = targetFighter;
                    }
                    else trueTarget = targetBlock;
                }
                else if (targetType == typeof(Fighter))
                {
                    targetFighter = (Fighter)_combatTarget;
                    trueTarget = targetFighter;
                }

                if (CanTarget(selectedAbility, trueTarget))
                {
                    selectedTargets.Add(trueTarget);

                    if (selectedTargets.Count == selectedAbility.requiredTargetAmount)
                    {
                        raycaster.isRaycasting = false;
                        gridSystem.UnhighlightPath(tempPath);

                        if (selectedAbility.requiresTarget && targetFighter == null) return;

                        battleHandler.OnPlayerMove(selectedTargets, selectedAbility);
                        selectedAbility = null;
                        selectedTargets.Clear();

                    }
                    else
                    {
                        print("needs more targets");
                    }
                }
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

        private void ActivateRaycaster()
        {
            raycaster.isRaycasting = true;
        }

        private GridBlock GetTargetBlock(CombatTarget _combatTarget)
        {
            GridBlock targetBlock = null;
            Type targetType = _combatTarget.GetType();

            if (targetType == typeof(Fighter))
            {
                Fighter fighter = _combatTarget.GetComponent<Fighter>();
                targetBlock = battleGridManager.GetGridBlockByFighter(fighter);
            }
            else if(targetType == typeof(BattleTeleporter))
            {
                BattleTeleporter battleTeleporter = _combatTarget.GetComponent<BattleTeleporter>();
                targetBlock = battleTeleporter.teleportBlock;
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

        private bool CanTarget(Ability _ability, CombatTarget _target)
        {
            TargetingType targetingType = _ability.targetingType;

            if (targetingType == TargetingType.Everything) return true;

            Fighter targetFighter = _target as Fighter;
            bool isCharacter = targetFighter != null;

            if (isCharacter)
            {
                if (targetingType == TargetingType.Everyone) return true;

                bool isFriendlyTargeting = (targetingType == TargetingType.PlayersOnly || targetingType == TargetingType.SelfOnly);
                bool isTeammate = (currentUnitTurn.unitInfo.isPlayer == targetFighter.unitInfo.isPlayer);

                if (isTeammate && isFriendlyTargeting) return true;
                else if (!isTeammate && targetingType == TargetingType.EnemiesOnly) return true;
            }
            else
            {
                GridBlock targetBlock = _target as GridBlock;
                if (targetBlock != null && targetingType == TargetingType.GridBlocksOnly) return true;
            }

            print("cannot target");
            return false;
        }
    }
}
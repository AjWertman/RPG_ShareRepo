using RPGProject.Combat;
using RPGProject.Combat.Grid;
using RPGProject.Core;
using System;
using System.Collections;
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

        GridBlock blockToSelectFrom = null;
        List<GridBlock> neighborBlocks = new List<GridBlock>();
        bool isSelectingFaceDirection = false;
        bool hasHighlightedNeighbors = false;

        private void Awake()
        {
            battleHandler = GetComponent<BattleHandler>();
            raycaster = GetComponent<Raycaster>();

            battleGridManager = GetComponentInChildren<BattleGridManager>();
            gridSystem = GetComponentInChildren<GridSystem>();
            pathfinder = GetComponentInChildren<Pathfinder>();

            battleCamera = FindObjectOfType<BattleCamera>();

            battleHandler.onUnitTurnUpdate += UpdateCurrentUnitTurn;
            battleHandler.onPlayerMoveCompletion += OnPlayerMoveCompletion;
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

            if (isSelectingFaceDirection)
            {
                if (!hasHighlightedNeighbors)
                {
                    hasHighlightedNeighbors = true;
                    neighborBlocks = pathfinder.GetNeighbors(blockToSelectFrom);
                    gridSystem.HighlightBlocks(neighborBlocks);
                }
            }

            if (combatTarget != null)
            {
                GridBlock targetBlock = GetTargetBlock(combatTarget);
                if (isPathfinding) HandlePathfinding(targetBlock);

                if (Input.GetMouseButtonDown(0))
                {
                    if (tempPath == null) return;
                    path = GetFurthestPath(tempPath);
                    StartCoroutine(OnPlayerClick(combatTarget));
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    Fighter fighter = (Fighter)combatTarget;

                    if (fighter == null) return;
                    battleCamera.SetFollowTarget(fighter.GetAimTransform());
                }
            }
        }

        private IEnumerator OnPlayerClick(CombatTarget _selectedTarget)
        { 
            if (_selectedTarget == null) yield break;

            Type targetType = _selectedTarget.GetType();

            CombatTarget trueTarget = GetTrueTarget(_selectedTarget, targetType);

                                    

            if(targetType == typeof(Fighter))
            {
                Fighter fighter = (Fighter)trueTarget;
                if (fighter == currentUnitTurn.GetFighter()) yield break;
            }

            if (!isSelectingFaceDirection)
            {
                if (selectedAbility != null)
                {
                    if (CanTarget(selectedAbility, trueTarget))
                    {
                        selectedTargets.Add(trueTarget);

                        if (selectedTargets.Count == selectedAbility.requiredTargetAmount)
                        {
                            raycaster.isRaycasting = false;
                            gridSystem.UnhighlightBlocks(tempPath);

                            if (selectedAbility.requiresTarget && (Fighter)trueTarget == null) yield break;

                            battleHandler.OnPlayerMove(selectedTargets, selectedAbility);
                            selectedAbility = null;
                            selectedTargets.Clear();
                        }
                    }
                }
                else
                {
                    isPathfinding = false;
                    yield return currentUnitTurn.PathExecution(path);

                    bool isTeleporter = targetType == typeof(BattleTeleporter) && (BattleTeleporter)trueTarget != null;
                    bool isFighter = (targetType == typeof(Fighter) && (Fighter)trueTarget != null);

                    //Specifed cast is not valid when clicking specifically on a Block that has a fighter on it
                    bool isGridBlock = (targetType == typeof(GridBlock) && (GridBlock)trueTarget != null);

                    if(isGridBlock)
                    {
                        print(isGridBlock);
                        GridBlock block = (GridBlock)trueTarget;
                        if (block.contestedFighter != null && block.contestedFighter != currentUnitTurn.GetFighter()) isFighter = true;
                    }

                    if (!isTeleporter && isFighter)
                    {
                        isSelectingFaceDirection = false;
                        isPathfinding = true;
                        yield break;
                    }

                    if (isTeleporter)
                    {
                        BattleTeleporter battleTeleporter = (BattleTeleporter)trueTarget;
                        GridBlock linkedTeleporterBlock = battleTeleporter.linkedTeleporter.myBlock;
                        blockToSelectFrom = GetDirectionSelectionBlock(linkedTeleporterBlock, targetType);
                    }
                    else if(!isFighter && !isTeleporter)
                    {
                        blockToSelectFrom = GetDirectionSelectionBlock(trueTarget, targetType);
                    }

                    isSelectingFaceDirection = true;
                    gridSystem.UnhighlightBlocks(tempPath);
                }
            }
            else
            {
                OnDirectionBlockSelect(_selectedTarget);
            }
        }

        private void OnDirectionBlockSelect(CombatTarget _selectedTarget)
        {
            GridBlock selectedDirection = GetTargetBlock(_selectedTarget);
            if (selectedDirection != null && pathfinder.IsNeighborBlock(blockToSelectFrom, selectedDirection))
            {
                Vector3 lookDestination = selectedDirection.travelDestination.position;
                Vector3 lookPosition = new Vector3(lookDestination.x, currentUnitTurn.transform.position.y, lookDestination.z);
                currentUnitTurn.transform.LookAt(lookPosition);

                isSelectingFaceDirection = false;
                isPathfinding = true;
                gridSystem.UnhighlightBlocks(neighborBlocks);
                hasHighlightedNeighbors = false;
                blockToSelectFrom = null;
                neighborBlocks.Clear();
            }
        }

        private GridBlock GetDirectionSelectionBlock(CombatTarget _combatTarget, Type _targetType)
        {
            GridBlock directionSelectionBlock = null;
            if (_targetType == typeof(GridBlock))
            {
                if ((GridBlock)_combatTarget != null) directionSelectionBlock = (GridBlock)_combatTarget;
            }
            else if (_targetType == typeof(Fighter))
            {
                if((Fighter)_combatTarget != null) directionSelectionBlock = battleGridManager.GetGridBlockByFighter((Fighter)_combatTarget);
            }
            else if (_targetType == typeof(BattleTeleporter))
            {
                if ((BattleTeleporter)_combatTarget != null)
                {
                    BattleTeleporter battleTeleporter = (BattleTeleporter)_combatTarget;
                    AbilityBehavior parentBehavior = (AbilityBehavior)battleTeleporter;
                    directionSelectionBlock=  battleGridManager.GetGridBlockByAbility(parentBehavior);
                }
            }

            return directionSelectionBlock;
        }

        private CombatTarget GetTrueTarget(CombatTarget _selectedTarget, Type _targetType)
        {
            CombatTarget trueTarget = _selectedTarget;
            GridBlock targetBlock = null;
            Fighter targetFighter = null;

            if (_targetType == typeof(GridBlock))
            {
                targetBlock = (GridBlock)_selectedTarget;
                Fighter contestedFighter = targetBlock.contestedFighter;
                if (contestedFighter != null)
                {
                    targetFighter = contestedFighter;
                    trueTarget = targetFighter;
                }
                else trueTarget = targetBlock;
            }
            else if (_targetType == typeof(Fighter))
            {
                targetFighter = (Fighter)_selectedTarget;
                trueTarget = targetFighter;
            }

            return trueTarget;
        }

        private void HandlePathfinding(GridBlock _targetBlock)
        {
            if (_targetBlock == null) return;

            GridBlock currentBlock = null;
            if (currentUnitTurn != null) currentBlock = currentUnitTurn.currentBlock;

            if (currentBlock == null) return;
            if (_targetBlock == currentBlock)
            {
                gridSystem.UnhighlightBlocks(tempPath);
                return;
            }

            if (_targetBlock.IsMovable(currentBlock, _targetBlock) == false) return;

            gridSystem.UnhighlightBlocks(tempPath);
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

        private void OnPlayerMoveCompletion()
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
                targetBlock = battleTeleporter.myBlock;
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
            bool canTarget = false;
            TargetingType targetingType = _ability.targetingType;

            if (targetingType == TargetingType.Everything) canTarget= true;

            Fighter targetFighter = _target as Fighter;
            bool isCharacter = targetFighter != null;

            if (isCharacter)
            {
                if (targetingType == TargetingType.Everyone) canTarget= true;

                bool isFriendlyTargeting = (targetingType == TargetingType.PlayersOnly || targetingType == TargetingType.SelfOnly);
                bool isTeammate = (currentUnitTurn.unitInfo.isPlayer == targetFighter.unitInfo.isPlayer);

                if (isTeammate && isFriendlyTargeting) canTarget= true;
                else if (!isTeammate && targetingType == TargetingType.EnemiesOnly) canTarget= true;
            }
            else
            {
                GridBlock targetBlock = _target as GridBlock;
                if (targetBlock != null && targetingType == TargetingType.GridBlocksOnly) canTarget= true;
            }

            print(canTarget.ToString());
            return canTarget;
        }
    }
}
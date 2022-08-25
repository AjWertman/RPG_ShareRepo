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
        BattleHandler battleHandler = null;
        BattleUIManager battleUIManager = null;
        BattleGridManager battleGridManager = null;
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

        Fighter highlightedTarget = null;

        bool isPathfinding = true;

        GridBlock blockToSelectFrom = null;
        List<GridBlock> neighborBlocks = new List<GridBlock>();
        bool isSelectingFaceDirection = false;
        bool hasHighlightedNeighbors = false;

        public bool canAdvanceTurn = true;

        private void Awake()
        {
            battleHandler = GetComponent<BattleHandler>();
            raycaster = GetComponent<Raycaster>();

            battleUIManager = GetComponentInChildren<BattleUIManager>();
            battleGridManager = GetComponentInChildren<BattleGridManager>();
            gridSystem = GetComponentInChildren<GridSystem>();
            pathfinder = GetComponentInChildren<Pathfinder>();

            battleCamera = FindObjectOfType<BattleCamera>();

            battleHandler.onUnitTurnUpdate += UpdateCurrentUnitTurn;
            battleHandler.onPlayerMoveCompletion += OnPlayerMoveCompletion;
        }

        private void Start()
        {
            battleUIManager.onAbilitySelect += SetSelectedAbility;
            SetupBattleCamera();
        }

        private void Update()
        {
            HandleCameraControl();

            if (currentUnitTurn == null || !currentUnitTurn.unitInfo.isPlayer) return;
            HandlePlayerControls();

            if (!raycaster.isRaycasting) return;
            if (battleUIManager.isSelectingAbility) return;

            HandleRaycasting();
        }

        private void HandlePlayerControls()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (isSelectingFaceDirection) ClearNeighborSelection();
                else gridSystem.UnhighlightBlocks(tempPath);

                battleUIManager.ActivateAbilitySelectMenu();
            }

            if (Input.GetKeyDown(KeyCode.Return) && canAdvanceTurn)
            {
                ClearNeighborSelection();
                battleHandler.AdvanceTurn();
            }

            if(Input.GetKeyDown(KeyCode.Escape) && selectedAbility != null)
            {
                selectedAbility = null;
                currentUnitTurn.GetAimLine().ResetLine();
            }
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

            if (Input.GetKey(KeyCode.Q)) battleCamera.RotateFreeLook(true);
            else if (Input.GetKey(KeyCode.E)) battleCamera.RotateFreeLook(false);

            float scrollWheelInput = Input.GetAxisRaw("Mouse ScrollWheel");
            if (scrollWheelInput > 0) battleCamera.Zoom(true);
            else if(scrollWheelInput < 0) battleCamera.Zoom(false);

            
            if (Input.GetKeyDown(KeyCode.R))
            {
                battleCamera.RecenterCamera();
                battleCamera.SetFollowTarget(currentUnitTurn.transform);
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
            else
            {
                bool isBasicAttack = false;
                if (selectedAbility == null && currentUnitTurn.unitInfo.basicAttack.attackRange > 0)
                {
                    isBasicAttack = true;
                    selectedAbility = currentUnitTurn.unitInfo.basicAttack;
                }

                if(selectedAbility != null && selectedAbility.attackRange > 0)
                {
                    AimLine currentAimLine = currentUnitTurn.GetAimLine();
                    if (!isBasicAttack)
                    {
                        if (!DrawAimLine(hit, selectedAbility)) return;
                    }
                    else
                    {
                        if (DrawAimLine(hit, currentUnitTurn.unitInfo.basicAttack)) gridSystem.UnhighlightBlocks(tempPath);
                        else currentAimLine.ResetLine();
                    }

                    if (currentAimLine.hitFighter != null) combatTarget = currentAimLine.hitFighter;
                }
            }

            if (combatTarget != null)
            {
                GridBlock targetBlock = GetTargetBlock(combatTarget);
                if (isPathfinding) HandlePathfinding(targetBlock);

                HandlePhysicalHighlighting(combatTarget, targetBlock);

                if (Input.GetMouseButtonDown(0))
                {
                    if (tempPath == null) return;
                    battleUIManager.UnhighlightTarget();
                    path = GetFurthestPath(tempPath);
                    StartCoroutine(OnPlayerClick(combatTarget));
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    if (combatTarget.GetType() != typeof(Fighter)) return;
                    Fighter fighter = (Fighter)combatTarget;

                    if (fighter == null) return;
                    battleCamera.SetFollowTarget(fighter.GetAimTransform());
                }
            }
            else
            {
                if (gridSystem.isPathHighlighted)
                {
                    gridSystem.UnhighlightBlocks(tempPath);
                    tempPath.Clear();
                }
            }
        }

        private bool DrawAimLine(RaycastHit _hit, Ability _selectedAbility)
        {
            if (_hit.collider == null || _selectedAbility == null) return false;
            AimLine aimLine = currentUnitTurn.GetAimLine();

            Transform startTransform = null;
            Transform aimTransform = null;
            Vector3 aimPosition = Vector3.zero;

            startTransform = currentUnitTurn.GetCharacterMesh().rHandTransform;
            bool isRightHand = _selectedAbility.combo[0].spawnLocationOverride == SpawnLocation.RHand;
            if (!isRightHand) startTransform = currentUnitTurn.GetCharacterMesh().lHandTransform;

            Fighter fighter = _hit.collider.GetComponent<Fighter>();
            if (fighter != null)
            {
                aimTransform = fighter.characterMesh.aimTransform;
                aimPosition = aimTransform.position;
            }
            else
            {
                aimPosition = _hit.point;
            }

            return aimLine.DrawAimLine(startTransform, aimPosition, _selectedAbility.attackRange);
        }

        private void HandlePhysicalHighlighting(CombatTarget _combatTarget, GridBlock _targetBlock)
        {
            if (_combatTarget.GetType() == typeof(Fighter))
            {
                if (battleUIManager.highlightedTarget != null && battleUIManager.isUIHighlight) return;
                battleUIManager.HighlightTarget((Fighter)_combatTarget, false);
            }
            else if (_targetBlock.contestedFighter != null)
            {
                if (battleUIManager.highlightedTarget != null && battleUIManager.isUIHighlight) return;
                battleUIManager.HighlightTarget(_targetBlock.contestedFighter, false);
            }
            else if (battleUIManager.highlightedTarget != null && !battleUIManager.isUIHighlight)
            {
                battleUIManager.UnhighlightTarget();
            }
        }

        private IEnumerator OnPlayerClick(CombatTarget _selectedTarget)
        { 
            if (_selectedTarget == null) yield break;
            if (!battleHandler.IsBattling()) yield break;
            if (currentUnitTurn != null && !currentUnitTurn.unitInfo.isPlayer) yield break;

            currentUnitTurn.GetAimLine().ResetLine();

            Type targetType = _selectedTarget.GetType();
            CombatTarget trueTarget = GetTrueTarget(_selectedTarget, targetType);

            if (!isSelectingFaceDirection)
            {
                if (selectedAbility == null && currentUnitTurn.unitInfo.basicAttack.attackRange > 0)
                {
                    if(trueTarget.GetType() == typeof(Fighter)) selectedAbility = currentUnitTurn.unitInfo.basicAttack;
                }
           
                if (selectedAbility != null)
                {
                    if (CanTarget(selectedAbility, trueTarget))
                    {
                        selectedTargets.Add(trueTarget);

                        if (selectedTargets.Count == selectedAbility.requiredTargetAmount)
                        {
                            raycaster.isRaycasting = false;
                            canAdvanceTurn = false;
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
                    raycaster.isRaycasting = false;
                    canAdvanceTurn = false;
                    gridSystem.UnhighlightBlocks(tempPath);
                    isPathfinding = false;

                    yield return currentUnitTurn.PathExecution(path);

                    if (!currentUnitTurn.unitInfo.isPlayer) yield break;
                    
                    bool isTeleporter = targetType == typeof(BattleTeleporter) && (BattleTeleporter)_selectedTarget != null;
                    bool isFighter = (targetType == typeof(Fighter) && (Fighter)_selectedTarget != null);
                    bool isGridBlock = (targetType == typeof(GridBlock) && (GridBlock)_selectedTarget != null);
                    if(isGridBlock)
                    {
                        GridBlock block = (GridBlock)_selectedTarget;
                        if (block.contestedFighter != null && block.contestedFighter != currentUnitTurn.GetFighter()) isFighter = true;

                        else if (block.activeAbility != null && block.activeAbility.GetType() == typeof(BattleTeleporter))
                        {
                            BattleTeleporter battleTeleporter = (BattleTeleporter)block.activeAbility;
                            _selectedTarget = battleTeleporter;
                            targetType = typeof(BattleTeleporter);
                            isTeleporter = true;
                        }
                    }

                    if (isTeleporter || !isFighter)
                    {
                        blockToSelectFrom = GetDirectionSelectionBlock(_selectedTarget, targetType);
                    }
                    else
                    {
                        isSelectingFaceDirection = false;
                        isPathfinding = true;
                        yield break;
                    }

                    isSelectingFaceDirection = true;
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
                if (blockToSelectFrom.activeAbility != null && blockToSelectFrom.activeAbility.GetType() == typeof(BattleTeleporter))
                {
                    currentUnitTurn.Teleport(selectedDirection);              
                }
                else
                {
                    Vector3 lookDestination = selectedDirection.travelDestination.position;
                    Vector3 lookPosition = new Vector3(lookDestination.x, currentUnitTurn.transform.position.y, lookDestination.z);
                    currentUnitTurn.transform.LookAt(lookPosition);
                }

                ClearNeighborSelection();
            }
        }

        private void ClearNeighborSelection()
        {
            isSelectingFaceDirection = false;
            isPathfinding = true;
            gridSystem.UnhighlightBlocks(neighborBlocks);
            hasHighlightedNeighbors = false;
            blockToSelectFrom = null;
            neighborBlocks.Clear();

            raycaster.isRaycasting = true;
        }

        private GridBlock GetDirectionSelectionBlock(CombatTarget _combatTarget, Type _targetType)
        {
            GridBlock directionSelectionBlock = null;

            if (_targetType == typeof(Fighter))
            {
                if((Fighter)_combatTarget != null) directionSelectionBlock = battleGridManager.GetGridBlockByFighter((Fighter)_combatTarget);
            }
            else if (_targetType == typeof(BattleTeleporter))
            {
                if ((BattleTeleporter)_combatTarget != null)
                {
                    BattleTeleporter battleTeleporter = (BattleTeleporter)_combatTarget;
                    AbilityBehavior parentBehavior = battleTeleporter.linkedTeleporter;
                    directionSelectionBlock =  battleGridManager.GetGridBlockByAbility(parentBehavior);
                }
            }
            else
            {                
                directionSelectionBlock = currentUnitTurn.currentBlock;
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
                //Refactor - every frame
                gridSystem.UnhighlightBlocks(tempPath);
                return;
            }

            if (_targetBlock.IsMovable(currentBlock, _targetBlock) == false) return;

            List<GridBlock> optimalPath = pathfinder.FindOptimalPath(currentBlock, _targetBlock);
            if (!pathfinder.ArePathsEqual(tempPath, optimalPath))
            {
                if(gridSystem.isPathHighlighted) gridSystem.UnhighlightBlocks(tempPath);
                tempPath = optimalPath;

                furthestBlockIndex = GetFurthestBlockIndex(tempPath);

                if (selectedAbility != null) return;
                gridSystem.HighlightPath(tempPath, furthestBlockIndex);
            }
        }

        private void SetupBattleCamera()
        {
            GridCoordinates minCoordinates = gridSystem.minCoordinates;
            GridCoordinates maxCoordinates = gridSystem.maxCoordinates;

            battleCamera.InitalizeBattleCamera(minCoordinates.x, minCoordinates.z, maxCoordinates.x, maxCoordinates.z);
        }

        private void UpdateCurrentUnitTurn(UnitController _unitController)
        { 
            battleUIManager.UnhighlightTarget();
            gridSystem.UnhighlightBlocks(tempPath);

            currentUnitTurn = _unitController;
            if (currentUnitTurn.unitInfo.isPlayer)
            {
                raycaster.isRaycasting = true;
                canAdvanceTurn = true;
                isPathfinding = true;
            }
            else
            {
                raycaster.isRaycasting = false;
                canAdvanceTurn = false;
                isPathfinding = false;
            }
            battleCamera.SetFollowTarget(_unitController.GetCharacterMesh().aimTransform);
        }

        private void SetSelectedAbility(Ability _selectedAbility)
        {
            selectedAbility = _selectedAbility;
        }

        private void OnPlayerMoveCompletion()
        {
            raycaster.isRaycasting = true;
            canAdvanceTurn = true;
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
                if (targetFighter == currentUnitTurn.GetFighter()) canTarget = false;

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

            return canTarget;
        }
    }
}
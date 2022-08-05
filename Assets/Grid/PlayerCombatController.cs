using RPGProject.Combat;
using RPGProject.Combat.Grid;
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

        UnitController currentUnitTurn = null;

        bool isFindingPath = false;

        List<GridBlock> tempPath = new List<GridBlock>();
        List<GridBlock> path = new List<GridBlock>();
        int furthestBlockIndex = 0;

        Ability selectedAbility = null;

        bool isRaycasting = false;

        private void Awake()
        {
            battleGridManager = FindObjectOfType<BattleGridManager>();
            battleHandler = GetComponent<BattleHandler>();
            gridSystem = FindObjectOfType<GridSystem>();
            pathfinder = FindObjectOfType<Pathfinder>();

            battleHandler.onUnitTurnUpdate += UpdateCurrentUnitTurn;
        }
        private void Start()
        {
            battleHandler.GetBattleUIManager().onAbilitySelect += SetSelectedAbility;
        }

        private void SetSelectedAbility(Ability _selectedAbility)
        {
            selectedAbility = _selectedAbility;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                isRaycasting = true;
            }

            if (!isRaycasting) return;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                CombatTarget combatTarget = hit.collider.GetComponent<CombatTarget>();

                if (combatTarget != null)
                {
                    GridBlock targetBlock = GetTargetBlock(combatTarget);
                    HandlePathfinding(targetBlock);

                    if (Input.GetMouseButtonDown(0))
                    {
                        if (tempPath == null) return;
                        path = GetFurthestPath(tempPath);
                        OnClick(targetBlock);
                    }
                }

                //if (selectedAbility != null)
                //{
                //    float attackRange = selectedAbility.attackRange;

                //    //Melee
                //    if (attackRange == 0)
                //    {
                //        //If click 
                //        ///move to target
                //        ///attack
                //    }
                //    else
                //    {
                //        // if click 
                //        ///If not in range - dont cast????
                //        ///If in range - cast
                //    }

                //    return;
                //}
                //else
                //{

                //    //Fighter fighter = hit.collider.GetComponent<Fighter>();

                //    //if (fighter != null)
                //    //{
                //    //    gridSystem.UnhighlightPath(tempPath);

                //    //    tempPath = pathfinder.FindPath(currentBlock, GetGridBlockByFighter(fighter));
                //    //}
                //}
            }
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

        private void HandlePathfinding(GridBlock _targetBlock)
        {
            if (_targetBlock == null) return;

            GridBlock currentBlock = null;
            if (currentUnitTurn != null) currentBlock = currentUnitTurn.currentBlock;

            if (_targetBlock == currentBlock) return;
            if (!_targetBlock.IsMovable(currentBlock.contestedFighter,_targetBlock)) return;
            if (currentBlock == null) return;

            gridSystem.UnhighlightPath(tempPath);
            tempPath = pathfinder.FindPath(currentBlock, _targetBlock);

            furthestBlockIndex = GetFurthestBlockIndex(tempPath);

            if (selectedAbility != null) return;
            gridSystem.HighlightPath(tempPath, furthestBlockIndex);
        }

        private void OnClick(GridBlock _targetBlock)
        {
            isRaycasting = false;
            gridSystem.UnhighlightPath(tempPath);

            if (selectedAbility != null)
            {
                CombatTarget target = _targetBlock;
                Fighter targetBlockFighter = _targetBlock.contestedFighter;

                if (targetBlockFighter != null)
                {
                    target = targetBlockFighter;
                }

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

        public void UpdateCurrentUnitTurn(UnitController _unitController)
        {
            currentUnitTurn = _unitController;
            if (currentUnitTurn.GetUnitInfo().IsPlayer()) isRaycasting = true;
            else isRaycasting = false;
        }

        public int GetFurthestBlockIndex(List<GridBlock> _path)
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
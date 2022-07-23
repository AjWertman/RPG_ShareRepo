using RPGProject.Combat;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Control
{
    public class BattleGridManager : MonoBehaviour
    {
        //Refactor - move battleposition manager to this?
        GridSystem gridSystem = null;
        Pathfinder pathfinder = null;

        UnitController currentUnitTurn = null;

        GridBlock[] gridBlocks = null;

        Dictionary<Fighter, GridBlock> occupiedBlocksDict = new Dictionary<Fighter, GridBlock>(); 

        Dictionary<GridBlock, GridBlockStatus> gridBlockStatusDict = new Dictionary<GridBlock, GridBlockStatus>();

        List<GridBlock> tempPath = new List<GridBlock>();

        bool isFindingPath = false;

        private void Awake()
        {
            gridSystem = FindObjectOfType<GridSystem>();
            pathfinder = FindObjectOfType<Pathfinder>();
            InitializeBattleGridManager();
        }

        private void InitializeBattleGridManager()
        {
            gridBlocks = GetComponentsInChildren<GridBlock>();

            foreach(GridBlock gridBlock in gridBlocks)
            {
                gridBlock.onContestedFighterUpdate += SetNewFighterBlock;
            }
        }

        private void Update()
        {
            //Refactor - Need a check to ask if player is moving. If so, dont look for paths
            if (isFindingPath)
            {
                //Refactor - Place in combat controller?
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    GridBlock goalBlock = hit.collider.GetComponentInParent<GridBlock>();
                    GridBlock currentBlock = currentUnitTurn.currentBlock;

                    if (goalBlock != null)
                    {                     
                        if (goalBlock == currentBlock || !goalBlock.IsMovable()) return;

                        gridSystem.UnhighlightPath(tempPath);
                        tempPath = pathfinder.FindPath(currentBlock, goalBlock);

                        int furtherestBlockIndex = GetFurtherestBlockIndex(tempPath, GetTotalPossibleGCostAllowance(currentUnitTurn));

                        gridSystem.HighlightPath(tempPath, furtherestBlockIndex);

                        if (Input.GetMouseButtonDown(0))
                        {
                            //isFindingPath = false;
                            StartCoroutine(currentUnitTurn.GetMover().MoveToDestination(tempPath, furtherestBlockIndex));
                            gridSystem.UnhighlightPath(tempPath);
                        }
                    }

                    //Fighter fighter = hit.collider.GetComponent<Fighter>();

                    //if(fighter != null)
                    //{
                    //    gridSystem.UnhighlightPath(tempPath);

                    //    tempPath = pathfinder.FindPath(currentBlock, GetGridBlockByFighter(fighter));
                    //}
                }
            }
        }

        public int GetFurtherestBlockIndex(List<GridBlock> _path, float _totalGCostAllowance)
        {
            foreach(GridBlock gridBlock in _path)
            {
                if (_totalGCostAllowance >= gridBlock.pathfindingCostValues.gCost) continue;

                return _path.IndexOf(gridBlock) - 1;
            }

            return _path.Count - 1;
        }

        public void UpdateCurrentUnitTurn(UnitController _unitController)
        {
            currentUnitTurn = _unitController;
            if (currentUnitTurn.GetUnitInfo().IsPlayer()) isFindingPath = true;
            else isFindingPath = false;
        }

        public void SetNewFighterBlock(Fighter _fighter, GridBlock _gridBlock)
        {
            occupiedBlocksDict[_fighter] = _gridBlock;
        }

        public void OnFighterTurnAdvance(Fighter _contestedFighter)
        {
            GridBlockStatus contestedStatus = new GridBlockStatus();
            foreach (GridBlockStatus gridBlockStatus in gridBlockStatusDict.Values)
            {
                if (gridBlockStatus.contestedFighter == _contestedFighter)
                {
                    contestedStatus = gridBlockStatus;
                }
            }

            if (contestedStatus.currentEffect != null)
            {
                PerformAbility(contestedStatus.currentEffect, contestedStatus.contestedFighter);
            }
        }

        public void OnGridBlockEnter(Fighter _newFighter, GridBlock _gridBlockToTest)
        {
            GridBlockStatus gridBlockStatus = gridBlockStatusDict[_gridBlockToTest];

            gridBlockStatus.contestedFighter = _newFighter;

            if (gridBlockStatus.currentEffect != null)
            {
                PerformAbility(gridBlockStatus.currentEffect, gridBlockStatus.contestedFighter);
            }

            //if (_newFighter.hasGridBlockTrigger)
            //{
            //    if (gridBlockStatus.currentEffect == null)
            //    {
            //        gridBlockStatus.currentEffect = _newFighter.GetBlockEffect();
            //    }
            //    else
            //    {
            //        bool IsStrongEnoughToReplace();

            //    }
            //}
        }

        public void PerformAbility(Ability _currentEffect, Fighter _contestFighter)
        {
            //Perform Ability
            ///Damage?
            ///Poision/Radiation
            ///Gravity
            ///
        }

        public GridBlock GetGridBlockByFighter(Fighter _fighter)
        {
            foreach(GridBlock gridBlock  in gridBlockStatusDict.Keys)
            {
                GridBlockStatus gridBlockStatus = gridBlockStatusDict[gridBlock];

                if (gridBlock.contestedFighter == _fighter) return gridBlock;
               
            }
            return null;
        }

        public float GetActionPointsCost(GridBlock _goalBlock)
        {
            float gCost = _goalBlock.pathfindingCostValues.gCost;
            float currentGCostAllowance = currentUnitTurn.GetMover().gCostAllowance;

            if(gCost < currentGCostAllowance)
            {
                return 0;
            }
            else
            {
                float gCostAfterAllowance = gCost - currentGCostAllowance;

                float actionPointCost = gCostAfterAllowance/currentUnitTurn.GetMover().gCostPerAP;
                return Mathf.CeilToInt(actionPointCost);
            }
        }

        private float GetTotalPossibleGCostAllowance(UnitController _unitController)
        {
            float totalPossibleGCostAllowance = 0f;        

            Movement.CombatMover combatMover = _unitController.GetMover();
            float actionPoints = _unitController.GetUnitResources().actionPoints;

            totalPossibleGCostAllowance += combatMover.gCostAllowance;
            totalPossibleGCostAllowance += combatMover.gCostPerAP * actionPoints;

            return totalPossibleGCostAllowance;
        }
    }
}

public struct GridBlockStatus
{
    public Fighter contestedFighter;
    public Ability currentEffect;
    //public GridItem currentItem;
}
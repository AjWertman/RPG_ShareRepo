using RPGProject.Combat;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Control
{
    public class BattleGridManager : MonoBehaviour
    {
        GridSystem gridSystem = null;
        Pathfinder pathfinder = null;

        GridBlock[] gridBlocks = null;

        Dictionary<Fighter, GridBlock> occupiedBlocksDict = new Dictionary<Fighter, GridBlock>(); 
        Dictionary<GridBlock, GridBlockStatus> gridBlockStatusDict = new Dictionary<GridBlock, GridBlockStatus>();

        public Dictionary<GridBlock, Unit> playerStartingPositionsDict = new Dictionary<GridBlock, Unit>();
        public Dictionary<GridBlock, Unit> enemyStartingPositionsDict = new Dictionary<GridBlock, Unit>();

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

        public void SetUpBattlePositionManager(GridSystem _gridSystem, UnitStartingPosition[] _playerStartingPositions, UnitStartingPosition[] _enemyStartingPositions)
        {
            gridSystem = _gridSystem;
            GridCoordinates playerZeroCoordinates = gridSystem.playerZeroCoordinates;
            GridCoordinates enemyZeroCoordinates = gridSystem.enemyZeroCoordinates;

            playerStartingPositionsDict = SetupStartingPositions(playerZeroCoordinates, _playerStartingPositions);
            enemyStartingPositionsDict = SetupStartingPositions(enemyZeroCoordinates, _enemyStartingPositions);
        }
        private Dictionary<GridBlock, Unit> SetupStartingPositions(GridCoordinates _zeroCoordinates, UnitStartingPosition[] _unitStartingPositions)
        {
            Dictionary<GridBlock, Unit> startingPositionsDict = new Dictionary<GridBlock, Unit>();

            for (int i = 0; i < _unitStartingPositions.Length; i++)
            {
                UnitStartingPosition unitStartingPosition = _unitStartingPositions[i];
                GridCoordinates startingCoordinates = unitStartingPosition.startCoordinates;

                GridBlock startingBlock = GetGridBlock(_zeroCoordinates, startingCoordinates);
                startingPositionsDict.Add(startingBlock, unitStartingPosition.unit);
            }

            return startingPositionsDict;
        }

        private GridBlock GetGridBlock(GridCoordinates _zeroCoordinates, GridCoordinates _gridCoordinates)
        {
            GridBlock gridBlock = null;

            int xCoordinate = (_zeroCoordinates.x + _gridCoordinates.x);
            int zCoordinate = (_zeroCoordinates.z + _gridCoordinates.z);

            gridBlock = gridSystem.GetGridBlock(xCoordinate, zCoordinate);

            return gridBlock;
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
            foreach(GridBlock gridBlock  in gridBlocks)
            {
                if (gridBlock.contestedFighter == _fighter) return gridBlock;
               
            }

            return null;
        }

        //public float GetActionPointsCost(GridBlock _goalBlock)
        //{
        //    float gCost = _goalBlock.pathfindingCostValues.gCost;
        //    float currentGCostAllowance = currentUnitTurn.GetMover().gCostAllowance;

        //    if(gCost < currentGCostAllowance)
        //    {
        //        return 0;
        //    }
        //    else
        //    {
        //        float gCostAfterAllowance = gCost - currentGCostAllowance;

        //        float actionPointCost = gCostAfterAllowance/currentUnitTurn.GetMover().gCostPerAP;
        //        return Mathf.CeilToInt(actionPointCost);
        //    }
        //}
    }
}

public struct GridBlockStatus
{
    public Fighter contestedFighter;
    public Ability currentEffect;
    //public GridItem currentItem;
}
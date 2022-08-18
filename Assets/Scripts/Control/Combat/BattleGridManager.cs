using RPGProject.Combat;
using RPGProject.Combat.Grid;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Control.Combat
{
    public class BattleGridManager : MonoBehaviour
    {
        public Dictionary<GridBlock, Unit> playerStartingPositionsDict = new Dictionary<GridBlock, Unit>();
        public Dictionary<GridBlock, Unit> enemyStartingPositionsDict = new Dictionary<GridBlock, Unit>();

        public Vector3 playerTeamCenterPoint = Vector3.zero;
        public Vector3 enemyTeamCenterPoint = Vector3.zero;

        GridSystem gridSystem = null;
        Pathfinder pathfinder = null;

        GridBlock[] gridBlocks = null;

        Dictionary<Fighter, GridBlock> occupiedBlocksDict = new Dictionary<Fighter, GridBlock>();
        Dictionary<AbilityBehavior, GridBlock> affectedBlocksDict = new Dictionary<AbilityBehavior, GridBlock>();
        Dictionary<GridBlock, GridBlockStatus> gridBlockStatusDict = new Dictionary<GridBlock, GridBlockStatus>();

        public void InitializeBattleGridManager()
        {
            gridSystem = GetComponentInChildren<GridSystem>();
            pathfinder = GetComponentInChildren<Pathfinder>();

            foreach (GridBlock gridBlock in gridSystem.gridBlocks)
            {
                gridBlock.onContestedFighterUpdate += SetNewFighterBlock;
                gridBlock.onAffectedBlockUpdate += SetNewAbilityBlock;
            }
        }

        public void SetUpBattleGridManager(UnitStartingPosition[] _playerStartingPositions, UnitStartingPosition[] _enemyStartingPositions)
        {
            GridCoordinates playerZeroCoordinates = gridSystem.playerZeroCoordinates;
            GridCoordinates enemyZeroCoordinates = gridSystem.enemyZeroCoordinates;

            playerStartingPositionsDict = SetupStartingPositions(playerZeroCoordinates, _playerStartingPositions);
            enemyStartingPositionsDict = SetupStartingPositions(enemyZeroCoordinates, _enemyStartingPositions);

            UpdateCenter(true);
            UpdateCenter(false); 
        }

        public void SetNewFighterBlock(Fighter _fighter, GridBlock _gridBlock)
        {
            occupiedBlocksDict[_fighter] = _gridBlock;
            UpdateCenter(_fighter.unitInfo.isPlayer);
        }

        public void SetNewAbilityBlock(AbilityBehavior _abilityBehavior, GridBlock _gridBlock)
        {
            affectedBlocksDict[_abilityBehavior] = _gridBlock;
        }

        private void UpdateCenter(bool _isPlayerTeam)
        {
            Vector3 averagePosition = new Vector3();
            int numberOfTeammates = 0;

            foreach(Fighter fighter in occupiedBlocksDict.Keys)
            {
                if(fighter.unitInfo.isPlayer == _isPlayerTeam)
                {
                    GridBlock occupiedBlock = occupiedBlocksDict[fighter];
                    averagePosition += new Vector3(occupiedBlock.gridCoordinates.x, 0, occupiedBlock.gridCoordinates.z);
                    numberOfTeammates++;
                }
            }

            averagePosition = averagePosition / numberOfTeammates;
            if (_isPlayerTeam) playerTeamCenterPoint = averagePosition;
            else enemyTeamCenterPoint = averagePosition;
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
            return occupiedBlocksDict[_fighter];
        }

        public GridBlock GetGridBlockByAbility(AbilityBehavior _abilityBehavior)
        {
            return affectedBlocksDict[_abilityBehavior];
        }

        public Vector3 GetTeamCenterPoint(bool _isPlayer, bool _wantsOwnTeam)
        {
            if (_isPlayer)
            {
                if(_wantsOwnTeam) return playerTeamCenterPoint;
                else return enemyTeamCenterPoint;
            }
            else
            {
                if (_wantsOwnTeam) return enemyTeamCenterPoint;
                else return playerTeamCenterPoint;
            }          
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
    }
}
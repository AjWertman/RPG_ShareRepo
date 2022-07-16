using RPGProject.Combat;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Control
{
    public class BattlePositionManager : MonoBehaviour
    {
        public Dictionary<GridBlock, Unit> playerStartingPositionsDict = new Dictionary<GridBlock, Unit>();
        public Dictionary<GridBlock, Unit> enemyStartingPositionsDict = new Dictionary<GridBlock, Unit>();

        GridSystem gridSystem = null;

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

        // <summary>
        ///////////////////////////////////////////////////////////////
         //</summary>
        [SerializeField] GameObject playerBattlePosition = null;
        [SerializeField] GameObject enemyBattlePosition = null;

        List<Transform> playerPositions = new List<Transform>();
        Transform currentPlayerPositionsParent = null;

        List<Transform> enemyPositions = new List<Transform>();
        Transform currentEnemyPositionsParent = null;

        //private void SetUpBattlePositions(GameObject _allPositionsParent, int _teamSize, bool _isPlayerTeam)
        //{
        //    int index = 0;

        //    Transform currentPositionsParent = null;
        //    foreach (Transform positionsParent in _allPositionsParent.transform)
        //    {

        //        if (index == _teamSize)
        //        {
        //            currentPositionsParent = positionsParent;
        //            currentPositionsParent.gameObject.SetActive(true);
        //            break;
        //        }

        //        index++;
        //    }

        //    AssignPositionsToTeam(currentPositionsParent, _isPlayerTeam);
        //}

        private void AssignPositionsToTeam(Transform _newPositionsParent, bool _isPlayerTeam)
        {
            List<Transform> teamPositions = new List<Transform>();

            foreach (Transform position in _newPositionsParent)
            {
                teamPositions.Add(position);
            }

            if (_isPlayerTeam)
            {
                currentPlayerPositionsParent = _newPositionsParent;
                playerPositions = teamPositions;
            }
            else
            {
                currentEnemyPositionsParent = _newPositionsParent;
                enemyPositions = teamPositions;
            }
        }

        public void ResetPositionManager()
        {
            playerPositions.Clear();
            enemyPositions.Clear();
            currentPlayerPositionsParent.gameObject.SetActive(false);
            currentPlayerPositionsParent = null;
            currentEnemyPositionsParent.gameObject.SetActive(false);
            currentEnemyPositionsParent = null;
        }

        public List<Transform> GetPlayerPosList()
        {
            return playerPositions;
        }

        public List<Transform> GetEnemyPosList()
        {
            return enemyPositions;
        }
    }
}
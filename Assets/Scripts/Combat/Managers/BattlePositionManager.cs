using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Control
{
    public class BattlePositionManager : MonoBehaviour
    {
        [SerializeField] GameObject playerBattlePosition = null;
        [SerializeField] GameObject enemyBattlePosition = null;

        List<Transform> playerPositions = new List<Transform>();
        Transform currentPlayerPositionsParent = null;

        List<Transform> enemyPositions = new List<Transform>();
        Transform currentEnemyPositionsParent = null;

        public void SetUpBattlePositionManager(int _playerTeamSize, int _enemyTeamSize)
        {
            SetUpBattlePositions(playerBattlePosition, _playerTeamSize, true);
            SetUpBattlePositions(enemyBattlePosition, _enemyTeamSize, false);
        }

        private void SetUpBattlePositions(GameObject _allPositionsParent, int _teamSize, bool _isPlayerTeam)
        {
            int index = 0;

            Transform currentPositionsParent = null;

            foreach (Transform positionsParent in _allPositionsParent.transform)
            {
                if (index == _teamSize)
                {
                    currentPositionsParent = positionsParent;
                    currentPositionsParent.gameObject.SetActive(true);
                    break;
                }

                index++;
            }

            AssignPositionsToTeam(currentPositionsParent, _isPlayerTeam);
        }

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
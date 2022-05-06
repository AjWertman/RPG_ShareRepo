using System.Collections.Generic;
using UnityEngine;

public class BattlePositionManager : MonoBehaviour
{
    [SerializeField] GameObject playerBattlePosition = null;
    [SerializeField] GameObject enemyBattlePosition = null;

    List<Transform> playerPositions = new List<Transform>();
    List<Transform> enemyPositions = new List<Transform>();

    public void SetUpBattlePositions(int playerTeamSize, int enemyTeamSize)
    {
        SetUpPlayerBattlePositions(playerTeamSize);

        SetUpEnemyBattlePositions(enemyTeamSize);
    }

    private void SetUpPlayerBattlePositions(int playerTeamSize)
    {
        int index = 0;

        Transform currentPositionsObject = null;

        foreach (Transform positionsObject in playerBattlePosition.transform)
        {
            if (index == playerTeamSize)
            {
                currentPositionsObject = positionsObject;
                currentPositionsObject.gameObject.SetActive(true);
            }

            index++;
        }

        if (currentPositionsObject != null)
        {
            foreach (Transform position in currentPositionsObject.transform)
            {
                playerPositions.Add(position);
            }
        }
    }

    private void SetUpEnemyBattlePositions(int enemyTeamSize)
    {
        int index = 0;

        Transform currentPositionsObject = null;

        foreach (Transform positionsObject in enemyBattlePosition.transform)
        {
            if (index == enemyTeamSize)
            {
                currentPositionsObject = positionsObject;
                currentPositionsObject.gameObject.SetActive(true);
            }

            index++;
        }

        if (currentPositionsObject != null)
        {
            foreach (Transform position in currentPositionsObject.transform)
            {              
                enemyPositions.Add(position);
            }
        }
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

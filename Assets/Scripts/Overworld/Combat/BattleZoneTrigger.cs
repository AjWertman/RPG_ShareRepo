using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleZoneTrigger : MonoBehaviour
{
    [SerializeField] EnemyCluster[] enemyClusters = null;
    [SerializeField] BattleHandler battleHandlerOverride = null;
    [Range(0,99)][SerializeField] int chanceToStartBattle = 10;
    [SerializeField] bool isEnemyTrigger = false;

    List<Character> enemyTeam = new List<Character>();

    bool isInTrigger = false;

    public event Action<bool> updateShouldBeDisabled;

    public void BattleCheck()
    {
        int randomInt= GetRandomPercentage();

        if(randomInt <= chanceToStartBattle)
        {
            StartCoroutine(StartBattle());
        }
    }

    public void CallStartBattle()
    {
        StartCoroutine(StartBattle());
    }

    public IEnumerator StartBattle()
    {
        enemyTeam.Clear();

        if (isEnemyTrigger)
        {
            updateShouldBeDisabled(true);
        }

        foreach (OverworldEntity overworld in FindObjectsOfType<OverworldEntity>())
        {
            overworld.GetComponent<IOverworld>().BattleStartBehavior();
        }

        yield return FindObjectOfType<Fader>().FadeOut(Color.white, .5f);

        BattleHandler currentBattleHandler = GetClosestBattleHandler();

        if (battleHandlerOverride != null)
        {
            currentBattleHandler = battleHandlerOverride;
        }

        SetRandomEnemyTeam();

        yield return currentBattleHandler.SetupBattle(enemyTeam);

        yield return FindObjectOfType<Fader>().FadeIn(2f);

        if(!currentBattleHandler.IsTutorial())
        {
            currentBattleHandler.ExecuteNextTurn();
        }
    }

    private BattleHandler GetClosestBattleHandler()
    {
        BattleHandler[] battleHandlers = FindObjectsOfType<BattleHandler>();

        BattleHandler closestBattleHandler = null;
        float closestDistance = 0f;

        foreach (BattleHandler battleHandler in battleHandlers)
        {         
            if(closestBattleHandler == null || closestDistance > Vector3.Distance(transform.position, battleHandler.transform.position))
            {
                closestBattleHandler = battleHandler;
                closestDistance = Vector3.Distance(transform.position, closestBattleHandler.transform.position);
            }
        }

        return closestBattleHandler;
    }

    private void SetRandomEnemyTeam()
    {
        int randomNumber = UnityEngine.Random.Range(0, enemyClusters.Length - 1);
        EnemyCluster enemyCluster = enemyClusters[randomNumber];

        foreach (Character enemy in enemyCluster.GetEnemies())
        {
            enemyTeam.Add(enemy);
        }
    }

    private void ClearEnemyTeam()
    {
        List<Character> enemyTeam = new List<Character>();
    }

    public void SetIsInTrigger(bool shouldSet)
    {
        isInTrigger = shouldSet;
    }

    public bool IsEnemyTrigger()
    {
        return isEnemyTrigger;
    }

    private int GetRandomPercentage()
    {
        return UnityEngine.Random.Range(0, 99);
    }
}

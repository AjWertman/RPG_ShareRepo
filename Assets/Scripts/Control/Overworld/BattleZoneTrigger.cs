using RPGProject.Combat;
using RPGProject.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Control
{
    public class BattleZoneTrigger : MonoBehaviour
    {
        [SerializeField] EnemyCluster[] enemyClusters = null;
        [SerializeField] BattleHandler battleHandlerOverride = null;
        [Range(0, 99)] [SerializeField] int chanceToStartBattle = 10;
        [SerializeField] bool isEnemyTrigger = false;

        BattleHandler currentBattleHandler = null;

        PlayerTeam playerTeam = null;
        List<Unit> enemyTeam = new List<Unit>();

        bool isInTrigger = false;
        bool startedBattle = false;

        public event Action<bool> updateShouldBeDisabled;

        private void Awake()
        {
            playerTeam = FindObjectOfType<PlayerTeam>();
        }

        public void BattleCheck()
        {
            int randomInt = GetRandomPercentage();

            if (randomInt <= chanceToStartBattle)
            {
                StartCoroutine(StartBattle());
            }
        }

        public void CallStartBattle()
        {
            if (startedBattle) return;
            startedBattle = true;
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

            currentBattleHandler = GetClosestBattleHandler();

            if (battleHandlerOverride != null)
            {
                currentBattleHandler = battleHandlerOverride;
            }

            currentBattleHandler.onBattleEnd += EndBattle;

            SetRandomEnemyTeam();

            yield return currentBattleHandler.SetupBattle(playerTeam, enemyTeam);

            yield return FindObjectOfType<Fader>().FadeIn(2f);

            //Refactor
            //if(!currentBattleHandler.IsTutorial())
            //{
            //    currentBattleHandler.ExecuteNextTurn();
            //}
        }

        //Refactor EndBattle(Reward reward)
        public void EndBattle()
        {
            foreach (OverworldEntity overworld in FindObjectsOfType<OverworldEntity>())
            {
                overworld.GetComponent<IOverworld>().BattleEndBehavior();
            }

            currentBattleHandler.onBattleEnd -= EndBattle;
            currentBattleHandler = null;

            startedBattle = false;
        }

        private BattleHandler GetClosestBattleHandler()
        {
            BattleHandler[] battleHandlers = FindObjectsOfType<BattleHandler>();

            BattleHandler closestBattleHandler = null;
            float closestDistance = 0f;

            foreach (BattleHandler battleHandler in battleHandlers)
            {
                if (closestBattleHandler == null || closestDistance > Vector3.Distance(transform.position, battleHandler.transform.position))
                {
                    closestBattleHandler = battleHandler;
                    closestDistance = Vector3.Distance(transform.position, closestBattleHandler.transform.position);
                }
            }

            return closestBattleHandler;
        }

        private void SetRandomEnemyTeam()
        {
            int randomNumber = RandomGenerator.GetRandomNumber(0, enemyClusters.Length - 1);
            EnemyCluster enemyCluster = enemyClusters[randomNumber];

            foreach (Unit enemy in enemyCluster.GetEnemies())
            {
                enemyTeam.Add(enemy);
            }
        }
        public bool IsEnemyTrigger()
        {
            return isEnemyTrigger;
        }

        private int GetRandomPercentage()
        {
            return RandomGenerator.GetRandomNumber(0, 99);
        }
    }
}
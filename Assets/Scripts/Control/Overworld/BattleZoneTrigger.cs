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
        [SerializeField] NewBattleHandlerScript battleHandlerOverride = null;
        [Range(0, 99)] [SerializeField] int chanceToStartBattle = 10;
        [SerializeField] bool isEnemyTrigger = false;

        NewBattleHandlerScript currentBattleHandler = null;

        PlayerTeamManager playerTeam = null;

        bool isInTrigger = false;
        bool startedBattle = false;

        public event Action<bool> updateShouldBeDisabled;

        private void Start()
        {
            playerTeam = FindObjectOfType<PlayerTeamManager>();
        }

        private void Update()
        {
            if (startedBattle) return;
            if (Input.GetKeyDown(KeyCode.J))
            {
                StartCoroutine(StartBattle());
            }
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

            UnitStartingPosition[] enemyStartingPositions = GetRandomEnemyTeam();

            yield return currentBattleHandler.StartBattle(playerTeam, enemyStartingPositions);

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

        private NewBattleHandlerScript GetClosestBattleHandler()
        {
            NewBattleHandlerScript[] battleHandlers = FindObjectsOfType<NewBattleHandlerScript>();

            NewBattleHandlerScript closestBattleHandler = null;
            float closestDistance = 0f;

            foreach (NewBattleHandlerScript battleHandler in battleHandlers)
            {
                if (closestBattleHandler == null || closestDistance > Vector3.Distance(transform.position, battleHandler.transform.position))
                {
                    closestBattleHandler = battleHandler;
                    closestDistance = Vector3.Distance(transform.position, closestBattleHandler.transform.position);
                }
            }

            return closestBattleHandler;
        }

        private UnitStartingPosition[] GetRandomEnemyTeam()
        {
            int randomNumber = RandomGenerator.GetRandomNumber(0, enemyClusters.Length - 1);
            EnemyCluster enemyCluster = enemyClusters[randomNumber];

            return enemyCluster.enemies;
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
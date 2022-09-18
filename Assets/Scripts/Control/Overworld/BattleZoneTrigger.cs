using RPGProject.Combat;
using RPGProject.Combat.Grid;
using RPGProject.Core;
using System;
using System.Collections;
using UnityEngine;

namespace RPGProject.Control.Combat
{
    /// <summary>
    /// Trigger in the overworld that can start battles. 
    /// If is an enemy trigger, the battle will start on trigger enter. 
    /// Else it will run a battle check every footstep the player mesh takes.
    /// </summary>
    public class BattleZoneTrigger : MonoBehaviour
    {
        [SerializeField] EnemyCluster[] enemyClusters = null;
        [SerializeField] BattleHandler battleHandlerOverride = null; //Will find the closest battle handler unless this is not null.
        [Range(0, 99)] [SerializeField] int chanceToStartBattle = 10; //The percentage chance a battle will start on each footstep.

        public bool isEnemyTrigger = false;

        BattleHandler currentBattleHandler = null;
        PlayerTeamManager playerTeam = null;

        bool isInTrigger = false;
        bool startedBattle = false;

        public event Action<bool> updateShouldBeDisabled;

        private void Start()
        {
            playerTeam = FindObjectOfType<PlayerTeamManager>();
            StartCoroutine(BeginTestBattle());
        }

        private IEnumerator BeginTestBattle()
        {
            yield return new WaitForSeconds(1f);

            CallStartBattle();
        }

        public void BattleCheck()
        {
            int randomInt = RandomGenerator.GetRandomNumber(0,100);

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

        private UnitStartingPosition[] GetRandomEnemyTeam()
        {
            int randomNumber = RandomGenerator.GetRandomNumber(0, enemyClusters.Length - 1);
            EnemyCluster enemyCluster = enemyClusters[randomNumber];

            return enemyCluster.enemies;
        }
    }
}
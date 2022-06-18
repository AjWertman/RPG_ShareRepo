using RPGProject.Saving;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Control
{
    public class EnemyActivator : MonoBehaviour, ISaveable
    {
        [SerializeField] EnemyController[] enemyControllers = null;

        EnemyActivationTrigger enemyActivationTrigger = null;
        EnemyDeactivationTrigger enemyDeactivationTrigger = null;

        bool isActivated = false;
        bool hasEntered = false;

        private void Awake()
        {
            enemyActivationTrigger = GetComponentInChildren<EnemyActivationTrigger>();
            enemyDeactivationTrigger = GetComponentInChildren<EnemyDeactivationTrigger>();

            enemyActivationTrigger.onActivation += ActivateEnemies;
            enemyDeactivationTrigger.onDeactivation += DeactivateEnemies;

            ChangeActiveTrigger(isActivated);
        }

        private void ActivateEnemies()
        {          
            foreach(EnemyController enemyController in enemyControllers)
            {            
                enemyController.ActivateEnemy();

            }

            isActivated = true;

            ChangeActiveTrigger(isActivated);
        }

        private void DeactivateEnemies()
        {
            foreach (EnemyController enemyController in enemyControllers)
            {
                enemyController.DeactivateEnemy();
            }
           
            isActivated = false;

            ChangeActiveTrigger(isActivated);
        }

        private void ChangeActiveTrigger(bool _isActivated)
        {
            if (_isActivated)
            {
                enemyActivationTrigger.DeactivateTrigger();
                enemyDeactivationTrigger.gameObject.SetActive(true);
            }
            else
            {
                enemyDeactivationTrigger.DeactivateTrigger();
                enemyActivationTrigger.gameObject.SetActive(true);
            }
        }

        public object CaptureState()
        {
            return isActivated;
        }

        public void RestoreState(object _state)
        {
            isActivated = (bool)_state;

            ChangeActiveTrigger(isActivated);
        }
    }
}
using System;
using UnityEngine;

namespace RPGProject.GameResources
{
    public class Health : MonoBehaviour
    {
        [SerializeField] float healthPoints = 0f;
        [SerializeField] float maxHealthPoints = 100f;

        Animator animator = null;

        float healthPercentage = 1f;
        bool isDead = false;

        float baseMaxHealthPoints = 0f;
        float stamina = 10f;
        float staminaHealthAmount = 10f;

        float armor = 10f;
        float resistance = 10f;

        public event Action onHealthChange;
        public event Action<Health> onDeath;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            baseMaxHealthPoints = stamina * staminaHealthAmount;
        }

        public void ResetHealth()
        {
            healthPoints = 0f;
            maxHealthPoints = baseMaxHealthPoints;
            healthPercentage = 0f;
            isDead = false;
            UpdateAttributes(10f, 10f, 10f);
        }

        public void UpdateAttributes(float _stamina, float _armor, float _resistance)
        {
            stamina = _stamina;
            armor = _armor;
            resistance = _armor;
        }

        public void SetUnitHealth(float _healthPoints, float _maxHealthPoints)
        {
            healthPoints = _healthPoints;
            maxHealthPoints = _maxHealthPoints;

            SetCurrentHealthPercentage();
        }

        public void CalculateMaxHealthPoints(bool _isInitialUpdate)
        {
            float previousMaxHealthPoints = maxHealthPoints;
            float newMaxHealthPoints = stamina * staminaHealthAmount;

            maxHealthPoints = newMaxHealthPoints;

            if (_isInitialUpdate)
            {
                healthPoints = maxHealthPoints;
            }
            else
            {
                float healthPercentage = healthPoints / previousMaxHealthPoints;
                healthPoints = maxHealthPoints * healthPercentage;
            }

            SetCurrentHealthPercentage();
        }

        public void DamageHealth(float _damageAmount, bool _isCritical, bool _isPhysicalAttack)
        {
            float calculatedDamage = CalculateDamage(_damageAmount, _isPhysicalAttack);

            healthPoints -= calculatedDamage;
            healthPoints = Mathf.Clamp(healthPoints, 0, maxHealthPoints);

            SetCurrentHealthPercentage();

            if (DeathCheck())
            {
                Die();
            }
            else
            {
                animator.Play("TakeDamage");
            }
        }

        public void RestoreHealth(float _restoreAmount, bool _isCritical)
        {
            healthPoints += _restoreAmount;
            healthPoints = Mathf.Clamp(healthPoints, 0, maxHealthPoints);

            SetCurrentHealthPercentage();
        }

        //Refactor - move elsewhere - UnitControllerUI?
        //public void UIHealthChangeStuff()
        //{
               // Text above head that displays changeamount/critical

        //    uiHealthChange.ActivateCriticalCanvas(_isCritical, true);
        //    uiHealthChange.ActivateAmountCanvas(true, true, calculatedDamage);
        //    uiHealthChange.ActivateCriticalCanvas(false, true);
        //    uiHealthChange.ActivateAmountCanvas(false, true, 0);

        //    uiHealthChange.ActivateCriticalCanvas(_isCritical, false);
        //    uiHealthChange.ActivateAmountCanvas(true, false, _restoreAmount);

        //    uiHealthChange.ActivateCriticalCanvas(false, false);
        //    uiHealthChange.ActivateAmountCanvas(false, false, 0);
        //}

        private float CalculateDamage(float _damageAmount, bool _isPhysicalAttack)
        {
            float newDamageAmount = 0f;
            float statsModifier = 0f;

            if (_isPhysicalAttack)
            {
                statsModifier = armor - 10f;
            }
            else
            { 
                statsModifier = resistance - 10f;
            }

            if (statsModifier == 0)
            {
                newDamageAmount = _damageAmount;
            }
            else
            {
                float defensivePercentage = 1f - (statsModifier * .01f);

                newDamageAmount = _damageAmount * defensivePercentage;
            }

            return newDamageAmount;
        }

        public void Die()
        {
            if (!isDead)
            {
                isDead = true;
                animator.Play("Die");
                
                //Refactor play deeath sound
                
                HandleQuestCompletion();
            }
        }

        public void OnAnimDeath()
        {
            onDeath(this);
        }

        public bool DeathCheck()
        {
            if (healthPoints <= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsDead()
        {
            return isDead;
        }

        //Refactor
        private void HandleQuestCompletion()
        {
            RPGProject.Questing.QuestCompletion myQuestCompletion = GetComponent<RPGProject.Questing.QuestCompletion>();
            RPGProject.Questing.PlayerQuestList playerQuestList = FindObjectOfType<RPGProject.Questing.PlayerQuestList>();

            if (myQuestCompletion != null)
            {
                myQuestCompletion.CompleteObjective(playerQuestList);
            }
        }

        public float GetHealthPoints()
        {
            return healthPoints;
        }

        public float GetMaxHealthPoints()
        {
            return maxHealthPoints;
        }

        public void SetCurrentHealthPercentage()
        {
            healthPercentage = healthPoints / maxHealthPoints;
        }

        public float GetHealthPercentage()
        {
            return healthPercentage;
        }
    }
}
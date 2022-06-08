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

        public event Action<bool, float> onHealthChange;
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

        public void ChangeHealth(float _changeAmount, bool _isCritical, bool _isChangeMagical)
        {
            float calculatedAmount = CalculateChange(_changeAmount, _isChangeMagical);

            healthPoints += calculatedAmount;
            healthPoints = Mathf.Clamp(healthPoints, 0, maxHealthPoints);
            SetCurrentHealthPercentage();
            onHealthChange(_isCritical, calculatedAmount);

            if (DeathCheck()) Die();
            else animator.Play("TakeDamage"); 
        }

        private float CalculateChange(float _changeAmount, bool _isChangeMagical)
        {
            if (_changeAmount > 0) return _changeAmount;

            float newDamageAmount = 0f;
            float statsModifier = 0f;

            if (!_isChangeMagical)
            {
                statsModifier = armor - 10f;
            }
            else
            { 
                statsModifier = resistance - 10f;
            }

            if (statsModifier == 0)
            {
                newDamageAmount = _changeAmount;
            }
            else
            {
                float defensivePercentage = 1f - (statsModifier * .01f);

                newDamageAmount = _changeAmount * defensivePercentage;
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
                myQuestCompletion.CompleteObjective();
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
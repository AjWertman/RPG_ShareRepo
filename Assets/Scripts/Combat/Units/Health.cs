using RPGProject.Sound;
using System;
using UnityEngine;

namespace RPGProject.GameResources
{
    /// <summary>
    /// Manages a combatants health.
    /// </summary>
    public class Health : MonoBehaviour
    {
        [SerializeField] AudioClip hurtClip = null;
        [SerializeField] AudioClip deathClip = null;

        public float healthPoints = 0f;
        public float maxHealthPoints = 100f;

        public float healthPercentage = 1f;
        public bool isDead = false;

        Animator animator = null;
        SoundFXManager soundFXManager = null;

        float baseMaxHealthPoints = 0f;
        float stamina = 10f;
        float staminaHealthAmount = 10f;

        float armor = 10f;
        float resistance = 10f;

        /// <summary>
        /// Called whenever the health is changed to update UI to indicate the change.
        /// Bool = Is critical.
        /// Float = Amount of change.
        /// </summary>
        public event Action<bool, float> onHealthChange;

        /// <summary>
        /// Called when the event in the "death" animation is called.
        /// </summary>
        public event Action<Health> onAnimDeath;

        //Refactor - Not being used currently
        /// <summary>
        /// Called when healthpoints = 0.
        /// </summary>
        public event Action<Health> onHealthDeath;

        public void InitalizeHealth()
        {
            //animator = GetComponentInChildren<Animator>();
            soundFXManager = FindObjectOfType<SoundFXManager>();
            baseMaxHealthPoints = stamina * staminaHealthAmount;
        }

        public void SetAnimator(Animator _animator)
        {
            animator = _animator;
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
            if (isDead) return;
            if (_changeAmount == 0) return;

            float calculatedAmount = CalculateChange(_changeAmount, _isChangeMagical);

            healthPoints += calculatedAmount;
            healthPoints = Mathf.Clamp(healthPoints, 0, maxHealthPoints);
            SetCurrentHealthPercentage();
            onHealthChange(_isCritical, calculatedAmount);

            if (calculatedAmount < 0)
            {
                soundFXManager.CreateSoundFX(hurtClip, transform, .5f); 
            }

            if (DeathCheck()) Die();
            else
            {
                if(animator != null) animator.Play("TakeDamage");
            }
        }

        public void Die()
        {
            if (!isDead)
            {
                isDead = true;
                animator.Play("Die");
                soundFXManager.CreateSoundFX(deathClip, transform, .75f);
                //onHealthDeath(this);
            }
        }

        public void OnAnimDeath()
        {
            onAnimDeath(this);
        }

        public void SetCurrentHealthPercentage()
        {
            healthPercentage = healthPoints / maxHealthPoints;
        }

        public bool DeathCheck()
        {
            if (healthPoints <= 0) return true;
            else return false;
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
    }
}
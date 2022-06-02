using System;
using UnityEngine;

namespace RPGProject.GameResources
{
    public class Mana : MonoBehaviour
    {
        [SerializeField] float manaPoints = 0f;
        [SerializeField] float maxManaPoints = 100f;

        float manaPercentage = 1f;

        float baseMaxManaPoints = 0f;
        float intellect = 10f;
        float intellectManaAmount = 10f;

        public event Action onManaChange;

        private void Awake()
        {
            baseMaxManaPoints = intellect * intellectManaAmount;
        }

        public void UpdateAttributes(float _intellect)
        {
            intellect = _intellect;
        }

        public void SetMana(float _manaPoints, float _maxManaPoints)
        {
            manaPoints = _manaPoints;
            maxManaPoints = _maxManaPoints;

            SetManaPercentage();
        }

        public void CalculateMana(bool _isInitialUpdate)
        {
            float previousMaxManaPoints = maxManaPoints;
            float newMaxManaPoints = intellect * intellectManaAmount;

            maxManaPoints = newMaxManaPoints;

            if (_isInitialUpdate)
            {
                manaPoints = maxManaPoints;
            }
            else
            {
                float manaPercentage = manaPoints / previousMaxManaPoints;
                manaPoints = maxManaPoints * manaPercentage;
            }

            SetManaPercentage();
        }

        public void SpendManaPoints(float _costAmount)
        {
            manaPoints -= _costAmount;
            manaPoints = Mathf.Clamp(manaPoints, 0f, maxManaPoints);

            SetManaPercentage();
        }

        public void RestoreManaPoints(float _restoreAmount)
        {
            manaPoints += _restoreAmount;
            manaPoints = Mathf.Clamp(manaPoints, 0f, maxManaPoints);

            SetManaPercentage();
        }

        public void ResetMana()
        {
            manaPoints = 0f;
            maxManaPoints = baseMaxManaPoints;
            manaPercentage = 0f;
            UpdateAttributes(10f);
        }

        public float GetManaPoints()
        {
            return manaPoints;
        }

        public float GetMaxManaPoints()
        {
            return maxManaPoints;
        }

        public void SetManaPercentage()
        {
            manaPercentage = manaPoints / maxManaPoints;
            onManaChange();
        }

        public float GetManaPercentage()
        {
            return manaPercentage;
        }
    }
}


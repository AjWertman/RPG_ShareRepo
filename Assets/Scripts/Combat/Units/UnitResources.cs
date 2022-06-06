using System;
using UnityEngine;

namespace RPGProject.Combat
{
    [Serializable]
    public class UnitResources
    {
        [SerializeField] float healthPoints = 0;
        [SerializeField] float maxHealthPoints = 100;

        [SerializeField] float manaPoints = 0;
        [SerializeField] float maxManaPoints = 100;

        public void SetUnitResources(float _healthPoints, float _maxHealthPoints, float _manaPoints, float _maxManaPoints)
        {
            healthPoints = _healthPoints;
            maxHealthPoints = _maxHealthPoints;
            manaPoints = _manaPoints;
            maxManaPoints = _maxManaPoints;
        }

        public void SetUnitResources(UnitResources _unitResources)
        {
            healthPoints = _unitResources.GetHealthPoints();
            maxHealthPoints = _unitResources.GetMaxHealthPoints();
            manaPoints = _unitResources.GetManaPoints();
            maxManaPoints = _unitResources.GetMaxManaPoints();
        }

        public void SetHealthPoints(float _healthPoints)
        {
            healthPoints = _healthPoints;
        }

        public void SetMaxHealthPoints(float _maxHealthPoints)
        {
            maxHealthPoints = _maxHealthPoints;
        }

        public void SetManaPoints(float _manaPoints)
        {
            manaPoints = _manaPoints;
        }

        public void SetMaxManaPoints(float _maxManaPoints)
        {
            maxManaPoints = _maxManaPoints;
        }

        public void ResetUnitResources()
        {
            healthPoints = 0f;
            maxHealthPoints = 0f;
            manaPoints = 0f;
            maxManaPoints = 0f;
        }

        public float GetHealthPoints()
        {
            return healthPoints;
        }

        public float GetMaxHealthPoints()
        {
            return maxHealthPoints;
        }

        public float GetManaPoints()
        {
            return manaPoints;
        }

        public float GetMaxManaPoints()
        {
            return maxManaPoints;
        }
    }
}

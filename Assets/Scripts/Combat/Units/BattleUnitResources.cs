using System;
using UnityEngine;

namespace RPGProject.Combat
{
    [Serializable]
    public class BattleUnitResources
    {
        [SerializeField] float healthPoints = 0f;
        [SerializeField] float maxHealthPoints = 0f;

        [SerializeField] float manaPoints = 0f;
        [SerializeField] float maxManaPoints = 0f;

        public void SetBattleUnitResources(float _healthPoints, float _maxHealthPoints, float _manaPoints, float _maxManaPoints)
        {
            healthPoints = _healthPoints;
            maxHealthPoints = _maxHealthPoints;
            manaPoints = _manaPoints;
            maxManaPoints = _maxManaPoints;
        }

        public void SetBattleUnitResources(BattleUnitResources _battleUnitResources)
        {
            healthPoints = _battleUnitResources.GetHealthPoints();
            maxHealthPoints = _battleUnitResources.GetMaxHealthPoints();
            manaPoints = _battleUnitResources.GetManaPoints();
            maxManaPoints = _battleUnitResources.GetMaxManaPoints();
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

        public void ResetBattleUnitResources()
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
            return manaPoints;
        }
    }
}

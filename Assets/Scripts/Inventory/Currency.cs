using System;
using UnityEngine;

namespace RPGProject.Inventories
{
    public class Currency : MonoBehaviour
    {
        [SerializeField] float gold = 0;

        public event Action onCurrencyChange;

        public void SpendGold(float _amountToSpend)
        {
            if (!HasEnoughGold(_amountToSpend)) return;
            gold -= _amountToSpend;
            onCurrencyChange();
        }

        public void GainGold(float _amountToGain)
        {
            gold += _amountToGain;
            onCurrencyChange();
        }

        public float GetGold()
        {
            return gold;
        }

        public bool HasEnoughGold(float _amountToTest)
        {
            if (gold >= _amountToTest)
            {
                return true;
            }

            return false;
        }
    }

}
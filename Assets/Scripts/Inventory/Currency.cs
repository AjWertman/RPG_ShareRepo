using System;
using UnityEngine;

namespace RPGProject.Inventories
{
    /// <summary>
    /// Resource used in the buying and selling of items.
    /// </summary>
    public class Currency : MonoBehaviour
    {
        public float gold = 0;

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
using System;
using UnityEngine;

public class Currency : MonoBehaviour
{
    [SerializeField] float gold = 0;

    public event Action onCurrencyChange;

    public float GetGold()
    {
        return gold;
    }

    public void SpendGold(float amount)
    {
        if(!HasEnoughGold(amount)) return;
        gold -= amount;
        onCurrencyChange();
    }

    public void GainGold(float amount)
    {
        gold += amount;
        onCurrencyChange();
    }

    public bool HasEnoughGold(float amount)
    {
        if(gold >= amount)
        {
            return true;
        }

        return false;
    }
}

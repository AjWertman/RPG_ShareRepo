using System;
using UnityEngine;

public class Currency : MonoBehaviour
{
    [SerializeField] float leons = 0;

    public event Action onCurrencyChange;

    public float GetLeons()
    {
        return leons;
    }

    public void SpendLeons(float amount)
    {
        if(!HasEnoughLeons(amount)) return;
        leons -= amount;
        onCurrencyChange();
    }

    public void GainLeons(float amount)
    {
        leons += amount;
        onCurrencyChange();
    }

    public bool HasEnoughLeons(float amount)
    {
        if(leons >= amount)
        {
            return true;
        }

        return false;
    }
}

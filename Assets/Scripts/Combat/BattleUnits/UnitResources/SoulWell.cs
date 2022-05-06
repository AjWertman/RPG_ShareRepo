using System;
using UnityEngine;

public class SoulWell : MonoBehaviour
{
    [SerializeField] float soulWell = 0f;
    [SerializeField] float maxSoulWell = 100f;

    float soulWellPercentage = 1f;

    float intellect = 0f;

    public event Action onSoulWellChange;

    public void UpdateAttributes(float _intellect)
    {
        intellect = _intellect;
    }

    public void SetSoulWell(float _soulWell, float _maxSoulWell)
    {
        soulWell = _soulWell;
        maxSoulWell = _maxSoulWell;

        SetSoulWellPercentage();
    }

    public void CalculateSoulWell(bool initialUpdate, bool hasSoulWell)
    {
        if (hasSoulWell)
        {
            float currentMaxSoulWell = maxSoulWell;
            float newMaxSoulWell = maxSoulWell;
            float amountToAdd = intellect - 10;

            newMaxSoulWell += 10f * amountToAdd;

            maxSoulWell = newMaxSoulWell;

            if (initialUpdate)
            {
                soulWell = maxSoulWell;
            }
            else
            {
                float soulWellPercentage = soulWell / currentMaxSoulWell;
                soulWell = maxSoulWell * soulWellPercentage;
            }
        }
        else
        {
            soulWell = 0f;
            maxSoulWell = 0f;
        }                

        SetSoulWellPercentage();
    }

    public void SpendSoulWell(float costAmount)
    {
        soulWell -= costAmount;
        soulWell = Mathf.Clamp(soulWell, 0f, maxSoulWell);

        SetSoulWellPercentage();
    }

    public void RestoreSoulWell(float restoreAmount)
    {
        soulWell += restoreAmount;
        soulWell = Mathf.Clamp(soulWell, 0f, maxSoulWell);

        SetSoulWellPercentage();
    }

    public float GetSoulWell()
    {
        return soulWell;
    }

    public float GetMaxSoulWell()
    {
        return maxSoulWell;
    }

    public void SetSoulWellPercentage()
    {
        soulWellPercentage = soulWell / maxSoulWell;
        onSoulWellChange();
    }

    public float GetSoulWellPercentage()
    {
        return soulWellPercentage;
    }
}

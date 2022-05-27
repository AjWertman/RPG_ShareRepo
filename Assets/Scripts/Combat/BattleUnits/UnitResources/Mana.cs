using System;
using UnityEngine;

public class Mana : MonoBehaviour
{
    [SerializeField] float mana = 0f;
    [SerializeField] float maxMana = 100f;

    float manaPercentage = 1f;

    float intellect = 0f;

    public event Action onManaChange;

    public void ResetMana()
    {
        mana = 0f;
        maxMana = 0f;
        manaPercentage = 0f;
        UpdateAttributes(10f);
    }

    public void UpdateAttributes(float _intellect)
    {
        intellect = _intellect;
    }

    public void SetMana(float _mana, float _maxMana)
    {
        mana = _mana;
        maxMana = _maxMana;

        SetManaPercentage();
    }

    public void CalculateMana(bool initialUpdate)
    {
        float currentMaxMana = maxMana;
        float newMaxMana = maxMana;
        float amountToAdd = intellect - 10;

        newMaxMana += 10f * amountToAdd;

        maxMana = newMaxMana;

        if (initialUpdate)
        {
            mana = maxMana;
        }
        else
        {
            float manaPercentage = mana / currentMaxMana;
            mana = maxMana * manaPercentage;
        }

        SetManaPercentage();
    }

    public void SpendMana(float costAmount)
    {
        mana -= costAmount;
        mana = Mathf.Clamp(mana, 0f, maxMana);

        SetManaPercentage();
    }

    public void RestoreMana(float restoreAmount)
    {
        mana += restoreAmount;
        mana = Mathf.Clamp(mana, 0f, maxMana);

        SetManaPercentage();
    }

    public float GetMana()
    {
        return mana;
    }

    public float GetMaxMana()
    {
        return maxMana;
    }

    public void SetManaPercentage()
    {
        manaPercentage = mana / maxMana;
        onManaChange();
    }

    public float GetManaPercentage()
    {
        return manaPercentage;
    }
}

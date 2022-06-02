using RPGProject.Progression;
using System;
using UnityEngine;

public class StatBuff: MonoBehaviour
{
    [SerializeField] public string buffName = "Buff";
    [SerializeField] public GameObject buffPrefab = null;
    [SerializeField] public bool isDebuff = false;

    [SerializeField] public StatType affectedStat = StatType.Armor;

    [SerializeField] public int effectAmount = 0;
    [SerializeField] public int duration = 0;
    [SerializeField] int currentLifetime = 0;

    public event Action<StatType, float> onBuffDeath;

    private void OnEnable()
    {
        currentLifetime = duration;
    }

    public void ResetBuffLife(bool increaseDuration, int newDuration)
    {
        if (increaseDuration)
        {
            duration = newDuration;
        }

        currentLifetime = duration;
    }

    public void SetNewEffectAmount(int newEffectAmount)
    {
        effectAmount = newEffectAmount;
    }

    public void DecrementLife()
    {
        currentLifetime--;
        
        if(currentLifetime == 0)
        {
            KillBuff();
        }
    }

    public void KillBuff()
    {
        onBuffDeath(affectedStat, -effectAmount);

        Destroy(gameObject);
    }

    public StatType GetAffectedStat()
    {
        return affectedStat;
    }

    public int GetDuration()
    {
        return duration;
    }
    
    public bool GetIsDebuff()
    {
        return isDebuff;
    }

    public float GetEffectAmount()
    {
        return effectAmount;
    }
}

using System;
using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] float healthPoints = 0f;
    [SerializeField] float maxHealthPoints = 100f;

    [SerializeField] UIHealthChange uiHealthChange = null;

    Animator animator = null;
    SoundFXManager unitSoundFX = null;

    float healthPercentage = 1f;
    bool isDead = false;

    float stamina = 0f;
    float armor = 0f;
    float resistance = 0f;

    public event Action onHealthChange;
    public event Action<BattleUnit> onDeath;

    public void SetAnimator(Animator _animator)
    {
        animator = _animator;
    }

    public void SetUnitSoundFX(SoundFXManager _unitSoundFX)
    {
        unitSoundFX = _unitSoundFX;
    }

    public void UpdateAttributes(float _stamina, float _armor, float _resistance)
    {
        stamina = _stamina;
        armor = _armor;
        resistance = _armor;
    }

    public void SetUnitHealth(float _healthPoints, float _maxHealthPoints)
    {
        healthPoints = _healthPoints;
        maxHealthPoints = _maxHealthPoints;

        SetHealthPercentage();
    }

    public void CalculateMaxHealthPoints(bool initialUpdate)
    {
        float currentMaxHealthPoints = maxHealthPoints;
        float newMaxHealthPoints = maxHealthPoints;
        float amountToAdd = stamina - 10;

        newMaxHealthPoints +=  10f * amountToAdd;

        maxHealthPoints = newMaxHealthPoints;

        if (initialUpdate)
        {
            healthPoints = maxHealthPoints;
        }
        else
        {
            float healthPercentage = healthPoints/currentMaxHealthPoints;
            healthPoints = maxHealthPoints * healthPercentage;
        }

        SetHealthPercentage();
    }

    public IEnumerator DamageHealth(float damageAmount, bool isCritical, AbilityType type)
    {
        float calculatedDamage = CalculateDamage(damageAmount, type);

        healthPoints -= calculatedDamage;
        healthPoints = Mathf.Clamp(healthPoints, 0, maxHealthPoints);

        SetHealthPercentage();

        uiHealthChange.ActivateCriticalCanvas(isCritical, true);
        uiHealthChange.ActivateAmountCanvas(true, true, calculatedDamage);

        if (DeathCheck())
        {
            Die();
        }
        else
        {
            animator.SetTrigger("takeDamage");
            //unitSoundFX.CreateSoundFX(unitSoundFX.GetHurtSound());
        }

        yield return new WaitForSeconds(1f);

        uiHealthChange.ActivateCriticalCanvas(false, true);
        uiHealthChange.ActivateAmountCanvas(false, true, 0);
    }

    public IEnumerator RestoreHealth(float restoreAmount, bool isCritical)
    { 
        healthPoints += restoreAmount;
        healthPoints = Mathf.Clamp(healthPoints, 0, maxHealthPoints);

        SetHealthPercentage();

        uiHealthChange.ActivateCriticalCanvas(isCritical,false);
        uiHealthChange.ActivateAmountCanvas(true, false, restoreAmount);

        yield return new WaitForSeconds(1f);

        uiHealthChange.ActivateCriticalCanvas(false, false);
        uiHealthChange.ActivateAmountCanvas(false, false, 0);
    }    

    private float CalculateDamage(float damageAmount, AbilityType type)
    {
        float newDamageAmount = 0f;
        float statsModifier = 0f;

        if(type == AbilityType.Physical)
        {
            statsModifier = armor - 10f;
        }
        else if(type == AbilityType.Magical)
        {
            statsModifier = resistance - 10f;
        }

        if(statsModifier == 0)
        {
            newDamageAmount = damageAmount;
        }
        else
        {
            float defensivePercentage = 1f - (statsModifier * .01f);

            newDamageAmount = damageAmount * defensivePercentage;
        }

        return newDamageAmount;
    }

    public void Die()
    {
        if (!isDead)
        {
            isDead = true;
            animator.Play("Die");
            //unitSoundFX.CreateSoundFX(unitSoundFX.GetDieSound());
            GetComponent<BattleUnit>().DisableResourceSliders();
            HandleQuestCompletion();
        }
    }

    public void OnAnimDeath()
    {
        onDeath(GetComponent<BattleUnit>());
    }

    public bool DeathCheck()
    {
        if (healthPoints <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsDead()
    {
        return isDead;
    }

    private void HandleQuestCompletion()
    {
        QuestCompletion myQuestCompletion = GetComponent<QuestCompletion>();

        if(myQuestCompletion != null)
        {
            myQuestCompletion.CompleteObjective();
        }
    }
    
    public float GetHealthAmount()
    {
        return healthPoints;
    }

    public float GetMaxHealthAmount()
    {
        return maxHealthPoints;
    }

    public void SetHealthPercentage()
    {
        healthPercentage = healthPoints / maxHealthPoints;
        onHealthChange();
    }

    public float GetHealthPercentage()
    {
        return healthPercentage;
    }
}

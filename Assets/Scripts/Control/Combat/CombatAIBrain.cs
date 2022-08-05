using RPGProject.Combat;
using RPGProject.Core;
using RPGProject.GameResources;
using System.Collections.Generic;
using UnityEngine;

public class CombatAIBrain : MonoBehaviour
{
    [SerializeField] List<Agro> agros = new List<Agro>();

    [SerializeField] float agroPercentagePerDamagePercentage = 1f;

    Fighter fighter = null;

    List<Ability> abilities = new List<Ability>();

    public void InitalizeCombatAIBrain(Fighter _fighter)
    {
        fighter = _fighter;
        abilities = fighter.GetKnownAbilities();
    }

    public void InitalizeAgros(List<Fighter> _enemyFighters)
    {
        float initAgroPercentage = 100 / _enemyFighters.Count;
        foreach(Fighter fighter in _enemyFighters)
        {
            Agro newAgro = new Agro(fighter, initAgroPercentage);
            agros.Add(newAgro);

            fighter.onAgroAction += UpdateAgro;
        }
    }

    public void UpdateAgro(Fighter _agressor, float _changeAmount)
    {
        Fighter agressorTarget = (Fighter)_agressor.selectedTarget;
        if (fighter != agressorTarget) return;

        //Refactor - Sometimes causes total agro% to go above 100%
        float percentageChange = GetPercentageOfHealthChange(fighter.GetHealth(), _changeAmount);
        float agroPercentage = percentageChange * agroPercentagePerDamagePercentage;

        float agroToTakeFromOthers = GetEvenAgroSplit(percentageChange);

        for (int i = 0; i < agros.Count; i++)
        {
            Agro agro = agros[i];

            float newAgroPercentage;

            if (agro.fighter == _agressor) newAgroPercentage = agro.percentageOfAgro + percentageChange;
            else newAgroPercentage = agro.percentageOfAgro -= agroToTakeFromOthers;

            agros[i] = new Agro(agro.fighter, newAgroPercentage);
        }
    }

    public void RemoveFromAgrosList(Fighter _deadFighter)
    {
        float agroToRedistribute = GetAgroPercentage(_deadFighter);
        agros.Remove(GetAgro(_deadFighter));

        float evenSplit = GetEvenAgroSplit(agroToRedistribute);

        for (int i = 0; i < agros.Count - 1; i++)
        {
            Agro agroToDistributeTo = agros[i];
            agroToDistributeTo.percentageOfAgro += evenSplit;
        }
    }

    private Agro GetAgro(Fighter _fighter)
    {
        Agro agroToGet = new Agro();

        foreach(Agro agro in agros)
        {
            if (agro.fighter == _fighter) agroToGet = agro;
        }

        return agroToGet;
    }

    public Fighter GetRandomTarget()
    {
        float randomPercentage = RandomGenerator.GetRandomNumber(0f, 100f);

        float currentPercentageMax = 0f;

        foreach(Agro agro in agros)
        {
            currentPercentageMax += agro.percentageOfAgro;

            if (currentPercentageMax > randomPercentage) return agro.fighter;
        }

        return GetHighestAgroFighter();
    }

    public Fighter GetHighestAgroFighter()
    {
        Fighter highestAgroFighter = null;
        float highestPercentage = 0;

        foreach(Agro agro in agros)
        {
            if(agro.percentageOfAgro > highestPercentage)
            {
                highestAgroFighter = agro.fighter;
                highestPercentage = agro.percentageOfAgro;
            }
        }

        return highestAgroFighter;
    }

    private float GetEvenAgroSplit(float _percentageChange)
    {
        int enemyFighterCount = agros.Count;
        float splitAgro = (_percentageChange / enemyFighterCount);

        return splitAgro;
    }

    private float GetPercentageOfHealthChange(Health _health, float _changeAmount)
    {
        float percentageBeforeDamage = _health.GetMaxHealthPoints() / (_health.GetHealthPoints() + _changeAmount);
        float percentageAfterDamage = _health.GetHealthPercentage();
        float percentageChangeAmount = (Mathf.Abs(percentageBeforeDamage - percentageAfterDamage)) * 100f;

        return percentageChangeAmount;
    }

    private float GetAgroPercentage(Fighter _fighter)
    {
        foreach(Agro agro in agros)
        {
            if (agro.fighter == _fighter) return agro.percentageOfAgro;
        }

        return 0f;
    }
}

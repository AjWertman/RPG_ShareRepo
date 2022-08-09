using RPGProject.Combat;
using RPGProject.Core;
using RPGProject.GameResources;
using System.Collections.Generic;
using UnityEngine;

public class CombatAIBrain : MonoBehaviour
{
    [SerializeField] CombatAIType combatAIType = CombatAIType.mDamage;
    [SerializeField] CombatAIBehavior combatAIBehavior;
    [SerializeField] List<Agro> agros = new List<Agro>();

    [SerializeField] int agroPercentagePerDamagePercentage = 1;

    Fighter fighter = null;

    List<Ability> abilities = new List<Ability>();

    public void InitalizeCombatAIBrain(Fighter _fighter)
    {
        fighter = _fighter;
        abilities = fighter.GetKnownAbilities();
    }

    public void InitalizeAgros(List<Fighter> _enemyFighters)
    {
        int initAgroPercentage = 100 / _enemyFighters.Count;
        foreach(Fighter fighter in _enemyFighters)
        {
            Agro newAgro = new Agro(fighter, initAgroPercentage);
            agros.Add(newAgro);

            fighter.onAgroAction += UpdateAgro;
        }
    }

    public void PlanNextMove(List<Fighter> _allFighters)
    {
        Fighter randomTarget = GetRandomTarget();
        if (randomTarget == null) return;

        Health targetHealth = randomTarget.GetHealthComponent();

        //https://docs.larian.game/Combat_AI

        //How many AP to spend to get in attack range? 
        //Is target below X percentage of health? Can I kill them on this turn? 
        //What is the best move for me to use on my target? Would my best moves be overkill?
        //Do I know my targets strengths/weaknesses? Can I exploit them?

        //Whats my role in combat? 
        ///IDEA - create different behaviors (Tank, mDamage, rDamage, Healer/Support)
        ///Each has a percentage of different actions (based on combatAIBrain.GetRandomTarget()) and randomly executes each one
        ///Should above list be Dynamically changing based on battle conditions?
        ///

        //Do any of my teammates need support?
        //Whats my health at? Should/Can I heal myself?
        //How can I make my/my teammates next turn easier
        //Whos the best teammate of mine? How can I make them even better?

        //If I am gonna move to X, is there anything in my way? Does it hurt or benefit me?
        //Am I too close/far from my enemies? Should I move somewhere else?
    }

    public void UpdateAgro(Fighter _agressor, float _changeAmount)
    {
        Fighter agressorTarget = (Fighter)_agressor.selectedTarget;
        if (fighter != agressorTarget) return;

        int percentageChange = GetPercentageOfHealthChange(fighter.GetHealthComponent(), _changeAmount);

        int agroPercentage = percentageChange * agroPercentagePerDamagePercentage;

        int agroToTakeFromOthers = GetEvenAgroSplit(percentageChange);
        //print("agro to take from others " + agroToTakeFromOthers.ToString());

        for (int i = 0; i < agros.Count; i++)
        {
            Agro agro = agros[i];
            if (agro.fighter == _agressor)agro.percentageOfAgro += percentageChange;
            else agro.percentageOfAgro -= agroToTakeFromOthers;

            agros[i] = agro;
        }

        FormatAgros(_agressor);
    }

    private void FormatAgros(Fighter _agressor)
    {
        int totalPercentageAmount = 0;
        for (int i = 0; i < agros.Count; i++)
        {
            totalPercentageAmount += agros[i].percentageOfAgro;
        }

        //print("total = " + totalPercentageAmount.ToString());

        int excessPercentage= 0;
        if (totalPercentageAmount > 100)
        {
            //print("Total > 100");
            excessPercentage = totalPercentageAmount - 100;

            int splitAgro = GetEvenAgroSplit(excessPercentage);
            for (int i = 0; i < agros.Count; i++)
            {
                Agro agro = agros[i];
                agro.percentageOfAgro -= splitAgro;

                agros[i] = agro;
            }
        }
        else if (totalPercentageAmount < 100)
        {
            //print("Total < 100");
            excessPercentage = 100 - totalPercentageAmount;
            for (int i = 0; i < agros.Count; i++)
            {
                if(agros[i].fighter == _agressor)
                {
                    Agro agressorAgro = agros[i];
                    agressorAgro.percentageOfAgro += excessPercentage;
                    agros[i] = agressorAgro;
                }
            }
        }
    }

    public void RemoveFromAgrosList(Fighter _deadFighter)
    {
        int agroToRedistribute = GetAgroPercentage(_deadFighter);
        agros.Remove(GetAgro(_deadFighter));

        int evenSplit = GetEvenAgroSplit(agroToRedistribute);

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

    private int GetEvenAgroSplit(int _percentageChange)
    {
        int enemyFighterCount = (agros.Count-1);
        int splitAgro = (_percentageChange / enemyFighterCount);

        return splitAgro;
    }

    private int GetPercentageOfHealthChange(Health _health, float _changeAmount)
    {
        bool isDamage = _changeAmount < 0;

        float percentageBeforeDamage = ((_health.healthPoints + Mathf.Abs(_changeAmount)) / _health.maxHealthPoints);
        float percentageAfterDamage =_health.healthPercentage;

        int percentageChangeAmount = (int)((Mathf.Abs(percentageBeforeDamage - percentageAfterDamage)) * 100f);

        return percentageChangeAmount;
    }

    private int GetAgroPercentage(Fighter _fighter)
    {
        foreach(Agro agro in agros)
        {
            if (agro.fighter == _fighter) return agro.percentageOfAgro;
        }

        return 0;
    }
}

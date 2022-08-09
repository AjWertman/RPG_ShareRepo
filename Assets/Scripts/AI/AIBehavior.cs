using RPGProject.Combat;
using RPGProject.Control.Combat;
using System;
using System.Collections.Generic;
using UnityEngine;

//Refactor - rename to AIMoveCalculator
public enum TargetPreferences { Ignore, Indifferent, Preferred, Ideal }

public static class AIBehavior
{
    public static List<AICombatAction> GetViableActions(UnitController _currentUnitTurn, List<Fighter> _allFighters)
    {
        List<AICombatAction> combatActions = new List<AICombatAction>();
        Fighter currentFighter = _currentUnitTurn.GetFighter();

        List<Ability> usableAbilities = GetUsableAbilities(currentFighter, currentFighter.GetKnownAbilities());

        Debug.Log(currentFighter.name);
        Dictionary<Fighter, TargetPreferences> targetPrefences = GetTargetPreferences(_currentUnitTurn,_allFighters);

        foreach(Fighter fighter in targetPrefences.Keys)
        {
            Debug.Log(fighter.name.ToString() + " - " + targetPrefences[fighter].ToString());
        }

        return combatActions;
    }

    private static Dictionary<Fighter, TargetPreferences> GetTargetPreferences(UnitController _currentUnitTurn, List<Fighter> _allFighters)
    {
        Dictionary<Fighter, TargetPreferences> targetPrefences = new Dictionary<Fighter, TargetPreferences>();

        Fighter currentFighter = _currentUnitTurn.GetFighter();
        CombatAIType combatAIType = _currentUnitTurn.combatAIType;

        foreach(Fighter fighter in _allFighters)
        {
            //Refactor - could ask about self
            if (fighter == currentFighter) continue;

            TargetPreferences targetPreference = TargetPreferences.Indifferent;

            bool isTeammate = (currentFighter.unitInfo.isPlayer == fighter.unitInfo.isPlayer);
            float healthPercentage = fighter.GetHealthComponent().healthPercentage;

            if (isTeammate)
            {
                if (combatAIType == CombatAIType.Healer)
                {
                    targetPreference = GetTargetPreferenceByHealth(healthPercentage);
                }
            }
            else
            {
                List<TargetPreferences> tempPreferences = new List<TargetPreferences>();

                bool isDamageDealer = (combatAIType == CombatAIType.mDamage || combatAIType == CombatAIType.rDamage);
                
                UnitAgro unitAgro = _currentUnitTurn.GetUnitAgro();
                int unitAgroPercentage = unitAgro.GetAgroPercentage(fighter);

                tempPreferences.Add(GetTargetPreferenceByAgro(unitAgroPercentage));
                tempPreferences.Add(GetTargetPreferenceByHealth(healthPercentage));

                targetPreference = GetTargetPreferenceByAverage(tempPreferences);

            }

            targetPrefences.Add(fighter, targetPreference);
        }

        return targetPrefences;
    }

    private static List<Ability> GetUsableAbilities(Fighter _fighter, List<Ability> _abilities)
    {
        List<Ability> usableAbilities = new List<Ability>();

        foreach (Ability ability in _abilities)
        {
            if(_fighter.unitResources.actionPoints >= ability.actionPointsCost)
            {
                usableAbilities.Add(ability);
            }
        }

        return usableAbilities;
    }

    private static Dictionary<Fighter, bool> SortAllFighters(List<Fighter> _allFighters)
    {
        Dictionary<Fighter, bool> allFighters = new Dictionary<Fighter, bool>();

        foreach (Fighter fighter in _allFighters)
        {
            bool isPlayer = fighter.unitInfo.isPlayer;
            allFighters.Add(fighter, isPlayer);         
        }

        return allFighters;
    }

    private static TargetPreferences GetTargetPreferenceByHealth(float _healthPercentage)
    {
        if (_healthPercentage <= .10f) return TargetPreferences.Ideal;
        else if (_healthPercentage > .10f && _healthPercentage <= .25f) return TargetPreferences.Preferred;
        else if (_healthPercentage > .25f && _healthPercentage <= .50f) return TargetPreferences.Indifferent;
        else return TargetPreferences.Ignore;
    }

    private static TargetPreferences GetTargetPreferenceByAgro(int _agroPercentage)
    {
        if (_agroPercentage >= 90) return TargetPreferences.Ideal;
        else if (_agroPercentage < 90 && _agroPercentage >= 60) return TargetPreferences.Preferred;
        else if (_agroPercentage < 60 && _agroPercentage >= 10) return TargetPreferences.Indifferent;
        else return TargetPreferences.Ignore;
    }

    private static TargetPreferences GetTargetPreferenceByAverage(List<TargetPreferences> _targetPreferences)
    {
        TargetPreferences targetPreference = TargetPreferences.Indifferent;

        int listCount = _targetPreferences.Count;
        int currentTotal = 0;

        foreach(TargetPreferences pref in _targetPreferences)
        {
            if (pref == TargetPreferences.Ideal) return TargetPreferences.Ideal;
            Debug.Log(pref.ToString());

            int prefIndex = (int)pref;
            currentTotal += (prefIndex + 1);
        }
        Debug.Log(currentTotal.ToString() + " " + listCount.ToString());

        int averageIndex = Mathf.RoundToInt(currentTotal / listCount);
        targetPreference = (TargetPreferences)averageIndex;

        return targetPreference;
    }
}

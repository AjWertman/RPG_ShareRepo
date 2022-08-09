using RPGProject.Combat;
using RPGProject.Combat.Grid;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Control.Combat
{
    public enum TargetPreference { Ignore, Indifferent, Preferred, Ideal }

    //Refactor - rename to AIMoveCalculator
    public static class AIBehavior
    {
        public static List<AICombatAction> GetViableActions(UnitController _currentUnitTurn, List<UnitController> _allUnits)
        {

            Fighter currentFighter = _currentUnitTurn.GetFighter();

            List<Ability> usableAbilities = GetUsableAbilities(currentFighter, currentFighter.GetKnownAbilities());
            Dictionary<UnitController, TargetPreference> targetPreferences = SetTargetPreferences(_currentUnitTurn, _allUnits);

            int preferredDistance = GetPreferredDistance(_currentUnitTurn, usableAbilities);
            float currentPositionScore = CalculateCurrentPositionScore(_currentUnitTurn, targetPreferences);

            //Debug.Log(currentFighter.name);
            //foreach (Fighter fighter in targetPrefences.Keys)
            //{
            //    Debug.Log(fighter.name.ToString() + " - " + targetPrefences[fighter].ToString());
            //}

            Dictionary<AICombatAction, float> possibleActions = GetPossibleActions(_currentUnitTurn, usableAbilities, targetPreferences);
            List<AICombatAction> viableActions = new List<AICombatAction>();

            return viableActions;
        }

        private static Dictionary<AICombatAction, float> GetPossibleActions(UnitController _currentUnitTurn, List<Ability> _usableAbilities, Dictionary<UnitController, TargetPreference> _targetPreferences)
        {
            Dictionary<AICombatAction, float> possibleActions = new Dictionary<AICombatAction, float>();

            CombatAIType combatAIType = _currentUnitTurn.combatAIType;

            foreach(Ability ability in _usableAbilities)
            {
                //Have calculation that takes into account stats (damage amount range);
                float baseAbilityAmount = ability.baseAbilityAmount;
                float attackRange = ability.attackRange;

                foreach(UnitController unit in _targetPreferences.Keys)
                {
                    AICombatAction combatAction = new AICombatAction();
                    float score = 0;

                    TargetPreference targetPreference = _targetPreferences[unit];
                    bool isTeammate = _currentUnitTurn.unitInfo.isPlayer == unit.unitInfo.isPlayer;
                    bool isInRange = false;

                    if (isTeammate)
                    {

                    }
                    else
                    { 
                        //If is melee
                        if(combatAIType == CombatAIType.mDamage || combatAIType == CombatAIType.Tank)
                        {
                            //IsNeighborBlock cannot be static?
                            //bool isNeighbor = Pathfinder.IsNeighborBlock(_currentUnitTurn.currentBlock, unit.currentBlock));

                            bool isNeighbor = true;

                            //If can attack without moving. What about for ranged tho?
                            if (attackRange <= 0 && isNeighbor)
                            {
                                score += 10;
                            }
                        }

                        //If move will kill target
                        if (unit.GetHealth().healthPoints + baseAbilityAmount <= 0)
                        {
                            score += 10;
                        }
                    }

                    if (!combatAction.Equals(default(AICombatAction))) possibleActions.Add(combatAction, score);
                }
            }

            return possibleActions;
        }

        private static float CalculateCurrentPositionScore(UnitController _currentUnitTurn, Dictionary<UnitController, TargetPreference> _targetPrefences)
        {
            GridBlock currentBlock = _currentUnitTurn.currentBlock;
            float currentPositionScore = 10;

            //Am I standing in something? is it good or bad?
            //Am I close to my preferred target and do I want to be?

            foreach(UnitController unit in _targetPrefences.Keys)
            {
                TargetPreference targetPreference = _targetPrefences[unit];             
                //bool isNeighbor = Pathfinder.IsNeighborBlock(_currentUnitTurn.currentBlock, unit.currentBlock);

                bool wantsToBeClose = (unit.combatAIType == CombatAIType.mDamage || unit.combatAIType == CombatAIType.Tank);

                if(true && wantsToBeClose)
                {
                    
                }
            }
     
            //if (currentBlock.status == negativeStatus) return 0;


            return currentPositionScore;
        }

        private static float GetPreferenceModifier(TargetPreference _targetPreference)
        {
            switch (_targetPreference)
            {
                case TargetPreference.Ignore:
                    return .5f;
                case TargetPreference.Indifferent:
                    return 1f;
                case TargetPreference.Preferred:
                    return 5f;
                case TargetPreference.Ideal:
                    return 10f;
            }

            return 1f;
        }

        private static int GetPreferredDistance(UnitController _currentUnit, List<Ability> _usableAbilities)
        {
            CombatAIType combatAIType = _currentUnit.combatAIType;
            bool wantsToBeClose = (combatAIType == CombatAIType.mDamage || combatAIType == CombatAIType.Tank);

            if (!wantsToBeClose)
            {
                //Should units have own "Attack Range" or should individual abilities contain that data;
                return 6;
            }
            else return 1;
        }

        private static List<Ability> GetUsableAbilities(Fighter _fighter, List<Ability> _abilities)
        {
            List<Ability> usableAbilities = new List<Ability>();

            foreach (Ability ability in _abilities)
            {
                if (_fighter.unitResources.actionPoints >= ability.actionPointsCost)
                {
                    usableAbilities.Add(ability);
                }
            }

            return usableAbilities;
        }

        private static Dictionary<UnitController, TargetPreference> SetTargetPreferences(UnitController _currentUnitTurn, List<UnitController> _allUnits)
        {
            Dictionary<UnitController, TargetPreference> targetPrefences = new Dictionary<UnitController, TargetPreference>();

            Fighter currentFighter = _currentUnitTurn.GetFighter();
            CombatAIType combatAIType = _currentUnitTurn.combatAIType;

            foreach (UnitController unit in _allUnits)
            {
                Fighter fighter = unit.GetFighter();
                List<TargetPreference> tempPreferences = new List<TargetPreference>();

                bool isTeammate = (currentFighter.unitInfo.isPlayer == fighter.unitInfo.isPlayer);
                float healthPercentage = fighter.GetHealthComponent().healthPercentage;

                if (isTeammate)
                {
                    tempPreferences.Add(GetTargetPreferenceByHealth(healthPercentage));
                }
                else
                {
                    bool isDamageDealer = (combatAIType == CombatAIType.mDamage || combatAIType == CombatAIType.rDamage);

                    UnitAgro unitAgro = _currentUnitTurn.GetUnitAgro();
                    int unitAgroPercentage = unitAgro.GetAgroPercentage(fighter);

                    tempPreferences.Add(GetTargetPreferenceByAgro(unitAgroPercentage));
                    tempPreferences.Add(GetTargetPreferenceByHealth(healthPercentage));
                }

                //tempPreferences.Add(GetTargetPreferenceByDistance(currentFighter.transform, fighter.transform));

                TargetPreference targetPreference = GetTargetPreferenceByAverage(tempPreferences);
                targetPrefences.Add(unit, targetPreference);
            }

            return targetPrefences;
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

        private static TargetPreference GetTargetPreferenceByHealth(float _healthPercentage)
        {
            if (_healthPercentage <= .10f) return TargetPreference.Ideal;
            else if (_healthPercentage > .10f && _healthPercentage <= .25f) return TargetPreference.Preferred;
            else if (_healthPercentage > .25f && _healthPercentage <= .50f) return TargetPreference.Indifferent;
            else return TargetPreference.Ignore;
        }

        private static TargetPreference GetTargetPreferenceByAgro(int _agroPercentage)
        {
            if (_agroPercentage >= 90) return TargetPreference.Ideal;
            else if (_agroPercentage < 90 && _agroPercentage >= 60) return TargetPreference.Preferred;
            else if (_agroPercentage < 60 && _agroPercentage >= 10) return TargetPreference.Indifferent;
            else return TargetPreference.Ignore;
        }

        private static TargetPreference GetTargetPreferenceByDistance(Transform _myTransform, Transform _targetTransform)
        {
            Vector3 myPosition = new Vector3(_myTransform.position.x, 0, _myTransform.position.z);
            Vector3 targetPosition = new Vector3(_targetTransform.position.x, 0, _targetTransform.position.z);
            float distance = Vector3.Distance(myPosition, targetPosition);

            if (distance <= 2) return TargetPreference.Preferred;
            else if (distance > 2 && distance <= 10) return TargetPreference.Indifferent;
            else return TargetPreference.Ignore;
        }

        private static TargetPreference GetTargetPreferenceByAverage(List<TargetPreference> _targetPreferences)
        {
            TargetPreference targetPreference = TargetPreference.Indifferent;

            int listCount = _targetPreferences.Count;

            if (listCount > 0)
            {
                int currentTotal = 0;
                foreach (TargetPreference pref in _targetPreferences)
                {
                    if (pref == TargetPreference.Ideal) return TargetPreference.Ideal;

                    int prefIndex = (int)pref;
                    currentTotal += (prefIndex + 1);
                }

                int averageIndex = Mathf.RoundToInt(currentTotal / listCount);
                targetPreference = (TargetPreference)averageIndex;
            }

            return targetPreference;
        }
    }
}

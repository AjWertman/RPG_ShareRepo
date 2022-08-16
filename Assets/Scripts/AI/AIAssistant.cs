using RPGProject.Core;
using RPGProject.GameResources;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Combat.AI
{
    //Useful functions that are used by the BattleAIBrain 
    public static class AIAssistant
    {
        public static AIRanking GetAPCostRanking(int _currentAP, int _apCost)
        {
            if (_apCost == 0) return AIRanking.Great;
            if (_currentAP > _apCost)
            {
                float percentageOfAP = _currentAP / _apCost;

                if (percentageOfAP <= .25f) return AIRanking.Great;
                else if (percentageOfAP > .25f && percentageOfAP <= .50f) return AIRanking.Good;
                else if (percentageOfAP > .50f && percentageOfAP <= .75f) return AIRanking.Mediocre;
                else return AIRanking.Bad;
            }
            else return AIRanking.Mediocre;
        }

        public static AIRanking GetImpactRanking(AIBattleAction _combatAction, bool _isPlayer)
        {
            float impactScore = 0f;

            Ability ability = _combatAction.selectedAbility;
            float abilityAmount = ability.baseAbilityAmount;
            bool isDamage = abilityAmount < 0;

            Fighter target = _combatAction.target;
            Health targetHealth = target.GetHealthComponent();
            float currentHealthPoints = targetHealth.healthPoints;
            float currentHealthPercentage = targetHealth.healthPercentage;
            float percentageAfterAbility = (currentHealthPoints + abilityAmount) / targetHealth.maxHealthPoints;

            bool isTeammate = target.unitInfo.isPlayer == _isPlayer;
                     
            if (currentHealthPercentage <= .25f) impactScore += 25f;
            else if (currentHealthPercentage > .25f && currentHealthPercentage < .50f) impactScore += 15f;
            else impactScore += 5f;

            if (isDamage && !isTeammate)
            {
                if (percentageAfterAbility <= 0) impactScore += 40f;
                else if (MathAssistant.IsBetween(percentageAfterAbility, 0, .25f)) impactScore += 20f;
                else if (MathAssistant.IsBetween(percentageAfterAbility, .25f, .50f)) impactScore += 10f;
            }
            else if(!isDamage && isTeammate)
            {
                if (percentageAfterAbility >= .75) impactScore += 25f;
                else if (MathAssistant.IsBetween(percentageAfterAbility, .49f, .74f)) impactScore += 15f;
                else impactScore += 5f;
            }

            if (impactScore >= 40) return AIRanking.Great;
            else if (MathAssistant.IsBetween(impactScore, 29, 39)) return AIRanking.Good;
            else if (MathAssistant.IsBetween(impactScore, 14, 28)) return AIRanking.Mediocre;
            else return AIRanking.Bad;
        }

        public static AIActionType GetHighestActionType(AIBattleBehavior _behaviorPreset, List<AIActionType> _actionTypes)
        {
            if (_actionTypes.Count == 1) return _actionTypes[0];

            int highestIndex = int.MaxValue;

            foreach(AIActionType testActionType in _actionTypes)
            {
                int currentIndex = 0;
                foreach(AIActionType presetType in _behaviorPreset.actionTypes)
                {
                    if (testActionType == presetType) break;

                    currentIndex++;
                }

                if (highestIndex > currentIndex) highestIndex = currentIndex;
            }

            return (AIActionType)highestIndex;
        }

        public static AIRanking GetBehaviorRanking(AIBattleBehavior _behaviorPreset, AIActionType _actionType)
        {
            int typeIndex = 0;
            foreach(AIActionType actionType in _behaviorPreset.actionTypes)
            {
                if (actionType == _actionType) break;         
                typeIndex++;
            }

            if (typeIndex == 0) return AIRanking.Great;
            else if (typeIndex == 1 || typeIndex == 2) return AIRanking.Good;
            else if (typeIndex == 3 || typeIndex == 4) return AIRanking.Mediocre;
            else return AIRanking.Bad;
        }

        public static AIRanking GetRankingByTargetStatus(Fighter _target)
        {       
            Health targetHealth = _target.GetHealthComponent();
            float currentHealthPercentage = targetHealth.healthPercentage;

            if (MathAssistant.IsBetween(currentHealthPercentage, 0, .25f)) return AIRanking.Great;
            else if (MathAssistant.IsBetween(currentHealthPercentage, .25f, .50f)) return AIRanking.Good;
            else if (MathAssistant.IsBetween(currentHealthPercentage, .50f, .90f)) return AIRanking.Mediocre;
            else return AIRanking.Bad;
            
            //Add buff/debuff strength
            //Add Position strength
        }

        //public static AIRanking GetBehaviorRanking(Fighter _currentFighter, AIBattleAction _action)
        //{
        //    Fighter target = _action.target;
        //    Ability ability = _action.selectedAbility;
            
        //}

        public static float GetModifier(AIRanking _ranking)
        {
            switch (_ranking)
            {
                case AIRanking.Bad:
                    return -10f;
                case AIRanking.Mediocre:
                    return 0f;
                case AIRanking.Good:
                    return 15f;
                case AIRanking.Great:
                    return 25f;
            }

            return 0;
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

        public static float GetHealthPercentageModifier(float _healthPercentage)
        {
            if (MathAssistant.IsBetween(_healthPercentage, 0f, .25f)) return 25f;
            else if (MathAssistant.IsBetween(_healthPercentage, .25f, .50f)) return 15f;
            else if (MathAssistant.IsBetween(_healthPercentage, .50f, .75f)) return 5f;
            else return 0f;
        }

        public static TargetPreference GetTargetPreferenceByAgro(int _agroPercentage)
        {
            if (_agroPercentage >= 90) return TargetPreference.Ideal;
            else if (_agroPercentage < 90 && _agroPercentage >= 60) return TargetPreference.Preferred;
            else if (_agroPercentage < 60 && _agroPercentage >= 10) return TargetPreference.Indifferent;
            else return TargetPreference.Ignore;
        }

        //Gets preferences by distance (not amount of grid blocks)
        public static TargetPreference GetTargetPreferenceByDistance(Transform _currentTransform, Transform _testTransform)
        {
            float distance = GetDistance(_currentTransform.position, _testTransform.position);

            if (distance <= 2) return TargetPreference.Preferred;
            else if (distance > 2 && distance <= 10) return TargetPreference.Indifferent;
            else return TargetPreference.Ignore;
        }

        //Gets the average preference based on a list of preferences
        public static TargetPreference GetTargetPreferenceByAverage(List<TargetPreference> _targetPreferences)
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

        public static float GetPreferenceModifier(TargetPreference _targetPreference)
        {
            switch (_targetPreference)
            {
                case TargetPreference.Ignore:
                    return 1f;
                case TargetPreference.Indifferent:
                    return 2f;
                case TargetPreference.Preferred:
                    return 5f;
                case TargetPreference.Ideal:
                    return 10f;
            }

            return 1f;
        }

        public static float GetDistance(Vector3 _currentPosition, Vector3 _testPosition)
        {
            Vector3 myPosition = new Vector3(_currentPosition.x, 0, _currentPosition.z);
            Vector3 targetPosition = new Vector3(_testPosition.x, 0, _testPosition.z);
            return Vector3.Distance(myPosition, targetPosition);
        }

        public static float GetScoreByGCost(int _gCost)
        {
            if (_gCost <= 24) return 2f;
            else if (_gCost > 24 && _gCost <= 38) return 1f;
            else return 0f;
        }

        public static float GetAITypeModifier(AIBattleType _currentType, AIBattleAction _actionToTest)
        {
            bool isTypeMatch = false;

            foreach(AIBattleType combatAIType in GetCompatableAITypes(_actionToTest))
            {
                if (_currentType == combatAIType) isTypeMatch = true;
            }

            if (isTypeMatch) return 2f;
            else return .5f;
        } 


        private static IEnumerable<AIBattleType> GetCompatableAITypes(AIBattleAction _actionToTest)
        {
            bool isDamage = _actionToTest.selectedAbility.baseAbilityAmount < 0f;
            if (isDamage)
            {
                yield return AIBattleType.mDamage;
                yield return AIBattleType.rDamage;
                yield return AIBattleType.Tank;
            }
            else
            {
                yield return AIBattleType.Healer;
            }
        }
    }
}
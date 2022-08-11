using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Combat.AI
{
    //Has many useful functions that are used by the AIMoveCalculator 
    public static class AIAssistant
    {
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

        public static TargetPreference GetTargetPreferenceByHealth(float _healthPercentage)
        {
            if (_healthPercentage <= .10f) return TargetPreference.Ideal;
            else if (_healthPercentage > .10f && _healthPercentage <= .25f) return TargetPreference.Preferred;
            else if (_healthPercentage > .25f && _healthPercentage <= .50f) return TargetPreference.Indifferent;
            else return TargetPreference.Ignore;
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
            float distance = GetDistance(_currentTransform, _testTransform);

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

        public static float GetDistance(Transform _currentTransform, Transform _testTransform)
        {
            Vector3 myPosition = new Vector3(_currentTransform.position.x, 0, _currentTransform.position.z);
            Vector3 targetPosition = new Vector3(_testTransform.position.x, 0, _testTransform.position.z);
            return Vector3.Distance(myPosition, targetPosition);
        }

        public static float GetScoreByActionPointsCost(int _currentAP, int _apCost)
        {
            if (_apCost == 0) return 5f;
            if (_currentAP > _apCost)
            {
                float percentageOfAP = _currentAP / _apCost;

                if (percentageOfAP <= .25f) return 3f;
                else if (percentageOfAP > .25f && percentageOfAP <= .50f) return 2f;
                else return 1f;
            }
            else return 1f;
        }

        public static float GetScoreByGCost(int _gCost)
        {
            if (_gCost <= 24) return 2f;
            else if (_gCost > 24 && _gCost <= 38) return 1f;
            else return 0f;
        }

        public static float GetAITypeModifier(CombatAIType _currentType, AICombatAction _actionToTest)
        {
            bool isTypeMatch = false;

            foreach(CombatAIType combatAIType in GetCompatableAITypes(_actionToTest))
            {
                if (_currentType == combatAIType) isTypeMatch = true;
            }

            if (isTypeMatch) return 2f;
            else return .5f;
        } 

        private static IEnumerable<CombatAIType> GetCompatableAITypes(AICombatAction _actionToTest)
        {
            bool isDamage = _actionToTest.selectedAbility.baseAbilityAmount < 0f;
            if (isDamage)
            {
                yield return CombatAIType.mDamage;
                yield return CombatAIType.rDamage;
                yield return CombatAIType.Tank;
            }
            else
            {
                yield return CombatAIType.Healer;
            }
        }
    }
}
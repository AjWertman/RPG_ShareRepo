using RPGProject.Core;
using RPGProject.GameResources;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Combat.AI
{
    /// <summary>
    /// Useful functions that are used by the BattleAIBrain to calculate moves.
    /// </summary>
    public static class AIAssistant
    {
        /// <summary>
        /// Returns the ranking of the cost to use an ability.
        /// This also includes the cost to move in range if necessary.
        /// </summary>
        public static AIRanking GetEnergyCostRanking(int _currentEnergy, int _energyCost)
        {
            if (_energyCost == 0) return AIRanking.Great;
            if (_currentEnergy > _energyCost)
            {
                float percentageOfEnergyUsed = (float)_energyCost / (float)_currentEnergy;

                if (MathAssistant.IsBetween(percentageOfEnergyUsed, 0, .15f)) return AIRanking.Great;
                else if (MathAssistant.IsBetween(percentageOfEnergyUsed, .15f, .25f)) return AIRanking.Good;
                else if (MathAssistant.IsBetween(percentageOfEnergyUsed, .25f, .50f)) return AIRanking.Mediocre;
                else return AIRanking.Bad;
            }
            else return AIRanking.Bad;
        }

        /// <summary>
        /// Returns the ranking of the impact/strength of an action. 
        /// Lower health targets are high priority for both damage and healing.
        /// If the action does a lot of damage or healing, it gets a higher ranking.
        /// </summary>
        public static AIRanking GetImpactRanking(AIBattleAction _combatAction, bool _isPlayer)
        {
            float impactScore = 0f;

            Ability ability = _combatAction.selectedAbility;
            float abilityAmount = ability.baseAbilityAmount;
            bool isDamage = abilityAmount < 0;

            Fighter target = _combatAction.target;
            Health targetHealth = target.GetHealth();
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
                if (currentHealthPercentage == 1) impactScore = 0;
                else
                {
                    if (percentageAfterAbility >= .75) impactScore += 25f;
                    else if (MathAssistant.IsBetween(percentageAfterAbility, .49f, .74f)) impactScore += 15f;
                    else impactScore += 5f;
                }
            }

            if (impactScore >= 40) return AIRanking.Great;
            else if (MathAssistant.IsBetween(impactScore, 29, 39)) return AIRanking.Good;
            else if (MathAssistant.IsBetween(impactScore, 14, 28)) return AIRanking.Mediocre;
            else return AIRanking.Bad;
        }

        /// <summary>
        /// Determines what type of action(s) a move is and returns the highest action based on the type of AI.
        /// For example, if an action has the type "Damage" and "Taunt," a damage dealer will return damage,
        /// where a tank would return taunt.
        /// </summary>
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

        /// <summary>
        /// Returns the ranking of an Action Type based on the type of AI.
        /// For example, if the type of AI is a healer, it will rank healing and support abilities high
        /// and it would rank taunt and damage abilities low.
        /// </summary>
        public static AIRanking GetBehaviorRanking(AIBattleBehavior _behaviorPreset, AIActionType _actionType)
        {
            if (_behaviorPreset.aiType == AIBattleType.Custom) return AIRanking.Great;

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

        /// <summary>
        /// Returns the ranking of the target status based on their current health percentage,
        /// any buffs/debuffs the target has, and the strength of their current position.
        /// </summary>
        public static AIRanking GetRankingByTargetStatus(Fighter _target)
        {       
            Health targetHealth = _target.GetHealth();
            float currentHealthPercentage = targetHealth.healthPercentage;

            if (MathAssistant.IsBetween(currentHealthPercentage, 0, .25f)) return AIRanking.Great;
            else if (MathAssistant.IsBetween(currentHealthPercentage, .25f, .50f)) return AIRanking.Good;
            else if (MathAssistant.IsBetween(currentHealthPercentage, .50f, .90f)) return AIRanking.Mediocre;
            else return AIRanking.Bad;
            
            //Add buff/debuff strength
            //Add Position strength
        }

        /// <summary>
        /// Returns a ranking based on the Agro of a certain enemy. The higher the agro, the higher the ranking. 
        /// Healers and other support AI are less influenced by agro.
        /// </summary>
        public static AIRanking GetRankingByAgro(AIBattleType _aiType, Agro _agro)
        {
            int agroPercentage = _agro.percentageOfAgro;
            bool isDamageOrTank = _aiType == AIBattleType.Tank || _aiType == AIBattleType.mDamage || _aiType == AIBattleType.rDamage;

            if (isDamageOrTank)
            {
                if (MathAssistant.IsBetween(agroPercentage, 75f, 100f)) return AIRanking.Great;
                else if (MathAssistant.IsBetween(agroPercentage, 50f, 75f)) return AIRanking.Good;
                else if (MathAssistant.IsBetween(agroPercentage, 25f, 50f)) return AIRanking.Mediocre;
                else return AIRanking.Bad;
            }
            else
            {
                if (MathAssistant.IsBetween(agroPercentage, 90f, 100f)) return AIRanking.Great;
                else if (MathAssistant.IsBetween(agroPercentage, 75f, 90f)) return AIRanking.Good;
                else if (MathAssistant.IsBetween(agroPercentage, 60f, 75f)) return AIRanking.Mediocre;
                else return AIRanking.Bad;
            }
        }

        /// <summary>
        /// Returns the average ranking from the list of rankings.
        /// </summary>
        public static AIRanking GetRankAverage(List<AIRanking> _rankings)
        {
            AIRanking averageRank = AIRanking.Mediocre;

            int listCount = _rankings.Count;

            if (listCount > 0)
            {
                int currentTotal = 0;
                foreach (AIRanking rank in _rankings)
                {
                    int rankIndex = (int)rank;
                    currentTotal += (rankIndex + 1);
                }

                int averageIndex = Mathf.RoundToInt(currentTotal / listCount);
                averageRank = (AIRanking)averageIndex;
            }

            return averageRank;
        }

        public static bool IsHigherRanking(AIRanking _myRanking, AIRanking _rankingToTest)
        {
            if (_myRanking == _rankingToTest) return false;

            int myRankingIndex = (int)_myRanking;
            int testRankingIndex = (int)_rankingToTest;

            if (myRankingIndex > testRankingIndex) return false;
            else return true;
        }

        public static float GetScore(List<AIRanking> _aiRankings)
        {
            float score = 0;

            foreach (AIRanking aiRanking in _aiRankings)
            {
                float modifier = GetModifier(aiRanking);
                score += modifier;
            }

            return score;
        }

        public static float GetDistance(Vector3 _currentPosition, Vector3 _testPosition)
        {
            Vector3 myPosition = new Vector3(_currentPosition.x, 0, _currentPosition.z);
            Vector3 targetPosition = new Vector3(_testPosition.x, 0, _testPosition.z);
            return Vector3.Distance(myPosition, targetPosition);
        }

        public static bool IsTeammate(bool _isPlayerA, bool _isPlayerB)
        {
            return _isPlayerA == _isPlayerB;
        }

        /// <summary>
        /// Returns a "score" based on the ranking. 
        /// </summary>
        private static float GetModifier(AIRanking _ranking)
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
    }
}
using RPGProject.Combat;
using RPGProject.Combat.AI;
using RPGProject.Combat.Grid;
using RPGProject.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Control.Combat
{
    public class CombatAIBrain : MonoBehaviour
    {
        [SerializeField] List<AIBattleBehavior> presetBehaviors = new List<AIBattleBehavior>();

        BattleGridManager battleGridManager = null;
        Pathfinder pathfinder = null;

        private void Awake()
        {
            battleGridManager = GetComponentInChildren<BattleGridManager>();
            pathfinder = GetComponentInChildren<Pathfinder>();
        }

        public AIBattleAction GetBestAction(UnitController _currentUnitTurn, List<UnitController> _allUnits)
        {
            Fighter currentFighter = _currentUnitTurn.GetFighter();
            AIBattleType combatAIType = _currentUnitTurn.aiType;
   
            List<Ability> usableAbilities = GetUsableAbilities(currentFighter, currentFighter.GetKnownAbilities());

            Dictionary<AIBattleAction, float> possibleActions = GetPossibleActions(_currentUnitTurn, usableAbilities, _allUnits);

            AIBattleAction bestAction = CalculateBestAction(possibleActions);
            return bestAction;
        }

        private AIBattleAction CalculateBestAction(Dictionary<AIBattleAction, float> _possibleActions)
        {
            AIBattleAction bestAction = new AIBattleAction();
            List<AIBattleAction> bestActions = new List<AIBattleAction>();
            float highestScore = 0;

            foreach(AIBattleAction combatAction in _possibleActions.Keys)
            {
                float score = _possibleActions[combatAction];

                if (highestScore < score)
                {
                    bestActions.Clear();
                    bestActions.Add(combatAction);
                    highestScore = score;
                }
                else if(highestScore == score)
                {
                    bestActions.Add(combatAction);
                }
            }

            if(bestActions.Count > 0)
            {
                int randomIndex = RandomGenerator.GetRandomNumber(0, bestActions.Count - 1);
                bestAction = bestActions[randomIndex];
            }
            else
            {
                bestAction = GetLastResortAction();
            }

            print(BattleActionToString(bestAction, highestScore));

            return bestAction;
        }

        private Dictionary<AIBattleAction, float> GetPossibleActions(UnitController _currentUnitTurn,
           List<Ability> _usableAbilities, List<UnitController> _allUnits)
        {
            Dictionary<AIBattleAction, float> possibleActions = new Dictionary<AIBattleAction, float>();
            AIBattleType aiType = _currentUnitTurn.aiType;

            AIBattleBehavior battleBehavior = GetBehaviorPreset(aiType);
            Dictionary<Fighter, AIRanking> preferredTargets = GetPreferredTargets(_currentUnitTurn, _allUnits);

            foreach (Ability ability in _usableAbilities)
            {
                if (_currentUnitTurn.GetFighter().GetCooldown(ability) > 0) continue;
                int currentEnergy = _currentUnitTurn.GetEnergy().energyPoints;
                int costToUseAbility = ability.energyPointsCost;

                if (currentEnergy < costToUseAbility) continue;

                float baseAbilityAmount = ability.baseAbilityAmount;
                float attackRange = ability.attackRange;
                bool isHeal = ability.baseAbilityAmount > 0;

                foreach (UnitController unit in _allUnits)
                {
                    if (unit.GetHealth().isDead) continue;

                    AIBattleAction combatAction = new AIBattleAction();
                    combatAction.target = unit.GetFighter();
                    combatAction.selectedAbility = ability;
                    float score = 0;
                   
                    if(!IsInRange(_currentUnitTurn.currentBlock, unit.currentBlock, ability))
                    {                       
                        GridBlock targetBlock = GetTargetBlock(_currentUnitTurn, unit, ability);
                        if (targetBlock == null) continue;

                        combatAction.targetBlock = targetBlock;
                    }

                    int totalAPCost = GetFullEnergyCost(_currentUnitTurn, combatAction);
                    //print(unit.name + " total cost = " + totalAPCost.ToString());
                    if (currentEnergy < totalAPCost) continue;

                    ///SCORES////////////////////////////////////////////////////////////////////////

                    List<AIRanking> moveRankings = new List<AIRanking>();

                    AIRanking apCostRanking = AIAssistant.GetEnergyCostRanking(currentEnergy, totalAPCost);
                    //print(unit.name + " " + apCostRanking.ToString());
                    moveRankings.Add(apCostRanking);
    
                    //Refactor - take into account current position score vs new position score;
                    AIRanking impactRanking = AIAssistant.GetImpactRanking(combatAction, _currentUnitTurn.unitInfo.isPlayer);
                    moveRankings.Add(impactRanking);

                    AIActionType actionType = GetActionType(battleBehavior, _currentUnitTurn.GetFighter(), combatAction);
                    AIRanking behaviorRanking = AIAssistant.GetBehaviorRanking(GetBehaviorPreset(aiType), actionType);
                    moveRankings.Add(behaviorRanking);
  
                    AIRanking targetPreferrenceRanking = preferredTargets[unit.GetFighter()];
                    moveRankings.Add(targetPreferrenceRanking);

                    score += AIAssistant.GetScore(moveRankings);

                    //AIRanking positionRanking = GetPositionStrengthRanking(_currentUnitTurn, preferredTarget);
                    //score += AIAssistant.GetModifier(positionRanking);

                    ////////////////////////////////////////////////////////////////////////////////
                    
                    bool isTeammate = _currentUnitTurn.unitInfo.isPlayer == unit.unitInfo.isPlayer;
                    if (isTeammate)
                    {
                        //or debuff
                        if (!isHeal) score -= 50f;
                    }
                    else
                    {
                        //or buff
                        if (isHeal) score -= 50f;
                    }

                    //if (score < 0) continue;

                    //print(CombatActionToString(combatAction, score));
                    if (!possibleActions.ContainsKey(combatAction))
                    {
                        possibleActions.Add(combatAction, score);
                    }
                }
            }

            return possibleActions;
        }

        private Dictionary<Fighter, AIRanking> GetPreferredTargets(UnitController _currentUnitTurn, List<UnitController> _allUnits)
        {
            AIBattleType aiType = _currentUnitTurn.aiType;
            bool isPlayer = _currentUnitTurn.unitInfo.isPlayer;
            bool isDamageOrTank = aiType == AIBattleType.Tank || aiType == AIBattleType.mDamage || aiType == AIBattleType.rDamage;

            Dictionary<Fighter, AIRanking> fightersDict = new Dictionary<Fighter, AIRanking>();

            UnitAgro unitAgro = _currentUnitTurn.GetUnitAgro();

            foreach (UnitController unit in _allUnits)
            {
                List<AIRanking> unitRanking = new List<AIRanking>();
                Fighter unitFighter = unit.GetFighter();
                bool isTeammate = AIAssistant.IsTeammate(isPlayer, unit.unitInfo.isPlayer);
                unitRanking.Add(AIAssistant.GetRankingByTargetStatus(unitFighter));

                if (!isTeammate)
                {
                    foreach (Agro agro in unitAgro.agros)
                    {
                        if (agro.fighter != unitFighter) continue;
                        
                        AIRanking agroRanking = AIAssistant.GetRankingByAgro(aiType, agro);
                        unitRanking.Add(agroRanking);
                    }

                    if (isDamageOrTank) unitRanking.Add(AIRanking.Good);
                    else unitRanking.Add(AIRanking.Mediocre);
                }
                else
                {
                    if (isDamageOrTank) unitRanking.Add(AIRanking.Mediocre);
                    else unitRanking.Add(AIRanking.Good);
                }
                AIRanking averageRank = AIAssistant.GetRankAverage(unitRanking);
                fightersDict.Add(unitFighter, averageRank);               
            }

            //AIRanking highestRank = AIRanking.Bad;
            //foreach(Fighter fighter in fightersDict.Keys)
            //{
            //    AIRanking currentRank = fightersDict[fighter];
            //    int currentRankIndex = (int)currentRank;

            //    bool isTeammate = AIAssistant.IsTeammate(isPlayer, fighter.unitInfo.isPlayer);
            //    if (isTeammate)
            //    {
            //        if (isDamageOrTank) currentRankIndex--;
            //        else currentRankIndex++;
            //    }
            //    else
            //    {
            //        if (isDamageOrTank) currentRankIndex++;
            //        else  currentRankIndex--;
            //    }

            //    currentRank = (AIRanking)currentRankIndex;

            //    if (AIAssistant.IsHigherRanking(highestRank, currentRank))
            //    {
            //        highestRank = currentRank;
            //        preferredTarget = fighter;
            //    }
            //}

            return fightersDict;
        }

        private AIActionType GetActionType(AIBattleBehavior _combatAIBehavior, Fighter _currentFighter, AIBattleAction _action)
        {
            List<AIActionType> actionTypes = new List<AIActionType>();
            Ability ability = _action.selectedAbility;
            Fighter target = _action.target;
            GridBlock targetBlock = _action.targetBlock;

            //Self Interest
            bool isSelf = _currentFighter == target;

            if (ability != null && target != null)
            {
                //Refactor - take into acount buffs or debuffs or taunts/pull agro
                bool? isDamage = null;
                bool? isBuff = null;
                
                if (ability.baseAbilityAmount != 0) isDamage = (ability.baseAbilityAmount < 0);

                if (isDamage == true || isBuff == false) actionTypes.Add(AIActionType.DealDamage_Or_Debuff);
                else if (isDamage == false) actionTypes.Add(AIActionType.Heal);
            }

            if (ability == null && target == null && targetBlock != null) actionTypes.Add(AIActionType.FindBetterPosition);

            return AIAssistant.GetHighestActionType(_combatAIBehavior, actionTypes);
        }

        private AIRanking GetPositionStrengthRanking(UnitController _currentUnitTurn, Fighter _preferredTarget)
        {
            GridBlock currentBlock = _currentUnitTurn.currentBlock;
            float positionScore = 100f;

            if (currentBlock.activeAbility != null)
            {
                //Self Interest mod
                bool isBadEffect = true;
                if (isBadEffect) positionScore -= 40f;

                else positionScore += 40f;
            }

            bool isPlayer = _currentUnitTurn.unitInfo.isPlayer;
            Vector3 currentPosition = new Vector3(currentBlock.gridCoordinates.x, 0, currentBlock.gridCoordinates.z);

            GridCoordinates preferredTargetCoords = battleGridManager.GetGridBlockByFighter(_preferredTarget).gridCoordinates;
            Vector3 targetPosition = new Vector3(preferredTargetCoords.x, 0, preferredTargetCoords.z);

            //Refactor - plug in scoring 
            float distanceFromOpposingTeam = AIAssistant.GetDistance(currentPosition, battleGridManager.GetTeamCenterPoint(isPlayer, false));
            float distanceFromOwnTeam = AIAssistant.GetDistance(currentPosition, battleGridManager.GetTeamCenterPoint(isPlayer, true));
            float distanceFromPreferredTarget = AIAssistant.GetDistance(currentPosition, targetPosition);

            positionScore = Mathf.Clamp(positionScore, 0f, 100f);
            //positionScore *= WorriedAboutPositionMod (BehaviorPreset)

            if (MathAssistant.IsBetween(positionScore, 80f, 100f)) return AIRanking.Great;
            else if (MathAssistant.IsBetween(positionScore, 60f, 80f)) return AIRanking.Good;
            else if (MathAssistant.IsBetween(positionScore, 40f, 60f)) return AIRanking.Mediocre;
            else return AIRanking.Bad;
        }

        private int GetFullEnergyCost(UnitController _currentUnit, AIBattleAction _aiCombatAction)
        {
            int fullEnergyCost = 0;
            int currentEnergy = _currentUnit.GetEnergy().energyPoints;

            if (_aiCombatAction.targetBlock != null)
            {
                int energyToGetInRange = GetMovementEnergyCost(_currentUnit.currentBlock, _aiCombatAction.targetBlock);
                //print(_aiCombatAction.target.name + " energycost = " + energyToGetInRange.ToString());
                fullEnergyCost += energyToGetInRange;
            }

            if (_aiCombatAction.selectedAbility != null && _aiCombatAction.selectedAbility.energyPointsCost > 0) fullEnergyCost += _aiCombatAction.selectedAbility.energyPointsCost;

            return fullEnergyCost;
        }

        private bool IsInRange(GridBlock _currentBlock, GridBlock _targetBlock, Ability _ability)
        {
            float distance = AIAssistant.GetDistance(_currentBlock.travelDestination.position, _targetBlock.travelDestination.position);
            float attackRange = _ability.attackRange;

            if (attackRange > 0) return attackRange > distance;
            else
            {
                int gCost = Pathfinder.CalculateDistance(_currentBlock, _targetBlock);

                if (gCost == 0 || gCost == 10 || gCost == 14) return true;
                return false;
            }
        }

        private GridBlock GetTargetBlock(UnitController _unitController, UnitController _target, Ability _ability)
        {
            GridBlock currentBlock = _unitController.currentBlock;
            GridBlock targetBlock = _target.currentBlock;
            if (currentBlock == null || targetBlock == null) return null;

            if (_target != null)
            {
                Fighter tenativeTarget = _target.GetFighter();
                _unitController.GetFighter().selectedTarget = tenativeTarget;
            }

            List<GridBlock> path = pathfinder.FindOptimalPath(currentBlock, targetBlock);

            if (path == null || path.Count <= 1) return null;

            foreach (GridBlock gridBlock in path)
            {
                if (IsInRange(gridBlock, targetBlock, _ability))
                {
                    _unitController.GetFighter().selectedTarget = null;                  
                    return gridBlock;
                }
            }

            return null;
        }

        private int GetMovementEnergyCost(GridBlock _blockA, GridBlock _blockB)
        {
            if (_blockA == null || _blockB == null) return int.MaxValue;
            List<GridBlock> path = pathfinder.FindOptimalPath(_blockA, _blockB);

            return (path.Count -1 ) * BattleHandler.energyCostPerBlock;
        }

        private AIBattleBehavior GetBehaviorPreset(AIBattleType _aiType)
        {
            AIBattleBehavior behaviorPreset = new AIBattleBehavior();

            foreach (AIBattleBehavior aiBehavior in presetBehaviors)
            {
                if(aiBehavior.aiType == _aiType)
                {
                    behaviorPreset = aiBehavior;
                    break;
                }
            }

            return behaviorPreset;
        }

        private AIBattleAction GetLastResortAction()
        {
            print("getting last resort");
            //Am I in a decent position? if not find a better one

            //An empty AICombatAction will signify an end of turn
            return new AIBattleAction();
        }

        private int GetPreferredDistance(UnitController _currentUnit, List<Ability> _usableAbilities)
        {
            AIBattleType combatAIType = _currentUnit.aiType;
            bool wantsToBeClose = (combatAIType == AIBattleType.mDamage || combatAIType == AIBattleType.Tank);

            if (!wantsToBeClose)
            {
                //Should units have own "Attack Range" or should individual abilities contain that data;
                return 6;
            }
            else return 1;
        }

        private List<Ability> GetUsableAbilities(Fighter _fighter, List<Ability> _abilities)
        {
            List<Ability> usableAbilities = new List<Ability>();

            foreach (Ability ability in _abilities)
            {
                if (_fighter.GetEnergy().energyPoints >= ability.energyPointsCost) usableAbilities.Add(ability);
            }

            return usableAbilities;
        }     

        private string BattleActionToString(AIBattleAction _aiCombatAction, float _score)
        {
            Fighter target = _aiCombatAction.target;
            GridBlock blockToMoveTo = _aiCombatAction.targetBlock;
            Ability abilityToUse = _aiCombatAction.selectedAbility;

            string targetName = "null";
            string blockCoordinates = "null";
            string abilityName = "null";

            if (target != null) targetName = target.name;
            if (blockToMoveTo != null)
            {
                GridCoordinates coords = blockToMoveTo.gridCoordinates;
                blockCoordinates = ("(" + coords.x.ToString() + "," + coords.z.ToString() + ")");
            }
            if (abilityToUse != null) abilityName = abilityToUse.abilityName;

            return "Target = " + targetName + ", Coords = " + blockCoordinates + ", Ability = " + abilityName + ", Score = " + _score.ToString();
        }
    }
}
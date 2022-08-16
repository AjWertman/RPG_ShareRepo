using RPGProject.Combat;
using RPGProject.Combat.AI;
using RPGProject.Combat.Grid;
using RPGProject.Core;
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

        public List<AIBattleAction> GetViableActions(UnitController _currentUnitTurn, List<UnitController> _allUnits)
        {
            Fighter currentFighter = _currentUnitTurn.GetFighter();
            AIBattleType combatAIType = _currentUnitTurn.aiType;

            currentFighter.SetActionPoints(6);

            List<Ability> usableAbilities = GetUsableAbilities(currentFighter, currentFighter.GetKnownAbilities());
            //Dictionary<UnitController, AIRanking> targetPreferences = SetTargetRankings(_currentUnitTurn, _allUnits);

            int preferredDistance = GetPreferredDistance(_currentUnitTurn, usableAbilities);
            //float currentPositionScore = GetPositionStrengthRanking(_currentUnitTurn, targetPreferences);

            Dictionary<AIBattleAction, float> possibleActions = GetPossibleActions(_currentUnitTurn, usableAbilities, _allUnits);          
            List<AIBattleAction> viableActions = new List<AIBattleAction>();

            return viableActions;
        }

        //Refactor - for testing purposes. Simply gets the best action
        public AIBattleAction GetViableAction(UnitController _currentUnitTurn, List<UnitController> _allUnits)
        {
            Fighter currentFighter = _currentUnitTurn.GetFighter();
            AIBattleType combatAIType = _currentUnitTurn.aiType;

            currentFighter.SetActionPoints(6);

            List<Ability> usableAbilities = GetUsableAbilities(currentFighter, currentFighter.GetKnownAbilities());
            //Dictionary<UnitController, AIRanking> targetPreferences = SetTargetRankings(_currentUnitTurn, _allUnits);

            int preferredDistance = GetPreferredDistance(_currentUnitTurn, usableAbilities);
            //float currentPositionScore = GetPositionStrengthRanking(_currentUnitTurn, targetPreferences);

            Dictionary<AIBattleAction, float> possibleActions = GetPossibleActions(_currentUnitTurn, usableAbilities, _allUnits);

            return GetBestAction(possibleActions);
        }

        private AIBattleAction GetBestAction(Dictionary<AIBattleAction, float> _possibleActions)
        {
            AIBattleAction bestAction = new AIBattleAction();
            float highestScore = 0;

            foreach(AIBattleAction combatAction in _possibleActions.Keys)
            {
                float score = _possibleActions[combatAction];

                if (highestScore <= score)
                {
                    bestAction = combatAction;
                    highestScore = score;
                }
            }

            //PrintCombatAction(bestAction, highestScore);
            return bestAction; 
        }

        private Dictionary<AIBattleAction, float> GetPossibleActions(UnitController _currentUnitTurn,
           List<Ability> _usableAbilities, List<UnitController> _allUnits)
        {
            Dictionary<AIBattleAction, float> possibleActions = new Dictionary<AIBattleAction, float>();
            AIBattleType aiType = _currentUnitTurn.aiType;

            AIBattleBehavior battleBehavior = GetBehaviorPreset(aiType);

            foreach (Ability ability in _usableAbilities)
            {
                int currentAP = _currentUnitTurn.unitResources.actionPoints;
                int costToUseAbility = ability.actionPointsCost;

                if (currentAP < costToUseAbility) continue;

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

                        int gCost = GetGCost(_currentUnitTurn.currentBlock, targetBlock);

                        combatAction.targetBlock = targetBlock;
                    }

                    int totalAPCost = GetActionPointsCost(_currentUnitTurn, combatAction);
                    if (currentAP < totalAPCost) continue;

                    //Agro -- Used to calculate preferred target? Or what?

                    ///SCORES////////////////////////////////////////////////////////////////////////
                    AIRanking apCostRanking = AIAssistant.GetAPCostRanking(currentAP, totalAPCost);
                    score += AIAssistant.GetModifier(apCostRanking);
                  

                    //Refactor - take into account current position score vs new position score;
                    AIRanking impactRanking = AIAssistant.GetImpactRanking(combatAction, _currentUnitTurn.unitInfo.isPlayer);
                    score += AIAssistant.GetModifier(impactRanking);


                    AIActionType actionType = GetActionType(battleBehavior, _currentUnitTurn.GetFighter(), combatAction);
                    AIRanking behaviorRanking = AIAssistant.GetBehaviorRanking(GetBehaviorPreset(aiType), actionType);
                    score += AIAssistant.GetModifier(behaviorRanking);

                    //Fighter preferredTarget = GetPreferredTarget(_currentUnitTurn);
                    //AIRanking positionRanking = GetPositionStrengthRanking(_currentUnitTurn, preferredTarget)
                    //score += AIAssistant.GetModifier(positionRanking);

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
                    //print("ability - " + ability.abilityName + " / cost - " + apCostRanking.ToString() + " / impact - " + impactRanking.ToString() + " / behavior - " + behaviorRanking.ToString());

                    /////////////////////////////////////////////////////////////////////////////////

                    //score *= AIAssistant.GetAITypeModifier(aiType, combatAction);

                    //int actionPointsCost = GetActionPointsCost(_currentUnitTurn, combatAction);
                    //AIAssistant.GetAPCostRanking(_currentUnitTurn.unitResources.actionPoints, actionPointsCost);
                    //score *= 1f;

                    if (!possibleActions.ContainsKey(combatAction)) possibleActions.Add(combatAction, score);
                }
            }

            return possibleActions;
        }

        private AIActionType GetActionType(AIBattleBehavior _combatAIBehavior, Fighter _currentFighter, AIBattleAction _action)
        {
            List<AIActionType> actionTypes = new List<AIActionType>();
            Ability ability = _action.selectedAbility;
            Fighter target = _action.target;
            GridBlock targetBlock = _action.targetBlock;

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

        private int GetActionPointsCost(UnitController _currentUnit, AIBattleAction _aiCombatAction)
        {
            int actionPointsCost = 0;
            int currentAP = _currentUnit.unitResources.actionPoints;

            if (_aiCombatAction.targetBlock != null)
            {
                int gCostToGetInRange = GetGCost(_currentUnit.currentBlock, _aiCombatAction.targetBlock);
                int remainingGCost = gCostToGetInRange;

                int currentGCostAllowance = _currentUnit.unitResources.gCostMoveAllowance;
                int gCostAllowancePerAP = _currentUnit.GetMover().gCostPerAP;

                remainingGCost -= currentGCostAllowance;
                currentGCostAllowance = 0;
            }

            if (_aiCombatAction.selectedAbility != null) actionPointsCost += _aiCombatAction.selectedAbility.actionPointsCost;

            print(actionPointsCost.ToString());
            return actionPointsCost;
        }

        private bool IsInRange(GridBlock _currentBlock, GridBlock _targetBlock, Ability _ability)
        {
            float distance = AIAssistant.GetDistance(_currentBlock.travelDestination.position, _targetBlock.travelDestination.position);
            float attackRange = _ability.attackRange;

            if (attackRange > 0) return attackRange > distance;
            else
            {
                int gCost = GetGCost(_currentBlock, _targetBlock);

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

        private int GetGCost(GridBlock _blockA, GridBlock _blockB)
        {
            if (_blockA == null || _blockB == null) return int.MaxValue;
            return Pathfinder.CalculateDistance(_blockA, _blockB);
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
                if (_fighter.unitResources.actionPoints >= ability.actionPointsCost) usableAbilities.Add(ability);
            }

            return usableAbilities;
        }     

        private void PrintCombatAction(AIBattleAction _aiCombatAction, float _score)
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

            print("Target = " + targetName + ", Coords = " + blockCoordinates + ", Ability = " + abilityName + ", Score = " + _score.ToString());
        }
    }
}
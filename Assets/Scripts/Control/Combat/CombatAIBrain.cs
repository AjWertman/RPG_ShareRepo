using RPGProject.Combat;
using RPGProject.Combat.AI;
using RPGProject.Combat.Grid;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Control.Combat
{
    public class CombatAIBrain : MonoBehaviour
    {
        Pathfinder pathfinder = null;

        private void Awake()
        {
            pathfinder = GetComponentInChildren<Pathfinder>();
        }

        public List<AICombatAction> GetViableActions(UnitController _currentUnitTurn, List<UnitController> _allUnits)
        {
            Fighter currentFighter = _currentUnitTurn.GetFighter();

            //Testing
            currentFighter.unitResources.actionPoints = 6;
            //

            List<Ability> usableAbilities = GetUsableAbilities(currentFighter, currentFighter.GetKnownAbilities());
            Dictionary<UnitController, TargetPreference> targetPreferences = SetTargetPreferences(_currentUnitTurn, _allUnits);

            int preferredDistance = GetPreferredDistance(_currentUnitTurn, usableAbilities);
            float currentPositionScore = CalculateCurrentPositionScore(_currentUnitTurn, targetPreferences);

            Dictionary<AICombatAction, float> possibleActions = GetPossibleActions(_currentUnitTurn, usableAbilities, targetPreferences);
            PrintPossibleActions(possibleActions);

            List<AICombatAction> viableActions = new List<AICombatAction>();

            return viableActions;
        }

        private void PrintPossibleActions(Dictionary<AICombatAction, float> _possibleActions)
        {
            foreach (AICombatAction combatAction in _possibleActions.Keys)
            {
                Fighter target = combatAction.target;
                GridBlock blockToMoveTo = combatAction.targetBlock;
                Ability abilityToUse = combatAction.selectedAbility;

                float score = _possibleActions[combatAction];

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


                print("Target = " + targetName + ", Coords = " + blockCoordinates + ", Ability = " + abilityName + ", Score = " + score.ToString());
            }
        }

        private Dictionary<AICombatAction, float> GetPossibleActions(UnitController _currentUnitTurn,
            List<Ability> _usableAbilities, Dictionary<UnitController, TargetPreference> _targetPreferences)
        {
            Dictionary<AICombatAction, float> possibleActions = new Dictionary<AICombatAction, float>();

            CombatAIType combatAIType = _currentUnitTurn.combatAIType;

            foreach (Ability ability in _usableAbilities)
            {
                //Have calculation that takes into account stats (damage amount range);
                float baseAbilityAmount = ability.baseAbilityAmount;
                float attackRange = ability.attackRange;

                bool isHeal = ability.baseAbilityAmount > 0;

                foreach (UnitController unit in _targetPreferences.Keys)
                {
                    AICombatAction combatAction = new AICombatAction();
                    combatAction.target = unit.GetFighter();
                    combatAction.selectedAbility = ability;
                    float score = 0;

                    TargetPreference targetPreference = _targetPreferences[unit];
                    bool isTeammate = _currentUnitTurn.unitInfo.isPlayer == unit.unitInfo.isPlayer;
                    bool isInRange = IsInRange(_currentUnitTurn.currentBlock, unit.currentBlock, ability);

                    if (isInRange) score += 5;
                    else
                    {
                        GridBlock targetBlock = GetTargetBlock(_currentUnitTurn, unit, ability);
                        int gCost = GetGCost(_currentUnitTurn.currentBlock, targetBlock);

                        combatAction.targetBlock = targetBlock;
                        score += AIAssistant.GetScoreByGCost(gCost);
                    }

                    if (isTeammate)
                    {
                        if (isHeal)
                        {
                            score += 5;
                        }
                        else score -= 10;
                    }
                    else
                    {
                        if (isHeal) score -= 10;
                        else if (unit.GetHealth().healthPoints + baseAbilityAmount <= 0) score += 10;                    
                    }

                    score *= AIAssistant.GetPreferenceModifier(targetPreference);

                    int actionPointsCost = GetActionPointsCost(_currentUnitTurn, combatAction);
                    score *= AIAssistant.GetScoreByActionPointsCost(_currentUnitTurn.unitResources.actionPoints, actionPointsCost);
                    if (!possibleActions.ContainsKey(combatAction)) possibleActions.Add(combatAction, score);
                }
            }

            return possibleActions;
        }

        private bool IsInRange(GridBlock _currentBlock, GridBlock _targetBlock, Ability _ability)
        {
            float distance = AIAssistant.GetDistance(_currentBlock.travelDestination, _targetBlock.travelDestination);
            float attackRange = _ability.attackRange;

            if (attackRange > 0) return attackRange > distance;
            else
            {
                int gCost = GetGCost(_currentBlock, _targetBlock);

                if (gCost == 0 || gCost == 10 || gCost == 14) return true;
                return false;
            }
        }

        private float CalculateCurrentPositionScore(UnitController _currentUnitTurn, Dictionary<UnitController, TargetPreference> _targetPrefences)
        {
            GridBlock currentBlock = _currentUnitTurn.currentBlock;
            float currentPositionScore = 10;

            //Am I standing in something? is it good or bad?
            //Am I close to my preferred target and do I want to be?

            foreach (UnitController unit in _targetPrefences.Keys)
            {
                TargetPreference targetPreference = _targetPrefences[unit];
                //bool isNeighbor = Pathfinder.IsNeighborBlock(_currentUnitTurn.currentBlock, unit.currentBlock);

                bool wantsToBeClose = (unit.combatAIType == CombatAIType.mDamage || unit.combatAIType == CombatAIType.Tank);

                if (true && wantsToBeClose)
                {

                }
            }

            //if (currentBlock.status == negativeStatus) return 0;


            return currentPositionScore;
        }


        private int GetPreferredDistance(UnitController _currentUnit, List<Ability> _usableAbilities)
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

        private List<Ability> GetUsableAbilities(Fighter _fighter, List<Ability> _abilities)
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

        private Dictionary<UnitController, TargetPreference> SetTargetPreferences(UnitController _currentUnitTurn, List<UnitController> _allUnits)
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
                    tempPreferences.Add(AIAssistant.GetTargetPreferenceByHealth(healthPercentage));
                }
                else
                {
                    bool isDamageDealer = (combatAIType == CombatAIType.mDamage || combatAIType == CombatAIType.rDamage);

                    UnitAgro unitAgro = _currentUnitTurn.GetUnitAgro();
                    int unitAgroPercentage = unitAgro.GetAgroPercentage(fighter);

                    tempPreferences.Add(AIAssistant.GetTargetPreferenceByAgro(unitAgroPercentage));
                    tempPreferences.Add(AIAssistant.GetTargetPreferenceByHealth(healthPercentage));
                }

                //tempPreferences.Add(GetTargetPreferenceByDistance(currentFighter.transform, fighter.transform));

                TargetPreference targetPreference = AIAssistant.GetTargetPreferenceByAverage(tempPreferences);
                targetPrefences.Add(unit, targetPreference);
            }

            return targetPrefences;
        }

        private GridBlock GetTargetBlock(UnitController _unitController, UnitController _target, Ability _ability)
        {
            GridBlock currentBlock = _unitController.currentBlock;
            GridBlock targetBlock = _target.currentBlock;
            if (currentBlock == null || targetBlock == null) return null;

            List<GridBlock> path = pathfinder.FindPath(currentBlock,targetBlock);

            if (path == null || path.Count <= 1) return null;

            foreach (GridBlock gridBlock in path)
            {
                if (IsInRange(gridBlock, targetBlock, _ability)) return gridBlock;
            }

            return null;
        }

        private int GetActionPointsCost(UnitController _currentUnit, AICombatAction _aiCombatAction)
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

            return actionPointsCost;
        }

        private int GetGCost(GridBlock _blockA, GridBlock _blockB)
        {
            if (_blockA == null || _blockB == null) return int.MaxValue;
            return Pathfinder.CalculateDistance(_blockA, _blockB);
        }

        //public void PlanNextMove(List<Fighter> _allFighters)
        //{
        //    //Fighter randomTarget = GetRandomTarget();
        //    //if (randomTarget == null) return;

        //    //Health targetHealth = randomTarget.GetHealthComponent();

        //    //How many AP to spend to get in attack range? 
        //    //Is target below X percentage of health? Can I kill them on this turn? 
        //    //What is the best move for me to use on my target? Would my best moves be overkill?
        //    //Do I know my targets strengths/weaknesses? Can I exploit them?

        //    //Whats my role in combat? 
        //    ///IDEA - create different behaviors (Tank, mDamage, rDamage, Healer/Support)
        //    ///Each has a percentage of different actions (based on combatAIBrain.GetRandomTarget()) and randomly executes each one
        //    ///Should above list be Dynamically changing based on battle conditions?
        //    ///

        //    //Do any of my teammates need support?
        //    //Whats my health at? Should/Can I heal myself?
        //    //How can I make my/my teammates next turn easier
        //    //Whos the best teammate of mine? How can I make them even better?

        //    //If I am gonna move to X, is there anything in my way? Does it hurt or benefit me?
        //    //Am I too close/far from my enemies? Should I move somewhere else?
        //}   
    }
}
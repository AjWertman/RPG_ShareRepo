
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

//private Dictionary<UnitController, TargetPreference> SetTargetPreferences(UnitController _currentUnitTurn, List<UnitController> _allUnits)
//{
//    Dictionary<UnitController, TargetPreference> targetPrefences = new Dictionary<UnitController, TargetPreference>();

//    Fighter currentFighter = _currentUnitTurn.GetFighter();
//    AIBattleType aiType = _currentUnitTurn.aiType;

//    foreach (UnitController unit in _allUnits)
//    {
//        Fighter fighter = unit.GetFighter();
//        List<TargetPreference> tempPreferences = new List<TargetPreference>();

//        bool isTeammate = (currentFighter.unitInfo.isPlayer == fighter.unitInfo.isPlayer);
//        float healthPercentage = fighter.GetHealthComponent().healthPercentage;

//        if (isTeammate)
//        {
//            tempPreferences.Add(AIAssistant.GetTargetPreferenceByHealth(healthPercentage));
//        }
//        else
//        {
//            bool isDamageDealer = (aiType == AIBattleType.mDamage || aiType == AIBattleType.rDamage);

//            UnitAgro unitAgro = _currentUnitTurn.GetUnitAgro();
//            int unitAgroPercentage = unitAgro.GetAgroPercentage(fighter);

//            tempPreferences.Add(AIAssistant.GetTargetPreferenceByAgro(unitAgroPercentage));
//            tempPreferences.Add(AIAssistant.GetTargetPreferenceByHealth(healthPercentage));
//        }

//        //tempPreferences.Add(GetTargetPreferenceByDistance(_currentUnitTurn.currentBlock, unit.currentBlock));

//        TargetPreference targetPreference = AIAssistant.GetTargetPreferenceByAverage(tempPreferences);
//        targetPrefences.Add(unit, targetPreference);
//    }

//    return targetPrefences;
//}

//private Dictionary<UnitController, AIRanking> SetTargetRankings(UnitController _currentUnitTurn, List<UnitController> _allUnits)
//{
//    // remove target preference and get target AIRanking?
//    Dictionary<UnitController, AIRanking> targetRankings = new Dictionary<UnitController, AIRanking>();

//    Fighter currentFighter = _currentUnitTurn.GetFighter();
//    AIBattleType aiType = _currentUnitTurn.aiType;

//    foreach (UnitController unit in _allUnits)
//    {
//        targetRankings.Add(unit, AIAssistant.GetRankingByTargetStatus(unit.GetFighter()));

//        //How many moves until unit's turn - close == worse

//        //Cost to get in range
//        ///What would new block position strength be - GetPositionStrength(aiType)
//        ///Is there a better position thats still in range

//        //Agro 

//        //Behavior modifier - Whats my role? Do i prefer to help my team or hurt the enemy team?
//    }

//    return targetRankings;
//}

//private Dictionary<AIBattleAction, float> GetPossibleActions(UnitController _currentUnitTurn,
//    List<Ability> _usableAbilities, Dictionary<UnitController, TargetPreference> _targetPreferences)
//{
//    Dictionary<AIBattleAction, float> possibleActions = new Dictionary<AIBattleAction, float>();

//    AIBattleType combatAIType = _currentUnitTurn.aiType;

//    foreach (Ability ability in _usableAbilities)
//    {
//        if (_currentUnitTurn.unitResources.actionPoints < ability.actionPointsCost) continue;

//        float baseAbilityAmount = ability.baseAbilityAmount;
//        float attackRange = ability.attackRange;

//        bool isHeal = ability.baseAbilityAmount > 0;

//        foreach (UnitController unit in _targetPreferences.Keys)
//        {
//            if (unit.GetHealth().isDead) continue;
//            AIBattleAction combatAction = new AIBattleAction();
//            combatAction.target = unit.GetFighter();
//            combatAction.selectedAbility = ability;
//            float score = 0;

//            TargetPreference targetPreference = _targetPreferences[unit];
//            bool isTeammate = _currentUnitTurn.unitInfo.isPlayer == unit.unitInfo.isPlayer;
//            bool isInRange = IsInRange(_currentUnitTurn.currentBlock, unit.currentBlock, ability);

//            if (isInRange) score += 5;
//            else
//            {
//                GridBlock targetBlock = GetTargetBlock(_currentUnitTurn, unit, ability);
//                int gCost = GetGCost(_currentUnitTurn.currentBlock, targetBlock);

//                combatAction.targetBlock = targetBlock;
//                score += AIAssistant.GetScoreByGCost(gCost);
//            }

//            if (isTeammate)
//            {
//                if (isHeal) score += 5;
//                else score -= 10;
//            }
//            else
//            {
//                if (isHeal) score -= 10;
//                else if (unit.GetHealth().healthPoints + baseAbilityAmount <= 0) score += 10;                    
//            }

//            score *= AIAssistant.GetPreferenceModifier(targetPreference);
//            score *= AIAssistant.GetAITypeModifier(combatAIType, combatAction);

//            int actionPointsCost = GetActionPointsCost(_currentUnitTurn, combatAction);
//            AIAssistant.GetAPCostRanking(_currentUnitTurn.unitResources.actionPoints, actionPointsCost);
//            score *= 1f;
//            if (!possibleActions.ContainsKey(combatAction)) possibleActions.Add(combatAction, score);
//        }
//    }

//    return possibleActions;
//}


//private float GetPositionStrengthRanking(UnitController _currentUnitTurn, Dictionary<UnitController, AIRanking> _aiRankings)
//{
//    GridBlock currentBlock = _currentUnitTurn.currentBlock;
//    float currentPositionScore = 10;

//    //Am I standing in something? is it good or bad?
//    //Am I close to my preferred target and do I want to be?

//    foreach (UnitController unit in _aiRankings.Keys)
//    {
//        AIRanking targetPreference = _aiRankings[unit];
//        //bool isNeighbor = Pathfinder.IsNeighborBlock(_currentUnitTurn.currentBlock, unit.currentBlock);

//        bool wantsToBeClose = (unit.aiType == AIBattleType.mDamage || unit.aiType == AIBattleType.Tank);

//        if (wantsToBeClose)
//        {

//        }
//    }

//    //if (currentBlock.status == negativeStatus) return 0;

//    return currentPositionScore;
//}
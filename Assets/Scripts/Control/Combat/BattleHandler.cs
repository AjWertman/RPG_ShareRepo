using RPGProject.Combat;
using RPGProject.Combat.Grid;
using RPGProject.Core;
using RPGProject.GameResources;
using RPGProject.Questing;
using RPGProject.Sound;
using RPGProject.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Control.Combat
{
    public class BattleHandler : MonoBehaviour
    {
        CombatAIBrain aiBrain = null;
        BattleUIManager battleUIManager = null;
        BattleGridManager battleGridManager = null;
        UnitManager unitManager = null;
        TurnManager turnManager = null;

        PlayerTeamManager playerTeamManager = null;
        GridSystem gridSystem = null;
        Pathfinder pathfinder = null;

        GridCoordinates playerZeroCoordinates;
        GridCoordinates enemyZeroCoordinates;

        bool isBattling = false;

        public event Action<UnitController> onUnitTurnUpdate;
        public event Action onPlayerMoveCompletion;

        private void Awake()
        {
            aiBrain = GetComponent<CombatAIBrain>();
            battleUIManager = GetComponentInChildren<BattleUIManager>(false);
            battleGridManager = GetComponentInChildren<BattleGridManager>();
            unitManager = GetComponentInChildren<UnitManager>();
            turnManager = GetComponentInChildren<TurnManager>();

            playerTeamManager = FindObjectOfType<PlayerTeamManager>();
            gridSystem = GetComponentInChildren<GridSystem>();
            pathfinder = GetComponentInChildren<Pathfinder>();

            battleUIManager.InitalizeBattleUIManager();
            battleGridManager.InitializeBattleGridManager();
            unitManager.InitalizeUnitManager();
        }

        private void Update()
        {
            //Refactor - testing
            if (Input.GetKeyDown(KeyCode.L))
            {
                aiBrain.GetViableActions(unitManager.enemyUnits[1], unitManager.unitControllers);
            }
        }

        public IEnumerator StartBattle(PlayerTeamManager _playerTeamManager, UnitStartingPosition[] _enemyTeam)
        {
            isBattleOver = false;

            playerTeamManager = _playerTeamManager;        
            SetupManagers(_enemyTeam);

            SetCurrentUnitTurn(turnManager.GetFirstMoveUnit());

            //musicOverride.OverrideMusic();

            isBattling = true;
            StartCoroutine(ExecuteNextTurn());

            yield return null;
        }

        private void SetupManagers(UnitStartingPosition[] _enemyTeam)
        {            
            battleGridManager.SetUpBattleGridManager(playerTeamManager.playerStartingPositions, _enemyTeam);
            SetupUnitManager();
            SetupTurnManager();
            SetupUIManager();
        }
        public void SetupUnitManager()
        {
            Dictionary<GridBlock, Unit> playerStartingPositions = battleGridManager.playerStartingPositionsDict;
            Dictionary<GridBlock, Unit> enemyStartingPositions = battleGridManager.enemyStartingPositionsDict;

            unitManager.SetUpUnits(playerStartingPositions, enemyStartingPositions);

            unitManager.onMoveCompletion += OnMoveCompletion;
            unitManager.onTeamWipe += EndBattle;
            unitManager.onUnitDeath += OnUnitDeath;
        }
        public void SetupTurnManager()
        {
            turnManager.SetUpTurns(unitManager.unitControllers, unitManager.playerUnits, unitManager.enemyUnits);
            turnManager.onTurnChange += SetCurrentUnitTurn;
        }
        public void SetupUIManager()
        {
            List<Fighter> _playerCombatants = GetUnitFighters(unitManager.playerUnits);
            List<Fighter> _enemyCombatants = GetUnitFighters(unitManager.enemyUnits);
            List<Fighter> _combatantTurnOrder = GetUnitFighters(turnManager.turnOrder);
            battleUIManager.SetupUIManager(_playerCombatants, _enemyCombatants, _combatantTurnOrder);
            battleUIManager.SetUILookAts(Camera.main.transform);
            battleUIManager.onEscape += Escape;
            battleUIManager.onEndTurn += () => AdvanceTurn();
        }

        public BattleUIManager GetBattleUIManager()
        {
            return battleUIManager;
        }

        public void OnPlayerMove(List<CombatTarget> _targets, Ability _selectedAbility)
        {
            battleUIManager.DeactivateAllMenus();

            CombatTarget _target = _targets[0];

            string cantUseAbilityReason = CombatAssistant.CanUseAbilityCheck(currentUnitTurn.GetFighter(), _target, _selectedAbility);

            if (cantUseAbilityReason == "")
            {
                UseAbility(_targets, _selectedAbility);
            }
            else
            {
                StartCoroutine(battleUIManager.GetBattleHUD().ActivateCantUseAbilityUI(cantUseAbilityReason));
                battleUIManager.ActivateBattleUIMenu(BattleUIMenuKey.PlayerMoveSelect);
                OnMoveCompletion();
            }
        }

        public void UseAbility(List<CombatTarget> _targets, Ability _selectedAbility)
        {
            if (_selectedAbility.canTargetAll)
            {
                TargetAll(_selectedAbility);
                return;
            }

            currentAttack = currentUnitTurn.UseAbilityBehavior(_targets, _selectedAbility);
            StartCoroutine(currentAttack);
        }

        private void OnMoveCompletion()
        {
            //Refactor - likely being called twice or paired with something else to cause skipping moves
            bool isPlayer = currentUnitTurn.unitInfo.isPlayer;
            float currentEnergy = currentUnitTurn.GetEnergy().energyPoints;

            if (isPlayer)
            {
                if(currentEnergy <= 0)
                {
                    AdvanceTurn();
                }
                else
                {
                    onPlayerMoveCompletion();
                }
            }
            else
            {  
                //Refactor - Enemies only get one move (FIX THAT)
                AdvanceTurn();
            }
        }

        public void AdvanceTurn()
        {
            if (isBattleOver) return;

            if (turnManager.currentUnitTurn != null) turnManager.currentUnitTurn.GetUnitUI().ActivateUnitIndicator(false);

            turnManager.AdvanceTurn();

            if (isBattling)
            {
                StartCoroutine(ExecuteNextTurn());
            }
        }

        public IEnumerator ExecuteNextTurn()
        {
            //if (isTutorial) return;
            yield return new WaitForSeconds(1f);
            turnManager.currentUnitTurn.GetUnitUI().ActivateUnitIndicator(true);
            List<Fighter> unitFighters = GetUnitFighters(turnManager.turnOrder);
            battleUIManager.ExecuteNextTurn(unitFighters, currentUnitTurn.GetFighter());

            if (!turnManager.IsPlayerTurn())
            {
                AIBattleAction bestAction = aiBrain.GetViableAction(currentUnitTurn, unitManager.unitControllers);
                StartCoroutine(AIUseAbility(bestAction));
            }
        }

        public IEnumerator AIUseAbility(AIBattleAction _aiCombatAction)
        {
            yield return new WaitForSeconds(1f);

            if (!_aiCombatAction.Equals(new AIBattleAction()))
            {
                Fighter currentUnitFighter = currentUnitTurn.GetFighter();
                bool isPlayerAI = currentUnitTurn.unitInfo.isPlayer;

                GridBlock actionBlock = _aiCombatAction.targetBlock;
                Ability actionAbility = _aiCombatAction.selectedAbility;
                Fighter actionTarget = _aiCombatAction.target;

                List<GridBlock> path = new List<GridBlock>();

                if (actionTarget != null) currentUnitFighter.selectedTarget = actionTarget;

                if (actionBlock != null)
                {
                    path = pathfinder.FindOptimalPath(currentUnitTurn.currentBlock, actionBlock);
                    if (actionBlock.contestedFighter != null) path.Remove(actionBlock);

                    yield return currentUnitTurn.FollowPath(path);

                }
                if (actionAbility != null)
                {
                    currentUnitFighter.selectedAbility = actionAbility;

                    //Refactor - testing 
                    List<CombatTarget> singleCombatTarget = new List<CombatTarget>();
                    singleCombatTarget.Add((CombatTarget)actionTarget);
                    UseAbility(singleCombatTarget, actionAbility);
                }
            }
            else
            {
                AdvanceTurn();
                yield break;
            }
        }

        public void SetCurrentUnitTurn(UnitController _currentUnitTurn)
        {
            if (currentUnitTurn != null)
            {
                ApplyActiveAbilitys(currentUnitTurn.GetFighter());
                currentUnitTurn.SetIsTurn(false);
            }

            currentUnitTurn = _currentUnitTurn;

            currentUnitTurn.SetIsTurn(true);
            onUnitTurnUpdate(_currentUnitTurn);
            battleUIManager.SetCurrentCombatantTurn(currentUnitTurn.GetFighter());
        }

        public List<Fighter> GetUnitFighters(List<UnitController> _unitControllers)
        {
            List<Fighter> unitFighters = new List<Fighter>();

            foreach (UnitController unitController in _unitControllers)
            {
                unitFighters.Add(unitController.GetFighter());
            }

            return unitFighters;
        }

        /// <summary>
        /// //
        /// </summary>

        BattleManagersPool battleManagersPool = null;
        AbilityObjectPool abilityObjectPool = null;

        List<Unit> playerTeam = new List<Unit>();
        int playerTeamSize = 0;

        List<Unit> enemyTeam = new List<Unit>();
        int enemyTeamSize = 0;

        UnitController currentUnitTurn = null;
        IEnumerator currentAttack = null;

        MusicOverride musicOverride = null;

        bool isBattleOver = true;

        public event Action onBattleEnd;
        public event Action<string, UnitResources> onUnitResourcesUpdate;

        private void TargetAll(Ability _selectedAbility)
        {
            List<UnitController> targetTeam = unitManager.enemyUnits;
            if(_selectedAbility.targetingType == TargetingType.PlayersOnly)
            {
                targetTeam = unitManager.playerUnits;
            }

            currentAttack = currentUnitTurn.UseAbilityOnAllBehavior(targetTeam, _selectedAbility);
            StartCoroutine(currentAttack);
        }

        //public IEnumerator AIUseAbility()
        //{
        //    Fighter currentUnitFighter = currentUnitTurn.GetFighter();
        //    bool isPlayerAI = currentUnitTurn.unitInfo.isPlayer;

        //    Ability randomAbility = currentUnitFighter.GetRandomAbility();
        //    // Fighter randomTarget = currentUnitTurn.GetCombatAIBrain().GetRandomTarget();
        //    Fighter randomTarget = null;

        //    randomAbility = currentUnitFighter.GetBasicAttack();
        //    //if (randomAbility != null && randomTarget != null)
        //    //{
        //    //    if (CombatAssistant.IsAlreadyEffected(randomAbility.GetAbilityName(), randomTarget.GetUnitStatus()))
        //    //    {
        //    //        randomAbility = currentUnitFighter.GetBasicAttack();
        //    //    }
        //    // }
        //    Pathfinder pathfinder = GetComponentInChildren<Pathfinder>();

        //    List<GridBlock> path = pathfinder.FindPath(currentUnitTurn.currentBlock, battleGridManager.GetGridBlockByFighter(randomTarget));

        //    yield return currentUnitTurn.PathExecution(path);

        //    //UseAbility(randomTarget.GetFighter(), randomAbility);
        //    battleUIManager.ActivateUnitTurnUI(currentUnitFighter, false);
        //}

        private void EndBattle(bool? _won)
        {
            if (isBattleOver) return;
            isBattleOver = true;

            isBattling = false;

            StartCoroutine(EndBattleBehavior(_won));
        }

        private IEnumerator EndBattleBehavior(bool? _won)
        {
            StopCoroutine(currentAttack);

            if (_won == true) 
            {
                yield return FindObjectOfType<Fader>().FadeOut(Color.white, .5f);

                UpdateTeamResources(unitManager.playerUnits);

                yield return new WaitForSeconds(1f);

                //ResetManagers();

                yield return new WaitForSeconds(2f);

                //FindObjectOfType<SavingWrapper>().Save();     

                //GetComponent<MusicOverride>().ClearOverride();

                CalculateBattleRewards();

                onBattleEnd();

                yield return FindObjectOfType<Fader>().FadeIn(.5f);
            }

            else if (_won == false)
            {
                FindObjectOfType<SceneManagerScript>().LoadMainMenu();
            }
            //else if(_won == null)
            //{
            //    Refactor Win ending without reward??
            //}

            currentAttack = null;
            playerTeam.Clear();
            playerTeamSize = 0;
            enemyTeam.Clear();
            enemyTeamSize = 0;
        }

        private void CalculateBattleRewards()
        {

        }

        public List<Transform> GetTravelDestinations(List<GridBlock> _path)
        {
            List<Transform> blockTransforms = new List<Transform>();

            foreach (GridBlock gridBlock in _path)
            {
                blockTransforms.Add(gridBlock.travelDestination);
            }

            return blockTransforms;
        }

        private void UpdateTeamResources(List<UnitController> _playerUnits)
        {
            foreach (UnitController unit in _playerUnits)
            {
                UnitResources unitResources = UpdateUnitResources(unit);

                if (unit.GetHealth().isDead)
                {
                    unitResources.healthPoints = 1f;
                }

                CharacterKey characterKey = unit.unitInfo.characterKey;
                PlayerKey playerKey = CharacterKeyComparison.GetPlayerKey(characterKey);
                playerTeamManager.UpdateTeamInfo(playerKey, unitResources);
            }
        }

        private UnitResources UpdateUnitResources(UnitController _unit)
        {
            UnitResources unitResources = _unit.unitResources;

            Health unitHealth = _unit.GetHealth();
            unitResources.maxHealthPoints = unitHealth.maxHealthPoints;
            unitResources.healthPoints = unitHealth.healthPoints;
            return unitResources;
        }

        private void Escape()
        {
            StartCoroutine(EscapeBehavior());
        }

        private IEnumerator EscapeBehavior()
        {
            foreach (UnitController player in unitManager.playerUnits)
            {
                //StartCoroutine(player.GetMover().Retreat());
            }

            yield return new WaitForSeconds(1f);

            EndBattle(null);
        }

        private void ApplyActiveAbilitys(Fighter _fighter)
        {
            foreach (AbilityBehavior abilityBehavior in _fighter.unitStatus.GetActiveAbilityBehaviors())
            {
                abilityBehavior.OnTurnAdvance();
            }
        }

        private void OnUnitDeath(UnitController _unitThatCausedUpdate)
        {
            List<Fighter> playerCombatants = GetUnitFighters(unitManager.playerUnits);
            List<Fighter> enemyCombatants = GetUnitFighters(unitManager.enemyUnits);

            battleUIManager.UpdateUnitLists(playerCombatants, enemyCombatants);
            turnManager.UpdateTurnOrder(_unitThatCausedUpdate);
            List<Fighter> unitFighters = GetUnitFighters(turnManager.turnOrder);
            battleUIManager.GetBattleHUD().UpdateTurnOrderUIItems(unitFighters, currentUnitTurn.GetFighter());
        }
        private void ResetManagers()
        {
            unitManager.onMoveCompletion -= AdvanceTurn;
            unitManager.onTeamWipe -= EndBattle;
            unitManager.onUnitDeath -= OnUnitDeath;

            battleUIManager.onEscape -= Escape;

            turnManager.onTurnChange -= SetCurrentUnitTurn;

            unitManager = null;
            battleUIManager = null;
            turnManager = null;

            battleManagersPool.ResetManagersPool();
            abilityObjectPool.ResetAbilityObjectPool();
        }

        public UnitController GetRandomTarget(bool _isPlayer, TargetingType _targetingType)
        {         
            if (_targetingType != TargetingType.SelfOnly)
            {
                bool returnRandomPlayer = (_isPlayer && _targetingType == TargetingType.PlayersOnly) ||
                    (!_isPlayer && _targetingType == TargetingType.EnemiesOnly);

                if (returnRandomPlayer)
                {
                    return unitManager.GetRandomAlivePlayerUnit();
                }
                else
                {
                    return unitManager.GetRandomAliveEnemyUnit();
                }
            }
            else
            {
                return currentUnitTurn;
            }
        }

        public bool IsBattling()
        {
            return isBattling;
        }     
    }
}
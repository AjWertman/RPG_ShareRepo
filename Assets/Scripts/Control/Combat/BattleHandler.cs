using RPGProject.Combat;
using RPGProject.Combat.Grid;
using RPGProject.Core;
using RPGProject.GameResources;
using RPGProject.Sound;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Control.Combat
{
    public class BattleHandler : MonoBehaviour
    {
        [SerializeField] int amountOfEnergyPointsPerTurn = 40;

        public static int energyCostPerBlock = 5;

        CombatAIBrain aiBrain = null;

        AbilityManager abilityManager = null;
        BattleUIManager battleUIManager = null;
        BattleGridManager battleGridManager = null;
        UnitManager unitManager = null;
        TurnManager turnManager = null;

        BattleManagersPool battleManagersPool = null;
        AbilityObjectPool abilityObjectPool = null;

        PlayerTeamManager playerTeamManager = null;
        GridSystem gridSystem = null;
        Pathfinder pathfinder = null;

        GridCoordinates playerZeroCoordinates;
        GridCoordinates enemyZeroCoordinates;

        UnitController currentUnitTurn = null;
        IEnumerator currentAttack = null;

        MusicOverride musicOverride = null;

        bool isBattleOver = true;
        bool isBattling = false;

        Dictionary<UnitController, Fighter> combatantDict = new Dictionary<UnitController, Fighter>();

        public event Action<UnitController> onUnitTurnUpdate;
        public event Action onPlayerMoveCompletion;
        public event Action onBattleEnd;

        private void Awake()
        {
            aiBrain = GetComponent<CombatAIBrain>();

            abilityManager = GetComponentInChildren<AbilityManager>();
            battleUIManager = GetComponentInChildren<BattleUIManager>();
            battleGridManager = GetComponentInChildren<BattleGridManager>();
            unitManager = GetComponentInChildren<UnitManager>();
            turnManager = GetComponentInChildren<TurnManager>();

            playerTeamManager = FindObjectOfType<PlayerTeamManager>();
            gridSystem = GetComponentInChildren<GridSystem>();
            pathfinder = GetComponentInChildren<Pathfinder>();
        }

        private void Start()
        {
            abilityManager.InitalizeAbilityManager();
            battleUIManager.InitalizeBattleUIManager();
            battleGridManager.InitializeBattleGridManager();
            unitManager.InitalizeUnitManager();
        }

        private void Update()
        {
            //Refactor - testing
            if (!isBattling) return;
            if (Input.GetKeyDown(KeyCode.J))
            {
                UnitController testUnit = unitManager.enemyUnits[0];
                if (testUnit == null) return;
                aiBrain.GetBestAction(testUnit, unitManager.unitControllers);
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
            SetupAbilityManager();
        }

        private void SetupUnitManager()
        {
            Dictionary<GridBlock, Unit> playerStartingPositions = battleGridManager.playerStartingPositionsDict;
            Dictionary<GridBlock, Unit> enemyStartingPositions = battleGridManager.enemyStartingPositionsDict;

            unitManager.SetUpUnits(playerStartingPositions, enemyStartingPositions);

            unitManager.onMoveCompletion += OnMoveCompletion;
            unitManager.onTeamWipe += EndBattle;

            foreach(UnitController unitController in unitManager.unitControllers)
            {
                unitController.onMoveCompletion += OnMoveCompletion;
                unitController.GetHealth().onAnimDeath += OnUnitDeath;
            }
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

        public void SetupAbilityManager()
        {            
            foreach (Fighter fighter in GetUnitFighters(unitManager.unitControllers))
            {
                fighter.onAbilityUse += abilityManager.PerformAbility;

                ComboLinker comboLinker = fighter.GetComboLinker();
                comboLinker.onComboStarted += abilityManager.SetCurrentAbilityKey;
            }
        }

        public void OnPlayerMove(List<CombatTarget> _targets, Ability _selectedAbility)
        {
            CombatTarget _target = _targets[0];

            string cantUseAbilityReason = CombatAssistant.CanUseAbilityCheck(currentUnitTurn.GetFighter(), _target, _selectedAbility);

            if (cantUseAbilityReason == "")
            {
                UseAbility(_targets, _selectedAbility);
            }
            else
            {
                CantUseAbility(cantUseAbilityReason);
            }
        }

        public void CantUseAbility(string _reason)
        {
            StartCoroutine(battleUIManager.GetBattleHUD().ActivateCantUseAbilityUI(_reason));
            OnMoveCompletion();
        }

        public void UseAbility(List<CombatTarget> _targets, Ability _selectedAbility)
        {
            currentAttack = currentUnitTurn.UseAbilityBehavior(_targets, _selectedAbility);
            StartCoroutine(currentAttack);
        }

        private void OnMoveCompletion()
        {
            bool isPlayer = currentUnitTurn.unitInfo.isPlayer;
            float currentEnergy = currentUnitTurn.GetEnergy().energyPoints;

            if (currentEnergy <= 0)
            {
                AdvanceTurn();
            }
            else
            {
                if (isPlayer) onPlayerMoveCompletion();
                else
                {
                    AIBattleAction bestAction = aiBrain.GetBestAction(currentUnitTurn, unitManager.unitControllers);
                    StartCoroutine(AIUseAbility(bestAction));
                }
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
            currentUnitTurn.GetUnitUI().ActivateUnitIndicator(true);
            List<Fighter> unitFighters = GetUnitFighters(turnManager.turnOrder);
            battleUIManager.ExecuteNextTurn(unitFighters, currentUnitTurn.GetFighter());

            currentUnitTurn.GetEnergy().RestoreEnergyPoints(amountOfEnergyPointsPerTurn);

            if (!turnManager.IsPlayerTurn())
            {
                AIBattleAction bestAction = aiBrain.GetBestAction(currentUnitTurn, unitManager.unitControllers);
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

                yield return new WaitForSeconds(.5f);
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
            abilityManager.SetCurrentCombatantTurn(currentUnitTurn.GetFighter());
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

        public bool IsBattling()
        {
            return isBattling;
        }

        public BattleUIManager GetBattleUIManager()
        {
            return battleUIManager;
        }
        private void EndBattle(bool? _won)
        {
            if (isBattleOver) return;
            isBattleOver = true;

            isBattling = false;

            StartCoroutine(EndBattleBehavior(_won));
        }

        private IEnumerator EndBattleBehavior(bool? _won)
        {
            //StopCoroutine(currentAttack);

            yield return FindObjectOfType<BattleEndScreen>(true).EndDemo(_won);

            //if (_won == true) 
            //{
            //    yield return FindObjectOfType<Fader>().FadeOut(Color.white, .5f);

            //    UpdateTeamResources(unitManager.playerUnits);

            //    yield return new WaitForSeconds(1f);

            //    //ResetManagers();

            //    yield return new WaitForSeconds(2f);

            //    //FindObjectOfType<SavingWrapper>().Save();     

            //    //GetComponent<MusicOverride>().ClearOverride();

            //    CalculateBattleRewards();

            //    //onBattleEnd();

            //    yield return FindObjectOfType<Fader>().FadeIn(.5f);
            //}

            //else if (_won == false)
            //{
            //    FindObjectOfType<SceneManagerScript>().LoadMainMenu();
            //}
            ////else if(_won == null)
            ////{
            ////    Refactor Win ending without reward??
            ////}

            //currentAttack = null;
            ////playerTeam.Clear();
            ////playerTeamSize = 0;
            ////enemyTeam.Clear();
            ////enemyTeamSize = 0;
        }

        private void CalculateBattleRewards()
        {

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

        private void OnUnitDeath(Health _deadUnitHealth)
        {
            UnitController deadUnit = _deadUnitHealth.GetComponent<UnitController>();

            deadUnit.GetUnitUI().ActivateResourceSliders(false);

            bool? wonBattle = unitManager.TeamWipeCheck();
            if (wonBattle != null) EndBattle(wonBattle); 

            deadUnit.UpdateCurrentBlock(null);
            deadUnit.gameObject.SetActive(false);
            List<Fighter> playerCombatants = GetUnitFighters(unitManager.playerUnits);
            List<Fighter> enemyCombatants = GetUnitFighters(unitManager.enemyUnits);

            battleUIManager.UpdateUnitLists(playerCombatants, enemyCombatants);
            turnManager.UpdateTurnOrder(deadUnit);
            List<Fighter> unitFighters = GetUnitFighters(turnManager.turnOrder);
            battleUIManager.GetBattleHUD().UpdateTurnOrderUIItems(unitFighters, currentUnitTurn.GetFighter());
        }

        private void ResetManagers()
        {
            unitManager.onMoveCompletion -= AdvanceTurn;
            unitManager.onTeamWipe -= EndBattle;

            battleUIManager.onEscape -= Escape;

            turnManager.onTurnChange -= SetCurrentUnitTurn;

            unitManager = null;
            battleUIManager = null;
            turnManager = null;

            battleManagersPool.ResetManagersPool();
            abilityObjectPool.ResetAbilityObjectPool();
        }
    }
}
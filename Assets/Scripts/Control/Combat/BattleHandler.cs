using RPGProject.Combat;
using RPGProject.Combat.Grid;
using RPGProject.Core;
using RPGProject.GameResources;
using RPGProject.Progression;
using RPGProject.Sound;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Control.Combat
{
    /// <summary>
    /// Handles all of the battle behaviors and actions, 
    /// including all of the sub managers that handle their respective behaviors.
    /// </summary>
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
        public event Action<UnitController> onUnitDeath;
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
                unitController.onGridBlockEnter += battleGridManager.OnGridBlockEnter;
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
            List<Fighter> allFighters = GetUnitFighters(unitManager.unitControllers);

            abilityManager.SetupAbilityManager(allFighters);
            abilityManager.onCombatantAbilitySpawn += OnCombatantAbilitySpawn;

            foreach (Fighter fighter in allFighters)
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
            bool isAI = currentUnitTurn.unitInfo.isAI;
            float currentEnergy = currentUnitTurn.GetEnergy().energyPoints;

            currentUnitTurn.GetFighter().selectedAbility = null;

            if (currentEnergy <= 0)
            {
                AdvanceTurn();
            }
            else
            {
                if (!isAI) onPlayerMoveCompletion();
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

            if(currentUnitTurn.GetComponent<Turret>())
            {
                currentUnitTurn.GetComponent<Turret>().Shoot(null, true);
                yield return new WaitForSeconds(1f);
                AdvanceTurn();
                
                yield break;
            }

            if (currentUnitTurn.unitInfo.isAI)
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

                    bool isPathNullOrEmpty = (path == null || path.Count <= 0);
                    if (!isPathNullOrEmpty)
                    {
                        if (actionBlock.contestedFighter != null) path.Remove(actionBlock);

                        yield return currentUnitTurn.FollowPath(path);
                    }
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

        /// <summary>
        /// Updates the team infos for all player combatants to make health persistent outside of combat.
        /// </summary>
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
                playerTeamManager.UpdateTeamInfo(characterKey, unitResources);
            }
        }

        /// <summary>
        /// Updates the unit resources of a combatant hold the values of the health and energy components.
        /// </summary>
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
            if (_fighter.unitStatus.Equals(new UnitStatus())) return;
            foreach (AbilityBehavior abilityBehavior in _fighter.unitStatus.GetActiveAbilityBehaviors())
            {
                abilityBehavior.OnTurnAdvance();
            }
        }

        /// <summary>
        /// Handles the addition of a new unit to combat.
        /// </summary>
        private void OnNewUnit(UnitController _newUnit)
        {
            turnManager.UpdateTurnOrder(_newUnit);
            unitManager.OnUnitAdd(_newUnit);
            UpdateUIManager();
        }

        /// <summary>
        /// Handles the spawning of an ability that is considered a combatant.
        /// </summary>
        private void OnCombatantAbilitySpawn(AbilityBehavior _combatantAbiility)
        {
            UnitController unitController = _combatantAbiility.GetComponent<UnitController>();
            if (unitController == null) return;

            Fighter caster = _combatantAbiility.caster;
            Stats stats = caster.unitInfo.stats;
            unitController.InitalizeUnitController();

            GridBlock gridBlock = (GridBlock)_combatantAbiility.target;
            bool isPlayerAbility = _combatantAbiility.caster.unitInfo.isPlayer;

            UnitInfo unitInfo = new UnitInfo("Turret", CharacterKey.None, caster.unitInfo.unitLevel, isPlayerAbility, true, stats, null, null);
            UnitResources unitResources = new UnitResources();
            unitController.SetupUnitController(unitInfo, unitResources, gridBlock, unitController.GetComponent<CharacterMesh>(), false);

            gridBlock.SetContestedFighter(unitController.GetFighter());

            OnNewUnit(unitController);

            _combatantAbiility.onAbilityDeath += RemoveCombatantAbility;
        }

        private void UpdateUIManager()
        {
            List<Fighter> playerCombatants = GetUnitFighters(unitManager.playerUnits);
            List<Fighter> enemyCombatants = GetUnitFighters(unitManager.enemyUnits);
            List<Fighter> unitFighters = GetUnitFighters(turnManager.turnOrder);

            battleUIManager.UpdateUnitLists(playerCombatants, enemyCombatants);
            battleUIManager.GetBattleHUD().UpdateTurnOrderUIItems(unitFighters, currentUnitTurn.GetFighter());
        }

        private void RemoveCombatantAbility(AbilityBehavior _combatantAbiility)
        {
            OnUnitDeath(_combatantAbiility.GetComponent<Health>());
        }

        /// <summary>
        /// Handles the removal of a unit from combat due to death.
        /// </summary>
        private void OnUnitDeath(Health _deadUnitHealth)
        {
            UnitController deadUnit = _deadUnitHealth.GetComponent<UnitController>();

            bool? wonBattle = unitManager.TeamWipeCheck();
            if (wonBattle != null) EndBattle(wonBattle);

            onUnitDeath(deadUnit);
            deadUnit.StopMovement();

            deadUnit.UpdateCurrentBlock(null);
            deadUnit.gameObject.SetActive(false);

            turnManager.UpdateTurnOrder(deadUnit);
            UpdateUIManager();
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
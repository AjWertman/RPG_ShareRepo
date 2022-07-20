using RPGProject.Combat;
using RPGProject.Core;
using RPGProject.GameResources;
using RPGProject.Questing;
using RPGProject.Sound;
using RPGProject.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Control
{
    //public enum BattleState { Null, Battling }

    public class NewBattleHandlerScript : MonoBehaviour
    {
        BattlePositionManager battlePositionManager = null;
        BattleUIManager battleUIManager = null;
        BattleGridManager battleGridManager = null;
        UnitManager unitManager = null;
        TurnManager turnManager = null;

        PlayerTeamManager playerTeamManager = null;
        GridSystem gridSystem = null;

        GridCoordinates playerZeroCoordinates;
        GridCoordinates enemyZeroCoordinates;

        private void Start()
        {
            battlePositionManager = GetComponentInChildren<BattlePositionManager>();
            battleUIManager = GetComponentInChildren<BattleUIManager>();
            battleGridManager = GetComponentInChildren<BattleGridManager>();
            unitManager = GetComponentInChildren<UnitManager>();
            turnManager = GetComponentInChildren<TurnManager>();

            playerTeamManager = FindObjectOfType<PlayerTeamManager>();
            gridSystem = GetComponentInChildren<GridSystem>();


            battleUIManager.InitalizeBattleUIManager();
            unitManager.InitalizeUnitManager();
        }

        public IEnumerator StartBattle(PlayerTeamManager _playerTeamManager, UnitStartingPosition[] _enemyTeam)
        {
            isBattleOver = false;

            playerTeamManager = _playerTeamManager;        
            SetupManagers(_enemyTeam);

            //////////////////
            SetCurrentUnitTurn(turnManager.GetFirstMoveUnit());

            //musicOverride.OverrideMusic();

            battleState = BattleState.Battling;
            StartCoroutine(ExecuteNextTurn());

            yield return null;
        }

        private void SetupManagers(UnitStartingPosition[] _enemyTeam)
        {            
            battlePositionManager.SetUpBattlePositionManager(gridSystem, playerTeamManager.playerStartingPositions, _enemyTeam);
            SetupUnitManager();
            SetupTurnManager();
            SetupUIManager();
        }
        public void SetupUnitManager()
        {
            Dictionary<GridBlock, Unit> playerStartingPositions = battlePositionManager.playerStartingPositionsDict;
            Dictionary<GridBlock, Unit> enemyStartingPositions = battlePositionManager.enemyStartingPositionsDict;

            unitManager.SetUpUnits(playerStartingPositions, enemyStartingPositions);

            unitManager.onMoveCompletion += AdvanceTurn;
            unitManager.onTeamWipe += EndBattle;
            unitManager.onUnitDeath += OnUnitDeath;
        }
        public void SetupTurnManager()
        {
            turnManager.SetUpTurns(unitManager.GetAllUnits(), unitManager.GetPlayerUnits(), unitManager.GetEnemyUnits());
            turnManager.onTurnChange += SetCurrentUnitTurn;
        }
        public void SetupUIManager()
        {
            List<Fighter> _playerCombatants = GetUnitFighters(unitManager.GetPlayerUnits());
            List<Fighter> _enemyCombatants = GetUnitFighters(unitManager.GetEnemyUnits());
            List<Fighter> _combatantTurnOrder = GetUnitFighters(turnManager.GetTurnOrder());
            battleUIManager.SetupUIManager(_playerCombatants, _enemyCombatants, _combatantTurnOrder);
            battleUIManager.SetUILookAts(camTransform);
            battleUIManager.onPlayerMove += OnPlayerMove;
            battleUIManager.onEscape += Escape;
        }

        public void OnPlayerMove(Fighter _target, Ability _selectedAbility)
        {
            battleUIManager.DeactivateAllMenus();

            string cantUseAbilityReason = CombatAssistant.CanUseAbilityCheck(currentUnitTurn.GetFighter(), _target, _selectedAbility);

            if (cantUseAbilityReason == "")
            {
                UseAbility(_target, _selectedAbility);
            }
            else
            {
                StartCoroutine(battleUIManager.GetBattleHUD().ActivateCantUseAbilityUI(cantUseAbilityReason));
                battleUIManager.ActivateBattleUIMenu(BattleUIMenuKey.PlayerMoveSelect);
            }
        }

        public void UseAbility(Fighter _target, Ability _selectedAbility)
        {
            if (_selectedAbility.CanTargetAll())
            {
                TargetAll(_selectedAbility);
                return;
            }

            currentAttack = currentUnitTurn.UseAbilityBehavior(_target, _selectedAbility);
            StartCoroutine(currentAttack);
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
            battleGridManager.UpdateCurrentUnitTurn(currentUnitTurn);
            battleUIManager.SetCurrentCombatantTurn(currentUnitTurn.GetFighter());
        }

        /// <summary>
        /// ///////////
        /// </summary>

        //private Dictionary<GridCoordinates,Unit> SetupTeams(Dictionary<GridCoordinates, Unit> _enemyTeam)
        //{
        //    Dictionary<GridBlock, Unit> startingTeams = new Dictionary<GridBlock, Unit>();

        //    foreach(Unit unit in _enemyTeam.Keys)
        //    {
        //        GridCoordinates startingCoordinates = _enemyTeam[unit]
        //        startingTeams.Add
        //    }

        //    foreach (PlayableCharacter playableCharacter in _playerTeam)
        //    {
        //        PlayerKey playerKey = playableCharacter.GetPlayerKey();
        //        Unit playerUnit = playerTeamManager.GetUnit(playerKey);
        //        TeamInfo teamInfo = playerTeamManager.GetTeamInfo(playerKey);

        //        unitManager.SetupPlayerUnit(playerUnit);

        //        playerTeam.Add(playerUnit);
        //    }

        //    foreach (Unit enemy in _enemyTeam.Keys)
        //    {
        //        enemyTeam.Add(enemy);
        //    }

        //    playerTeamSize = playerTeam.Count - 1;
        //    enemyTeamSize = enemyTeam.Count - 1;

        //    return startingTeams;
        //}

        /// <summary>
        /// //
        /// </summary>
        [SerializeField] Transform camTransform = null;

        BattleManagersPool battleManagersPool = null;
        AbilityObjectPool abilityObjectPool = null;

        List<Unit> playerTeam = new List<Unit>();
        int playerTeamSize = 0;

        List<Unit> enemyTeam = new List<Unit>();
        int enemyTeamSize = 0;

        BattleState battleState = BattleState.Null;

        UnitController currentUnitTurn = null;
        IEnumerator currentAttack = null;

        MusicOverride musicOverride = null;

        bool isBattleOver = true;

        public event Action onBattleEnd;
        public event Action<string, UnitResources> onUnitResourcesUpdate;
        
        //private void Start()
        //{
        //    battleManagersPool = FindObjectOfType<BattleManagersPool>();
        //    abilityObjectPool = FindObjectOfType<AbilityObjectPool>();
        //    musicOverride = GetComponent<MusicOverride>();
        //}

        //public IEnumerator SetupBattle(PlayerTeamManager _playerTeamManager, List<Unit> _enemyTeam)
        //{
        //    battleState = BattleState.Null;
        //    isBattleOver = false;

        //    SetBattleManagers();

        //    playerTeamManager = _playerTeamManager;
        //    SetupTeams(playerTeamManager.GetPlayableCharacters(), _enemyTeam);
        //    SetupManagers();

        //    SetCurrentUnitTurn(turnManager.GetFirstMoveUnit());
            
        //    //musicOverride.OverrideMusic();

        //    battleState = BattleState.Battling;

        //    StartCoroutine(ExecuteNextTurn());

        //    yield return null;
        //}

        private void TargetAll(Ability _selectedAbility)
        {
            List<UnitController> targetTeam = unitManager.GetEnemyUnits();
            if(_selectedAbility.GetTargetingType() == TargetingType.PlayersOnly)
            {
                targetTeam = unitManager.GetPlayerUnits();
            }

            currentAttack = currentUnitTurn.UseAbilityOnAllBehavior(targetTeam, _selectedAbility);
            StartCoroutine(currentAttack);
        }

        public void AIUseAbility()
        {
            Fighter currentUnitFighter = currentUnitTurn.GetFighter();
            bool isPlayerAI = currentUnitTurn.GetUnitInfo().IsPlayer();

            Ability randomAbility = currentUnitFighter.GetRandomAbility();
            Fighter randomTarget = GetRandomTarget(isPlayerAI, randomAbility.GetTargetingType());

            if(randomAbility != null && randomTarget != null)
            {
                if (CombatAssistant.IsAlreadyEffected(randomAbility.GetAbilityName(), randomTarget.GetUnitStatus()))
                {
                    randomAbility = currentUnitFighter.GetBasicAttack();
                }
            }

            UseAbility(randomTarget, randomAbility);
            battleUIManager.ActivateUnitTurnUI(currentUnitFighter, false);
        }

        private void AdvanceTurn()
        {
            if (isBattleOver) return;

            if (turnManager.GetCurrentUnitTurn() != null) turnManager.GetCurrentUnitTurn().GetUnitUI().ActivateUnitIndicator(false);

            turnManager.AdvanceTurn();

            if (battleState == BattleState.Battling)
            {
                StartCoroutine(ExecuteNextTurn());
            }
        }

        public IEnumerator ExecuteNextTurn()
        {
            //if (isTutorial) return;
            yield return new WaitForSeconds(1f);
            turnManager.GetCurrentUnitTurn().GetUnitUI().ActivateUnitIndicator(true);
            List<Fighter> unitFighters = GetUnitFighters(turnManager.GetTurnOrder());
            battleUIManager.ExecuteNextTurn(unitFighters, currentUnitTurn.GetFighter());

            if (!turnManager.IsPlayerTurn())
            {
                AIUseAbility();
            }
        }

        public List<Fighter> GetUnitFighters(List<UnitController> _unitControllers)
        {
            List<Fighter> unitFighters = new List<Fighter>();

            foreach(UnitController unitController in _unitControllers)
            {
                unitFighters.Add(unitController.GetFighter());
            }

            return unitFighters;
        }

        private void EndBattle(bool? _won)
        {
            if (isBattleOver) return;
            isBattleOver = true;

            battleState = BattleState.Null;

            StartCoroutine(EndBattleBehavior(_won));
        }

        private IEnumerator EndBattleBehavior(bool? _won)
        {
            StopCoroutine(currentAttack);

            if (_won == true) 
            {
                yield return FindObjectOfType<Fader>().FadeOut(Color.white, .5f);

                UpdateTeamResources(unitManager.GetPlayerUnits());

                yield return new WaitForSeconds(1f);

                ResetManagers();

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

        private void UpdateTeamResources(List<UnitController> _playerUnits)
        {
            foreach (UnitController unit in _playerUnits)
            {
                UnitResources unitResources = UpdateUnitResources(unit);

                if (unit.GetHealth().IsDead())
                {
                    unitResources.SetHealthPoints(1f);
                }

                CharacterKey characterKey = unit.GetUnitInfo().GetCharacterKey();
                PlayerKey playerKey = CharacterKeyComparison.GetPlayerKey(characterKey);
                playerTeamManager.UpdateTeamInfo(playerKey, unitResources);
            }
        }

        private UnitResources UpdateUnitResources(UnitController _unit)
        {
            UnitResources unitResources = _unit.GetUnitResources();

            Health unitHealth = _unit.GetHealth();
            Mana unitMana = _unit.GetMana();
            unitResources.SetUnitResources
                (
                unitHealth.GetHealthPoints(),
                unitHealth.GetMaxHealthPoints(),
                unitMana.GetManaPoints(),
                unitMana.GetMaxManaPoints()
                );

            return unitResources;
        }

        private void Escape()
        {
            StartCoroutine(EscapeBehavior());
        }

        private IEnumerator EscapeBehavior()
        {
            foreach (UnitController player in unitManager.GetPlayerUnits())
            {
                StartCoroutine(player.GetMover().Retreat());
            }

            yield return new WaitForSeconds(1f);

            EndBattle(null);
        }


        private void ApplyActiveAbilitys(Fighter _fighter)
        {
            foreach (AbilityBehavior abilityBehavior in _fighter.GetUnitStatus().GetActiveAbilityBehaviors())
            {
                abilityBehavior.OnTurnAdvance();
            }
        }



        private void OnUnitDeath(UnitController _unitThatCausedUpdate)
        {
            List<Fighter> playerCombatants = GetUnitFighters(unitManager.GetPlayerUnits());
            List<Fighter> enemyCombatants = GetUnitFighters(unitManager.GetEnemyUnits());

            battleUIManager.UpdateUnitLists(playerCombatants, enemyCombatants);
            turnManager.UpdateTurnOrder(_unitThatCausedUpdate);
        }
        private void ResetManagers()
        {
            unitManager.onMoveCompletion -= AdvanceTurn;
            unitManager.onTeamWipe -= EndBattle;
            unitManager.onUnitDeath -= OnUnitDeath;

            battleUIManager.onPlayerMove -= OnPlayerMove;
            battleUIManager.onEscape -= Escape;

            turnManager.onTurnChange -= SetCurrentUnitTurn;

            battlePositionManager = null;
            unitManager = null;
            battleUIManager = null;
            turnManager = null;

            battleManagersPool.ResetManagersPool();
            abilityObjectPool.ResetAbilityObjectPool();
        }

        public Fighter GetRandomTarget(bool _isPlayer, TargetingType _targetingType)
        {         
            if (_targetingType != TargetingType.SelfOnly)
            {
                bool returnRandomPlayer = (_isPlayer && _targetingType == TargetingType.PlayersOnly) ||
                    (!_isPlayer && _targetingType == TargetingType.EnemiesOnly);

                if (returnRandomPlayer)
                {
                    return unitManager.GetRandomAlivePlayerUnit().GetFighter();
                }
                else
                {
                    return unitManager.GetRandomAliveEnemyUnit().GetFighter();
                }
            }
            else
            {
                return currentUnitTurn.GetFighter();
            }
        }

        private bool IsBattling()
        {
            return battleState == BattleState.Battling;
        }     
    }
}
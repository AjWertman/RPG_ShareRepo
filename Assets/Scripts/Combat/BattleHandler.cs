using RPGProject.Core;
using RPGProject.Sound;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Combat
{
    public enum BattleState { Null, Battling }

    public class BattleHandler : MonoBehaviour
    {
        [SerializeField] Transform camTransform = null;

        BattleManagersPool battleManagersPool = null;
        AbilityObjectPool abilityObjectPool = null;

        BattlePositionManager battlePositionManager = null;
        BattleUIManager battleUIManager = null;
        BattleUnitManager battleUnitManager = null;
        TurnManager turnManager = null;

        PlayerTeam playerTeamInfo = null;

        List<Unit> playerTeam = new List<Unit>();
        int playerTeamSize = 0;

        List<Unit> enemyTeam = new List<Unit>();
        int enemyTeamSize = 0;

        BattleState battleState = BattleState.Null;

        BattleUnit currentBattleUnitTurn = null;

        MusicOverride musicOverride = null;

        bool isBattleOver = true;

        public event Action onBattleEnd; 

        private void Awake()
        {
            battleManagersPool = FindObjectOfType<BattleManagersPool>();
            abilityObjectPool = FindObjectOfType<AbilityObjectPool>();
            musicOverride = GetComponent<MusicOverride>();
        }

        private void Start()
        {
            playerTeamInfo = FindObjectOfType<PlayerTeam>();
        }

        public IEnumerator SetupBattle(List<Unit> _enemyTeam)
        {
            battleState = BattleState.Null;
            isBattleOver = false;

            SetupTeams(playerTeamInfo.GetPlayerTeam(), _enemyTeam);
            SetupManagers();

            SetCurrentBattleUnitTurn(turnManager.GetFirstMoveUnit());
            
            //musicOverride.OverrideMusic();

            battleState = BattleState.Battling;

            ExecuteNextTurn();

            yield return null;
        }

        public void OnPlayerMove(BattleUnit _target, Ability _selectedAbility)
        {
            battleUIManager.DeactivateAllMenus();

            string cantUseAbilityReason = CombatAssistant.CanUseAbilityCheck(currentBattleUnitTurn, _target, _selectedAbility);

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
     
        public void UseAbility(BattleUnit _target, Ability _selectedAbility)
        {
            StartCoroutine(currentBattleUnitTurn.UseAbilityBehavior(_target, _selectedAbility));
        }

        public void AIUseAbility()
        {
            Fighter currentUnitFighter = currentBattleUnitTurn.GetFighter();
            bool isPlayerAI = currentBattleUnitTurn.GetBattleUnitInfo().IsPlayer();

            Ability randomAbility = currentBattleUnitTurn.GetRandomAbility();
            BattleUnit randomTarget = GetRandomTarget(isPlayerAI, randomAbility.GetTargetingType());

            StartCoroutine(currentBattleUnitTurn.UseAbilityBehavior(randomTarget, randomAbility));
        }

        private void AdvanceTurn()
        {
            if (isBattleOver) return;

            turnManager.AdvanceTurn();

            if (battleState == BattleState.Battling)
            {
                ExecuteNextTurn();
            }
        }

        public void ExecuteNextTurn()
        {
            //if (isTutorial) return;
            battleUIManager.ExecuteNextTurn(turnManager.GetTurnOrder(), currentBattleUnitTurn);

            if (!turnManager.IsPlayerTurn())
            {
                AIUseAbility();
            }
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
            playerTeam.Clear();
            playerTeamSize = 0;
            enemyTeam.Clear();
            enemyTeamSize = 0;

            if (_won == true)
            {
                //CalculateBattleRewards();

                yield return FindObjectOfType<Fader>().FadeOut(Color.white, .5f);

                UpdateTeamResources(battleUnitManager.GetPlayerUnits());

                yield return new WaitForSeconds(1f);

                ResetManagers();

                yield return new WaitForSeconds(2f);

                //FindObjectOfType<SavingWrapper>().Save();     

                //GetComponent<MusicOverride>().ClearOverride();

                onBattleEnd();


                yield return FindObjectOfType<Fader>().FadeIn(.5f);
            }
            else if (_won == false)
            {
                //Refactor - Handle deaths/Loss
            }
            else
            {
                //Refactor - basically win ending without reward
            }
        }

        private void UpdateTeamResources(List<BattleUnit> _playerUnits)
        {
            foreach (BattleUnit battleUnit in _playerUnits)
            {
                string unitName = battleUnit.GetBattleUnitInfo().GetUnitName();
                BattleUnitResources battleUnitResources = battleUnit.GetBattleUnitResources();

                if (battleUnit.GetHealth().IsDead())
                {
                    battleUnitResources.SetHealthPoints(1f);
                }

                playerTeamInfo.UpdateTeamInfo(unitName, battleUnitResources);
            }
        }

        private void Escape()
        {
            StartCoroutine(EscapeBehavior());
        }

        private IEnumerator EscapeBehavior()
        {
            foreach (BattleUnit player in battleUnitManager.GetPlayerUnits())
            {
                StartCoroutine(player.GetMover().Retreat());
            }

            yield return new WaitForSeconds(1f);

            EndBattle(null);
        }

        public void SetCurrentBattleUnitTurn(BattleUnit _currentBattleUnitTurn)
        {
            if(currentBattleUnitTurn != null)
            {
                currentBattleUnitTurn.SetIsTurn(false);
            }

            currentBattleUnitTurn = _currentBattleUnitTurn;
            currentBattleUnitTurn.SetIsTurn(true);
            battleUIManager.SetCurrentBattleUnitTurn(currentBattleUnitTurn);
        }

        private void SetupTeams(List<Unit> _playerTeam, List<Unit> _enemyTeam)
        {            
            foreach (Unit player in _playerTeam)
            {
                playerTeam.Add(player);
            }

            foreach (Unit enemy in _enemyTeam)
            {
                enemyTeam.Add(enemy);
            }

            playerTeamSize = playerTeam.Count - 1;
            enemyTeamSize = enemyTeam.Count - 1;
        }
        private void SetupManagers()
        {
            battleManagersPool.transform.parent = transform;
            battleManagersPool.transform.localPosition = Vector3.zero;
            battleManagersPool.transform.localEulerAngles = Vector3.zero;
            battleManagersPool.ActivateManagersPool();

            battlePositionManager = battleManagersPool.GetBattlePositionManager();
            battleUnitManager = battleManagersPool.GetBattleUnitManager();
            turnManager = battleManagersPool.GetTurnManager();
            battleUIManager = battleManagersPool.GetBattleUIManager();

            GameObject battleCamInstance = battleManagersPool.GetBattleCamInstance();
            battleCamInstance.transform.parent = camTransform;
            battleCamInstance.transform.localPosition = Vector3.zero;
            battleCamInstance.transform.localEulerAngles = Vector3.zero;

            SetupPositionManager();
            SetupUnitManager();
            SetupTurnManager();
            SetupUIManager();
        }

        public void SetupPositionManager()
        {
            battlePositionManager.SetUpBattlePositionManager(playerTeamSize, enemyTeamSize);
        }
        public void SetupUnitManager()
        {
            battleUnitManager.SetUpUnits(playerTeam, enemyTeam, battlePositionManager.GetPlayerPosList(), battlePositionManager.GetEnemyPosList());
            battleUnitManager.onMoveCompletion += AdvanceTurn;
            battleUnitManager.onTeamWipe += EndBattle;
            battleUnitManager.onUnitListUpdate += UpdateManagerLists;
        }
        public void SetupTurnManager()
        {
            turnManager.SetUpTurns(battleUnitManager.GetBattleUnits(), battleUnitManager.GetPlayerUnits(), battleUnitManager.GetEnemyUnits());
            turnManager.onTurnChange += SetCurrentBattleUnitTurn;
        }
        public void SetupUIManager()
        {
            battleUIManager.SetupUIManager(battleUnitManager.GetPlayerUnits(), battleUnitManager.GetEnemyUnits(), turnManager.GetTurnOrder());
            battleUIManager.SetUILookAts(camTransform);
            battleUIManager.onPlayerMove += OnPlayerMove;
            battleUIManager.onEscape += Escape;
        }

        private void UpdateManagerLists(BattleUnit _unitThatCausedUpdate)
        {
            List<BattleUnit> playerUnits = battleUnitManager.GetPlayerUnits();
            List<BattleUnit> enemyUnits = battleUnitManager.GetEnemyUnits();

            battleUIManager.UpdateBattleUnitLists(playerUnits, enemyUnits);
            turnManager.UpdateTurnOrder(_unitThatCausedUpdate);
        }
        private void ResetManagers()
        {
            battleUnitManager.onMoveCompletion -= AdvanceTurn;
            battleUnitManager.onTeamWipe -= EndBattle;
            battleUnitManager.onUnitListUpdate -= UpdateManagerLists;

            battleUIManager.onPlayerMove -= OnPlayerMove;
            battleUIManager.onEscape -= Escape;

            turnManager.onTurnChange -= SetCurrentBattleUnitTurn;

            battlePositionManager = null;
            battleUnitManager = null;
            battleUIManager = null;
            turnManager = null;

            battleManagersPool.ResetManagersPool();
            abilityObjectPool.ResetAbilityObjectPool();
        }

        public BattleUnit GetRandomTarget(bool _isPlayer, TargetingType _targetingType)
        {         
            if (_targetingType != TargetingType.SelfOnly)
            {
                bool returnRandomPlayer = (_isPlayer && _targetingType == TargetingType.PlayersOnly) ||
                    (!_isPlayer && _targetingType == TargetingType.EnemiesOnly);

                if (returnRandomPlayer)
                {
                    return battleUnitManager.GetRandomPlayerUnit();
                }
                else
                {
                    return battleUnitManager.GetRandomEnemyUnit();
                }
            }
            else
            {
                return currentBattleUnitTurn;
            }
        }

        private bool IsBattling()
        {
            return battleState == BattleState.Battling;
        }     
    }
}
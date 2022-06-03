using RPGProject.Combat;
using RPGProject.Core;
using RPGProject.GameResources;
using RPGProject.Sound;
using RPGProject.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Control
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

        PlayerTeam playerTeamManager = null;
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
        public event Action<string, BattleUnitResources> onUnitResourcesUpdate;
        
        private void Awake()
        {
            battleManagersPool = FindObjectOfType<BattleManagersPool>();
            abilityObjectPool = FindObjectOfType<AbilityObjectPool>();
            musicOverride = GetComponent<MusicOverride>();
        }

        public IEnumerator SetupBattle(PlayerTeam _playerTeamManager, List<Unit> _enemyTeam)
        {
            battleState = BattleState.Null;
            isBattleOver = false;

            SetBattleManagers();

            playerTeamManager = _playerTeamManager;
            SetupTeams(playerTeamManager.GetPlayableCharacters(), _enemyTeam);
            SetupManagers();

            SetCurrentBattleUnitTurn(turnManager.GetFirstMoveUnit());
            
            //musicOverride.OverrideMusic();

            battleState = BattleState.Battling;

            ExecuteNextTurn();

            yield return null;
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
            currentAttack = currentUnitTurn.UseAbilityBehavior(_target, _selectedAbility);
            StartCoroutine(currentAttack);
        }

        public void AIUseAbility()
        {
            Fighter currentUnitFighter = currentUnitTurn.GetFighter();
            bool isPlayerAI = currentUnitTurn.GetBattleUnitInfo().IsPlayer();

            Ability randomAbility = currentUnitFighter.GetRandomAbility();
            Fighter randomTarget = GetRandomTarget(isPlayerAI, randomAbility.GetTargetingType());

            UseAbility(randomTarget, randomAbility);
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
            currentAttack = null;
            playerTeam.Clear();
            playerTeamSize = 0;
            enemyTeam.Clear();
            enemyTeamSize = 0;

            if (_won == true)
            {

                yield return FindObjectOfType<Fader>().FadeOut(Color.white, .5f);

                UpdateTeamResources(battleUnitManager.GetPlayerUnits());

                yield return new WaitForSeconds(1f);

                ResetManagers();

                yield return new WaitForSeconds(2f);

                //FindObjectOfType<SavingWrapper>().Save();     

                //GetComponent<MusicOverride>().ClearOverride();

                //CalculateBattleRewards();

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

        private void UpdateTeamResources(List<UnitController> _playerUnits)
        {
            foreach (UnitController unit in _playerUnits)
            {
                BattleUnitResources unitResources = UpdateUnitResources(unit);

                if (unit.GetHealth().IsDead())
                {
                    unitResources.SetHealthPoints(1f);
                }

                CharacterKey characterKey = unit.GetBattleUnitInfo().GetCharacterKey();
                PlayerKey playerKey = CharacterKeyComparison.GetPlayerKey(characterKey);
                playerTeamManager.UpdateTeamInfo(playerKey, unitResources);
            }
        }

        private BattleUnitResources UpdateUnitResources(UnitController _unit)
        {
            BattleUnitResources unitResources = _unit.GetUnitResources();

            Health unitHealth = _unit.GetHealth();
            Mana unitMana = _unit.GetMana();
            unitResources.SetBattleUnitResources
                (
                unitHealth.GetHealthPoints(),
                unitHealth.GetMaxHealthPoints(),
                unitMana.GetManaPoints(),
                unitMana.GetMaxManaPoints()
                );

            print(unitMana.GetManaPoints() + " " + unitMana.GetMaxManaPoints());

            return unitResources;
        }

        private void Escape()
        {
            StartCoroutine(EscapeBehavior());
        }

        private IEnumerator EscapeBehavior()
        {
            foreach (UnitController player in battleUnitManager.GetPlayerUnits())
            {
                StartCoroutine(player.GetMover().Retreat());
            }

            yield return new WaitForSeconds(1f);

            EndBattle(null);
        }

        public void SetCurrentBattleUnitTurn(UnitController _currentUnitTurn)
        {
            if(currentUnitTurn != null)
            {
                currentUnitTurn.SetIsTurn(false);
            }

            currentUnitTurn = _currentUnitTurn;
            currentUnitTurn.SetIsTurn(true);
            battleUIManager.SetCurrentCombatantTurn(currentUnitTurn.GetFighter());
        }

        private void SetupTeams(List<PlayableCharacter> _playerTeam, List<Unit> _enemyTeam)
        {
            foreach (PlayableCharacter playableCharacter in _playerTeam)
            {
                PlayerKey playerKey = playableCharacter.GetPlayerKey();
                Unit playerUnit = playerTeamManager.GetUnit(playerKey);
                TeamInfo teamInfo = playerTeamManager.GetTeamInfo(playerKey);

                battleUnitManager.SetupPlayerUnit(playerUnit);

                playerTeam.Add(playerUnit);
            }

            foreach (Unit enemy in _enemyTeam)
            {
                enemyTeam.Add(enemy);
            }

            playerTeamSize = playerTeam.Count - 1;
            enemyTeamSize = enemyTeam.Count - 1;
        }
        private void SetBattleManagers()
        {
            battleManagersPool.transform.parent = transform;
            battleManagersPool.transform.localPosition = Vector3.zero;
            battleManagersPool.transform.localEulerAngles = Vector3.zero;
            battleManagersPool.ActivateManagersPool();

            battlePositionManager = battleManagersPool.GetBattlePositionManager();
            battleUnitManager = battleManagersPool.GetBattleUnitManager();
            turnManager = battleManagersPool.GetTurnManager();
            battleUIManager = battleManagersPool.GetBattleUIManager();
        }
        private void SetupManagers()
        {
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
            battleUnitManager.SetUpUnits(enemyTeam, battlePositionManager.GetPlayerPosList(), battlePositionManager.GetEnemyPosList());
            battleUnitManager.onMoveCompletion += AdvanceTurn;
            battleUnitManager.onTeamWipe += EndBattle;
            battleUnitManager.onUnitListUpdate += UpdateManagerLists;
        }
        public void SetupTurnManager()
        {
            turnManager.SetUpTurns(battleUnitManager.GetAllUnits(), battleUnitManager.GetPlayerUnits(), battleUnitManager.GetEnemyUnits());
            turnManager.onTurnChange += SetCurrentBattleUnitTurn;
        }
        public void SetupUIManager()
        {
            List<Fighter> _playerCombatants = GetUnitFighters(battleUnitManager.GetPlayerUnits());
            List<Fighter> _enemyCombatants = GetUnitFighters(battleUnitManager.GetEnemyUnits());
            List<Fighter> _combatantTurnOrder = GetUnitFighters(turnManager.GetTurnOrder());
            battleUIManager.SetupUIManager(_playerCombatants, _enemyCombatants, _combatantTurnOrder);
            battleUIManager.SetUILookAts(camTransform);
            battleUIManager.onPlayerMove += OnPlayerMove;
            battleUIManager.onEscape += Escape;
        }

        private void UpdateManagerLists(UnitController _unitThatCausedUpdate)
        {
            List<Fighter> playerCombatants = GetUnitFighters(battleUnitManager.GetPlayerUnits());
            List<Fighter> enemyCombatants = GetUnitFighters(battleUnitManager.GetEnemyUnits());

            battleUIManager.UpdateBattleUnitLists(playerCombatants, enemyCombatants);
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

        public Fighter GetRandomTarget(bool _isPlayer, TargetingType _targetingType)
        {         
            if (_targetingType != TargetingType.SelfOnly)
            {
                bool returnRandomPlayer = (_isPlayer && _targetingType == TargetingType.PlayersOnly) ||
                    (!_isPlayer && _targetingType == TargetingType.EnemiesOnly);

                if (returnRandomPlayer)
                {
                    return battleUnitManager.GetRandomPlayerUnit().GetFighter();
                }
                else
                {
                    return battleUnitManager.GetRandomEnemyUnit().GetFighter();
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
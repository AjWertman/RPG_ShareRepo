using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { Null, Battling}

public class BattleHandler : MonoBehaviour
{
    [SerializeField] GameObject deathScreenPrefab = null;

    [SerializeField] GameObject battleCamPrefab = null;
    [SerializeField] Transform camTransform = null;

    GameObject battleCamInstance = null;

    BattleManagersPool battleManagersPool = null;

    //Managers//
    BattlePositionManager battlePositionManager = null;
    BattleUIManager battleUIManager = null;
    BattleUnitManager battleUnitManager = null;
    TurnManager turnManager = null;
    //Managers//
   
    PlayerTeam playerTeamInfo = null;

    List<Unit> playerTeam = new List<Unit>();
    int playerTeamSize = 0;

    List<Unit> enemyTeam = new List<Unit>();
    int enemyTeamSize = 0;

    BattleUnit currentBattleUnitTurn = null;

    BattleState battleState = BattleState.Null;

    float playerXPAward = 0;

    bool isTutorial = false;

    public event Action onBattleSetup;
    public event Action onAdvanceTutorial;

    IEnumerator currentAction = null;

    bool isBattleOver = true;

    //Initialization///////////////////////////////////////////////////////////////////////////////////////////

    private void Awake()
    {
        battleManagersPool = FindObjectOfType<BattleManagersPool>();
    }

    private void Start()
    {
        playerTeamInfo = FindObjectOfType<PlayerTeam>();

        if (GetComponent<TutorialBattleHandler>())
        {
            isTutorial = true;
        }
    }

    public IEnumerator SetupBattle(List<Unit> _enemyTeam)
    {
        battleState = BattleState.Null;
        isBattleOver = false;
        //Create combat cam that moves to position
        battleCamInstance = Instantiate(battleCamPrefab, camTransform);

        SetupTeams(_enemyTeam);
        SetupManagers();

        battleState = BattleState.Battling;

        SetCurrentBattleUnitTurn();

        currentBattleUnitTurn.ActivateUnitIndicatorUI(true);

        battleUIManager.SetCurrentBattleUnitTurn(currentBattleUnitTurn);

        //GetComponent<MusicOverride>().OverrideMusic();

        if (isTutorial)
        {
            onBattleSetup();
        }

        yield return null;
    }

    private void SetupTeams(List<Unit> _enemyTeam)
    {
        foreach(Unit player in playerTeamInfo.GetPlayerTeam())
        {
            playerTeam.Add(player);
        }
        playerTeamSize = playerTeam.Count - 1;

        foreach (Unit enemy in _enemyTeam)
        {
            enemyTeam.Add(enemy);
        }
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
        battleUnitManager.onTeamWipe += EndBattle;
        battleUnitManager.onUnitListUpdate += UpdateManagerLists;

        battleUnitManager.HandlePlayerDeaths();
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

    private void ResetManagers()
    {
        battleManagersPool.ResetManagersPool();

        battleUnitManager.onTeamWipe -= EndBattle;
        battleUnitManager.onUnitListUpdate -= UpdateManagerLists;

        battleUIManager.onPlayerMove -= OnPlayerMove;
        battleUIManager.onEscape -= Escape;

        turnManager.onTurnChange -= SetCurrentBattleUnitTurn;

        battlePositionManager = null;
        battleUnitManager = null;
        battleUIManager = null;
        turnManager = null;
    }


    public void SetCurrentBattleUnitTurn()
    {
        currentBattleUnitTurn = turnManager.GetBattleUnitTurn();
        battleUIManager.SetCurrentBattleUnitTurn(currentBattleUnitTurn);
    }
    //BattleBehaviors///////////////////////////////////////////////////////////////////////////////////////////

    public void OnPlayerMove(BattleUnit _target, Ability _selectedAbility)
    {
        battleUIManager.DeactivateAllMenus();

        string cantUseAbilityReason = currentBattleUnitTurn.GetCantUseAbilityReason(_target, _selectedAbility);

        if (cantUseAbilityReason == "")
        {
            if (!currentBattleUnitTurn.GetFighter().HasSubstitute())
            {
                if (_selectedAbility.canTargetAll)
                {
                    currentBattleUnitTurn.GetFighter().SetAllTargets(battleUnitManager.GetEnemyUnits());
                }
                else
                {

                    currentBattleUnitTurn.GetFighter().SetTarget(_target);
                }

                currentBattleUnitTurn.GetFighter().SetAbility(_selectedAbility);

                StartCoroutine(UseAbilityBehavior(_selectedAbility));
            }
            else
            {
                Substitute activeSubstitute = currentBattleUnitTurn.GetFighter().GetActiveSubstitute();
                activeSubstitute.onSubstituteTurnEnd += AdvanceTurn;

                activeSubstitute.StartExecutingAttackBehavior(_target);
            }
        }
        else
        {
            StartCoroutine(battleUIManager.GetBattleHUD().ActivateCantUseAbilityUI(cantUseAbilityReason));
            battleUIManager.ActivateBattleUIMenu(BattleUIMenuKey.PlayerMoveSelect);
        }
           
    }

    public void UseAbility(Ability selectedAbility)
    {
        currentAction = UseAbilityBehavior(selectedAbility);
        StartCoroutine(currentAction);
    }

    private IEnumerator UseAbilityBehavior(Ability selectedAbility)
    {
        currentBattleUnitTurn.ActivateUnitIndicatorUI(false);
        if (IsBattling())
        {
            while (turnManager.IsUnitTurn())
            {
                if (currentBattleUnitTurn.GetFighter().HasTarget())
                {
                    BattleUnit target = currentBattleUnitTurn.GetFighter().GetTarget();
                    Fighter targetFighter = target.GetFighter();

                    bool targetHasSubstitute = targetFighter.HasSubstitute();

                    if (!currentBattleUnitTurn.GetFighter().IsInRange())
                    {
                        Vector3 targetPosition = Vector3.zero;
                        yield return null;
                        if (!targetHasSubstitute)
                        {
                            targetPosition = targetFighter.GetMeleeTransform().position;
                        }
                        else
                        {
                            targetPosition = targetFighter.GetActiveSubstitute().GetFighter().GetMeleeTransform().position;
                        }

                        //currentBattleUnit.GetMover().MoveTo(targetPosition);

                        Quaternion currentRotation = transform.rotation;
                        yield return currentBattleUnitTurn.GetMover().JumpToPos(targetPosition, currentRotation,true);
                    }
                    else
                    {
                        if (!selectedAbility.canTargetAll)
                        {
                            Transform lookTransform = null;
                            if (!targetHasSubstitute)
                            {
                                lookTransform = target.transform;
                            }
                            else
                            {
                                lookTransform = targetFighter.GetActiveSubstitute().transform;
                            }

                            currentBattleUnitTurn.GetFighter().LookAtTransform(lookTransform);
                        }

                        currentBattleUnitTurn.UseAbility(selectedAbility);
                        //AddToRenCopyList(selectedAbility,isRenCopy);

                        yield return new WaitForSeconds(selectedAbility.moveDuration);

                        if (!currentBattleUnitTurn.IsDead())
                        {
                            if (selectedAbility.spellType == SpellType.Melee)
                            {
                                yield return currentBattleUnitTurn.GetMover().ReturnToStart();
                            }
                            else
                            {
                                currentBattleUnitTurn.transform.rotation = currentBattleUnitTurn.GetMover().GetStartRotation();
                            }
                        }
                      
                        //currentBattleUnit.DecrementSpellLifetimes();
                        yield return new WaitForSeconds(1.5f);

                        currentBattleUnitTurn.GetFighter().ResetTarget();
                        currentBattleUnitTurn.GetFighter().ResetAbility();

                        if (isTutorial)
                        {
                            onAdvanceTutorial();
                        }
                        
                        AdvanceTurn();

                        yield break;
                    }
                }
                else
                {
                    yield break;
                }
            }
        }
    }

    public IEnumerator AIUseAbility()
    {
        Ability randomAbility = currentBattleUnitTurn.GetRandomAbility();

        if (!isTutorial)
        {
            if (randomAbility.targetingType == TargetingType.EnemiesOnly)
            {
                if (randomAbility.canTargetAll)
                {
                    currentBattleUnitTurn.GetFighter().SetAllTargets(battleUnitManager.GetPlayerUnits());
                }
                else
                {
                    currentBattleUnitTurn.GetFighter().SetTarget(battleUnitManager.GetRandomPlayerUnit());
                }
            }
            else if (randomAbility.targetingType == TargetingType.SelfOnly)
            {
                currentBattleUnitTurn.GetFighter().SetTarget(currentBattleUnitTurn);
            }

            if (!CanAICastSpell(currentBattleUnitTurn.GetFighter().GetTarget(), randomAbility))
            {
                randomAbility = currentBattleUnitTurn.GetBattleUnitInfo().GetBasicAttack();
                currentBattleUnitTurn.GetFighter().SetTarget(battleUnitManager.GetRandomPlayerUnit());
            }
        }
        else
        {
            //Tutorial stuff
        }

        currentBattleUnitTurn.GetFighter().SetAbility(randomAbility);

        yield return new WaitForSeconds(1.5f);

        StartCoroutine(UseAbilityBehavior(randomAbility));
    }

    private bool CanAICastSpell(BattleUnit target, Ability selectedAbility)
    {
        if (!currentBattleUnitTurn.HasEnoughMana(selectedAbility.manaCost))
        {
            return false;
        }

        if (selectedAbility.spellType == SpellType.Static)
        {
            StaticSpellType staticSpellType = selectedAbility.spellPrefab.GetComponent<StaticSpell>().GetStaticSpellType();

            if (staticSpellType == StaticSpellType.PhysicalReflector && target.GetFighter().GetPhysicalReflectionDamage() > 0) return false;           
            else if (staticSpellType == StaticSpellType.SpellReflector && target.GetFighter().IsReflectingSpells()) return false;           
            else if (staticSpellType == StaticSpellType.Silence && target.GetFighter().IsSilenced()) return false;            
            else if (staticSpellType == StaticSpellType.Substitute && target.GetFighter().HasSubstitute()) return false;           
        }

        return true;
    }

    private void AdvanceTurn()
    {
        if (isBattleOver) return;

        currentAction = null;
        turnManager.AdvanceTurn();
        SetCurrentBattleUnitTurn();
         
        if (battleState == BattleState.Battling)
        {
            battleUIManager.GetBattleHUD().UpdateTurnOrderUIItems(turnManager.GetTurnOrder(), turnManager.GetTurn());
            currentBattleUnitTurn.ActivateUnitIndicatorUI(true);
            ExecuteNextTurn();
        }
    }

    public void ExecuteNextTurn()
    {
        if (isTutorial) return;
        if (turnManager.IsPlayerTurn())
        {
            battleUIManager.ActivateBattleUIMenu(BattleUIMenuKey.PlayerMoveSelect);
        }
        else
        {
            StartCoroutine(AIUseAbility());
        }
    }

    //private void AddToRenCopyList(Ability abilityToTest, bool isRenCopy)
    //{
    //    if (isRenCopy)
    //    {
    //        return;
    //    }
    //    if (abilityToTest.CanBeRenCopied())
    //    {
    //        if (renCopySpellList.Count > 0)
    //        {
    //            foreach (Ability ability in renCopySpellList)
    //            {
    //                if (ability == abilityToTest)
    //                {
    //                    return;
    //                }
    //            }
    //        }

    //        renCopySpellList.Add(abilityToTest);

    //        if (renCopySpellList.Count == 5)
    //        {
    //            renCopySpellList.Remove(renCopySpellList[0]);
    //        }

    //        battleUIManager.UpdateRenCopyList(renCopySpellList);
    //    }
    //}

    private void UpdateManagerLists()
    {
        battleUIManager.UpdateBattleUnitLists
            (
            battleUnitManager.GetPlayerUnits(),
            battleUnitManager.GetDeadPlayerUnits(),
            battleUnitManager.GetEnemyUnits(),
            battleUnitManager.GetDeadEnemyUnits()
            );
    }

    private bool IsBattling()
    {
        if (battleState == BattleState.Battling)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //EndBattle///////////////////////////////////////////////////////////////////////////////////////////

    private void Escape()
    {
        StartCoroutine(EscapeBehavior());
    }

    public IEnumerator EscapeBehavior()
    {
        foreach (BattleUnit player in battleUnitManager.GetPlayerUnits())
        {
            StartCoroutine(player.GetMover().Retreat());
        }

        yield return new WaitForSeconds(1f);

        EndBattle(true);
    }

    private void EndBattle(bool won)
    {
        battleState = BattleState.Null;

        StartCoroutine(EndBattleBehavior(won));
    }

    private IEnumerator EndBattleBehavior(bool won)
    {
        isBattleOver = true;
        StopCurrentAttack();
        playerTeam.Clear();
        playerTeamSize = 0;
        enemyTeam.Clear();
        enemyTeamSize = 0;

        if (won)
        {
            CalculateXPAwards();

            //battleUIManager.DeactivateAllUI();

            yield return FindObjectOfType<Fader>().FadeOut(Color.white, .5f);

            UpdateTeamResources(GetPlayerTeam());

            yield return new WaitForSeconds(2f);

            ResetManagers();

            //Spell pool 
            //DestroyDestroyableObjects();

            Destroy(battleCamInstance);

            yield return new WaitForSeconds(2f);

            //FindObjectOfType<SavingWrapper>().Save();     

            //GetComponent<MusicOverride>().ClearOverride();

            foreach (OverworldEntity overworld in FindObjectsOfType<OverworldEntity>())
            {
                overworld.GetComponent<IOverworld>().BattleEndBehavior();
            }
 
            yield return FindObjectOfType<Fader>().FadeIn(.5f);
        }
        else
        {
            yield return FindObjectOfType<Fader>().FadeOut(Color.black, 1f);

            GameObject deathScreen = Instantiate(deathScreenPrefab);
            yield return deathScreen.GetComponent<DeathScreen>().FadeInDeathScreen();

            yield return new WaitForSeconds(1f);

            deathScreen.GetComponent<DeathScreen>().ActivateMenuOptions();
        }
    }

    private void StopCurrentAttack()
    {
        if(currentAction != null)
        {
            StopCoroutine(currentAction);
        }
    }

    private void UpdateTeamResources(List<BattleUnit> _playerUnits)
    {
        foreach (BattleUnit battleUnit in _playerUnits)
        {
            string unitName = battleUnit.GetBattleUnitInfo().GetUnitName();
            BattleUnitResources battleUnitResources = battleUnit.GetBattleUnitResources();

            if (battleUnit.IsDead())
            {
                battleUnitResources.SetHealthPoints(1f);
            }

            playerTeamInfo.UpdateTeamInfo(unitName, battleUnitResources);
        }
    }

    private void CalculateXPAwards()
    {
        foreach (BattleUnit deadEnemy in battleUnitManager.GetDeadEnemyUnits())
        {
            playerXPAward += deadEnemy.GetXPAward();
        }

        playerTeamInfo.AwardTeamXP(playerXPAward);

        playerXPAward = 0;
    }

    private List<BattleUnit> GetPlayerTeam()
    {
        List<BattleUnit> returnableList = new List<BattleUnit>();

        foreach(BattleUnit player in battleUnitManager.GetAllPlayerUnits())
        {
            returnableList.Add(player);
        }

        return returnableList;
    }

    //Tutorial???
    //public void OverrideUIManager(BattleUIManager overrideManager)
    //{
    //    battleUIManager = overrideManager;

    //    battleUIManager.SetUpBattleUI(battleUnitManager.GetPlayerUnits(), battleUnitManager.GetEnemyUnits());
    //    battleUIManager.SetupTurnOrderUI(turnManager.GetTurnOrder());
    //    battleUIManager.SetUILookAts(camTransform);
    //    battleUIManager.onPlayerMove += OnPlayerMove;
    //    battleUIManager.onEscape += Escape;
    //}

    public bool IsTutorial()
    {
        return isTutorial;
    }

    public void RelinquishTutorialControl()
    {
        isTutorial = false;
    }
}

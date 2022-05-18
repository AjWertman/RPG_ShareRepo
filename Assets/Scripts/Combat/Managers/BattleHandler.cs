using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { Null, Battling}

public class BattleHandler : MonoBehaviour
{
    [SerializeField] GameObject battleUIManagerGO = null;
    [SerializeField] GameObject battleUnitManagerGO = null;
    [SerializeField] GameObject turnManagerGO = null;

    [SerializeField] GameObject deathScreenPrefab = null;

    [SerializeField] GameObject battleCamPrefab = null;
    [SerializeField] Transform camTransform = null;

    GameObject battleCamInstance = null;

    //Managers//
    BattlePositionManager battlePositionManager = null;
    BattleUIManager battleUIManager = null;
    BattleUnitManager battleUnitManager = null;
    TurnManager turnManager = null;

    List<GameObject> managerInstances = new List<GameObject>();
    //Managers//
   
    PlayerTeam playerTeamInfo = null;

    List<Unit> playerTeam = new List<Unit>();
    int playerTeamSize = 0;

    List<Unit> enemyTeam = new List<Unit>();
    int enemyTeamSize = 0;

    BattleUnit currentBattleUnit = null;

    BattleState battleState = BattleState.Null;

    float playerXPAward = 0;

    bool isTutorial = false;

    public event Action onBattleSetup;
    public event Action onAdvanceTutorial;

    //Initialization///////////////////////////////////////////////////////////////////////////////////////////

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
        battleCamInstance = Instantiate(battleCamPrefab, camTransform);

        SpawnManagers();
        SetUpManagers();

        battleState = BattleState.Null;

        playerTeam = playerTeamInfo.GetPlayerTeam();
        enemyTeam = _enemyTeam;

        playerTeamSize = playerTeam.Count - 1;
        enemyTeamSize = enemyTeam.Count - 1;

        battlePositionManager.SetUpBattlePositions(playerTeamSize, enemyTeamSize);

        battleUnitManager.SetUpUnits(playerTeamInfo, enemyTeam, battlePositionManager.GetPlayerPosList(), battlePositionManager.GetEnemyPosList());
        battleUnitManager.onTeamWipe += EndBattle;
        battleUnitManager.onUnitListUpdate += UpdateManagerLists;

        battleUnitManager.HandlePlayerDeaths();

        turnManager.SetUpTurns(battleUnitManager.GetBattleUnits(), battleUnitManager.GetPlayerUnits(), battleUnitManager.GetEnemyUnits());
        turnManager.onTurnChange += UpdateUIManagerCurrentUnit;

        battleUIManager.SetUpBattleUI(battleUnitManager.GetPlayerUnits(), battleUnitManager.GetEnemyUnits());
        battleUIManager.SetupTurnOrderUI(turnManager.GetTurnOrder());
        battleUIManager.SetUILookAts(camTransform);
        battleUIManager.onPlayerMove += OnPlayerMove;
        battleUIManager.onEscape += Escape;

        battleState = BattleState.Battling;

        currentBattleUnit = turnManager.GetBattleUnitTurn();
        currentBattleUnit.ActivateUnitIndicatorUI(true);

        battleUIManager.SetCurrentBattleUnit(currentBattleUnit);

        //GetComponent<MusicOverride>().OverrideMusic();

        if (isTutorial)
        {
            onBattleSetup();
        }

        yield return null;
    }

    private void SpawnManagers()
    {
        managerInstances.Add(Instantiate(battleUnitManagerGO, transform));
        managerInstances.Add(Instantiate(battleUIManagerGO, transform));
        managerInstances.Add(Instantiate(turnManagerGO, transform));
    }

    private void SetUpManagers()
    {
        battlePositionManager = GetComponentInChildren<BattlePositionManager>();
        battleUIManager = GetComponentInChildren<BattleUIManager>();
        battleUnitManager = GetComponentInChildren<BattleUnitManager>();
        turnManager = GetComponentInChildren<TurnManager>();    
    }

    //BattleBehaviors///////////////////////////////////////////////////////////////////////////////////////////

    public void OnPlayerMove(BattleUnit target, Ability selectedAbility)
    {
        if (!currentBattleUnit.GetFighter().HasSubstitute())
        {
            if (CanPlayerCastSpell(target, selectedAbility))
            {
                if (selectedAbility.canTargetAll)
                {
                    currentBattleUnit.GetFighter().SetAllTargets(battleUnitManager.GetEnemyUnits());
                }
                else
                {
                    battleUIManager.DeactivateTargetSelectCanvas();
                    currentBattleUnit.GetFighter().SetTarget(target);
                }

                currentBattleUnit.GetFighter().SetAbility(selectedAbility);

                StartCoroutine(UseAbilityBehavior(selectedAbility));
            }
        }
        else
        {
            Substitute activeSubstitute = currentBattleUnit.GetFighter().GetActiveSubstitute();
            activeSubstitute.onSubstituteTurnEnd += AdvanceTurn;

            activeSubstitute.StartExecutingAttackBehavior(target);
        }     
    }
    private bool CanPlayerCastSpell(BattleUnit target, Ability selectedAbility)
    {
        if (selectedAbility.spellType == SpellType.Static)
        {
            StaticSpellType staticSpellType = selectedAbility.spellPrefab.GetComponent<StaticSpell>().GetStaticSpellType();
            print(staticSpellType);

            if (staticSpellType == StaticSpellType.PhysicalReflector && target.GetFighter().GetPhysicalReflectionDamage() > 0)
            {
                StartCoroutine(battleUIManager.CantCastSpellUI(target.GetName() + " Is Already Reflecting Physical Damage"));
                return false;
            }
            else if (staticSpellType == StaticSpellType.SpellReflector && target.GetFighter().IsReflectingSpells())
            {
                StartCoroutine(battleUIManager.CantCastSpellUI(target.GetName() + " Is Already Reflecting Spells"));
                return false;
            }
            else if (staticSpellType == StaticSpellType.Silence && target.GetFighter().IsSilenced())
            {
                StartCoroutine(battleUIManager.CantCastSpellUI(target.GetName() + " Is Already Silenced"));
                return false;
            }
            else if (staticSpellType == StaticSpellType.Substitute && target.GetFighter().HasSubstitute())
            {
                StartCoroutine(battleUIManager.CantCastSpellUI(target.GetName() + " Already Has An Active Substitute"));
                return false;
            }
        }

        return true;
    }

    private IEnumerator UseAbilityBehavior(Ability selectedAbility)
    {
        currentBattleUnit.ActivateUnitIndicatorUI(false);
        if (IsBattling())
        {
            while (turnManager.IsUnitTurn())
            {
                if (currentBattleUnit.GetFighter().HasTarget())
                {
                    BattleUnit target = currentBattleUnit.GetFighter().GetTarget();

                    bool targetHasSubstitute = target.GetFighter().HasSubstitute();

                    if (!currentBattleUnit.GetFighter().IsInRange())
                    {
                        Vector3 targetPosition = Vector3.zero;
                        yield return null;
                        if (!targetHasSubstitute)
                        {
                            targetPosition = target.GetFighter().GetMeleeTransform().position;
                        }
                        else
                        {
                            targetPosition = target.GetFighter().GetActiveSubstitute().GetFighter().GetMeleeTransform().position;
                        }

                        //currentBattleUnit.GetMover().MoveTo(targetPosition);

                        Quaternion currentRotation = transform.rotation;
                        yield return currentBattleUnit.GetMover().JumpToPos(targetPosition, currentRotation,true);
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
                                lookTransform = target.GetFighter().GetActiveSubstitute().transform;
                            }

                            currentBattleUnit.GetFighter().LookAtTransform(lookTransform);
                        }

                        currentBattleUnit.UseAbility(selectedAbility);
                        //AddToRenCopyList(selectedAbility,isRenCopy);

                        yield return new WaitForSeconds(selectedAbility.moveDuration);

                        if (!currentBattleUnit.IsDead())
                        {
                            if (selectedAbility.spellType == SpellType.Melee)
                            {
                                yield return currentBattleUnit.GetMover().ReturnToStart();
                            }
                            else
                            {
                                currentBattleUnit.transform.rotation = currentBattleUnit.GetMover().GetStartRotation();
                            }
                        }
                      
                        //currentBattleUnit.DecrementSpellLifetimes();
                        yield return new WaitForSeconds(1.5f);

                        currentBattleUnit.GetFighter().ResetTarget();
                        currentBattleUnit.GetFighter().ResetAbility();

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
        Ability randomAbility = currentBattleUnit.GetRandomAbility();

        if (!isTutorial)
        {
            if (randomAbility.targetingType == TargetingType.EnemysOnly)
            {
                if (randomAbility.canTargetAll)
                {
                    currentBattleUnit.GetFighter().SetAllTargets(battleUnitManager.GetPlayerUnits());
                }
                else
                {
                    currentBattleUnit.GetFighter().SetTarget(battleUnitManager.GetRandomPlayerUnit());
                }
            }
            else if (randomAbility.targetingType == TargetingType.SelfOnly)
            {
                currentBattleUnit.GetFighter().SetTarget(currentBattleUnit);
            }

            if (!CanAICastSpell(currentBattleUnit.GetFighter().GetTarget(), randomAbility))
            {
                randomAbility = currentBattleUnit.GetBasicAttack();
                currentBattleUnit.GetFighter().SetTarget(battleUnitManager.GetRandomPlayerUnit());
            }
        }
        else
        {
            BattleUnit kibo = battleUnitManager.GetAllPlayerUnits()[2];

            currentBattleUnit.GetFighter().SetTarget(kibo);
            randomAbility = currentBattleUnit.GetBasicAttack();
        }

        currentBattleUnit.GetFighter().SetAbility(randomAbility);

        yield return new WaitForSeconds(1.5f);

        StartCoroutine(UseAbilityBehavior(randomAbility));
    }

    private bool CanAICastSpell(BattleUnit target, Ability selectedAbility)
    {
        if (!currentBattleUnit.HasEnoughSoulWell(selectedAbility.manaCost))
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
        turnManager.AdvanceTurn();
        currentBattleUnit = turnManager.GetBattleUnitTurn();
        
        if (battleState == BattleState.Battling)
        {
            battleUIManager.RotateTurnOrder();
            currentBattleUnit.ActivateUnitIndicatorUI(true);
            ExecuteNextTurn();
        }
    }

    public void ExecuteNextTurn()
    {
        if (isTutorial) return;
        if (turnManager.IsPlayerTurn())
        {
            battleUIManager.ActivatePlayerMoveCanvas();
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
        battleUIManager.UpdateUnitLists
            (
            battleUnitManager.GetPlayerUnits(),
            battleUnitManager.GetDeadPlayerUnits(),
            battleUnitManager.GetEnemyUnits(),
            battleUnitManager.GetDeadEnemyUnits()
            );
    }

    private void UpdateUIManagerCurrentUnit()
    {
        battleUIManager.SetCurrentBattleUnit(turnManager.GetBattleUnitTurn());
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
        if (won)
        {
            CalculateXPAwards();

            battleUIManager.DeactivateAllUI();

            yield return FindObjectOfType<Fader>().FadeOut(Color.white, .5f);

            UpdateTeamInfos(GetPlayerTeam());

            yield return new WaitForSeconds(2f);

            DestroyBattleUnits();
            DestroyDestroyableObjects();
            Destroy(battleCamInstance);

            yield return new WaitForSeconds(2f);
            
            DestroyManagers();

            //FindObjectOfType<SavingWrapper>().Save();     

            GetComponent<MusicOverride>().ClearOverride();

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

    private void UpdateTeamInfos(List<BattleUnit> playerUnits)
    {
        foreach (BattleUnit battleUnit in playerUnits)
        {
            if (!battleUnit.IsDead())
            {
                playerTeamInfo.UpdateTeamInfo(battleUnit.GetName(), battleUnit.GetUnitHealth(), battleUnit.GetUnitSoulWell());
            }
            else
            {
                playerTeamInfo.UpdateTeamInfo(battleUnit.GetName(), 1f, battleUnit.GetUnitSoulWell());
            }
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

    private void DestroyDestroyableObjects()
    {
        DestroyAfterBattle[] destroyables = FindObjectsOfType<DestroyAfterBattle>();
        if (destroyables != null)
        {
            foreach (DestroyAfterBattle destroyable in destroyables)
            {
                destroyable.DestroyGameObject();
            }
        }
    }

    private void DestroyBattleUnits()
    {
        foreach (BattleUnit unit in FindObjectsOfType<BattleUnit>())
        {
            Destroy(unit.gameObject);
        }
    }

    private void DestroyManagers()
    {
        foreach (GameObject manager in managerInstances)
        {
            Destroy(manager);
        }

        managerInstances.Clear();
    }

    public void OverrideUIManager(BattleUIManager overrideManager)
    {
        battleUIManager = overrideManager;

        battleUIManager.SetUpBattleUI(battleUnitManager.GetPlayerUnits(), battleUnitManager.GetEnemyUnits());
        battleUIManager.SetupTurnOrderUI(turnManager.GetTurnOrder());
        battleUIManager.SetUILookAts(camTransform);
        battleUIManager.onPlayerMove += OnPlayerMove;
        battleUIManager.onEscape += Escape;
    }

    public bool IsTutorial()
    {
        return isTutorial;
    }

    public void RelinquishTutorialControl()
    {
        isTutorial = false;
    }
}

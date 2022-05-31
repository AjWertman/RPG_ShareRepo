using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldBattleHandlerFunctions : MonoBehaviour
{
    //public void OnPlayerMove(BattleUnit _target, Ability _selectedAbility)
    //{
    //    battleUIManager.DeactivateAllMenus();

    //    string cantUseAbilityReason = currentBattleUnitTurn.GetCantUseAbilityReason(_target, _selectedAbility);

    //    if (cantUseAbilityReason == "")
    //    {
    //        if (!currentBattleUnitTurn.GetFighter().HasSubstitute())
    //        {
    //            if (_selectedAbility.CanTargetAll())
    //            {
    //                currentBattleUnitTurn.GetFighter().SetAllTargets(battleUnitManager.GetEnemyUnits());
    //            }
    //            else
    //            {

    //                currentBattleUnitTurn.GetFighter().SetTarget(_target);
    //            }

    //            currentBattleUnitTurn.GetFighter().SetAbility(_selectedAbility);

    //            StartCoroutine(UseAbilityBehavior(_selectedAbility));
    //        }
    //        else
    //        {
    //            Substitute activeSubstitute = currentBattleUnitTurn.GetFighter().GetActiveSubstitute();
    //            activeSubstitute.onSubstituteTurnEnd += AdvanceTurn;

    //            activeSubstitute.StartExecutingAttackBehavior(_target);
    //        }
    //    }
    //    else
    //    {
    //        StartCoroutine(battleUIManager.GetBattleHUD().ActivateCantUseAbilityUI(cantUseAbilityReason));
    //        battleUIManager.ActivateBattleUIMenu(BattleUIMenuKey.PlayerMoveSelect);
    //    }

    //}
    //private IEnumerator UseAbilityBehavior(Ability _selectedAbility)
    //{
    //    currentBattleUnitTurn.ActivateUnitIndicatorUI(false);
    //    if (IsBattling())
    //    {
    //        while (turnManager.IsUnitTurn())
    //        {
    //            if (currentBattleUnitTurn.GetFighter().HasTarget())
    //            {
    //                BattleUnit target = currentBattleUnitTurn.GetFighter().GetTarget();
    //                Fighter targetFighter = target.GetFighter();

    //                bool targetHasSubstitute = targetFighter.HasSubstitute();

    //                if (!currentBattleUnitTurn.GetFighter().IsInRange())
    //                {
    //                    Vector3 targetPosition = Vector3.zero;
    //                    yield return null;
    //                    if (!targetHasSubstitute)
    //                    {
    //                        targetPosition = targetFighter.GetMeleeTransform().position;
    //                    }
    //                    else
    //                    {
    //                        targetPosition = targetFighter.GetActiveSubstitute().GetFighter().GetMeleeTransform().position;
    //                    }

    //                    //currentBattleUnit.GetMover().MoveTo(targetPosition);

    //                    Quaternion currentRotation = transform.rotation;
    //                    yield return currentBattleUnitTurn.GetMover().JumpToPos(targetPosition, currentRotation, true);
    //                }
    //                else
    //                {
    //                    if (!_selectedAbility.CanTargetAll())
    //                    {
    //                        Transform lookTransform = null;
    //                        if (!targetHasSubstitute)
    //                        {
    //                            lookTransform = target.transform;
    //                        }
    //                        else
    //                        {
    //                            lookTransform = targetFighter.GetActiveSubstitute().transform;
    //                        }

    //                        currentBattleUnitTurn.GetFighter().LookAtTransform(lookTransform);
    //                    }

    //                    currentBattleUnitTurn.UseAbility(_selectedAbility);
    //                    //AddToRenCopyList(selectedAbility,isRenCopy);

    //                    //Refactor - move duration
    //                    yield return new WaitForSeconds(1.5f);

    //                    if (!currentBattleUnitTurn.IsDead())
    //                    {
    //                        if (_selectedAbility.GetAbilityType() == AbilityType.Melee)
    //                        {
    //                            yield return currentBattleUnitTurn.GetMover().ReturnToStart();
    //                        }
    //                        else
    //                        {
    //                            currentBattleUnitTurn.transform.rotation = currentBattleUnitTurn.GetMover().GetStartRotation();
    //                        }
    //                    }

    //                    //currentBattleUnit.DecrementSpellLifetimes();
    //                    yield return new WaitForSeconds(1.5f);

    //                    currentBattleUnitTurn.GetFighter().ResetTarget();
    //                    currentBattleUnitTurn.GetFighter().ResetAbility();

    //                    if (isTutorial)
    //                    {
    //                        onAdvanceTutorial();
    //                    }

    //                    AdvanceTurn();

    //                    yield break;
    //                }
    //            }
    //            else
    //            {
    //                yield break;
    //            }
    //        }
    //    }
    //}
    //private void CalculateXPAwards()
    //{
    //    foreach (BattleUnit deadEnemy in battleUnitManager.GetDeadEnemyUnits())
    //    {
    //        playerXPAward += deadEnemy.GetXPAward();
    //    }

    //    playerTeamInfo.AwardTeamXP(playerXPAward);

    //    playerXPAward = 0;
    //}

    //public IEnumerator AIUseAbility()
    //{
    //    Ability randomAbility = currentBattleUnitTurn.GetRandomAbility();

    //    if (!isTutorial)
    //    {
    //        if (randomAbility.GetTargetingType() == TargetingType.EnemiesOnly)
    //        {
    //            if (randomAbility.CanTargetAll())
    //            {
    //                currentBattleUnitTurn.GetFighter().SetAllTargets(battleUnitManager.GetPlayerUnits());
    //            }
    //            else
    //            {
    //                currentBattleUnitTurn.GetFighter().SetTarget(battleUnitManager.GetRandomPlayerUnit());
    //            }
    //        }
    //        else if (randomAbility.GetTargetingType() == TargetingType.SelfOnly)
    //        {
    //            currentBattleUnitTurn.GetFighter().SetTarget(currentBattleUnitTurn);
    //        }

    //        if (!CanAICastSpell(currentBattleUnitTurn.GetFighter().GetTarget(), randomAbility))
    //        {
    //            randomAbility = currentBattleUnitTurn.GetBattleUnitInfo().GetBasicAttack();
    //            currentBattleUnitTurn.GetFighter().SetTarget(battleUnitManager.GetRandomPlayerUnit());
    //        }
    //    }
    //    else
    //    {
    //        //Tutorial stuff
    //    }

    //    currentBattleUnitTurn.GetFighter().SetAbility(randomAbility);

    //    yield return new WaitForSeconds(1.5f);

    //    //StartCoroutine(UseAbilityBehavior(randomAbility));
    //}


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

    ///Tutorial Stuff///////////////////////////////////////////
    //bool isTutorial = false;
    //public event Action onBattleSetup;
    //public event Action onAdvanceTutorial;
    //--Was in Start--
    //if (GetComponent<TutorialBattleHandler>())
    //{
    //    isTutorial = true;
    //}
    //--Was at bottom of setupbattle--
    //if (isTutorial)
    //{
    //  onBattleSetup();
    //}

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

    //public bool IsTutorial()
    //{
    //    return isTutorial;
    //}

    //public void RelinquishTutorialControl()
    //{
    //    isTutorial = false;
    //}
    ///Tutorial Stuff ///////////////////////////////////////////
}

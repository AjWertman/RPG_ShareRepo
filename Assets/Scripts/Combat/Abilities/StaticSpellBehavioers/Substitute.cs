using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Substitute : StaticSpell
{
    [SerializeField] GameObject smokeDeathPrefab = null;
    [SerializeField] Color subColor = Color.white;

    Animator animator = null;
    Fighter fighter = null;
    Mover mover = null;

    Ability basicAttack = null;
    
    bool isExecutingAttackBehavior = false;

    public event Action onSubstituteTurnEnd;

    public override void InitalizeSpell(BattleUnit caster, BattleUnit target)
    {
        base.InitalizeSpell(caster, target);
        SetupSubstitute(target);
        StartCoroutine(target.GetMover().Retreat());
        target.GetComponent<Fighter>().SetHasSubsitute(true);
    }

    private void SetupSubstitute(BattleUnit target)
    {
        Instantiate(base.target.GetUnitMesh(), transform);
        animator = GetComponentInChildren<Animator>();
        fighter = GetComponent<Fighter>();
        mover = GetComponent<Mover>();

        fighter.SetAnimator(animator);
        mover.SetAnimator(animator, true);

        basicAttack = target.GetBasicAttack();

        Stats stats = target.GetStats();

        fighter.UpdateAttributes(stats.GetSpecificStatLevel(StatType.Strength), 0, stats.GetSpecificStatLevel(StatType.Luck));
        GetComponent<NavMeshAgent>().stoppingDistance = target.GetComponent<NavMeshAgent>().stoppingDistance;
        mover.SetStartingTransforms();
        fighter.SetAttackRange(true);
        SetUnitSoundFX(target.GetUnitSoundFX());
    }

    public override void DestroyStaticSpell()
    {
        StartCoroutine(target.GetMover().ReturnToStart(true));

        target.GetFighter().SetActiveSubstitute(null);
        target.GetComponent<Fighter>().SetHasSubsitute(false);
        Instantiate(smokeDeathPrefab, transform.position, Quaternion.identity);
        StopCoroutine(SubstituteAttackBehavior());
        Destroy(gameObject);
    }

    private void SetUnitSoundFX(UnitSoundFX unitSoundFX)
    {
        fighter.SetUnitSoundFX(unitSoundFX);
        mover.SetUnitSoundFX(unitSoundFX);
    }

    public void StartExecutingAttackBehavior(BattleUnit _target)
    {
        isExecutingAttackBehavior = true;
        target = _target;
        fighter.SetTarget(target);
        fighter.SetAbility(basicAttack, false);
        StartCoroutine(SubstituteAttackBehavior());
    }

    public IEnumerator SubstituteAttackBehavior()
    {
        while (isExecutingAttackBehavior)
        { 
            if (!fighter.IsInRange())
            {
                yield return null;
                mover.MoveTo(target.transform.position);
            }
            else
            {
                fighter.LookAtTransform(target.transform);
                fighter.Attack();

                yield return new WaitForSeconds(basicAttack.moveDuration);

                yield return mover.ReturnToStart(false);

                fighter.ResetTarget();

                onSubstituteTurnEnd();

                isExecutingAttackBehavior = false;
            }
        }       
    }

    public Fighter GetFighter()
    {
        return fighter;
    }

    public Color GetSubColor()
    {
        return subColor;
    }
}

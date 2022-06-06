using RPGProject.Movement;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace RPGProject.Combat
{
    public class Substitute : StaticSpell
    {
        [SerializeField] GameObject smokeDeathPrefab = null;
        [SerializeField] Color subColor = Color.white;

        Animator animator = null;
        Fighter fighter = null;
        CombatMover mover = null;

        Ability basicAttack = null;

        bool isExecutingAttackBehavior = false;

        public event Action onSubstituteTurnEnd;

        public override void InitalizeSpell(Fighter _caster, Fighter _target)
        {
            base.InitalizeSpell(_caster, _target);
            SetupSubstitute(_target);
            //StartCoroutine(target.GetMover().Retreat());
            _target.GetComponent<Fighter>().SetHasSubsitute(true);
        }

        private void SetupSubstitute(Fighter _target)
        {
            Instantiate(base.target.GetCharacterMesh(), transform);
            animator = GetComponentInChildren<Animator>();
            fighter = GetComponent<Fighter>();
            mover = GetComponent<CombatMover>();

            //fighter.SetAnimator(animator);
            //mover.SetAnimator(animator, true);

            //basicAttack = target.GetBasicAttack();

            //Stats stats = target.GetStats();

            //fighter.UpdateAttributes(stats.GetSpecificStatLevel(StatType.Strength), 0, stats.GetSpecificStatLevel(StatType.Luck));
            GetComponent<NavMeshAgent>().stoppingDistance = _target.GetComponent<NavMeshAgent>().stoppingDistance;
            //mover.SetStartingTransforms();
        }

        public override void DestroyStaticSpell()
        {
            //StartCoroutine(target.GetMover().ReturnToStart(true));

            //target.GetFighter().SetActiveSubstitute(null);
            target.GetComponent<Fighter>().SetHasSubsitute(false);
            Instantiate(smokeDeathPrefab, transform.position, Quaternion.identity);
            StopCoroutine(SubstituteAttackBehavior());
        }

        public void StartExecutingAttackBehavior(Fighter _target)
        {
            isExecutingAttackBehavior = true;
            target = _target;
            //fighter.SetTarget(target);
            //fighter.SetAbility(basicAttack);
            StartCoroutine(SubstituteAttackBehavior());
        }

        public IEnumerator SubstituteAttackBehavior()
        {
            while (isExecutingAttackBehavior)
            {
                if (true)
                {
                    yield return null;
                    mover.MoveTo(target.transform.position);
                }
                else
                {
                    fighter.LookAtTarget(target.transform);
                    //fighter.Attack();

                    //Refactor - Move duration
                    yield return new WaitForSeconds(1.5f);

                    //yield return mover.ReturnToStart(false);

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
}
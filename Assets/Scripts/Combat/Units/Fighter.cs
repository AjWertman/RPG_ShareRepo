using RPGProject.GameResources;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPGProject.Combat
{
    public class Fighter : MonoBehaviour
    {
        Animator animator = null;

        CharacterMesh characterMesh = null;

        Ability selectedAbility = null;
        BattleUnit selectedTarget = null;
        //List<BattleUnit> allTargets = new List<BattleUnit>();

        AbilityObjectPool abilityObjectPool = null;

        List<AbilityBehavior> currentAbilityStatuses = new List<AbilityBehavior>();

        float meleeRange = 0f;

        float strength = 0f;
        float skill = 0f;
        float luck = 0f;

        float physicalReflectionDamage = 0f;
        bool isReflectingSpells = false;
        bool isSilenced = false;
        bool hasSubstitute = false;

        public void InitalizeFighter()
        {
            animator = GetComponent<Animator>();
            abilityObjectPool = FindObjectOfType<AbilityObjectPool>();

            //Preset to fit the agents size
            meleeRange = GetComponent<NavMeshAgent>().stoppingDistance;

            ResetFighter();
        }

        public void UpdateAttributes(float _strength, float _skill, float _luck)
        {
            strength = _strength;
            skill = _skill;
            luck = _luck;
        }

        public void SetCharacterMesh(CharacterMesh _characterMesh)
        {
            characterMesh = _characterMesh;
        }

        public void Attack(BattleUnit _selectedTarget, Ability _selectedAbility)
        {
            //Set attack range before 
            selectedTarget = _selectedTarget;
            selectedAbility = _selectedAbility;

            animator.CrossFade(selectedAbility.GetAnimatorTrigger(), .1f);
        }
        
        private void PerformAbility()
        {
            AbilityType abilityType = selectedAbility.GetAbilityType();
            bool isCriticalHit = CombatAssistant.CriticalHitCheck(luck);

            float abilityAmountWStatModifier = GetStatsModifier(selectedAbility.GetBaseAbilityAmount());
            float calculatedAmount = CombatAssistant.GetCalculatedAmount(selectedAbility.GetBaseAbilityAmount(), isCriticalHit);

            Health targetHealth = selectedTarget.GetHealth();

            switch (abilityType)
            {
                case AbilityType.Melee:

                    targetHealth.DamageHealth(calculatedAmount, isCriticalHit, true);
                    if (selectedTarget.GetFighter().GetPhysicalReflectionDamage() > 0)
                    {
                        //Do Damage to self
                    }
                    break;

                case AbilityType.Copy:

                    //onCopyAbilitySelected
                    /// Open ability menu with copy list

                    break;

                case AbilityType.Cast:

                    AbilityBehavior abilityBehavior = abilityObjectPool.GetAbilityInstance(selectedAbility);
                    abilityBehavior.SetupAbility(GetComponent<BattleUnit>(), selectedTarget, calculatedAmount, isCriticalHit);
                    abilityBehavior.gameObject.SetActive(true);
                    abilityBehavior.PerformSpellBehavior();
                    break;
            }

            //Refactor - Play soundfx
        }

        public void LookAtTarget(Transform _target)
        {
            transform.LookAt(_target);
        }

        public void ResetFighter()
        {
            ResetTarget();
            ResetAbility();
            UpdateAttributes(10f, 10f, 10f);
            physicalReflectionDamage = 0;
            isReflectingSpells = false;
            isSilenced = false;
            hasSubstitute = false;

            SetCharacterMesh(null);
        }

        public void ResetTarget()
        {
            selectedTarget = null;
            //allTargets.Clear();
        }

        public void ResetAbility()
        {
            selectedAbility = null;
        }

        public void ApplyAbilityBehaviorStatus(AbilityBehavior _abilityBehavior)
        {
            _abilityBehavior.onAbilityDeath += RemoveAbilityBehaviorStatus;

            if (currentAbilityStatuses.Contains(_abilityBehavior)) return;
            currentAbilityStatuses.Add(_abilityBehavior);
        }

        private void RemoveAbilityBehaviorStatus(AbilityBehavior _abilityBehavior)
        {
            _abilityBehavior.onAbilityDeath -= RemoveAbilityBehaviorStatus;

            if (!currentAbilityStatuses.Contains(_abilityBehavior)) return;
            currentAbilityStatuses.Remove(_abilityBehavior);
        }

        //Animation Event
        void Hit()
        {
            PerformAbility();
        }

        public bool IsInRange(AbilityType _abilityType, BattleUnit _battleUnit)
        {
            if(_abilityType == AbilityType.Melee)
            {
                float distanceToTarget = GetDistanceToTarget(_battleUnit.transform.position);
                return (distanceToTarget <= meleeRange);
            }

            return true;
        }

        private float GetDistanceToTarget(Vector3 _targetPos)
        {
            return Vector3.Distance(transform.position, _targetPos);
        }

        public bool HasTarget()
        {
            if (selectedTarget != null) return true;
            else return false;
        }

        public void SetPhysicalReflectionDamage(float _damageToSet)
        {
            physicalReflectionDamage = _damageToSet;
        }

        public float GetPhysicalReflectionDamage()
        {
            return physicalReflectionDamage;
        }

        public void SetIsReflectingSpells(bool _shouldSet)
        {
            isReflectingSpells = _shouldSet;
        }

        public bool IsReflectingSpells()
        {
            return isReflectingSpells;
        }

        public void SetIsSilenced(bool _shouldSet)
        {
            isSilenced = _shouldSet;
        }

        public bool IsSilenced()
        {
            return isSilenced;
        }

        public void SetHasSubsitute(bool _shouldSet)
        {
            hasSubstitute = _shouldSet;
        }

        public bool HasSubstitute()
        {
            return hasSubstitute;
        }

        //public void SetAllTargets(List<BattleUnit> _targets)
        //{
        //    foreach (BattleUnit target in _targets)
        //    {
        //        allTargets.Add(target);
        //    }
        //}

        private float GetStatsModifier(float _changeAmount)
        {
            float newChangeAmount = _changeAmount;
            float statsModifier = 0f;

            if (selectedAbility.GetAbilityType() == AbilityType.Melee)
            {
                statsModifier = strength - 10f;
            }
            else if (selectedAbility.GetAbilityType() == AbilityType.Cast)
            {
                statsModifier = skill - 10f;
            }

            if (statsModifier > 0)
            {
                float offensivePercentage = statsModifier * .1f;
                newChangeAmount += (_changeAmount * offensivePercentage);
            }

            return newChangeAmount;
        }
    }
}
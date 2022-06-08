﻿using RPGProject.Core;
using RPGProject.GameResources;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPGProject.Combat
{
    public class Fighter : MonoBehaviour
    {
        Animator animator = null;
        ComboLinker comboLinker = null;
        CharacterMesh characterMesh = null;
        Health health = null;
        Mana mana = null;

        UnitInfo unitInfo = new UnitInfo();
        UnitResources unitResources = new UnitResources();

        Ability selectedAbility = null;
        Fighter selectedTarget = null;
        AbilityObjectKey currentAbilityObjectKey = AbilityObjectKey.None;
        //List<Fighter> allTargets = new List<Fighter>();

        AbilityObjectPool abilityObjectPool = null;

        List<AbilityBehavior> activeAbilityBehaviors = new List<AbilityBehavior>();

        float meleeRange = 0f;

        float strength = 0f;
        float skill = 0f;
        float luck = 0f;

        float physicalReflectionDamage = 0f;
        bool isReflectingSpells = false;
        bool isSilenced = false;
        bool hasSubstitute = false;

        bool isPlayerFighter = false;

        public void InitalizeFighter()
        {
            animator = GetComponent<Animator>();
            comboLinker = GetComponent<ComboLinker>();
            comboLinker.InitializeComboLinker();
            comboLinker.onComboStarted += SetCurrentAbilityInstance;

            abilityObjectPool = FindObjectOfType<AbilityObjectPool>();
            health = GetComponent<Health>();
            mana = GetComponent<Mana>();

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

        public IEnumerator Attack(Fighter _selectedTarget, Ability _selectedAbility)
        {
            selectedTarget = _selectedTarget;
            selectedAbility = _selectedAbility;

            yield return comboLinker.ExecuteCombo(selectedAbility.GetCombo());

            //animator.CrossFade(selectedAbility.GetAnimatorTrigger(), .1f);
        }

        private void PerformAbility()
        {
            AbilityType abilityType = selectedAbility.GetAbilityType();
            bool isCriticalHit = CombatAssistant.CriticalHitCheck(luck);
            bool isHeal = selectedAbility.IsHeal();

            float abilityAmountWStatModifier = GetStatsModifier(selectedAbility.GetBaseAbilityAmount());
            float calculatedAmount = CombatAssistant.GetCalculatedAmount(selectedAbility.GetBaseAbilityAmount(), isCriticalHit);

            Health targetHealth = selectedTarget.GetHealth();

            AbilityBehavior abilityBehavior = null;

            if (currentAbilityObjectKey != AbilityObjectKey.None)
            {
                abilityBehavior = abilityObjectPool.GetAbilityInstance(currentAbilityObjectKey);
                abilityBehavior.SetupAbility(this, selectedTarget, calculatedAmount, isCriticalHit, selectedAbility.GetAbilityLifetime());
                //Set transform ?
            }

            switch (abilityType)
            {
                case AbilityType.Melee:

                    targetHealth.ChangeHealth(calculatedAmount, isCriticalHit, false);
                    float reflectionAmount = selectedTarget.GetPhysicalReflectionDamage();
                    if (reflectionAmount > 0)
                    {
                        health.ChangeHealth(reflectionAmount, false, false);
                    }
                    break;

                case AbilityType.Copy:

                    //onCopyAbilitySelected
                    /// Open ability menu with copy list

                    break;

                case AbilityType.Cast:

                    abilityBehavior.gameObject.SetActive(true);
                    abilityBehavior.PerformSpellBehavior();
                    break;

                case AbilityType.InstaHit:

                    abilityBehavior.transform.parent = selectedTarget.transform;
                    abilityBehavior.transform.localPosition = Vector3.zero;
                    abilityBehavior.gameObject.SetActive(true);
                    abilityBehavior.PerformSpellBehavior();

                    targetHealth.ChangeHealth(calculatedAmount, isCriticalHit, true);

                    break;
            }

            //Refactor - Play soundfx
        }

        public void LookAtTarget(Transform _target)
        {
            transform.LookAt(_target);
        }

        public void SetCurrentAbilityInstance(AbilityObjectKey _abilityObjectKey)
        {
            currentAbilityObjectKey = _abilityObjectKey;
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

            isPlayerFighter = false;
            unitInfo = new UnitInfo();
            unitResources = new UnitResources();

            currentAbilityObjectKey = AbilityObjectKey.None;
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

        public void ApplyActiveAbilityBehavior(AbilityBehavior _abilityBehavior)
        {
            _abilityBehavior.onAbilityDeath += RemoveActiveAbilityBehavior;

            if (activeAbilityBehaviors.Contains(_abilityBehavior)) return;
            activeAbilityBehaviors.Add(_abilityBehavior);
        }

        private void RemoveActiveAbilityBehavior(AbilityBehavior _abilityBehavior)
        {
            _abilityBehavior.onAbilityDeath -= RemoveActiveAbilityBehavior;

            if (!activeAbilityBehaviors.Contains(_abilityBehavior)) return;
            activeAbilityBehaviors.Remove(_abilityBehavior);
        }

        public IEnumerable<AbilityBehavior> GetActiveAbilityBehaviors()
        {
            List<AbilityBehavior> abilityBehaviors = new List<AbilityBehavior>();

            foreach (AbilityBehavior abilityBehavior in activeAbilityBehaviors)
            {
                abilityBehaviors.Add(abilityBehavior);
            }

            foreach (AbilityBehavior abilityBehavior in abilityBehaviors)
            {
                yield return abilityBehavior;
            }
        }

        //Animation Events
        void Hit()
        {
            PerformAbility();
        }

        void Shoot()
        {
            PerformAbility();
        }
        //////////

        public bool IsInRange(AbilityType _abilityType, Fighter _target)
        {
            if (_abilityType == AbilityType.Melee)
            {
                float distanceToTarget = GetDistanceToTarget(_target.transform.position);
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

        public CharacterMesh GetCharacterMesh()
        {
            return characterMesh;
        }

        public Health GetHealth()
        {
            return health;
        }

        public Mana GetMana()
        {
            return mana;
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

        //public void SetAllTargets(List<Fighter> _targets)
        //{
        //    foreach (Fighter fighter in _targets)
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

        public void SetUnitInfo(UnitInfo _unitInfo)
        {
            unitInfo = _unitInfo;
        }

        public void SetUnitResources(UnitResources _unitResources)
        {
            unitResources= _unitResources;
        }

        public Ability GetRandomAbility()
        {
            Ability basicAttack = unitInfo.GetBasicAttack();
            Ability[] abilities = unitInfo.GetAbilities();

            if (!IsSilenced())
            {
                if (abilities.Length == 0 || abilities == null) return basicAttack;
                int randomInt = RandomGenerator.GetRandomNumber(0, abilities.Length - 1);
                return abilities[randomInt];
            }
            else
            {
                List<Ability> physicalAbilities = new List<Ability>();
                physicalAbilities.Add(basicAttack);

                foreach (Ability ability in abilities)
                {
                    if (ability.GetAbilityType() == AbilityType.Melee)
                    {
                        physicalAbilities.Add(ability);
                    }
                }

                int randomInt = RandomGenerator.GetRandomNumber(0, physicalAbilities.Count - 1);
                return physicalAbilities[randomInt];
            }
        }

        public List<Ability> GetKnownAbilities()
        {
            List<Ability> useableAbilities = new List<Ability>();
            int currentLevel = unitInfo.GetUnitLevel();

            foreach (Ability ability in unitInfo.GetAbilities())
            {
                if (currentLevel >= ability.GetRequiredLevel())
                {
                    useableAbilities.Add(ability);
                }
            }

            return useableAbilities;
        }
        public UnitInfo GetUnitInfo()
        {
            return unitInfo;
        }

        public UnitResources GetUnitResources()
        {
            return unitResources;
        }
    }
}
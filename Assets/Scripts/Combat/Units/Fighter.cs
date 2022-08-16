using RPGProject.Core;
using RPGProject.GameResources;
using RPGProject.Sound;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPGProject.Combat
{
    public class Fighter : MonoBehaviour, CombatTarget
    {
        public UnitStatus unitStatus = null;
        public UnitInfo unitInfo = new UnitInfo();
        public UnitResources unitResources = new UnitResources();

        public CharacterMesh characterMesh = null;

        public Ability selectedAbility = null;
        public CombatTarget selectedTarget = null;

        Animator animator = null;
        ComboLinker comboLinker = null;
        SoundFXManager soundFXManager = null;

        Health health = null;

        List<Fighter> selectedTargets = new List<Fighter>();
        AbilityObjectKey currentAbilityObjectKey = AbilityObjectKey.None;
        ComboLink currentComboLink = null;

        AbilityObjectPool abilityObjectPool = null;

        float meleeRange = 0f;

        float strength = 0f;
        float skill = 0f;
        float luck = 0f;

        bool isPlayerFighter = false;

        Dictionary<Ability, int> abilityCooldowns = new Dictionary<Ability, int>();

        public event Action<Fighter, float> onAgroAction;
        public event Action<int> onAPUpdate;

        public void InitalizeFighter()
        {
            //animator = GetComponentInChildren<Animator>();
            comboLinker = GetComponent<ComboLinker>();
            comboLinker.InitializeComboLinker();
            comboLinker.onComboStarted += SetCurrentAbilityInstance;
            comboLinker.onComboLinkExecution += SetCurrentComboLink;

            abilityObjectPool = FindObjectOfType<AbilityObjectPool>();
            health = GetComponent<Health>();
            unitStatus = GetComponent<UnitStatus>();

            //Preset to fit the agents size
            meleeRange = GetComponent<NavMeshAgent>().stoppingDistance;

            ResetFighter();
        }

        public void SetAnimator(Animator _animator)
        {
            animator = _animator;

            comboLinker.SetAnimator(_animator);
        }

        public void UpdateAttributes(float _strength, float _skill, float _luck)
        {
            strength = _strength;
            skill = _skill;
            luck = _luck;
        }

        public IEnumerator Attack(CombatTarget _selectedTarget, Ability _selectedAbility)
        {
            selectedTarget = _selectedTarget;
            selectedAbility = _selectedAbility;

            if(!abilityCooldowns.ContainsKey(_selectedAbility)) abilityCooldowns.Add(_selectedAbility, _selectedAbility.cooldown);
            yield return comboLinker.ExecuteCombo(selectedAbility.combo);
        }

        public IEnumerator AttackAll(List<Fighter> _targetTeam, Ability _selectedAbility)
        {
            selectedTargets = _targetTeam;
            selectedAbility = _selectedAbility;

            yield return comboLinker.ExecuteCombo(selectedAbility.combo);
        }

        public void ActivateAbilityBehavior(AbilityBehavior _abilityBehavior)
        {
            _abilityBehavior.gameObject.SetActive(true);
            _abilityBehavior.PerformAbilityBehavior();
        }

        public void LookAtTarget(Transform _target)
        {
            transform.LookAt(_target);
        }

        public void SetCurrentAbilityInstance(AbilityObjectKey _abilityObjectKey)
        {
            currentAbilityObjectKey = _abilityObjectKey;
        }

        public void SetCurrentComboLink(ComboLink _comboLink)
        {
            currentComboLink = _comboLink;
        }

        public void OnNewTurn()
        {
            DecrementCooldowns();
        }

        public void SetActionPoints(int _newAPAmount)
        {
            unitResources.actionPoints = _newAPAmount;
            onAPUpdate(_newAPAmount);
        }

        public void ResetFighter()
        {
            selectedTarget = null;
            selectedAbility = null;

            UpdateAttributes(10f, 10f, 10f);

            isPlayerFighter = false;
            unitInfo = new UnitInfo();
            unitResources = new UnitResources();

            currentAbilityObjectKey = AbilityObjectKey.None;
            characterMesh = null;
        }

        public bool IsInRange(Ability _selectedAbility, Fighter _target)
        {
            float distanceToTarget = CombatAssistant.GetDistance(transform.position, _target.transform.position);
            return _selectedAbility.attackRange > distanceToTarget;
        }

        public bool HasTarget()
        {
            if (selectedTarget != null) return true;
            else return false;
        }

        public Ability GetRandomAbility()
        {
            Ability randomAbility = null;

            List<Ability> knownAbilities = new List<Ability>();
            Ability basicAttack = unitInfo.basicAttack;
            Ability[] abilities = unitInfo.abilities;

            knownAbilities.Add(basicAttack);

            if (!unitStatus.isSilenced)
            {
                if (abilities.Length == 0 || abilities == null) randomAbility = basicAttack;
                foreach(Ability ability in abilities)
                {
                    knownAbilities.Add(ability);
                }

                randomAbility = knownAbilities[RandomGenerator.GetRandomNumber(0, knownAbilities.Count -1)];
            }
            else
            {
                List<Ability> physicalAbilities = new List<Ability>();
                physicalAbilities.Add(basicAttack);

                foreach (Ability ability in abilities)
                {
                    if (ability.abilityType == AbilityType.Melee)
                    {
                        physicalAbilities.Add(ability);
                    }
                }

                randomAbility = physicalAbilities[RandomGenerator.GetRandomNumber(0, physicalAbilities.Count - 1)];
            }

            return randomAbility;
        }

        public List<Ability> GetKnownAbilities()
        {
            List<Ability> knownAbilities = new List<Ability>();

            Ability basicAttack = unitInfo.basicAttack;
            knownAbilities.Add(basicAttack);

            Ability[] abilities = unitInfo.abilities;

            foreach(Ability ability in abilities)
            {
                //Refactor
                //int unitLevel = unitInfo.GetUnitLevel();
                int unitLevel = 1000;

                if (unitLevel >= ability.requiredLevel)
                {
                    knownAbilities.Add(ability);
                }
            }

            return knownAbilities;
        }

        public int GetCooldown(Ability _ability)
        {
            if (abilityCooldowns.ContainsKey(_ability)) return abilityCooldowns[_ability];
            else return 0;
        }

        public Health GetHealthComponent()
        {
            return health;
        }

        public Ability GetBasicAttack()
        {
            return unitInfo.basicAttack;
        }

        public Transform GetAimTransform()
        {
            return characterMesh.aimTransform;
        }

        private void PerformAbility()
        {
            AbilityType abilityType = selectedAbility.abilityType;
            bool isCriticalHit = CombatAssistant.CriticalHitCheck(luck);

            float abilityAmountWStatModifier = GetStatsModifier(selectedAbility.baseAbilityAmount);
            float calculatedAmount = CombatAssistant.GetCalculatedAmount(selectedAbility.baseAbilityAmount, isCriticalHit);

            AbilityBehavior abilityBehavior = null;
            Fighter targetFighter = selectedTarget.GetComponent<Fighter>();

            if (currentAbilityObjectKey != AbilityObjectKey.None)
            {
                abilityBehavior = abilityObjectPool.GetAbilityInstance(currentAbilityObjectKey);
                abilityBehavior.SetupAbility(this, selectedTarget, calculatedAmount, isCriticalHit, selectedAbility.abilityLifetime);

                if (currentComboLink != null)
                {
                    if (currentComboLink.spawnLocationOverride != SpawnLocation.None)
                    {
                        abilityBehavior.SetSpawnLocation(currentComboLink.spawnLocationOverride);
                    }
                }
            }

            if (selectedAbility.canTargetAll)
            {
                TargetAllTargets(abilityBehavior, calculatedAmount, isCriticalHit);
                return;
            }

            Health targetHealth = null;

            if (targetFighter != null)
            {
                targetHealth = targetFighter.health;
                targetHealth.onHealthChange += ApplyAgro;
            }

            switch (abilityType)
            {
                case AbilityType.Melee:

                    targetHealth.ChangeHealth(calculatedAmount, isCriticalHit, false);
                    if (abilityBehavior != null) ActivateAbilityBehavior(abilityBehavior);
                    float reflectionAmount = -targetFighter.unitStatus.physicalReflectionDamage;
                    if (reflectionAmount > 0) health.ChangeHealth(reflectionAmount, false, false);
                    break;

                case AbilityType.Copy:

                    //onCopyAbilitySelected
                    /// Open ability menu with copy list
                    break;

                case AbilityType.Cast:

                    ActivateAbilityBehavior(abilityBehavior);
                    break;

                case AbilityType.InstaHit:

                    ActivateAbilityBehavior(abilityBehavior);
                    if (calculatedAmount != 0) targetHealth.ChangeHealth(calculatedAmount, isCriticalHit, true);
                    break;
            }

            targetHealth.onHealthChange -= ApplyAgro;
        }

        private void DecrementCooldowns()
        {
            Dictionary<Ability, bool> cooldownsToRemove = new Dictionary<Ability, bool>();

            foreach (Ability coolingDownAbility in abilityCooldowns.Keys)
            {
                if (abilityCooldowns[coolingDownAbility] - 1 <= 0) cooldownsToRemove.Add(coolingDownAbility, true);
                else cooldownsToRemove.Add(coolingDownAbility, false);
            }

            foreach (Ability ability in cooldownsToRemove.Keys)
            {
                bool shouldRemove = cooldownsToRemove[ability];

                if (shouldRemove) abilityCooldowns.Remove(ability);
                else
                {
                    int newCooldown = abilityCooldowns[ability] - 1;
                    abilityCooldowns[ability] = newCooldown;
                }
            }
        }

        private float GetStatsModifier(float _changeAmount)
        {
            float newChangeAmount = _changeAmount;
            float statsModifier = 0f;

            if (selectedAbility.abilityType == AbilityType.Melee)
            {
                statsModifier = strength - 10f;
            }
            else if (selectedAbility.abilityType == AbilityType.Cast)
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

        private void ApplyAgro(bool _isCritical, float _changeAmount)
        {
            onAgroAction(this, _changeAmount);
        }

        private void TargetAllTargets(AbilityBehavior _abilityBehavior, float _changeAmount, bool _isCritical)
        {
            TargetAll targetAll = _abilityBehavior.GetComponent<TargetAll>();
            targetAll.transform.position = targetAll.GetCenterOfTargetsPoint(selectedTargets);
            ActivateAbilityBehavior(_abilityBehavior);
            foreach (Fighter fighter in selectedTargets)
            {
                fighter.health.ChangeHealth(_changeAmount, _isCritical, true);
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
    }
}
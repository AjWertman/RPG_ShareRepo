using RPGProject.Core;
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
        Health health = null;
        Mana mana = null;

        BattleUnitInfo unitInfo = new BattleUnitInfo();
        BattleUnitResources unitResources = new BattleUnitResources();

        Ability selectedAbility = null;
        Fighter selectedTarget = null;
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

        bool isPlayerFighter = false;

        public void InitalizeFighter()
        {
            animator = GetComponent<Animator>();
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

        public void Attack(Fighter _selectedTarget, Ability _selectedAbility)
        {
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
                    float reflectionAmount = selectedTarget.GetPhysicalReflectionDamage();
                    if (reflectionAmount > 0)
                    {
                        health.DamageHealth(reflectionAmount, false, true);
                    }
                    break;

                case AbilityType.Copy:

                    //onCopyAbilitySelected
                    /// Open ability menu with copy list

                    break;

                case AbilityType.Cast:

                    AbilityBehavior abilityBehavior = abilityObjectPool.GetAbilityInstance(selectedAbility);
                    abilityBehavior.SetupAbility(this, selectedTarget, calculatedAmount, isCriticalHit);
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

            isPlayerFighter = false;
            unitInfo = new BattleUnitInfo();
            unitResources = new BattleUnitResources();


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

        public void SetUnitInfo(BattleUnitInfo _unitInfo)
        {
            unitInfo = _unitInfo;
        }

        public void SetUnitResources(BattleUnitResources _unitResources)
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
        public BattleUnitInfo GetUnitInfo()
        {
            return unitInfo;
        }

        public BattleUnitResources GetUnitResources()
        {
            return unitResources;
        }
    }
}
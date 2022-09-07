using RPGProject.Combat.Grid;
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
    /// <summary>
    /// Handles combat for a Unit.
    /// </summary>
    public class Fighter : MonoBehaviour, CombatTarget
    {
        public UnitStatus unitStatus = null;
        public UnitInfo unitInfo = new UnitInfo();
        public UnitResources unitResources = new UnitResources();

        public CharacterMesh characterMesh = null;

        public Ability selectedAbility = null;
        public CombatTarget selectedTarget = null;
        public GridBlock currentBlock = null;

        Animator animator = null;
        ComboLinker comboLinker = null;
        SoundFXManager soundFXManager = null;

        Health health = null;
        Energy energy = null;

        List<Fighter> selectedTargets = new List<Fighter>();
        AbilityObjectKey currentAbilityObjectKey = AbilityObjectKey.None;
        ComboLink currentComboLink = null;

        AbilityObjectPool abilityObjectPool = null;

        float meleeRange = 0f;

        float strength = 0f;
        float skill = 0f;
        float luck = 0f;
        
        bool isHighlighted = false;

        Dictionary<Ability, int> abilityCooldowns = new Dictionary<Ability, int>();

        /// <summary>
        /// Called whenever the current Fighter does something that increases
        /// or decreases their agro percentage of the opposing team.
        /// </summary>
        public event Action<Fighter, int> onAgroAction;

        /// <summary>
        /// An event to notify the BattleHandler that the neighbor blocks should be affected because of an ability.
        ///Fighter = attacker, int = amount of blocks affected; AbilityBehavior = the grid behavior to apply;
        ///CombatTarget = main target; float = base damage amount. 
        /// </summary>
        public event Action<Fighter, int, AbilityBehavior, CombatTarget, float> onAffectNeighborBlocks;
        public event Action<bool> onHighlight;

        public void InitalizeFighter()
        {
            //animator = GetComponentInChildren<Animator>();
            comboLinker = GetComponent<ComboLinker>();
            comboLinker.InitializeComboLinker();
            comboLinker.onComboStarted += SetCurrentAbilityInstance;
            comboLinker.onComboLinkExecution += OnComboLinkUpdate;

            health = GetComponent<Health>();
            energy = GetComponent<Energy>();
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
            if (_abilityBehavior == null) return;
            _abilityBehavior.gameObject.SetActive(true);
            _abilityBehavior.PerformAbilityBehavior();

            if (_abilityBehavior.GetType() == typeof(Turret))
            {
                Turret turret = (Turret)_abilityBehavior;

                List<GridBlock> attackRadius = new List<GridBlock>();
                Pathfinder pathfinder = FindObjectOfType<Pathfinder>();

                foreach (GridBlock gridBlock in pathfinder.GetNeighbors(turret.myBlock, selectedAbility.amountOfNeighborBlocksAffected))
                {
                    attackRadius.Add(gridBlock);
                }
                turret.SetupAttackRadius(attackRadius);
            }
        }

        public void LookAtTarget(Transform _target)
        {
            transform.LookAt(_target);
        }

        public void SetCurrentAbilityInstance(AbilityObjectKey _abilityObjectKey)
        {
            currentAbilityObjectKey = _abilityObjectKey;
        }

        public void OnComboLinkUpdate(ComboLink _comboLink)
        {
            currentComboLink = _comboLink;
        }

        public void OnNewTurn()
        {
            DecrementCooldowns();
        }

        public void HighlightFighter(bool _shouldHighlight)
        {
            if (isHighlighted == _shouldHighlight) return;
            isHighlighted = _shouldHighlight;
            onHighlight(isHighlighted);
        }

        public void ResetFighter()
        {
            selectedTarget = null;
            selectedAbility = null;

            UpdateAttributes(10f, 10f, 10f);

            unitInfo = new UnitInfo();
            unitResources = new UnitResources();

            currentAbilityObjectKey = AbilityObjectKey.None;
            characterMesh = null;
        }

        public bool HasTarget()
        {
            if (selectedTarget != null) return true;
            else return false;
        }

        /// <summary>
        /// Returns all the abilities a fighter is the proper level to use.
        /// </summary>
        public List<Ability> GetKnownAbilities()
        {
            List<Ability> knownAbilities = new List<Ability>();

            Ability basicAttack = unitInfo.basicAttack;
            knownAbilities.Add(basicAttack);

            Ability[] abilities = unitInfo.abilities;

            foreach(Ability ability in abilities)
            {
                if (ability == null) continue;
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

        public Health GetHealth()
        {
            return health;
        }

        public Energy GetEnergy()
        {
            return energy;
        }

        public Ability GetBasicAttack()
        {
            return unitInfo.basicAttack;
        }

        public Transform GetAimTransform()
        {
            return characterMesh.aimTransform;
        }

        /// <summary>
        /// Performs the behavior of the selected ability this fighter is casting and executes it on the selected target.
        /// </summary>
        private void PerformAbility()
        {
            AbilityType abilityType = selectedAbility.abilityType;
            bool isCriticalHit = CombatAssistant.CriticalHitCheck(luck);

            float abilityAmountWStatModifier = GetStatsModifier(selectedAbility.baseAbilityAmount);
            float calculatedAmount = CombatAssistant.GetCalculatedAmount(selectedAbility.baseAbilityAmount, isCriticalHit);

            AbilityBehavior abilityBehavior = null;
            Fighter targetFighter = selectedTarget as Fighter;
            bool hasTarget = targetFighter != null;

            if (currentAbilityObjectKey != AbilityObjectKey.None)
            {
                abilityBehavior = GetAbilityBehavior(isCriticalHit, calculatedAmount);
            }

            Health targetHealth = null;
            if (hasTarget)
            {
                targetHealth = targetFighter.health;
                ApplyAgro(false, selectedAbility.baseAgroPercentageAmount);
            }

            switch (abilityType)
            {
                case AbilityType.Melee:

                    GameObject hitFX = null;
                    if (currentComboLink.hitFXObjectKey != HitFXObjectKey.None)
                    {
                        hitFX = abilityObjectPool.GetHitFX(currentComboLink.hitFXObjectKey);
                        hitFX.transform.position = targetFighter.transform.position;
                        hitFX.SetActive(true);
                    }

                    targetHealth.ChangeHealth(calculatedAmount, isCriticalHit, false);
                    AffectNeighborBlocks(abilityBehavior);
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
                    abilityBehavior.onAbilityDeath += AffectNeighborBlocks;
                    break;

                case AbilityType.InstaHit:

                    ActivateAbilityBehavior(abilityBehavior);
                    if (calculatedAmount != 0) targetHealth.ChangeHealth(calculatedAmount, isCriticalHit, true);
                    if(abilityBehavior != null) abilityBehavior.onAbilityDeath += AffectNeighborBlocks;
                    break;
            }
        }

        private void AffectNeighborBlocks(AbilityBehavior _abilityBehavior)
        {
            if (selectedAbility == null) return;
            if (selectedAbility.amountOfNeighborBlocksAffected <= 0) return;

            AbilityBehavior blockBehavior = null;
            if (_abilityBehavior != null)
            {
                blockBehavior = _abilityBehavior.childBehavior;
                _abilityBehavior.onAbilityDeath -= AffectNeighborBlocks;
            }
 
            int amountToAffect = selectedAbility.amountOfNeighborBlocksAffected;

            onAffectNeighborBlocks(this, amountToAffect, blockBehavior, selectedTarget, selectedAbility.baseAbilityAmount);
        }

        private AbilityBehavior GetAbilityBehavior(bool isCriticalHit, float calculatedAmount)
        {    
            AbilityBehavior newBehavior = abilityObjectPool.GetAbilityInstance(currentAbilityObjectKey);
            newBehavior.SetupAbility(this, selectedTarget, calculatedAmount, isCriticalHit, selectedAbility.abilityLifetime);

            if (currentComboLink != null)
            {
                if (currentComboLink.spawnLocationOverride != SpawnLocation.None)
                {
                    newBehavior.SetSpawnLocation(currentComboLink.spawnLocationOverride);
                }
            }

            return newBehavior;
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

        private void ApplyAgro(bool _NA, int _changeAmount)
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
        public void Hit()
        {
            PerformAbility();
        }

        public void Shoot()
        {
            PerformAbility();
        }
        //////////
        ///
    }
}
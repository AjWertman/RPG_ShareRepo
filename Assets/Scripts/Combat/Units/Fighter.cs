using RPGProject.Combat.Grid;
using RPGProject.GameResources;
using RPGProject.Progression;
using RPGProject.Sound;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Combat
{
    /// <summary>
    /// Handles combat for a Unit.
    /// </summary>
    public class Fighter : MonoBehaviour, CombatTarget
    {
        public UnitInfo unitInfo = null;
        public UnitStatus unitStatus = new UnitStatus();
        public UnitResources unitResources = new UnitResources();

        public CharacterMesh characterMesh = null;

        public Ability selectedAbility = null;
        public CombatTarget selectedTarget = null;
        public GridBlock currentBlock = null;
        public ComboLink currentComboLink = null;

        Animator animator = null;
        ComboLinker comboLinker = null;
        SoundFXManager soundFXManager = null;

        Health health = null;
        Energy energy = null;

        List<Fighter> selectedTargets = new List<Fighter>();

        float meleeRange = 0f;

        bool isHighlighted = false;

        Dictionary<Ability, int> abilityCooldowns = new Dictionary<Ability, int>();

        /// <summary>
        /// An event to notify the AbilityManager to create an ability and perform its behavior.
        /// </summary>
        public event Action onAbilityUse;

        /// <summary>
        /// Called whenever the current Fighter does something that increases
        /// or decreases their agro percentage of the opposing team.
        /// </summary>
        public event Action<Fighter, int> onAgroAction;

        /// <summary>
        /// An event called when this fighter has been highlighted or unhighlighted to handle
        /// the proper highlighting in other aspects of the combat system.
        /// </summary>
        public event Action<bool> onHighlight;

        public void InitalizeFighter()
        {
            //animator = GetComponentInChildren<Animator>();
            comboLinker = GetComponent<ComboLinker>();

            if (comboLinker != null)
            {
                comboLinker.InitializeComboLinker();
                comboLinker.onComboLinkExecution += OnComboLinkUpdate;
            }

            health = GetComponent<Health>();
            energy = GetComponent<Energy>();
            unitStatus = new UnitStatus(true);

            ResetFighter();
        }

        public void SetAnimator(Animator _animator)
        {
            animator = _animator;

            comboLinker.SetAnimator(_animator);
        }

        public void UpdateAttributes(float _strength, float _skill, float _luck)
        {
            //strength = _strength;
            //skill = _skill;
            //luck = _luck;
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

        public void LookAtTarget(Transform _target)
        {
            transform.LookAt(_target);
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

            unitInfo = null;
            unitResources = new UnitResources();
            
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
            if(basicAttack != null) knownAbilities.Add(basicAttack);

            List<Ability> abilities = unitInfo.abilities;

            foreach(Ability ability in abilities)
            {
                if (ability == null) continue;

                int unitLevel = unitInfo.unitLevel;
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

        public ComboLinker GetComboLinker()
        {
            return comboLinker;
        }

        public Ability GetBasicAttack()
        {
            return unitInfo.basicAttack;
        }

        public Transform GetAimTransform()
        {
            return characterMesh.aimTransform;
        }

        public void ApplyAgro(bool _NA, int _changeAmount)
        {
            onAgroAction(this, _changeAmount);
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

        //Animation Events///////////////////////////////////////////////////
        public void Hit()
        {
            onAbilityUse();
        }

        public void Shoot()
        {
            onAbilityUse();
        }
    }
}
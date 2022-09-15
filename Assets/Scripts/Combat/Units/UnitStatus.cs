using System;
using System.Collections.Generic;

namespace RPGProject.Combat
{
    /// <summary>
    /// Data of a combatant and the abilities that are currently affecting them.
    /// </summary>
    [Serializable]
    public struct UnitStatus
    {
        public float physicalReflectionDamage;
        public bool isReflectingSpells;
        public bool isSilenced;
        public bool hasSubstitute;

        public List<AbilityBehavior> activeAbilityBehaviors;

        public UnitStatus(bool _isInitialization)
        {
            physicalReflectionDamage = 0;
            isReflectingSpells = false;
            isSilenced = false;
            hasSubstitute = false;
            activeAbilityBehaviors = new List<AbilityBehavior>();
        }

        public void ApplyActiveAbilityBehavior(AbilityBehavior _abilityBehavior)
        {
            if (_abilityBehavior == null) return;
            if (IsAlreadyAffected(_abilityBehavior)) return;
            _abilityBehavior.onAbilityDeath += RemoveActiveAbilityBehavior;

            if (activeAbilityBehaviors.Contains(_abilityBehavior)) return;
            activeAbilityBehaviors.Add(_abilityBehavior);
        }

        public void RemoveActiveAbilityBehavior(AbilityBehavior _abilityBehavior)
        {
            _abilityBehavior.onAbilityDeath -= RemoveActiveAbilityBehavior;

            if (!activeAbilityBehaviors.Contains(_abilityBehavior)) return;
            activeAbilityBehaviors.Remove(_abilityBehavior);
        }

        public void ResetUnitStatus()
        {
            physicalReflectionDamage = 0;
            isReflectingSpells = false;
            isSilenced = false;
            hasSubstitute = false;
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

        public bool IsAlreadyAffected(AbilityBehavior _abilityBehavior)
        {
            return CombatAssistant.IsAlreadyEffected(_abilityBehavior.GetAbilityName(), this);
        }
    }
}

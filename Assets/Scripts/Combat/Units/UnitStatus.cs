using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Combat
{
    /// <summary>
    /// Data of a combatant and the abilities that are currently affecting them.
    /// </summary>
    public class UnitStatus : MonoBehaviour
    {
        public float physicalReflectionDamage = 0f;
        public bool isReflectingSpells = false;
        public bool isSilenced = false;
        public bool hasSubstitute = false;

        List<AbilityBehavior> activeAbilityBehaviors = new List<AbilityBehavior>();

        public void ApplyActiveAbilityBehavior(AbilityBehavior _abilityBehavior)
        {
            if (_abilityBehavior == null) return;
            bool isAlreadyEffected = CombatAssistant.IsAlreadyEffected(_abilityBehavior.GetAbilityName(), this);
            if (isAlreadyEffected) return;
            _abilityBehavior.onAbilityDeath += RemoveActiveAbilityBehavior;

            if (activeAbilityBehaviors.Contains(_abilityBehavior)) return;
            activeAbilityBehaviors.Add(_abilityBehavior);
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

        private void RemoveActiveAbilityBehavior(AbilityBehavior _abilityBehavior)
        {
            _abilityBehavior.onAbilityDeath -= RemoveActiveAbilityBehavior;

            if (!activeAbilityBehaviors.Contains(_abilityBehavior)) return;
            activeAbilityBehaviors.Remove(_abilityBehavior);
        }
    }
}

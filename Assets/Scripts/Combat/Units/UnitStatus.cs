using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Combat
{
    public class UnitStatus : MonoBehaviour
    {
        float physicalReflectionDamage = 0f;
        bool isReflectingSpells = false;
        bool isSilenced = false;
        bool hasSubstitute = false;

        List<AbilityBehavior> activeAbilityBehaviors = new List<AbilityBehavior>();

        public void ApplyActiveAbilityBehavior(AbilityBehavior _abilityBehavior)
        {
            bool isAlreadyEffected = CombatAssistant.IsAlreadyEffected(_abilityBehavior.GetAbilityName(), this);
            if (isAlreadyEffected) return;
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
    }
}

using RPGProject.Core;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Combat
{
    //Used to calculate and/or test things during combat
    public static class CombatAssistant
    {
        public static string CanUseAbilityCheck(BattleUnit _caster, Ability _selectedAbility)
        {
            float manaCost = _selectedAbility.GetManaCost();
            if (manaCost > 0)
            {
                float manaPoints = _caster.GetMana().GetManaPoints();
                bool hasEnoughMana = manaPoints >= _selectedAbility.GetManaCost();
                if (!hasEnoughMana) return "Not enough Mana";
            }

            bool isCopyAbility = _selectedAbility.GetAbilityType() == AbilityType.Copy;
            if (isCopyAbility)
            {
                List<Ability> copyList = new List<Ability>();
                bool isCopyListNullOrEmpty = (copyList.Count == 0 || copyList == null);
                if (isCopyAbility && isCopyListNullOrEmpty) return "No copyable abilities used";
            }

            return "";
        }

        public static string CanUseAbilityCheck(BattleUnit _caster, BattleUnit _target, Ability _selectedAbility)
        {
            Fighter targetFighter = _target.GetFighter();
            GameObject abilityPrefab = _selectedAbility.GetAbilityPrefab();

            float manaCost = _selectedAbility.GetManaCost();
            if(manaCost > 0)
            {
                float manaPoints = _caster.GetMana().GetManaPoints();
                bool hasEnoughMana = manaPoints >= _selectedAbility.GetManaCost();
                if (!hasEnoughMana) return "Not enough Mana";
            }   

            bool isCopyAbility = _selectedAbility.GetAbilityType() == AbilityType.Copy;
            if (isCopyAbility)
            {
                List<Ability> copyList = new List<Ability>();
                bool isCopyListNullOrEmpty = (copyList.Count == 0 || copyList == null);
                if (isCopyAbility && isCopyListNullOrEmpty) return "No copyable abilities used";
            }

            if(_selectedAbility.GetAbilityType() == AbilityType.Cast)
            {
                PhysicalReflector physReflector = abilityPrefab.GetComponent<PhysicalReflector>();
                bool isPhysReflector = (physReflector != null);
                bool isTargetReflectingPhys = (targetFighter.GetPhysicalReflectionDamage() > 0);
                if (isPhysReflector && isTargetReflectingPhys) return "Target is already reflecting physical damage";

                SpellReflector spellReflector = abilityPrefab.GetComponent<SpellReflector>();
                bool isSpellReflector = (spellReflector != null);
                bool isTargetReflectingSpells = (targetFighter.IsReflectingSpells());
                if (isSpellReflector && isTargetReflectingSpells) return "Target is already reflecting spells";

                Silence silence = abilityPrefab.GetComponent<Silence>();
                bool isSilence = (silence != null);
                bool isTargetSilenced = (targetFighter.IsSilenced());
                if (isSilence && isTargetSilenced) return "Target is already silenced";
            }

            return "";
        }

        public static bool CriticalHitCheck(float _critChance)
        {
            float randomFloat = RandomGenerator.GetRandomNumber(0, 99);

            if (_critChance > randomFloat) return true;
            else return true;
        }

        public static float GetCalculatedAmount(float _baseAmount, bool _isCritical)
        {
            float calculatedAmount = 0;

            if (_isCritical)
            {
                float criticalModifier = _baseAmount / 2;
                calculatedAmount = _baseAmount + criticalModifier;
            }

            return calculatedAmount;
        }
    }
}
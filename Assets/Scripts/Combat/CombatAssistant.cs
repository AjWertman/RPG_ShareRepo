using RPGProject.Core;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Combat
{
    /// <summary>
    /// Useful functions and calculations for combat.
    /// </summary>
    public static class CombatAssistant
    {
        /// <summary>
        /// Returns a string of why the current combatant cannot use an ability.
        /// If it returns an empty string (""), it implies that the current combatant can use the ability.
        /// </summary>
        public static string CanUseAbilityCheck(Fighter _caster, Ability _selectedAbility)
        {
            float energyCost = _selectedAbility.energyPointsCost;
            if (energyCost > 0)
            {
                float energyPoints = _caster.GetEnergy().energyPoints;
                bool hasEnoughAP = energyPoints >= energyCost;
                if (!hasEnoughAP) return "Not enough energy!";
            }

            if (_caster.GetCooldown(_selectedAbility) > 0) return "Ability is cooling down!";

            bool isCopyAbility = _selectedAbility.abilityType == AbilityType.Copy;
            if (isCopyAbility)
            {
                List<Ability> copyList = new List<Ability>();
                bool isCopyListNullOrEmpty = (copyList.Count == 0 || copyList == null);
                if (isCopyAbility && isCopyListNullOrEmpty) return "No copyable abilities used!";
            }

            bool isAbilityMagic = (_selectedAbility.abilityType!= AbilityType.Melee);
            bool isFighterSilenced = _caster.unitStatus.isSilenced;
            if (isAbilityMagic && isFighterSilenced) return "Caster is silenced!";

            return "";
        }

        /// <summary>
        /// Extra parameters for the "CanUseAbilityCheck" function.
        /// </summary>
        public static string CanUseAbilityCheck(Fighter _caster, CombatTarget _target, Ability _selectedAbility)
        {
            string canUseAbilityCheck = CanUseAbilityCheck(_caster, _selectedAbility);
            if (canUseAbilityCheck != "") return canUseAbilityCheck;

            float attackRange = _selectedAbility.attackRange;
            if(attackRange != 0)
            {
                Transform aimTransform = _target.GetAimTransform();
                if (!IsInRange(_caster.transform.position, aimTransform.position, attackRange)) return "Target is too far away!";
            }

            //CharacterBiology requiredBiology = _selectedAbility.requiredBiology;
            //if (requiredBiology != CharacterBiology.None)
            //{
            //    if (_target.GetType() == typeof(Fighter))
            //    {
            //        Fighter targetFighter = (Fighter)_target;
            //        CharacterBiology targetBiology = targetFighter.characterMesh.characterBiology;
            //        if(_selectedAbility.baseAbilityAmount >0)
            //        if (requiredBiology != targetBiology) return "Not a compatable target";
            //    }
            //    else return "Not a compatable target";
            //}

            return "";
        }

        /// <summary>
        /// Determines if the target already is effected by an ability.
        /// </summary>
        public static bool IsAlreadyEffected(string _abilityName, UnitStatus _targetStatus)
        {
            if (_abilityName == null || _abilityName == "" || _targetStatus == null) return false;

            foreach (AbilityBehavior abilityBehavior in _targetStatus.GetActiveAbilityBehaviors())
            {
                if (abilityBehavior.GetAbilityName() == _abilityName) return true;
            }

            return false;       
        }

        /// <summary>
        /// Calculation for critical hits.
        /// </summary>
        public static bool CriticalHitCheck(float _critChance)
        {
            float randomFloat = RandomGenerator.GetRandomNumber(0, 99);
            bool isCriticalHit = _critChance > randomFloat;

            return isCriticalHit;
        }

        /// <summary>
        /// Calculation for applying buff.
        /// </summary>
        public static bool ApplyBuffCheck(float _applyChance)
        {
            float randomFloat = RandomGenerator.GetRandomNumber(0, 99);
            bool isSuccessful = _applyChance > randomFloat;

            //Refactor - add buff to apply and target parameter
            ///Check if target has something applied already

            return isSuccessful;
        }

        /// <summary>
        /// Returns the amount of damage or healing with critical hit modifier.
        /// </summary>
        public static float GetCalculatedAmount(float _baseAmount, bool _isCritical)
        {
            float calculatedAmount = _baseAmount;
            bool isHeal = _baseAmount > 0;

            if (_isCritical)
            {
                float criticalModifier = _baseAmount / 2;
                calculatedAmount += criticalModifier;
            }

            return calculatedAmount;
        }

        public static bool IsInRange(Vector3 _myPosition, Vector3 _targetPosition, float _attackRange)
        {
            float distanceToTarget = GetDistance(_myPosition, _targetPosition);

            return _attackRange > distanceToTarget;
        }

        public static float GetDistance( Vector3 _myPosition, Vector3 _targetPosition)
        {
            Vector3 myPosition = new Vector3(_myPosition.x, 0, _myPosition.z);
            Vector3 targetPosition = new Vector3(_targetPosition.x, 0, _targetPosition.z);

            return Vector3.Distance(myPosition, targetPosition);
        }
    }
}
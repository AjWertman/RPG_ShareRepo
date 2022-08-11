using RPGProject.Core;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Combat
{
    //Used to calculate and/or test things during combat
    public static class CombatAssistant
    {
        public static string CanUseAbilityCheck(Fighter _caster, Ability _selectedAbility)
        {
            float apCost = _selectedAbility.actionPointsCost;
            if (apCost > 0)
            {
                float actionPoints = _caster.unitResources.actionPoints;
                bool hasEnoughAP = actionPoints >= apCost;
                if (!hasEnoughAP) return "Not enough Action Points";
            }

            if (_caster.GetCooldown(_selectedAbility) > 0) return "Ability is cooling down";

            bool isCopyAbility = _selectedAbility.abilityType == AbilityType.Copy;
            if (isCopyAbility)
            {
                List<Ability> copyList = new List<Ability>();
                bool isCopyListNullOrEmpty = (copyList.Count == 0 || copyList == null);
                if (isCopyAbility && isCopyListNullOrEmpty) return "No copyable abilities used";
            }

            bool isAbilityMagic = (_selectedAbility.abilityType!= AbilityType.Melee);
            bool isFighterSilenced = _caster.unitStatus.isSilenced;
            if (isAbilityMagic && isFighterSilenced) return "Caster is silenced";

            return "";
        }

        public static string CanUseAbilityCheck(Fighter _caster, CombatTarget _target, Ability _selectedAbility)
        {
            string canUseAbilityCheck = CanUseAbilityCheck(_caster, _selectedAbility);
            if (canUseAbilityCheck != "") return canUseAbilityCheck;

            float attackRange = _selectedAbility.attackRange;
            if(attackRange != 0)
            {
                Transform aimTransform = _target.GetAimTransform();
                if (!IsInRange(_caster.transform.position, aimTransform.position, attackRange)) return "Target is too far away";
            }

            return "";
        }

        public static bool IsAlreadyEffected(string _abilityName, UnitStatus _targetStatus)
        {
            if (_abilityName == null || _abilityName == "" || _targetStatus == null) return false;

            foreach (AbilityBehavior abilityBehavior in _targetStatus.GetActiveAbilityBehaviors())
            {
                if (abilityBehavior.GetAbilityName() == _abilityName) return true;
            }

            return false;       
        }

        public static bool CriticalHitCheck(float _critChance)
        {
            float randomFloat = RandomGenerator.GetRandomNumber(0, 99);
            bool isCriticalHit = _critChance > randomFloat;

            return isCriticalHit;
        }

        public static bool ApplyBuffCheck(float _applyChance)
        {
            float randomFloat = RandomGenerator.GetRandomNumber(0, 99);
            bool isSuccessful = _applyChance > randomFloat;

            //Refactor - add buff to apply and target parameter
            ///Check if target has something applied already

            return isSuccessful;
        }

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

            return distanceToTarget < _attackRange;
        }

        public static float GetDistance( Vector3 _myPosition, Vector3 _targetPosition)
        {
            Vector3 myPosition = new Vector3(_myPosition.x, 0, _myPosition.z);
            Vector3 targetPosition = new Vector3(_targetPosition.x, 0, _targetPosition.z);

            return Vector3.Distance(myPosition, targetPosition);
        }
    }
}
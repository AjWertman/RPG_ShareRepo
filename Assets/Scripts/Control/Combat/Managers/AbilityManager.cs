using RPGProject.Combat;
using RPGProject.Combat.Grid;
using RPGProject.GameResources;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Control.Combat
{
    public class AbilityManager : MonoBehaviour
    {
        Pathfinder pathfinder = null;
        AbilityObjectPool abilityObjectPool = null;

        Fighter currentFighter = null;
        AbilityObjectKey currentAbilityKey = AbilityObjectKey.None;

        /// <summary>
        /// An event to notify the BattleHandler that the neighbor blocks should be affected because of an ability.
        ///Fighter = attacker, int = amount of blocks affected; AbilityBehavior = the grid behavior to apply;
        ///CombatTarget = main target; float = base damage amount. 
        /// </summary>
        public event Action<Fighter, int, AbilityBehavior, CombatTarget, float> onAffectNeighborBlocks;

        public event Action<AbilityBehavior> onCombatantAbilitySpawn;

        public void InitalizeAbilityManager()
        {
            pathfinder = FindObjectOfType<Pathfinder>();
            abilityObjectPool = FindObjectOfType<AbilityObjectPool>();
        }

        public void SetupAbilityManager(List<Fighter> _allFighters)
        {
            abilityObjectPool.CreateAbilityObjects(_allFighters);
        }

        public void ActivateAbilityBehavior(AbilityBehavior _abilityBehavior)
        {
            Ability selectedAbility = currentFighter.selectedAbility;

            if (_abilityBehavior == null) return;
            _abilityBehavior.PerformAbilityBehavior();

            if (_abilityBehavior.GetType() == typeof(Turret))
            {
                onCombatantAbilitySpawn(_abilityBehavior);
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

        private AbilityBehavior GetAbilityBehavior(bool _isCritical, float _changeAmount)
        {
            AbilityBehavior newBehavior = abilityObjectPool.GetAbilityInstance(currentAbilityKey);
            ComboLink currentComboLink = currentFighter.currentComboLink;
            if (newBehavior != null)
            {
                newBehavior.gameObject.SetActive(true);
                newBehavior.SetupAbility(currentFighter, currentFighter.selectedTarget, _changeAmount, _isCritical, currentFighter.selectedAbility.abilityLifetime);
            }
            if (currentComboLink != null)
            {
                if (currentComboLink.spawnLocationOverride != SpawnLocation.None)
                {
                    newBehavior.SetSpawnLocation(currentComboLink.spawnLocationOverride);
                }
            }         

            return newBehavior;
        }

        public void SetCurrentCombatantTurn(Fighter _newFighter)
        {
            currentFighter = _newFighter;
        }

        public void SetCurrentAbilityKey(AbilityObjectKey _abilityObjectKey)
        {
            currentAbilityKey = _abilityObjectKey;
        }

        public void PerformAbility()
        {
            Ability selectedAbility = currentFighter.selectedAbility;
            ComboLink currentComboLink = currentFighter.currentComboLink;
            AbilityType abilityType = selectedAbility.abilityType;
            bool isCriticalHit = CombatAssistant.CriticalHitCheck(currentFighter.luck);

            float abilityAmountWStatModifier = GetStatsModifier(currentFighter, selectedAbility.abilityType,selectedAbility.baseAbilityAmount);
            float calculatedAmount = CombatAssistant.GetCalculatedAmount(selectedAbility.baseAbilityAmount, isCriticalHit);

            AbilityBehavior abilityBehavior = null;
            Fighter targetFighter = currentFighter.selectedTarget as Fighter;
            bool hasTarget = targetFighter != null;

            Health targetHealth = null;
            if (hasTarget)
            {
                targetHealth = targetFighter.GetHealth();
                currentFighter.ApplyAgro(false, selectedAbility.baseAgroPercentageAmount);

                CharacterBiology targetBiology = targetFighter.characterMesh.characterBiology;
                CharacterBiology abilityRequiredBiology = selectedAbility.requiredBiology;
                if (abilityRequiredBiology != CharacterBiology.None && targetBiology != abilityRequiredBiology)
                {
                    calculatedAmount = -calculatedAmount;
                }
            }

            if (currentAbilityKey != AbilityObjectKey.None)
            {
                abilityBehavior = GetAbilityBehavior(isCriticalHit, calculatedAmount);
            }

            switch (abilityType)
            {
                case AbilityType.Melee:

                    if (currentComboLink.hitFXObjectKey != HitFXObjectKey.None)
                    {
                        abilityObjectPool.SpawnHitFX(currentComboLink.hitFXObjectKey, targetFighter.transform.position);
                    }

                    targetHealth.ChangeHealth(calculatedAmount, isCriticalHit, false);
                    AffectNeighborBlocks(abilityBehavior);
                    if (abilityBehavior != null) ActivateAbilityBehavior(abilityBehavior);

                    if(targetFighter.unitStatus != null)
                    {
                        float reflectionAmount = -targetFighter.unitStatus.physicalReflectionDamage;
                        if (reflectionAmount > 0) currentFighter.GetHealth().ChangeHealth(reflectionAmount, false, false);
                    }

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
                    if (abilityBehavior != null) abilityBehavior.onAbilityDeath += AffectNeighborBlocks;
                    break;
            }

            currentAbilityKey = AbilityObjectKey.None;
        }

        private void AffectNeighborBlocks(AbilityBehavior _abilityBehavior)
        {
            Ability selectedAbility = currentFighter.selectedAbility;
            if (selectedAbility == null) return;
            if (selectedAbility.amountOfNeighborBlocksAffected <= 0) return;

            AbilityBehavior childBehavior = null;
            if (_abilityBehavior != null)
            {
                if (_abilityBehavior.GetType() != typeof(Turret))
                {
                    childBehavior = _abilityBehavior.childBehavior;
                }

                _abilityBehavior.onAbilityDeath -= AffectNeighborBlocks;
            }

            int amountToAffect = selectedAbility.amountOfNeighborBlocksAffected;

            AffectNeighborBehavior(currentFighter, amountToAffect, childBehavior, currentFighter.selectedTarget, selectedAbility.baseAbilityAmount);
        }

        private void AffectNeighborBehavior(Fighter _attacker, int _amountOfNeighbors, AbilityBehavior _childBehavior, CombatTarget _mainTarget, float _baseAbilityAmount)
        {
            GridBlock centerBlock = null;
            if (_mainTarget.GetType() == typeof(GridBlock)) centerBlock = (GridBlock)_mainTarget;
            else if (_mainTarget.GetType() == typeof(Fighter))
            {
                Fighter _target = (Fighter)_mainTarget;
                centerBlock = _target.currentBlock;
            }
            List<GridBlock> neighbors = pathfinder.GetNeighbors(centerBlock, _amountOfNeighbors);

            if (_childBehavior != null)
            {
                _childBehavior.SetupAbility(null, null, _baseAbilityAmount, false, 3);
            }
            foreach (GridBlock gridBlock in neighbors)
            {
                Fighter contestedFighter = gridBlock.contestedFighter;
                if (contestedFighter != null)
                {
                    if (contestedFighter == _attacker) continue;
                    bool isChildBehaviorNull = _childBehavior == null;
                    contestedFighter.GetHealth().ChangeHealth(_baseAbilityAmount, false, !isChildBehaviorNull);

                    if (!isChildBehaviorNull) contestedFighter.unitStatus.ApplyActiveAbilityBehavior(_childBehavior);
                }

                if (_childBehavior != null)
                {
                    gridBlock.SetActiveAbility(_childBehavior);
                }
            }

            if (_childBehavior != null) centerBlock.SetActiveAbility(_childBehavior);
        }

        private float GetStatsModifier(Fighter _caster, AbilityType _abilityType, float _changeAmount)
        {
            float newChangeAmount = _changeAmount;
            float statsModifier = 0f;

            if (_abilityType == AbilityType.Melee)
            {
                statsModifier = _caster.strength - 10f;
            }
            else if (_abilityType == AbilityType.Cast)
            {
                statsModifier = _caster.skill - 10f;
            }

            if (statsModifier > 0)
            {
                float offensivePercentage = statsModifier * .1f;
                newChangeAmount += (_changeAmount * offensivePercentage);
            }

            return newChangeAmount;
        }

        //private void TargetAllTargets(AbilityBehavior _abilityBehavior, float _changeAmount, bool _isCritical)
        //{
        //    TargetAll targetAll = _abilityBehavior.GetComponent<TargetAll>();
        //    targetAll.transform.position = targetAll.GetCenterOfTargetsPoint(selectedTargets);
        //    //ActivateAbilityBehavior(_abilityBehavior);
        //    foreach (Fighter fighter in selectedTargets)
        //    {
        //        fighter.health.ChangeHealth(_changeAmount, _isCritical, true);
        //    }
        //}
    }
}

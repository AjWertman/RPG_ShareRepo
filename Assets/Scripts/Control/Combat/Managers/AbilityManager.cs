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
        GridPatternHandler patternHandler = null;
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
            patternHandler = FindObjectOfType<GridPatternHandler>();
            abilityObjectPool = FindObjectOfType<AbilityObjectPool>();
        }

        public void SetupAbilityManager(List<Fighter> _allFighters)
        {
            abilityObjectPool.CreateAbilityObjects(_allFighters);

            foreach(Fighter fighter in _allFighters)
            {
                UniqueUnitBehavior uniqueUnitBehavior = fighter.characterMesh.GetComponent<UniqueUnitBehavior>();
                if (uniqueUnitBehavior == null) continue;
                List<AbilityBehavior> negatedBehaviors = uniqueUnitBehavior.GetNegatedBehaviors();

                fighter.negatedBehaviors = negatedBehaviors;
            }
        }

        public void ActivateAbilityBehavior(AbilityBehavior _abilityBehavior)
        {
            Ability selectedAbility = currentFighter.selectedAbility;

            if (_abilityBehavior == null) return;
            _abilityBehavior.PerformAbilityBehavior();

            if (_abilityBehavior.GetType() == typeof(Turret)) SetupTurret(_abilityBehavior, selectedAbility);
        }

        private void SetupTurret(AbilityBehavior _abilityBehavior, Ability _selectedAbility)
        {
            onCombatantAbilitySpawn(_abilityBehavior);
            Turret turret = (Turret)_abilityBehavior;

            List<GridBlock> attackRadius = new List<GridBlock>();

            List<GridBlock> neighbors = patternHandler.GetPattern(turret.myBlock, null, GridPattern.Neighbors, _selectedAbility.amountOfNeighborBlocksAffected);
            foreach (GridBlock gridBlock in neighbors)
            {
                attackRadius.Add(gridBlock);
            }
            turret.SetupAttackRadius(attackRadius);
            turret.damage = _selectedAbility.baseAbilityAmount;
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
            bool isCriticalHit = CombatAssistant.CriticalHitCheck(currentFighter.unitInfo.stats.luck);

            float abilityAmountWStatModifier = GetStatsModifier(currentFighter, selectedAbility.abilityType,selectedAbility.baseAbilityAmount);
            float calculatedAmount = CombatAssistant.GetCalculatedAmount(abilityAmountWStatModifier, isCriticalHit);

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

            bool isNegated = CombatAssistant.IsNegatedBehavior(targetFighter, abilityBehavior);
            switch (abilityType)
            {
                case AbilityType.Melee:

                    if (currentComboLink.hitFXObjectKey != HitFXObjectKey.None)
                    {
                        abilityObjectPool.SpawnHitFX(currentComboLink.hitFXObjectKey, targetFighter.transform.position);
                    }

                    if(!isNegated) targetHealth.ChangeHealth(calculatedAmount, isCriticalHit, false);
                    AffectNeighborBlocks(abilityBehavior);
                    if (abilityBehavior != null) ActivateAbilityBehavior(abilityBehavior);

                    if(!targetFighter.unitStatus.Equals(new UnitStatus()))
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
                    if (isNegated) break;
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

            float damageMod = GetStatsModifier(currentFighter, selectedAbility.abilityType, selectedAbility.baseAbilityAmount);

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

            AffectNeighborBehavior(currentFighter, amountToAffect, childBehavior, currentFighter.selectedTarget, damageMod);
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
            List<GridBlock> neighbors = patternHandler.GetPattern(centerBlock, null, currentFighter.selectedAbility.patternOfBlocksAffected, _amountOfNeighbors);

            if (_childBehavior != null)
            {
                float childAbilityAmount = _baseAbilityAmount / 3f;
                _childBehavior.SetupAbility(null, null, childAbilityAmount, false, 3);
            }

            foreach (GridBlock gridBlock in neighbors)
            {
                if (gridBlock == null) continue;
                Fighter contestedFighter = gridBlock.contestedFighter;
                bool isNegated = CombatAssistant.IsNegatedBehavior(contestedFighter, _childBehavior);
                if (contestedFighter != null)
                {
                    if (contestedFighter == _attacker) continue;
                    
                    bool isChildBehaviorNull = _childBehavior == null;
                    if (isNegated) continue;

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
                statsModifier = _caster.unitInfo.stats.strength - 10f;
            }
            else if (_abilityType == AbilityType.Cast)
            {
                statsModifier = _caster.unitInfo.stats.skill - 10f;
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

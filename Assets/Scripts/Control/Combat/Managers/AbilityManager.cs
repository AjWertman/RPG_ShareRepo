using RPGProject.Combat;
using RPGProject.Combat.Grid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Control.Combat
{
    public class AbilityManager : MonoBehaviour
    {
        Pathfinder pathfinder = null;
        AbilityObjectPool abilityObjectPool = null;


        private void Awake()
        {
            pathfinder = FindObjectOfType<Pathfinder>();
            abilityObjectPool = FindObjectOfType<AbilityObjectPool>();
        }

        public void SetupAbility(Ability _selectedAbility)
        {
            AbilityBehavior _abilityBehavior = null;
            if (_abilityBehavior == null) return;
            _abilityBehavior.gameObject.SetActive(true);
            _abilityBehavior.PerformAbilityBehavior();

            if (_abilityBehavior.GetType() == typeof(Turret))
            {
                Turret turret = (Turret)_abilityBehavior;

                List<GridBlock> attackRadius = new List<GridBlock>();
                Pathfinder pathfinder = FindObjectOfType<Pathfinder>();

                foreach (GridBlock gridBlock in pathfinder.GetNeighbors(turret.myBlock, _selectedAbility.amountOfNeighborBlocksAffected))
                {
                    attackRadius.Add(gridBlock);
                }
                turret.SetupAttackRadius(attackRadius);
            }
        }

        //private AbilityBehavior GetAbilityBehavior(bool isCriticalHit, float calculatedAmount)
        //{
        //    AbilityBehavior newBehavior = abilityObjectPool.GetAbilityInstance(currentAbilityObjectKey);
        //    newBehavior.SetupAbility(this, selectedTarget, calculatedAmount, isCriticalHit, selectedAbility.abilityLifetime);

        //    if (currentComboLink != null)
        //    {
        //        if (currentComboLink.spawnLocationOverride != SpawnLocation.None)
        //        {
        //            newBehavior.SetSpawnLocation(currentComboLink.spawnLocationOverride);
        //        }
        //    }

        //    return newBehavior;
        //}
    }
}

using RPGProject.Combat.Grid;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Combat
{
    public class BattleTeleporter : AbilityBehavior, CombatTarget
    {
        public BattleTeleporter linkedTeleporter = null;
        public GridBlock myBlock = null;
        public List<GridBlock> neighborBlocks = new List<GridBlock>();

        public bool canUse = false;

        public Transform GetAimTransform()
        {
            return null;
        }

        public override void PerformAbilityBehavior()
        {
            myBlock = (GridBlock)target;
            myBlock.activeAbility = this;

            if (linkedTeleporter != null) return;
            BattleTeleporter[] battleTeleporters = FindObjectsOfType<BattleTeleporter>();
            int length = battleTeleporters.Length;

            if (length == 2)
            {
                foreach(BattleTeleporter battleTeleporter in battleTeleporters)
                {
                    if (battleTeleporter != this)
                    {
                        battleTeleporter.linkedTeleporter = this;
                        linkedTeleporter = battleTeleporter;
                    }
                }
            }
        }

        public override void OnAbilityDeath()
        {
            myBlock.activeAbility = null;
            linkedTeleporter = null;
            myBlock = null;
            base.OnAbilityDeath();
        }
    }
}

using RPGProject.Combat.Grid;
using System;
using UnityEngine;

namespace RPGProject.Combat
{
    public class BattleTeleporter : AbilityBehavior, CombatTarget
    {
        public BattleTeleporter linkedTeleporter = null;
        public GridBlock teleportBlock = null;
        //[SerializeField] GameObject pointAGameObject = null;
        //[SerializeField] GameObject pointBGameObject = null;

        //TeleporterPoint pointA = new TeleporterPoint();
        //TeleporterPoint pointB = new TeleporterPoint();

        public bool canUse = false;

        public Transform GetAimTransform()
        {
            return null;
        }

        public override void PerformAbilityBehavior()
        {
            teleportBlock = (GridBlock)target;
            teleportBlock.activeAbility = this;

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
            teleportBlock.activeAbility = null;
            linkedTeleporter = null;
            teleportBlock = null;
            base.OnAbilityDeath();
        }

    }
}


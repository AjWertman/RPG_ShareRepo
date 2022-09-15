using RPGProject.Progression;
using System;
using UnityEngine;

namespace RPGProject.Combat
{
    public class Buff : AbilityBehavior
    {
        [SerializeField] Ability[] abilitiesToTeach;
        [SerializeField] StatBuffInfo[] statBuffs;

        public override void PerformAbilityBehavior()
        {
            if (statBuffs == null || statBuffs.Length <= 0) return;

            foreach(StatBuffInfo statBuff in statBuffs)
            {
                targetFighter.unitInfo.stats.BuffStat(statBuff.statType, statBuff.amountToChange);
            }
            foreach(Ability ability in abilitiesToTeach)
            {
                targetFighter.unitInfo.AddAbility(ability, true);
            }
        }

        public override void OnAbilityDeath()
        {
            targetFighter.unitInfo.ResetStatsToStartValues();

            foreach (Ability ability in abilitiesToTeach)
            {
                targetFighter.unitInfo.AddAbility(ability, false);
            }

            base.OnAbilityDeath();
        }

        [Serializable]
        private struct StatBuffInfo
        {
            public StatType statType;
            public int amountToChange;
        }
    }
}

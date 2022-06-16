using System;
using UnityEngine;

namespace RPGProject.Combat
{
    [Serializable]
    public class ComboLink
    {
        [SerializeField] string animationID = "";

        [SerializeField] int comboPointCost = 1;

        [SerializeField] float manaCost = 0f;
        [SerializeField] float enduranceCost = 0f;

        [SerializeField] AbilityObjectKey abilityObjectKey = AbilityObjectKey.None;
        [SerializeField] SpawnLocation spawnLocationOverride = SpawnLocation.None;
        [SerializeField] AudioClip abilityClip = null;

        //Refactor?
        //[SerializeField] Buff buffToApply = null;
        //[SerializeField] float chanceToApplyBuff = 0f;

        public string GetAnimationID()
        {
            return animationID;
        }

        public int GetComboPointCost()
        {
            return comboPointCost;
        }

        public float GetManaCost()
        {
            return manaCost;
        }

        public float GetEnduranceCost()
        {
            return enduranceCost;
        }

        public AbilityObjectKey GetAbilityObjectKey()
        {
            return abilityObjectKey;
        }

        public SpawnLocation GetSpawnLocationOverride()
        {
            return spawnLocationOverride;
        }

        public AudioClip GetAbilityClip()
        {
            return abilityClip;
        }
    }
}

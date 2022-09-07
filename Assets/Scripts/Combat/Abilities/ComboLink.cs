using System;
using UnityEngine;

namespace RPGProject.Combat
{
    /// <summary>
    /// Data of a combo and its behaviors.
    /// </summary>
    [Serializable]
    public class ComboLink
    {
        public string animationID = "";

        public int comboPointCost = 1;

        public AbilityObjectKey abilityObjectKey = AbilityObjectKey.None;
        public HitFXObjectKey hitFXObjectKey = HitFXObjectKey.None;
        public SpawnLocation spawnLocationOverride = SpawnLocation.None;
        public AudioClip abilityClip = null;

        //Refactor
        //[SerializeField] Buff buffToApply = null;
        //[SerializeField] float chanceToApplyBuff = 0f;
    }
}

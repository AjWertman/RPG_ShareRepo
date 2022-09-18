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

        public AbilityBehavior abilityBehavior = null;
        public HitFXObjectKey hitFXObjectKey = HitFXObjectKey.None;
        public SpawnLocation spawnLocationOverride = SpawnLocation.None;
        public AudioClip abilityClip = null;
    }
}

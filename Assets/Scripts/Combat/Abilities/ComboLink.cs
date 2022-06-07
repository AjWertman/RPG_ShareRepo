using System;
using UnityEngine;

namespace RPGProject.Combat
{
    [Serializable]
    public class ComboLink
    {
        [SerializeField] string animationID = "";
        //Refactor? was to be used to limit creating own combos
        [SerializeField] int comboPointCost = 1;
        [SerializeField] float manaCost = 0f;
        [SerializeField] float enduranceCost = 0f;

        [SerializeField] AbilityObjectKey abilityObjectKey = AbilityObjectKey.None;
        [SerializeField] GameObject hitFXPrefab = null;

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
    }
}

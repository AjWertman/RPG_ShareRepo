using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Combat
{
    public enum AbilityResource {None, Health, Endurance, Mana }
    public enum AbilityType { Melee, Cast, Copy, Buff, InstaHit }
    public enum TargetingType { EnemiesOnly, PlayersOnly, SelfOnly, Everyone }

    [CreateAssetMenu(fileName = "Ability", menuName = "Ability/Create New Ability", order = 1)]
    public class Ability : ScriptableObject
    {
        [SerializeField] List<ComboLink> combo = new List<ComboLink>();
        [SerializeField] AbilityResource abilityResource = AbilityResource.None;

        public int actionPointsCost = -1;
        public bool requiresTarget = false;
        public float attackRange = 5f;
        //decent attack for ranged attacks = 6-7  

        public List<ComboLink> GetCombo()
        {
            return combo;
        }

        /// <summary>
        /// 
        /// </summary>

        [Header("AbilityInfo")]
        [SerializeField] string abilityName = "";
        [SerializeField] int requiredLevel = 1;
        [SerializeField] int abilityLifetime = 1;
        [SerializeField] float baseAbilityAmount = 40;

        [TextArea(10, 10)]
        [SerializeField] string description = "";

        [Header("Design")]
        [SerializeField] bool shouldExpand = false;
        [SerializeField] Color buttonColor = Color.white;
        [SerializeField] Color textColor = Color.black;

        [Header("Behaviors")]

        [SerializeField] AbilityType abilityType = AbilityType.Melee;
        [SerializeField] TargetingType targetingType = TargetingType.Everyone;

        [SerializeField] bool canTargetAll = false;

        public string GetAbilityName()
        {
            return abilityName;
        }

        public int GetRequiredLevel()
        {
            return requiredLevel;
        }

        public int GetAbilityLifetime()
        {
            return abilityLifetime;
        }

        public float GetBaseAbilityAmount()
        {
            return baseAbilityAmount;
        }

        public string GetDescription()
        {
            return description;
        }

        public bool ShouldExpand()
        {
            return shouldExpand;
        }

        public Color GetButtonColor()
        {
            return buttonColor;
        }

        public Color GetTextColor()
        {
            return textColor;
        }

        public AbilityResource GetAbilityResource()
        {
            return abilityResource;
        }

        public AbilityType GetAbilityType()
        {
            return abilityType;
        }

        public TargetingType GetTargetingType()
        {
            return targetingType;
        }

        public bool CanTargetAll()
        {
            return canTargetAll;
        }
    }
}

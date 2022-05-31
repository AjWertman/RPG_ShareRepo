using UnityEngine;

namespace RPGProject.Combat
{
    public enum AbilityResource { Physical, Magic }
    public enum AbilityType { Melee, Cast, Copy }
    public enum TargetingType { EnemiesOnly, PlayersOnly, SelfOnly, Everyone }

    [CreateAssetMenu(fileName = "Ability", menuName = "Ability/Create New Ability", order = 1)]
    public class Ability : ScriptableObject
    {
        //Refactor

        [Header("AbilityInfo")]
        [SerializeField] string abilityName = "";
        [SerializeField] int requiredLevel = 1;
        [SerializeField] string animatorTrigger = "";
        //[SerializeField] float moveDuration = 1.5f;
        [SerializeField] int abilityLifetime = 1;
        [SerializeField] float baseAbilityAmount = 40;
        [SerializeField] float manaCost = 0;
        [TextArea(10, 10)]
        [SerializeField] string description = "";

        [Header("Design")]
        [SerializeField] bool shouldExpand = false;
        [SerializeField] Color buttonColor = Color.white;
        [SerializeField] Color textColor = Color.black;
        [SerializeField] GameObject abilityPrefab = null;
        [SerializeField] GameObject hitFXPrefab = null;

        [Header("Behaviors")]
        [SerializeField] AbilityResource abilityResource = AbilityResource.Physical;
        [SerializeField] AbilityType abilityType = AbilityType.Melee;
        [SerializeField] TargetingType targetingType = TargetingType.Everyone;
        [SerializeField] bool isInstaHit = false;
        [SerializeField] bool canTargetAll = false;
        [SerializeField] bool isHeal = false;

        public string GetAbilityName()
        {
            return abilityName;
        }

        public int GetRequiredLevel()
        {
            return requiredLevel;
        }

        public string GetAnimatorTrigger()
        {
            return animatorTrigger;
        }

        public int GetAbilityLifetime()
        {
            return abilityLifetime;
        }

        public float GetBaseAbilityAmount()
        {
            return baseAbilityAmount;
        }

        public float GetManaCost()
        {
            return manaCost;
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

        public GameObject GetAbilityPrefab()
        {
            return abilityPrefab;
        }

        public GameObject GetHitFXPrefab()
        {
            return hitFXPrefab;
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

        public bool IsInstaHit()
        {
            return isInstaHit;
        }

        public bool CanTargetAll()
        {
            return canTargetAll;
        }

        public bool IsHeal()
        {
            return isHeal;
        }
    }
}

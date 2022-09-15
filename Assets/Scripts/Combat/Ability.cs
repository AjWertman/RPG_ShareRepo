using RPGProject.Combat.Grid;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Combat
{
    /// <summary>
    /// A scriptable object containing the data of an ability that can be used in combat.
    /// </summary>
    [CreateAssetMenu(fileName = "Ability", menuName = "Ability/Create New Ability", order = 1)]
    public class Ability : ScriptableObject
    {
        [Header("AbilityInfo")]
        public string abilityName = "";
        public int requiredLevel = 1;

        /// <summary>
        /// Amount of turns does this ability will live before dissipating.
        /// </summary>
        public int abilityLifetime = 1;

        /// <summary>
        /// Amount the ability will affect a target when behavior is executed
        /// before other modifications are applied.
        /// </summary>
        public float baseAbilityAmount = 40f;
        public int baseAgroPercentageAmount = 40;
        [TextArea(10, 10)] public string description = "";

        [Header("Design")]     
        ///<summary>
        /// If true, the ability behavior will scale to the target.
        /// </summary>
        public bool shouldExpand = false;
        public Color buttonColor = Color.white;
        public Color textColor = Color.black;

        [Header("Behaviors")]
        public List<ComboLink> combo = new List<ComboLink>();
        public AbilityType abilityType = AbilityType.Melee;
        public TargetingType targetingType = TargetingType.Everyone;
        public CharacterBiology requiredBiology = CharacterBiology.None;
        public int requiredTargetAmount = 1;
        public float attackRange = 5f;
        public int energyPointsCost = -1;
        public int cooldown = 2;
        public bool requiresTarget = false;
        public bool canTargetAll = false;

        public GridPattern patternOfBlocksAffected = GridPattern.None;
        public int amountOfNeighborBlocksAffected = 0;
    }

    /// <summary>
    /// The type of an ability which changes how an ability is initialized and performed.
    /// </summary>
    public enum AbilityType { Melee, Cast, Copy, InstaHit }

    /// <summary>
    /// The type of CombatTarget that an ability can target
    /// </summary>
    public enum TargetingType { EnemiesOnly, PlayersOnly, SelfOnly, Everyone, GridBlocksOnly, Everything }
}

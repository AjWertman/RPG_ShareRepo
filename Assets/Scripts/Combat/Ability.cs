﻿using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Combat
{
    public enum AbilityType { Melee, Cast, Copy, Buff, InstaHit }
    public enum TargetingType { EnemiesOnly, PlayersOnly, SelfOnly, Everyone }

    [CreateAssetMenu(fileName = "Ability", menuName = "Ability/Create New Ability", order = 1)]
    public class Ability : ScriptableObject
    {
        [Header("AbilityInfo")]
        public string abilityName = "";
        public int requiredLevel = 1;
        public int abilityLifetime = 1;
        public float baseAbilityAmount = 40;
        [TextArea(10, 10)] public string description = "";

        [Header("Design")]
        public bool shouldExpand = false;
        public Color buttonColor = Color.white;
        public Color textColor = Color.black;

        [Header("Behaviors")]
        public List<ComboLink> combo = new List<ComboLink>();
        public AbilityType abilityType = AbilityType.Melee;
        public TargetingType targetingType = TargetingType.Everyone;
        public float attackRange = 5f;
        public int actionPointsCost = -1;
        public bool requiresTarget = false;
        public bool canTargetAll = false;
    }
}

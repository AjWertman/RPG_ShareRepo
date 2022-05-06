﻿using UnityEngine;

public enum AbilityType { Physical, Magical}
public enum SpellType { Melee, Ranged, Static, RenCopy}
public enum TargetingType { EnemysOnly, PlayersOnly, SelfOnly, Everyone}

[CreateAssetMenu(fileName = "Ability", menuName = "Ability/Create New Ability", order = 1)]
public class Ability : ScriptableObject
{
    [Header("AbilityInfo")]
    [SerializeField] public string abilityName = "";
    [SerializeField] public int requiredLevel = 1;
    [SerializeField] public CharacterEmblem emblem = CharacterEmblem.Normal;
    [SerializeField] public AbilityType abilityType = AbilityType.Physical;
    [SerializeField] public string animatorTrigger = "";
    [SerializeField] public float moveDuration = 1.5f;
    [SerializeField] public float baseAbilityAmount = 40;
    [SerializeField] public float soulWellCost = 0;
    [TextArea(10, 10)]
    [SerializeField] public string spellDescription = "";

    [Header("Design")]
    [SerializeField] public bool shouldExpand = false;
    [SerializeField] public Color buttonColor = Color.white;
    [SerializeField] public Color textColor = Color.black;
    [SerializeField] public GameObject spellPrefab = null;
    [SerializeField] public GameObject hitFXPrefab = null;
    [SerializeField] public GameObject renSpellPrefab = null;
    [SerializeField] public GameObject renHitFXPrefab = null;

    [Header("Behaviors")]
    [SerializeField] public SpellType spellType;
    [SerializeField] public TargetingType targetingType;
    [SerializeField] public bool isInstaHit = false;
    [SerializeField] public bool canTargetAll = false;
    [SerializeField] public bool isHeal = false;

    public bool CanBeRenCopied()
    {
        if(renSpellPrefab == null)
        {
            return false;
        }
        return true;
    }
}

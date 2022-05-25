using UnityEngine;

public enum AbilityType { Physical, Magical}
public enum SpellType { Melee, Ranged, Static, Copy}
public enum TargetingType { EnemiesOnly, PlayersOnly, SelfOnly, Everyone}

[CreateAssetMenu(fileName = "Ability", menuName = "Ability/Create New Ability", order = 1)]
public class Ability : ScriptableObject
{
    [Header("AbilityInfo")]
    [SerializeField] public string abilityName = "";
    [SerializeField] public int requiredLevel = 1;
    [SerializeField] public AbilityType abilityType = AbilityType.Physical;
    [SerializeField] public string animatorTrigger = "";
    [SerializeField] public float moveDuration = 1.5f;
    [SerializeField] public float baseAbilityAmount = 40;
    [SerializeField] public float manaCost = 0;
    [TextArea(10, 10)]
    [SerializeField] public string spellDescription = "";

    [Header("Design")]
    [SerializeField] public bool shouldExpand = false;
    [SerializeField] public Color buttonColor = Color.white;
    [SerializeField] public Color textColor = Color.black;
    [SerializeField] public GameObject spellPrefab = null;
    [SerializeField] public GameObject hitFXPrefab = null;

    [Header("Behaviors")]
    [SerializeField] public SpellType spellType;
    [SerializeField] public TargetingType targetingType;
    [SerializeField] public bool isInstaHit = false;
    [SerializeField] public bool canTargetAll = false;
    [SerializeField] public bool isHeal = false;
}

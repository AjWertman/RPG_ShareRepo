using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "Unit/Create New Unit", order = 1)]
[System.Serializable]
public class Character : ScriptableObject
{
    [SerializeField] string characterName  = "";
    [SerializeField] int age = 0;
    [SerializeField] CharacterEmblem emblem = CharacterEmblem.Normal;
    [SerializeField] string affiliation = "";
    [TextArea(10,10)][SerializeField] string summaryText = "";
    
    [Header("Combat Design")]
    [SerializeField] BattleUnit unitPrefab = null;
    [SerializeField] Stats baseStats;
    [SerializeField] Ability basicAttack = null;
    [SerializeField] Ability[] spells = null;
    [SerializeField] float xpAward = 100;

    [Header("UI Design")]
    [SerializeField] Sprite backgroundImage = null;
    [SerializeField] Sprite emblemImage = null;
    [SerializeField] Sprite faceImage = null;
    [SerializeField] Sprite fullBodyImage = null;
    [SerializeField] Sprite bookImage = null;

    public string GetName()
    {
        return characterName;
    }

    public string GetSummary()
    {
        return summaryText;
    }

    public int GetAge()
    {
        return age;
    }

    public CharacterEmblem GetCharacterEmblem()
    {
        return emblem;
    }

    public string GetAffiliation()
    {
        return affiliation;
    }

    public BattleUnit GetBattleUnit()
    {
        return unitPrefab;
    }

    public Sprite GetBackgroundImage()
    {
        return backgroundImage;
    }

    public Sprite GetEmblemImage()
    {
        return emblemImage;
    }

    public Sprite GetFaceImage()
    {
        return faceImage;
    }

    public Sprite GetFullBodyImage()
    {
        return fullBodyImage;
    }

    public Sprite GetBookImage()
    {
        return bookImage;
    }

    public Stats GetBaseStats()
    {
        return baseStats;
    }

    public Ability GetBasicAttack()
    {
        return basicAttack;
    }

    public Ability[] GetSpells()
    {
        return spells;
    }    

    public float GetXPAward()
    {
        return xpAward;
    }
}
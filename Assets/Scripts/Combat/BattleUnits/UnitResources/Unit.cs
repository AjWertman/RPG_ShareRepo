using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "Character/Create New Unit", order = 1)]
public class Unit : ScriptableObject
{
    [SerializeField] string unitName = "";
    [SerializeField] int baseLevel = 1;
    [SerializeField] CharacterMeshKey characterMeshKey = CharacterMeshKey.Aj;

    [Header("Combat")]
    [SerializeField] Stats baseStats;
    [SerializeField] Ability basicAttack = null;
    [SerializeField] Ability[] abilities = null;
    [SerializeField] float xpAward = 100;

    [Header("UI Design")]
    [SerializeField] Sprite backgroundImage = null;
    [SerializeField] Sprite faceImage = null;
    [SerializeField] Sprite fullBodyImage = null;

    public string GetUnitName()
    {
        return unitName;
    }
    
    public int GetBaseLevel()
    {
        return baseLevel;
    }

    public CharacterMeshKey GetCharacterMeshKey()
    {
        return characterMeshKey;
    }

    public Sprite GetBackgroundImage()
    {
        return backgroundImage;
    }

    public Sprite GetFaceImage()
    {
        return faceImage;
    }

    public Sprite GetFullBodyImage()
    {
        return fullBodyImage;
    }

    public Stats GetBaseStats()
    {
        return baseStats;
    }

    public Ability GetBasicAttack()
    {
        return basicAttack;
    }

    public Ability[] GetAbilities()
    {
        return abilities;
    }    

    public float GetXPAward()
    {
        return xpAward;
    }
}
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "Character/Create New Unit", order = 1)]
public class Unit : ScriptableObject
{
    [SerializeField] string unitName = "";
    [SerializeField] CharacterMeshKey characterMeshKey = CharacterMeshKey.Aj;

    [Header("Combat")]
    [SerializeField] Stats baseStats;
    [SerializeField] Ability basicAttack = null;
    [SerializeField] Ability[] spells = null;
    [SerializeField] float xpAward = 100;

    [Header("UI Design")]
    [SerializeField] Sprite backgroundImage = null;
    [SerializeField] Sprite faceImage = null;
    [SerializeField] Sprite fullBodyImage = null;

    public string GetName()
    {
        return unitName;
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

    public Ability[] GetSpells()
    {
        return spells;
    }    

    public float GetXPAward()
    {
        return xpAward;
    }
}
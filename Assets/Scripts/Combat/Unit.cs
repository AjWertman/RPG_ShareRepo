using UnityEngine;

namespace RPGProject.Combat
{
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
    }
}
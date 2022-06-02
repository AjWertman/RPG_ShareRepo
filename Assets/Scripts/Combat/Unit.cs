using RPGProject.Core;
using RPGProject.Progression;
using UnityEngine;

namespace RPGProject.Combat
{
    [CreateAssetMenu(fileName = "New Unit", menuName = "Character/Create New Unit", order = 1)]
    public class Unit : ScriptableObject
    {
        [SerializeField] string unitName = "";
        [SerializeField] int baseLevel = 1;
        [SerializeField] CharacterKey characterKey = CharacterKey.Aj;

        [SerializeField] BattleUnitResources battleUnitResources = null;
        [SerializeField] Stats stats = new Stats();
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

        public CharacterKey GetCharacterKey()
        {
            return characterKey;
        }

        public BattleUnitResources GetBattleUnitResources()
        {
            return battleUnitResources;
        }

        public Stats GetStats()
        {
            return stats;
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
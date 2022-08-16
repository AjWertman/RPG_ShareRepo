using RPGProject.Combat.AI;
using RPGProject.Core;
using RPGProject.Progression;
using UnityEngine;

namespace RPGProject.Combat
{
    [CreateAssetMenu(fileName = "New Unit", menuName = "Character/Create New Unit", order = 1)]
    public class Unit : ScriptableObject
    {
        public string unitName = "";
        public int baseLevel = 1;
        public CharacterKey characterKey = CharacterKey.None;
        public AIBattleType combatAIType = AIBattleType.mDamage;

        public UnitResources unitResources = new UnitResources();
        public Stats stats = new Stats();

        public Ability basicAttack = null;
        public Ability[] abilities = null;
    }
}
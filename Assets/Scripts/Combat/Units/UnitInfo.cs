using RPGProject.Core;
using RPGProject.Progression;
using System;

namespace RPGProject.Combat
{
    /// <summary>
    /// Information about a combatant
    /// </summary>
    [Serializable]
    public struct UnitInfo
    {
        public string unitName;
        public CharacterKey characterKey;
        public int unitLevel;

        public Stats stats;

        public Ability basicAttack;
        public Ability[] abilities;

        public bool isPlayer;

        public UnitInfo(string _unitName, CharacterKey _characterKey, int _unitLevel, bool _isPlayer,
            Stats _stats, Ability _basicAttack, Ability[] _abilities)
        {
            unitName = _unitName;
            characterKey = _characterKey;
            unitLevel = _unitLevel;
            isPlayer = _isPlayer;
            stats = _stats;
            basicAttack = _basicAttack;
            abilities = _abilities;
        }
   
        public void ResetUnitInfo()
        {
            characterKey = CharacterKey.None;
            unitName = "";
            unitLevel = 0;
            isPlayer = false;
            stats.ResetStats();
            basicAttack = null;
            abilities = null;
        }
    }
}

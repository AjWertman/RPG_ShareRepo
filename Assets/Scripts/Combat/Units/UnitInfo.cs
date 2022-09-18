using RPGProject.Core;
using RPGProject.Progression;
using System;
using System.Collections.Generic;

namespace RPGProject.Combat
{
    /// <summary>
    /// Information about a combatant.
    /// </summary>
    [Serializable]
    public class UnitInfo
    {
        public string unitName = "";
        public CharacterKey characterKey = CharacterKey.None;
        public int unitLevel = 0;

        public Stats stats = new Stats();
        Stats startingStats = new Stats();

        public Ability basicAttack = null;
        public List<Ability> abilities = new List<Ability>();

        public bool isPlayer = true;
        public bool isAI = false;

        public UnitInfo(string _unitName, CharacterKey _characterKey, int _unitLevel, bool _isPlayer, bool _isAI,
            Stats _stats, Ability _basicAttack, Ability[] _abilities)
        {
            unitName = _unitName;
            characterKey = _characterKey;
            unitLevel = _unitLevel;
            isAI = _isAI;
            isPlayer = _isPlayer;
            stats = _stats;
            startingStats = stats;
            basicAttack = _basicAttack;

            if (_abilities == null || _abilities.Length <= 0) return;
            foreach(Ability ability in _abilities)
            {
                AddAbility(ability, true);
            }
        }

        public void AddAbility(Ability _ability, bool _shouldAdd)
        {
            if (_shouldAdd)
            {
                if (abilities.Contains(_ability)) return;
                abilities.Add(_ability);
            }
            else
            {
                if (!abilities.Contains(_ability)) return;
                abilities.Remove(_ability);
            }
        }

        public void ResetStatsToStartValues()
        {
            stats = startingStats;
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

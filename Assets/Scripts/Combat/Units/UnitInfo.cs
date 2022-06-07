using RPGProject.Core;
using RPGProject.Progression;
using System;
using UnityEngine;

namespace RPGProject.Combat
{
    [Serializable]
    public class UnitInfo
    {
        [SerializeField] string unitName = "";
        [SerializeField] CharacterKey characterKey = CharacterKey.None;
        [SerializeField] int unitLevel = 0;

        [SerializeField] Stats stats = new Stats();

        Ability basicAttack = null;
        Ability[] abilities = null;

        bool isPlayer = true;

        public void SetUnitInfo(string _unitName, CharacterKey _characterKey, int _unitLevel, bool _isPlayer,
            Stats _stats, Ability _basicAttack, Ability[] _abilities)
        {
            unitName = _unitName;
            characterKey = _characterKey;
            unitLevel = _unitLevel;
            isPlayer = _isPlayer;
            stats.SetStats(_stats);
            SetAbilities(_basicAttack, _abilities);
        }

        public void SetUnitInfo(UnitInfo _unitInfo)
        {
            unitName = _unitInfo.GetUnitName();
            unitLevel = _unitInfo.GetUnitLevel();
            isPlayer = _unitInfo.IsPlayer();
            stats.SetStats(_unitInfo.GetStats());
            SetAbilities(_unitInfo.GetBasicAttack(), _unitInfo.GetAbilities());
        }

        public void SetUnitLevel(int _unitLevel)
        {
            unitLevel = _unitLevel;
        }

        public void SetAbilities(Ability _basicAttack, Ability[] _ability)
        {
            basicAttack = _basicAttack;
            abilities = _ability;
        }

        public CharacterKey GetCharacterKey()
        {
            return characterKey;
        }

        public string GetUnitName()
        {
            return unitName;
        }

        public int GetUnitLevel()
        {
            return unitLevel;
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

        public bool IsPlayer()
        {
            return isPlayer;
        }

        public void ResetUnitInfo()
        {
            characterKey = CharacterKey.None;
            unitName = "";
            unitLevel = 0;
            isPlayer = false;
            stats.ResetStats();
            SetAbilities(null, null);
        }
    }
}

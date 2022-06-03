using RPGProject.Core;
using RPGProject.Progression;
using System;
using UnityEngine;

namespace RPGProject.Combat
{
    [Serializable]
    public class BattleUnitInfo
    {
        [SerializeField] CharacterKey characterKey = CharacterKey.Aj;
        [SerializeField] string unitName = "";
        [SerializeField] int unitLevel = 0;

        [SerializeField] Stats stats = new Stats();

        Ability basicAttack = null;
        Ability[] abilities = null;

        bool isPlayer = true;

        public void SetBattleUnitInfo(CharacterKey _characterKey, int _unitLevel, bool _isPlayer,
            Stats _stats, Ability _basicAttack, Ability[] _abilities)
        {
            characterKey = _characterKey;
            unitName = characterKey.ToString();
            unitLevel = _unitLevel;
            isPlayer = _isPlayer;
            stats.SetStats(_stats);
            SetAbilities(_basicAttack, _abilities);
        }

        public void SetUnitInfo(BattleUnitInfo _battleUnitInfo)
        {
            unitName = _battleUnitInfo.GetUnitName();
            unitLevel = _battleUnitInfo.GetUnitLevel();
            isPlayer = _battleUnitInfo.IsPlayer();
            stats.SetStats(_battleUnitInfo.GetStats());
            SetAbilities(_battleUnitInfo.GetBasicAttack(), _battleUnitInfo.GetAbilities());
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

        public void ResetBattleUnitInfo()
        {
            unitName = "";
            unitLevel = 0;
            isPlayer = false;
            stats.ResetStats();
            SetAbilities(null, null);
        }
    }
}

using System;
using UnityEngine;

[Serializable]
public class BattleUnitInfo
{
    [SerializeField] string unitName = "";
    [SerializeField] int unitLevel = 0;

    [SerializeField] Stats stats;
    Stats startingStats;

    Ability basicAttack = null;
    Ability[] abilities = null;

    bool isPlayer = true;

    public void SetBattleUnitInfo(string _unitName, int _unitLevel, bool _isPlayer,
        Stats _startingStats, Ability _basicAttack, Ability[] _abilities)
    {
        unitName = _unitName;
        unitLevel = _unitLevel;
        isPlayer = _isPlayer;
        startingStats = _startingStats;
        SetStats(startingStats);
        SetAbilities(_basicAttack, _abilities);
    }

    public void SetUnitLevel(int _unitLevel)
    {
        unitLevel = _unitLevel;
    }

    public void SetStats(Stats _stats)
    {
        stats = _stats;
    }

    public void SetAbilities(Ability _basicAttack, Ability[] _ability)
    {
        basicAttack = _basicAttack;
        abilities = _ability;
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
        startingStats.ResetStats();
        SetAbilities(null, null);
    }
}

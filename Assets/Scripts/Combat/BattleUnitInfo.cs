using System;
using UnityEngine;

[Serializable]
public class BattleUnitInfo
{
    [SerializeField] string unitName = "";
    [SerializeField] int unitLevel = 0;

    [SerializeField] Stats stats = new Stats();
    //Stats startingStats;

    Ability basicAttack = null;
    Ability[] abilities = null;

    bool isPlayer = true;

    //Refactor
    [SerializeField] Sprite faceImage = null;

    public void SetBattleUnitInfo(string _unitName, int _unitLevel, bool _isPlayer,
        Stats _stats, Ability _basicAttack, Ability[] _abilities)
    {
        unitName = _unitName;
        unitLevel = _unitLevel;
        isPlayer = _isPlayer;
        stats.SetStats(_stats.GetAllStats());
        SetAbilities(_basicAttack, _abilities);
    }

    public void SetBattleUnitInfo(BattleUnitInfo _battleUnitInfo)
    {
        unitName = _battleUnitInfo.GetUnitName();
        unitLevel = _battleUnitInfo.GetUnitLevel();
        isPlayer = _battleUnitInfo.IsPlayer();
        stats.SetStats(_battleUnitInfo.GetStats().GetAllStats());
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

    public void SetFaceImage(Sprite _faceImage)
    {
        faceImage = _faceImage;
    }

    public Sprite GetFaceImage()
    {
        return faceImage;
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

using UnityEngine;

public enum StatType { Stamina, Spirit, Strength, Skill, Armor, Resistance, Speed, Luck}

[System.Serializable]
public class Stat
{
    [SerializeField] StatType statType = StatType.Stamina;
    [Range(10, 100)] [SerializeField] int statLevel = 10;
    [SerializeField] int levelUpPercent = 50;

    public StatType GetStatType()
    {
        return statType;
    }

    public int GetStatLevel()
    {
        return statLevel;
    }
    
    public void SetStatLevel(int newLevel)
    {
        statLevel = newLevel;
    }

    public int GetLevelUpPercent()
    {
        return levelUpPercent;
    }

    public void IncreaseLevel()
    { 
         statLevel++;
    }
}

[System.Serializable]
public class Stats
{
    [SerializeField] Stat[] stats = null;
    int baseStatLevel = 10;

    public void SetStats(Stat[] statsToSet)
    {
        stats = statsToSet;
    }

    public Stat[] GetStats()
    {
        return stats;
    }

    public void SetStat(Stat statToSet, int newAmount)
    {
        foreach(Stat stat in stats)
        {
            if(statToSet.GetStatType() == stat.GetStatType())
            { 
                stat.SetStatLevel(newAmount);
            }
        }
    }
    
    public Stat GetStat(StatType statType)
    {
        foreach (Stat stat in stats)
        {
            if(stat.GetStatType() == statType)
            {
                return stat;
            }
        }

        return null;
    }

    public float GetSpecificStatLevel(StatType statType)
    {
        Stat specificStat = GetStat(statType);

        if(specificStat == null)
        {
            return 0;
        }

        return specificStat.GetStatLevel();
    }

    public void ResetStats()
    {
        foreach(Stat stat in stats)
        {
            stat.SetStatLevel(baseStatLevel);
        }
    }
}

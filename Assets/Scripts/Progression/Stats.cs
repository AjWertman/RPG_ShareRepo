using System;
using UnityEngine;

namespace RPGProject.Progression
{
    public enum StatType { Stamina, Spirit, Strength, Skill, Armor, Resistance, Speed, Luck }

    [Serializable]
    public class Stats
    {
        [SerializeField] Stat[] stats = null;
        int baseStatLevel = 10;

        public void SetStats(Stats _stats)
        {
            foreach (Stat stat in stats)
            {
                int newStatLevel = (int)_stats.GetSpecificStatLevel(stat.GetStatType());
                stat.SetStatLevel(newStatLevel);
            }
        }

        public void SetStat(StatType _statToSet, int _newAmount)
        {
            foreach (Stat stat in stats)
            {
                if (_statToSet == stat.GetStatType())
                {
                    stat.SetStatLevel(_newAmount);
                }
            }
        }

        public Stat[] GetAllStats()
        {
            return stats;
        }

        public Stat GetStat(StatType _statType)
        {
            foreach (Stat stat in stats)
            {
                if (stat.GetStatType() == _statType)
                {
                    return stat;
                }
            }

            return null;
        }

        public float GetSpecificStatLevel(StatType _statType)
        {
            Stat specificStat = GetStat(_statType);

            if (specificStat == null)
            {
                return 0;
            }

            return specificStat.GetStatLevel();
        }

        public void ResetStats()
        {
            foreach (Stat stat in stats)
            {
                stat.SetStatLevel(baseStatLevel);
            }
        }
    }

    [Serializable]
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

        public void SetStatLevel(int _newLevel)
        {
            statLevel = _newLevel;
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
}
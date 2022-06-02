using System;
using UnityEngine;

namespace RPGProject.Progression
{
    public enum StatType { Stamina, Spirit, Strength, Skill, Armor, Resistance, Speed, Luck }

    [Serializable]
    public class Stats
    {
        [Range(10, 100)] [SerializeField] int stamina = 10;
        [Range(10, 100)] [SerializeField] int spirit = 10;
        [Range(10, 100)] [SerializeField] int strength = 10;
        [Range(10, 100)] [SerializeField] int skill = 10;
        [Range(10, 100)] [SerializeField] int armor = 10;
        [Range(10, 100)] [SerializeField] int resistance = 10;
        [Range(10, 100)] [SerializeField] int speed = 10;
        [Range(10, 100)] [SerializeField] int luck = 10;

        int baseStatLevel = 10;

        public int GetStat(StatType _statType)
        {
            int stat = 0;

            switch (_statType)
            {
                case StatType.Stamina:
                    stat = stamina;
                    break;

                case StatType.Spirit:
                    stat = spirit;
                    break;

                case StatType.Strength:
                    stat = strength;
                    break;

                case StatType.Skill:
                    stat = skill;
                    break;

                case StatType.Armor:
                    stat = armor;
                    break;

                case StatType.Resistance:
                    stat = resistance;
                    break;

                case StatType.Speed:
                    stat = speed;
                    break;

                case StatType.Luck:
                    stat = luck;
                    break;
            }

            return stat;
        }

        public void SetStat(StatType _statType, int _level)
        {
            switch (_statType)
            {
                case StatType.Stamina:
                    stamina = _level;
                    break;

                case StatType.Spirit:
                    spirit = _level;
                    break;

                case StatType.Strength:
                    strength = _level;
                    break;

                case StatType.Skill:
                    skill = _level;
                    break;

                case StatType.Armor:
                    armor = _level;
                    break;

                case StatType.Resistance:
                    resistance = _level;
                    break;

                case StatType.Speed:
                    speed = _level;
                    break;

                case StatType.Luck:
                    luck = _level;
                    break;
            }
        }

        public void SetStats(Stats _stats)
        {
            foreach(StatType statType in GetStatTypes())
            {
                int statLevel = _stats.GetStat(statType);
                SetStat(statType, statLevel);
            }
        }

        public void ResetStats()
        {          
            foreach (StatType statType in GetStatTypes())
            {
                SetStat(statType, baseStatLevel);
            }
        }

        private StatType[] GetStatTypes()
        {
            return (StatType[])Enum.GetValues(typeof(StatType));
        }
    }

    //[Serializable]
    //public class Stat
    //{
    //    [SerializeField] StatType statType = StatType.Stamina;
    //    [Range(10, 100)] [SerializeField] int statLevel = 10;
    //    [SerializeField] int levelUpPercent = 50;

    //    public void SetStatType(StatType _statType)
    //    {
    //        statType = _statType;
    //    }

    //    public void SetStatLevel(int _newLevel)
    //    {
    //        statLevel = _newLevel;
    //    }

    //    public void IncreaseLevel()
    //    {
    //        statLevel++;
    //    }

    //    public StatType GetStatType()
    //    {
    //        return statType;
    //    }

    //    public int GetStatLevel()
    //    {
    //        return statLevel;
    //    }

    //    public int GetLevelUpPercent()
    //    {
    //        return levelUpPercent;
    //    }
    //}
}
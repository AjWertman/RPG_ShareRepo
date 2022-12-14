using System;
using UnityEngine;

namespace RPGProject.Progression
{
    /// <summary>
    /// Used to reference a specific stat.
    /// </summary>
    public enum StatType { Stamina, Spirit, Strength, Skill, Armor, Resistance, Speed, Luck }

    /// <summary>
    /// The stats of a character.
    /// </summary>
    [Serializable]
    public struct Stats
    {
        [Range(10, 100)] public int stamina;
        [Range(10, 100)] public int spirit;
        [Range(10, 100)] public int strength;
        [Range(10, 100)] public int skill;
        [Range(10, 100)] public int armor;
        [Range(10, 100)] public int resistance;
        [Range(10, 100)] public int speed;
        [Range(10, 100)] public int luck;

        public Stats(Stats _stats)
        {
            stamina = _stats.stamina;
            spirit = _stats.spirit;
            strength = _stats.strength;
            skill = _stats.skill;
            armor = _stats.armor;
            resistance = _stats.resistance;
            speed = _stats.speed;
            luck = _stats.luck;
        }

        public void BuffStat(StatType _statType, int _amountToChange)
        {
            switch (_statType)
            {
                case StatType.Stamina:
                    stamina += _amountToChange;
                    break;
                case StatType.Spirit:
                    spirit += _amountToChange;
                    break;
                case StatType.Strength:
                    strength += _amountToChange;
                    break;
                case StatType.Skill:
                    skill += _amountToChange;
                    break;
                case StatType.Armor:
                    armor += _amountToChange;
                    break;
                case StatType.Resistance:
                    resistance += _amountToChange;
                    break;
                case StatType.Speed:
                    speed += _amountToChange;
                    break;
                case StatType.Luck:
                    luck += _amountToChange;
                    break;
            }
        }


        public int GetStatLevel(StatType _statType)
        {
            switch (_statType)
            {
                case StatType.Stamina:
                    return stamina;

                case StatType.Spirit:
                    return spirit;

                case StatType.Strength:
                    return strength;

                case StatType.Skill:
                    return skill;

                case StatType.Armor:
                    return armor;

                case StatType.Resistance:
                    return resistance;

                case StatType.Speed:
                    return speed;

                case StatType.Luck:
                    return luck;
            }

            return 0;
        }

        public void ResetStats()
        {
            stamina = 10;
            spirit = 10;
            strength = 10;
            skill = 10;
            armor = 10;
            resistance = 10;
            speed = 10;
            luck = 10;
        }
    }
}
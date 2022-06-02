using RPGProject.Progression;
using RPGProject.Saving;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Core
{
    public class PlayerTeam : MonoBehaviour, ISaveable
    {
        [SerializeField] TeamInfo[] teamInfos;
        [SerializeField] Stat[] statsTemplate;

        ProgressionHandler progressionHandler = null;

        List<Unit> playerTeam = new List<Unit>();

        private void Awake()
        {
            progressionHandler = GetComponentInChildren<ProgressionHandler>();
            PopulateTeamInfos();
        }

        private void PopulateTeamInfos()
        {
            foreach (TeamInfo teamInfo in teamInfos)
            {
                Unit newCharacter = GetCharacter(teamInfo.GetName());

                playerTeam.Add(newCharacter);

                teamInfo.SetStats(newCharacter.GetBaseStats());

                float maxHealthPoints = CalculateMaxHealthPoints(teamInfo.GetStats().GetSpecificStatLevel(StatType.Stamina));
                float maxManaPoints = CalculateMaxMana(teamInfo.GetStats().GetSpecificStatLevel(StatType.Spirit));

                //Refactor core using combat again. Move battleunitResources
                BattleUnitResources battleUnitResources = teamInfo.GetBattleUnitResources();
                battleUnitResources.SetBattleUnitResources(maxHealthPoints, maxHealthPoints, maxManaPoints, maxManaPoints);
            }
        }

        public void UpdateTeamInfo(string _name, BattleUnitResources _battleUnitResources)
        {
            foreach (TeamInfo teamInfo in teamInfos)
            {
                if (teamInfo.GetName() == _name)
                {
                    BattleUnitResources battleUnitResources = teamInfo.GetBattleUnitResources();
                    battleUnitResources.SetBattleUnitResources(_battleUnitResources);
                }
            }
        }

        public void AwardTeamXP(float _xpToAward)
        {
            foreach (TeamInfo teamInfo in teamInfos)
            {
                teamInfo.GainXP(_xpToAward);

                HandleLevelingUp(teamInfo);
            }
        }

        private void HandleLevelingUp(TeamInfo _teamInfo)
        {
            int currentLevel = _teamInfo.GetLevel();
            int updatedLevel = progressionHandler.GetLevel(_teamInfo.GetXP());
            int levelsGained = updatedLevel - currentLevel;

            for (int i = 0; i < levelsGained; i++)
            {
                _teamInfo.LevelUp();
            }
        }

        public void RestoreAllResources()
        {
            foreach (TeamInfo teamInfo in teamInfos)
            {
                BattleUnitResources battleUnitResources = teamInfo.GetBattleUnitResources();
                float maxHealthPoints = battleUnitResources.GetMaxHealthPoints();
                float maxManaPoints = battleUnitResources.GetMaxManaPoints();

                battleUnitResources.SetHealthPoints(maxHealthPoints);
                battleUnitResources.SetMaxManaPoints(maxManaPoints);
            }
        }

        public Unit GetCharacter(string _characterName)
        {
            return Resources.Load<Unit>(_characterName);
        }

        public TeamInfo GetTeamInfo(Unit _characterToGet)
        {
            TeamInfo newInfo = null;

            foreach (TeamInfo teamInfo in teamInfos)
            {
                if (GetCharacter(teamInfo.GetName()) == _characterToGet)
                {
                    newInfo = teamInfo;
                }
            }

            return newInfo;
        }

        public TeamInfo[] GetTeamInfos()
        {
            return teamInfos;
        }

        public List<Unit> GetPlayerTeam()
        {
            return playerTeam;
        }

        private float CalculateMaxHealthPoints(float _stamina)
        {
            float maxHealthPoints = 100f;

            float nonBaseStamina = _stamina - 10f;

            maxHealthPoints += 10f * nonBaseStamina;

            return maxHealthPoints;
        }

        private float CalculateMaxMana(float _mana)
        {
            float maxMana = 100f;

            float nonBaseMana = _mana - 10f;

            maxMana += 10f * nonBaseMana;

            return maxMana;
        }

        public object CaptureState()
        {
            return teamInfos;
        }

        public void RestoreState(object _state)
        {
            teamInfos = (TeamInfo[])_state;
        }
    }

    [Serializable]
    public class TeamInfo
    {
        [SerializeField] string characterName = "";

        [Header("Resources")]
        [SerializeField] Stats stats;
        [SerializeField] BattleUnitResources battleUnitResources = new BattleUnitResources();

        [Header("Progression")]
        [SerializeField] int level = 1;
        [SerializeField] float experiencePoints = 0f;

        public void SetName(string _characterName)
        {
            characterName = _characterName;
        }

        public void SetStats(Stats _stats)
        {
            stats.SetStats(_stats);
        }

        public void SetBattleUnitResources(BattleUnitResources _battleUnitResources)
        {
            battleUnitResources.SetBattleUnitResources(_battleUnitResources);
        }

        public void LevelUp()
        {
            level++;
            
            //Refactor - Handle leveling up differently from Soulless

            foreach (Stat stat in stats.GetAllStats())
            {
                if (stat.GetLevelUpPercent() >= RandomGenerator.GetRandomNumber(0, 99))
                {
                    stat.IncreaseLevel();
                }
            }
        }

        public string GetName()
        {
            return characterName;
        }

        public Stats GetStats()
        {
            return stats;
        }

        public BattleUnitResources GetBattleUnitResources()
        {
            return battleUnitResources;
        }

        public int GetLevel()
        {
            return level;
        }

        public float GetXP()
        {
            return experiencePoints;
        }

        public void GainXP(float xpToGain)
        {
            experiencePoints += xpToGain;
        }
    }
}


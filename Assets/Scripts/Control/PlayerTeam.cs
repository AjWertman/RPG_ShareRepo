using RPGProject.Combat;
using RPGProject.Core;
using RPGProject.Progression;
using RPGProject.Saving;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Control
{
    //Refactor rename PlayerTeamManager
    public class PlayerTeam : MonoBehaviour, ISaveable
    {
        [SerializeField] PlayerKey[] startingPlayerKeys = null;
        [SerializeField] List<TeamInfo> teamInfos = new List<TeamInfo>();

        PlayableCharacterDatabase playableCharacterDatabase = null;
        UnitDatabase unitDatabase = null;

        ProgressionHandler progressionHandler = null;

        List<PlayableCharacter> playerTeam = new List<PlayableCharacter>();

        private void Awake()
        {
            playableCharacterDatabase = FindObjectOfType<PlayableCharacterDatabase>();
            unitDatabase = FindObjectOfType<UnitDatabase>();

            playableCharacterDatabase.PopulateDatabase();
            unitDatabase.PopulateDatabase();

            progressionHandler = GetComponentInChildren<ProgressionHandler>();
            
            PopulateTeamInfos(startingPlayerKeys);
        }

        private void PopulateTeamInfos(PlayerKey[] _playerKeys)
        {
            teamInfos.Clear();
            foreach (PlayerKey playerKey in _playerKeys)
            {
                PlayableCharacter playableCharacter = playableCharacterDatabase.GetPlayableCharacter(playerKey);
                CharacterKey characterKey = CharacterKeyComparison.GetCharacterKey(playerKey);

                Unit unit = unitDatabase.GetUnit(characterKey);

                TeamInfo teamInfo = new TeamInfo();
                teamInfo.SetPlayerKey(playerKey);
                teamInfo.GetLevel();

                teamInfo.SetStats(unit.GetStats());

                float maxHealthPoints = CalculateMaxHealthPoints(teamInfo.GetStats().GetStat(StatType.Stamina));
                float maxManaPoints = CalculateMaxMana(teamInfo.GetStats().GetStat(StatType.Spirit));

                BattleUnitResources battleUnitResources = new BattleUnitResources();
                battleUnitResources.SetBattleUnitResources(maxHealthPoints, maxHealthPoints, maxManaPoints, maxManaPoints);

                teamInfo.SetBattleUnitResources(battleUnitResources);

                teamInfos.Add(teamInfo);

                playerTeam.Add(playableCharacter);
            }
        }

        public void UpdateTeamInfo(PlayerKey _playerKey, BattleUnitResources _battleUnitResources)
        {
            foreach (TeamInfo teamInfo in teamInfos)
            {
                if (teamInfo.GetPlayerKey() == _playerKey)
                {
                    BattleUnitResources battleUnitResources = teamInfo.GetBattleUnitResources();
                    battleUnitResources.SetBattleUnitResources(_battleUnitResources);
                }
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

        public List<PlayableCharacter> GetPlayableCharacters()
        {
            return playerTeam;
        }

        public PlayableCharacter GetPlayableCharacter(PlayerKey _playerKey)
        {
            return playableCharacterDatabase.GetPlayableCharacter(_playerKey);
        }

        public List<TeamInfo> GetTeamInfos()
        { 
            return teamInfos;
        }

        public TeamInfo GetTeamInfo(PlayerKey _playerKey)
        {
            TeamInfo newInfo = null;

            foreach (TeamInfo teamInfo in teamInfos)
            {
                if (teamInfo.GetPlayerKey() == _playerKey)
                {
                    newInfo = teamInfo;
                }
            }

            return newInfo;
        }

        public List<Unit> GetPlayerUnits()
        {
            List<Unit> playerUnits = new List<Unit>();
            
            foreach(PlayableCharacter playableCharacter in playerTeam)
            {            
                Unit playerUnit = GetUnit(playableCharacter.GetPlayerKey());
                playerUnits.Add(playerUnit);
            }

            return playerUnits;
        }

        public Unit GetUnit(PlayerKey _playerKey)
        {
            CharacterKey characterKey = CharacterKeyComparison.GetCharacterKey(_playerKey);
            return unitDatabase.GetUnit(characterKey);
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
            teamInfos = (List<TeamInfo>)_state;
        }
    }

    [Serializable]
    public class TeamInfo
    {
        [SerializeField] PlayerKey playerKey = PlayerKey.Aj;

        [SerializeField] Stats stats = new Stats();
        [SerializeField] BattleUnitResources battleUnitResources = new BattleUnitResources();

        [SerializeField] int level = 1;
        [SerializeField] float experiencePoints = 0f;

        public void SetPlayerKey(PlayerKey _playerKey)
        {
            playerKey = _playerKey;
        }

        public void SetStats(Stats _stats)
        {
            stats.SetStats(_stats);
        }

        public void SetBattleUnitResources(BattleUnitResources _battleUnitResources)
        {
            battleUnitResources.SetBattleUnitResources(_battleUnitResources);
        }

        public void SetLevel(int _level)
        {
            level = _level;
        }

        public void LevelUp()
        {
            level++;

            ////Refactor - Handle leveling up differently from Soulless
            //foreach (StatType stat in stats.GetAllStats())
            //{
            //    if (stat.GetLevelUpPercent() >= RandomGenerator.GetRandomNumber(0, 99))
            //    {
            //        stat.IncreaseLevel();
            //    }
            //}
        }

        public PlayerKey GetPlayerKey()
        {
            return playerKey;
        }

        public string GetName()
        {
            return playerKey.ToString();
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

        public void GainXP(float _xpToGain)
        {
            experiencePoints += _xpToGain;
        }
    }
}


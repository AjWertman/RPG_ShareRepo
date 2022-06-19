using RPGProject.Combat;
using RPGProject.Core;
using RPGProject.Progression;
using RPGProject.Saving;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Control
{
    public class PlayerTeamManager : MonoBehaviour, ISaveable
    {
        [SerializeField] List<PlayerKey> currentPlayerKeys = new List<PlayerKey>();
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
        }

        public void PopulateTeamInfos(List<PlayerKey> _playerKeys)
        {
            teamInfos.Clear();
            currentPlayerKeys.Clear();
            playerTeam.Clear();
            foreach (PlayerKey playerKey in _playerKeys)
            {                           
                currentPlayerKeys.Add(playerKey);

                PlayableCharacter playableCharacter = playableCharacterDatabase.GetPlayableCharacter(playerKey);
                CharacterKey characterKey = CharacterKeyComparison.GetCharacterKey(playerKey);

                Unit unit = null;

                unit = unitDatabase.GetUnit(characterKey);
                TeamInfo teamInfo = new TeamInfo();
                teamInfo.SetPlayerKey(playerKey);
                teamInfo.SetLevel(unit.GetBaseLevel());
                teamInfo.SetStats(unit.GetStats());

                float maxHealthPoints = CalculateMaxHealthPoints(teamInfo.GetStats().GetStat(StatType.Stamina));
                float maxManaPoints = CalculateMaxMana(teamInfo.GetStats().GetStat(StatType.Spirit));

                UnitResources unitResources = new UnitResources();
                unitResources.SetUnitResources(maxHealthPoints, maxHealthPoints, maxManaPoints, maxManaPoints);

                teamInfo.SetUnitResources(unitResources);

                teamInfos.Add(teamInfo);
                playerTeam.Add(playableCharacter);
            }
        }

        public void UpdateTeamInfo(PlayerKey _playerKey, UnitResources _unitResources)
        {
            foreach (TeamInfo teamInfo in teamInfos)
            {
                if (teamInfo.GetPlayerKey() == _playerKey)
                {
                    UnitResources unitResources = teamInfo.GetUnitResources();
                    teamInfo.SetUnitResources(_unitResources);
                }
            }
        }

        public void AddTeammate(PlayerKey _playerKey)
        {
            List<PlayerKey> playerKeys = new List<PlayerKey>();
            bool hasKey = false;
            foreach(TeamInfo teamInfo in teamInfos)
            {
                PlayerKey playerKey = teamInfo.GetPlayerKey();
                if (playerKey == _playerKey) hasKey = true;
            }

            if (!hasKey && teamInfos.Count < 4)
            {
                playerKeys.Add(_playerKey);
                PopulateTeamInfos(playerKeys);
            }
        }

        public void RemoveTeammate(PlayerKey _playerKey)
        {
            List<PlayerKey> playerKeys = new List<PlayerKey>(currentPlayerKeys);

            bool hasKey = false;
            foreach (TeamInfo teamInfo in teamInfos)
            {
                PlayerKey playerKey = teamInfo.GetPlayerKey();
                if (playerKey == _playerKey) hasKey = true;
            }

            if (hasKey && teamInfos.Count > 1)
            {
                playerKeys.Remove(_playerKey);
                PopulateTeamInfos(playerKeys);
            }
        }

        public void RestoreAllResources()
        {
            foreach (TeamInfo teamInfo in teamInfos)
            {
                UnitResources unitResources = teamInfo.GetUnitResources();
                float maxHealthPoints = unitResources.GetMaxHealthPoints();
                float maxManaPoints = unitResources.GetMaxManaPoints();

                unitResources.SetUnitResources(maxHealthPoints, maxHealthPoints, maxManaPoints, maxHealthPoints);
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
        [SerializeField] PlayerKey playerKey = PlayerKey.None;

        [SerializeField] UnitInfo unitInfo = new UnitInfo();
        [SerializeField] UnitResources unitResources = new UnitResources();
        [SerializeField] Stats stats = new Stats();

        [SerializeField] int level = 1;
        [SerializeField] float experiencePoints = 0f;

        public void SetPlayerKey(PlayerKey _playerKey)
        {
            playerKey = _playerKey;
        }

        public void SetUnitInfo(UnitInfo _unitInfo)
        {
            unitInfo.SetUnitInfo(unitInfo);
        }

        public void SetUnitResources(UnitResources _unitResources)
        {
            unitResources.SetUnitResources(_unitResources);
        }

        public void SetStats(Stats _stats)
        {
            stats.SetStats(_stats);
        }

        public void SetLevel(int _level)
        {
            level = _level;
        }

        public void LevelUp()
        {
            level++;
        }

        public PlayerKey GetPlayerKey()
        {
            return playerKey;
        }

        public string GetName()
        {
            return playerKey.ToString();
        }

        public UnitInfo GetUnitInfo()
        {
            return unitInfo;
        }

        public UnitResources GetUnitResources()
        {
            return unitResources;
        }

        public Stats GetStats()
        {
            return stats;
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


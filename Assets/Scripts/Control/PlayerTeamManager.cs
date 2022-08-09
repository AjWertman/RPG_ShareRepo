using RPGProject.Combat;
using RPGProject.Combat.Grid;
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
        public List<TeamInfo> teamInfos = new List<TeamInfo>();
        public UnitStartingPosition[] playerStartingPositions = null;
        public List<PlayableCharacter> playerTeam = new List<PlayableCharacter>();

        List<PlayerKey> currentPlayerKeys = new List<PlayerKey>();

        PlayableCharacterDatabase playableCharacterDatabase = null;
        UnitDatabase unitDatabase = null;

        ProgressionHandler progressionHandler = null;

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

                Unit unit = unitDatabase.GetUnit(characterKey);

                TeamInfo teamInfo = new TeamInfo();
                teamInfo.playerKey = playerKey;
                teamInfo.level = unit.baseLevel;
                teamInfo.stats = unit.stats;

                teamInfo.startingCoordinates = GetStartingPosition(unit);

                float maxHealthPoints = CalculateMaxHealthPoints(teamInfo.stats.GetStatLevel(StatType.Stamina));

                UnitResources unitResources = new UnitResources(maxHealthPoints);
                teamInfo.unitResources = unitResources;

                teamInfos.Add(teamInfo);
                playerTeam.Add(playableCharacter);
            }
        }

        public void UpdateTeamInfo(PlayerKey _playerKey, UnitResources _unitResources)
        {
            foreach (TeamInfo teamInfo in teamInfos)
            {
                if (teamInfo.playerKey == _playerKey)
                {
                    UnitResources unitResources = teamInfo.unitResources;
                    teamInfo.unitResources = _unitResources;
                }
            }
        }

        public void AddTeammate(PlayerKey _playerKey)
        {
            List<PlayerKey> playerKeys = new List<PlayerKey>();
            bool hasKey = false;
            foreach(TeamInfo teamInfo in teamInfos)
            {
                PlayerKey playerKey = teamInfo.playerKey;
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
                PlayerKey playerKey = teamInfo.playerKey;
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
                teamInfo.unitResources.RestoreHealth();             
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
            int currentLevel = _teamInfo.level;
            int updatedLevel = progressionHandler.GetLevel(_teamInfo.experiencePoints);
            int levelsGained = updatedLevel - currentLevel;

            for (int i = 0; i < levelsGained; i++)
            {
                _teamInfo.level++;
            }
        }

        public PlayableCharacter GetPlayableCharacter(PlayerKey _playerKey)
        {
            return playableCharacterDatabase.GetPlayableCharacter(_playerKey);
        }

        public TeamInfo GetTeamInfo(PlayerKey _playerKey)
        {
            TeamInfo newInfo = null;

            foreach (TeamInfo teamInfo in teamInfos)
            {
                if (teamInfo.playerKey == _playerKey)
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
                Unit playerUnit = GetUnit(playableCharacter.playerKey);
                playerUnits.Add(playerUnit);
            }

            return playerUnits;
        }
      
        public Unit GetUnit(PlayerKey _playerKey)
        {
            CharacterKey characterKey = CharacterKeyComparison.GetCharacterKey(_playerKey);
            return unitDatabase.GetUnit(characterKey);
        }

        private GridCoordinates GetStartingPosition(Unit _unit)
        {
            GridCoordinates startingCoordinates = new GridCoordinates();

            foreach (UnitStartingPosition playerStartingPosition in playerStartingPositions)
            {
                Unit unit = playerStartingPosition.unit;

                if (unit == _unit) startingCoordinates = playerStartingPosition.startCoordinates;
            }

            return startingCoordinates;
        }

        private float CalculateMaxHealthPoints(float _stamina)
        {
            float maxHealthPoints = 100f;

            float nonBaseStamina = _stamina - 10f;

            maxHealthPoints += 10f * nonBaseStamina;

            return maxHealthPoints;
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
        public PlayerKey playerKey = PlayerKey.None;

        public UnitInfo unitInfo = new UnitInfo();
        public UnitResources unitResources = new UnitResources();
        public Stats stats = new Stats();

        public GridCoordinates startingCoordinates;

        public int level = 1;
        public float experiencePoints = 0f;

        public string GetName()
        {
            return playerKey.ToString();
        }

        public void GainXP(float _xpToGain)
        {
            experiencePoints += _xpToGain;
        }
    }
}


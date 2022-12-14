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
    /// <summary>
    /// Holds and handles the information of the player team.
    /// </summary>
    public class PlayerTeamManager : MonoBehaviour, ISaveable
    {
        public List<TeamInfo> teamInfos = new List<TeamInfo>();
        public UnitStartingPosition[] playerStartingPositions = null;
        public List<PlayableCharacter> playerTeam = new List<PlayableCharacter>();

        List<CharacterKey> currentPlayerKeys = new List<CharacterKey>();

        UnitDatabase unitDatabase = null;

        ProgressionHandler progressionHandler = null;

        private void Awake()
        {
            unitDatabase = FindObjectOfType<UnitDatabase>();

            unitDatabase.PopulateDatabase();

            progressionHandler = GetComponentInChildren<ProgressionHandler>();
        }

        public void PopulateTeamInfos(List<CharacterKey> _playerKeys)
        {
            teamInfos.Clear();
            currentPlayerKeys.Clear();
            playerTeam.Clear();
            foreach (CharacterKey playerKey in _playerKeys)
            {                           
                currentPlayerKeys.Add(playerKey);

                PlayableCharacter playableCharacter = unitDatabase.GetPlayableCharacter(playerKey);
                Unit unit = unitDatabase.GetUnit(playerKey);

                TeamInfo teamInfo = new TeamInfo();
                teamInfo.playerKey = playerKey;
                teamInfo.level = unit.baseLevel;
                teamInfo.stats = unit.stats;

                teamInfo.startingCoordinates = GetStartingPosition(unit);

                float maxHealthPoints = CalculateMaxHealthPoints(teamInfo.stats.GetStatLevel(StatType.Stamina));

                //Refactor - Does this require a calculation? or a specified amount?
                int maxEnergyPoints = 100;

                UnitResources unitResources = new UnitResources(maxHealthPoints, maxEnergyPoints);
                teamInfo.unitResources = unitResources;

                teamInfos.Add(teamInfo);
                playerTeam.Add(playableCharacter);
            }
        }

        public void UpdateTeamInfo(CharacterKey _playerKey, UnitResources _unitResources)
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

        public void AddTeammate(CharacterKey _playerKey)
        {
            List<CharacterKey> playerKeys = new List<CharacterKey>();
            bool hasKey = false;
            foreach(TeamInfo teamInfo in teamInfos)
            {
                CharacterKey playerKey = teamInfo.playerKey;
                if (playerKey == _playerKey) hasKey = true;
            }

            if (!hasKey && teamInfos.Count < 4)
            {
                playerKeys.Add(_playerKey);
                PopulateTeamInfos(playerKeys);
            }
        }

        public void RemoveTeammate(CharacterKey _playerKey)
        {
            List<CharacterKey> playerKeys = new List<CharacterKey>(currentPlayerKeys);

            bool hasKey = false;
            foreach (TeamInfo teamInfo in teamInfos)
            {
                CharacterKey playerKey = teamInfo.playerKey;
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

        public PlayableCharacter GetPlayableCharacter(CharacterKey _playerKey)
        {
            return unitDatabase.GetPlayableCharacter(_playerKey);
        }

        public TeamInfo GetTeamInfo(CharacterKey _playerKey)
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
      
        public Unit GetUnit(CharacterKey _playerKey)
        {
            return unitDatabase.GetUnit(_playerKey);
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
        public CharacterKey playerKey = CharacterKey.None;

        public UnitInfo unitInfo = null;
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


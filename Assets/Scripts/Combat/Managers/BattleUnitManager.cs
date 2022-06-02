using RPGProject.Core;
using RPGProject.GameResources;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Combat
{
    public class BattleUnitManager : MonoBehaviour
    {
        List<BattleUnit> battleUnits = new List<BattleUnit>();

        List<Unit> playerTeam = new List<Unit>();
        List<BattleUnit> playerUnits = new List<BattleUnit>();

        Dictionary<Unit, BattleUnitResources> initialPlayerResources = new Dictionary<Unit, BattleUnitResources>();

        List<Unit> enemyTeam = new List<Unit>();
        List<BattleUnit> enemyUnits = new List<BattleUnit>();

        BattleUnitPool battleUnitPool = null;
        CharacterMeshPool characterMeshPool = null;

        public event Action onMoveCompletion;
        public event Action<bool?> onTeamWipe;
        public event Action<BattleUnit> onUnitListUpdate;

        int startingPlayerTeamSize = 0;
        int startingEnemyTeamSize = 0;

        public void InitalizeUnitManager()
        {
            battleUnitPool = FindObjectOfType<BattleUnitPool>();
            characterMeshPool = FindObjectOfType<CharacterMeshPool>();
            SetupEvents();
        }

        public void SetupPlayerUnit(Unit _playerUnit, BattleUnitResources _playerResources)
        {
            playerTeam.Add(_playerUnit);
            initialPlayerResources.Add(_playerUnit, _playerResources);
        }

        public void SetUpUnits(List<Unit> _enemyTeam, List<Transform> _playerPositions, List<Transform> _enemyPositions)
        {
            enemyTeam = _enemyTeam;

            startingPlayerTeamSize = playerTeam.Count;
            startingEnemyTeamSize = enemyTeam.Count;

            SetupUnitTeam(playerTeam, _playerPositions, true);
            SetupUnitTeam(enemyTeam, _enemyPositions, false);
        }

        public void SetupUnitTeam(List<Unit> _team, List<Transform> _teamPositions, bool _isPlayerTeam)
        {
            for (int i = 0; i < _team.Count; i++)
            {
                Unit unit = _team[i];
                BattleUnit battleUnit = SetupNewBattleUnit(unit, _teamPositions[i], _isPlayerTeam);

                AddBattleUnitToLists(battleUnit, _isPlayerTeam);
            }
        }

        private BattleUnit SetupNewBattleUnit(Unit _unit, Transform _teamPosition, bool _isPlayerTeam)
        {
            BattleUnit battleUnit = battleUnitPool.GetAvailableBattleUnit();

            SetBattleUnitTransform(battleUnit, _teamPosition);

            BattleUnitInfo battleUnitInfo = battleUnit.GetBattleUnitInfo();
            battleUnitInfo.SetBattleUnitInfo(_unit.GetUnitName(), _unit.GetBaseLevel(),
                _isPlayerTeam, _unit.GetStats(), _unit.GetBasicAttack(), _unit.GetAbilities());

            BattleUnitResources battleUnitResources = battleUnit.GetBattleUnitResources();

            CharacterMesh newMesh = characterMeshPool.GetMesh(_unit.GetCharacterKey());

            if (_isPlayerTeam)
            {
                //Refactor add player level ovverride
                //battleUnitInfo.SetUnitLevel(teamInfo.GetLevel());

                BattleUnitResources playerResources = initialPlayerResources[_unit];
                battleUnitResources.SetBattleUnitResources(playerResources);
            }
            else
            {
                //Refactor 
                //Create a xp award script on enemy
                //battleUnit.SetUnitXPAward(unit.GetXPAward());
            }

            battleUnit.SetupBattleUnit(battleUnitInfo, battleUnitResources, _isPlayerTeam, newMesh);
            battleUnit.gameObject.SetActive(true);

            return battleUnit;
        }

        private void SetBattleUnitTransform(BattleUnit _battleUnit, Transform _newTransform)
        {
            _battleUnit.transform.position = _newTransform.position;
            _battleUnit.transform.rotation = _newTransform.rotation;
        }

        private void AddBattleUnitToLists(BattleUnit _battleUnit, bool _isPlayer)
        {
            if (_isPlayer)
            {
                playerUnits.Add(_battleUnit);
            }
            else
            {
                enemyUnits.Add(_battleUnit);
            }

            battleUnits.Add(_battleUnit);
        }

        private void OnUnitDeath(Health _deadUnitHealth)
        {
            BattleUnit deadUnit = _deadUnitHealth.GetComponent<BattleUnit>();

            deadUnit.ActivateResourceSliders(false);
            TeamWipeCheck();
            onUnitListUpdate(deadUnit);
        }

        private void TeamWipeCheck()
        {
            if (GetDeadPlayerUnits().Count == startingPlayerTeamSize)
            {
                onTeamWipe(false);
            }
            else if (GetDeadEnemyUnits().Count == startingEnemyTeamSize)
            {
                onTeamWipe(true);
            }
        }

        public void ResetUnitManager()
        {
            characterMeshPool.ResetCharacterMeshPool();
            battleUnitPool.ResetBattleUnitPool();
            ResetEvents();
            ResetLists();
        }

        private void ResetLists()
        {
            battleUnits.Clear();

            playerTeam.Clear();
            playerUnits.Clear();
            initialPlayerResources.Clear();

            enemyTeam.Clear();
            enemyUnits.Clear();
        }

        private void SetupEvents()
        {
            foreach (BattleUnit battleUnit in battleUnitPool.GetAllBattleUnits())
            {
                battleUnit.onMoveCompletion += OnMoveCompletion;
                battleUnit.GetHealth().onDeath += OnUnitDeath;
            }
        }

        private void ResetEvents()
        {
            foreach (BattleUnit battleUnit in battleUnitPool.GetAllBattleUnits())
            {
                battleUnit.onMoveCompletion -= OnMoveCompletion;
                battleUnit.GetHealth().onDeath -= OnUnitDeath;
            }
        }

        public void OnMoveCompletion()
        {
            onMoveCompletion();
        }

        public List<BattleUnit> GetBattleUnits()
        {
            return battleUnits;
        }

        public List<BattleUnit> GetPlayerUnits()
        {
            return playerUnits;
        }

        public List<BattleUnit> GetDeadPlayerUnits()
        {
            List<BattleUnit> deadPlayerUnits = new List<BattleUnit>();

            foreach (BattleUnit playerUnit in playerUnits)
            {
                if (playerUnit.GetHealth().IsDead())
                {
                    deadPlayerUnits.Add(playerUnit);
                }
            }

            return deadPlayerUnits;
        }

        public List<BattleUnit> GetEnemyUnits()
        {
            return enemyUnits;
        }

        public List<BattleUnit> GetDeadEnemyUnits()
        {
            List<BattleUnit> deadEnemyUnits = new List<BattleUnit>();

            foreach (BattleUnit enemyUnit in enemyUnits)
            {
                if (enemyUnit.GetHealth().IsDead())
                {
                    deadEnemyUnits.Add(enemyUnit);
                }
            }

            return deadEnemyUnits;
        }

        public BattleUnit GetRandomPlayerUnit()
        {
            int randomInt = RandomGenerator.GetRandomNumber(0, playerUnits.Count - 1);

            return playerUnits[randomInt];
        }

        public BattleUnit GetRandomEnemyUnit()
        {
            int randomInt = RandomGenerator.GetRandomNumber(0, enemyUnits.Count - 1);

            return enemyUnits[randomInt];
        }
    }
}
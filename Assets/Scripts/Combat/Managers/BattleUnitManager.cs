using RPGProject.Combat;
using RPGProject.Core;
using RPGProject.GameResources;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Control
{
    public class BattleUnitManager : MonoBehaviour
    {
        List<UnitController> unitControllers = new List<UnitController>();

        List<Unit> playerTeam = new List<Unit>();
        List<UnitController> playerUnits = new List<UnitController>();

        Dictionary<Unit, BattleUnitResources> initialPlayerResources = new Dictionary<Unit, BattleUnitResources>();

        List<Unit> enemyTeam = new List<Unit>();
        List<UnitController> enemyUnits = new List<UnitController>();

        BattleUnitPool battleUnitPool = null;
        CharacterMeshPool characterMeshPool = null;

        public event Action onMoveCompletion;
        public event Action<bool?> onTeamWipe;
        public event Action<UnitController> onUnitListUpdate;

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
                UnitController battleUnit = SetupNewBattleUnit(unit, _teamPositions[i], _isPlayerTeam);

                AddBattleUnitToLists(battleUnit, _isPlayerTeam);
            }
        }

        private UnitController SetupNewBattleUnit(Unit _unit, Transform _teamPosition, bool _isPlayerTeam)
        {
            UnitController unitController = battleUnitPool.GetAvailableBattleUnit();
            Fighter fighter = unitController.GetFighter();
       
            SetBattleUnitTransform(unitController, _teamPosition);

            BattleUnitInfo battleUnitInfo = unitController.GetBattleUnitInfo();
            battleUnitInfo.SetBattleUnitInfo(_unit.GetUnitName(), _unit.GetBaseLevel(),
                _isPlayerTeam, _unit.GetStats(), _unit.GetBasicAttack(), _unit.GetAbilities());

            BattleUnitResources battleUnitResources = unitController.GetBattleUnitResources();

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

            unitController.SetupBattleUnit(battleUnitInfo, battleUnitResources, _isPlayerTeam, newMesh);
            fighter.SetUnitInfo(battleUnitInfo);
            fighter.SetUnitResources(battleUnitResources);
            unitController.gameObject.SetActive(true);

            return unitController;
        }

        private void SetBattleUnitTransform(UnitController _battleUnit, Transform _newTransform)
        {
            _battleUnit.transform.position = _newTransform.position;
            _battleUnit.transform.rotation = _newTransform.rotation;
        }

        private void AddBattleUnitToLists(UnitController _battleUnit, bool _isPlayer)
        {
            if (_isPlayer)
            {
                playerUnits.Add(_battleUnit);
            }
            else
            {
                enemyUnits.Add(_battleUnit);
            }

            unitControllers.Add(_battleUnit);
        }

        private void OnUnitDeath(Health _deadUnitHealth)
        {
            UnitController deadUnit = _deadUnitHealth.GetComponent<UnitController>();

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
            unitControllers.Clear();

            playerTeam.Clear();
            playerUnits.Clear();
            initialPlayerResources.Clear();

            enemyTeam.Clear();
            enemyUnits.Clear();
        }

        private void SetupEvents()
        {
            foreach (UnitController battleUnit in battleUnitPool.GetAllBattleUnits())
            {
                battleUnit.onMoveCompletion += OnMoveCompletion;
                battleUnit.GetHealth().onDeath += OnUnitDeath;
            }
        }

        private void ResetEvents()
        {
            foreach (UnitController battleUnit in battleUnitPool.GetAllBattleUnits())
            {
                battleUnit.onMoveCompletion -= OnMoveCompletion;
                battleUnit.GetHealth().onDeath -= OnUnitDeath;
            }
        }

        public void OnMoveCompletion()
        {
            onMoveCompletion();
        }

        public List<UnitController> GetBattleUnits()
        {
            return unitControllers;
        }

        public List<UnitController> GetPlayerUnits()
        {
            return playerUnits;
        }

        public List<UnitController> GetDeadPlayerUnits()
        {
            List<UnitController> deadPlayerUnits = new List<UnitController>();

            foreach (UnitController playerUnit in playerUnits)
            {
                if (playerUnit.GetHealth().IsDead())
                {
                    deadPlayerUnits.Add(playerUnit);
                }
            }

            return deadPlayerUnits;
        }

        public List<UnitController> GetEnemyUnits()
        {
            return enemyUnits;
        }

        public List<UnitController> GetDeadEnemyUnits()
        {
            List<UnitController> deadEnemyUnits = new List<UnitController>();

            foreach (UnitController enemyUnit in enemyUnits)
            {
                if (enemyUnit.GetHealth().IsDead())
                {
                    deadEnemyUnits.Add(enemyUnit);
                }
            }

            return deadEnemyUnits;
        }

        public UnitController GetRandomPlayerUnit()
        {
            int randomInt = RandomGenerator.GetRandomNumber(0, playerUnits.Count - 1);

            return playerUnits[randomInt];
        }

        public UnitController GetRandomEnemyUnit()
        {
            int randomInt = RandomGenerator.GetRandomNumber(0, enemyUnits.Count - 1);

            return enemyUnits[randomInt];
        }
    }
}
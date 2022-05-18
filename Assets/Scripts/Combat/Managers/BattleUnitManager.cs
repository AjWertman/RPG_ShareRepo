using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnitManager : MonoBehaviour
{

    List<BattleUnit> battleUnits = new List<BattleUnit>();

    PlayerTeam playerTeamInfo = null;
    List<Unit> playerTeam = new List<Unit>();
    List<BattleUnit> playerUnits = new List<BattleUnit>();
    List<BattleUnit> allPlayerUnits = new List<BattleUnit>();
    List<BattleUnit> deadPlayerUnits = new List<BattleUnit>();
    int startingPlayerTeamSize = 0;

    List<Unit> enemyTeam = new List<Unit>();
    List<BattleUnit> enemyUnits = new List<BattleUnit>();
    List<BattleUnit> deadEnemyUnits = new List<BattleUnit>();
    int startingEnemyTeamSize = 0;

    BattleUnitPool battleUnitPool = null;
    CharacterMeshPool characterMeshPool = null;
    
    public event Action<bool> onTeamWipe;
    public event Action onUnitListUpdate;

    private void Awake()
    {
        battleUnitPool = FindObjectOfType<BattleUnitPool>();
        characterMeshPool = FindObjectOfType<CharacterMeshPool>();
    }

    public void SetUpUnits(PlayerTeam _playerTeamInfo, List<Unit> _enemyTeam, List<Transform> playerPositions, List<Transform> enemyPositions)
    {
        playerTeamInfo = _playerTeamInfo;

        playerTeam = playerTeamInfo.GetPlayerTeam();
        enemyTeam = _enemyTeam;

        SetUpPlayerUnits(playerTeam.Count, playerPositions);
        SetUpEnemyUnits(enemyTeam.Count, enemyPositions);

        startingPlayerTeamSize = playerTeam.Count;
        startingEnemyTeamSize = enemyTeam.Count;
    }

    private void SetUpPlayerUnits(int playerTeamSize, List<Transform> playerPositions)
    {
        for (int i = 0; i < playerTeamSize; i++)
        {
            Unit unit = playerTeam[i];
            TeamInfo teamInfo = playerTeamInfo.GetTeamInfo(unit);
            BattleUnit battleUnit = battleUnitPool.GetBattleUnit();
            AddBattleUnitToLists(battleUnit, true);

            SetBattleUnitTransform(battleUnit, playerPositions[i]);

            SetupUnitComponents(unit, battleUnit, true);

            battleUnit.SetName(unit.GetName());
            battleUnit.SetNewMesh(characterMeshPool.GetMesh(unit.GetCharacterMeshKey()));
            battleUnit.SetStats(unit.GetBaseStats());
            battleUnit.SetSpells(unit.GetBasicAttack(), unit.GetSpells());

            battleUnit.SetResources(teamInfo.GetHealth(), teamInfo.GetMaxHealth(), teamInfo.GetSoulWell(), teamInfo.GetMaxSoulWell());
            battleUnit.SetUnitLevel(teamInfo.GetLevel());
           
            battleUnit.gameObject.SetActive(true);
        }
    }

    private void SetUpEnemyUnits(int enemyTeamSize, List<Transform> enemyPositions)
    {
        for (int i = 0; i < enemyTeamSize; i++)
        {
            Unit unit = enemyTeam[i];
            BattleUnit battleUnit = battleUnitPool.GetBattleUnit();
            AddBattleUnitToLists(battleUnit, false);

            SetBattleUnitTransform(battleUnit, enemyPositions[i]);

            SetupUnitComponents(unit, battleUnit, false);

            battleUnit.SetName(unit.GetName());
            battleUnit.SetNewMesh(characterMeshPool.GetMesh(unit.GetCharacterMeshKey()));
            battleUnit.SetStats(unit.GetBaseStats());
            battleUnit.SetSpells(unit.GetBasicAttack(), unit.GetSpells());
            battleUnit.SetUnitXPAward(unit.GetXPAward());
            battleUnit.CalculateResources();

            battleUnit.gameObject.SetActive(true);
        }
    }

    private void SetupUnitComponents(Unit character, BattleUnit unit, bool isPlayer)
    {
        unit.SetupBattleUnitComponents();
        unit.GetMover().SetStartingTransforms();

        unit.SetIsPlayer(isPlayer);
        unit.SetFaceImage(character.GetFaceImage());
        unit.SetAnimators();
        unit.SetupIndicator();
        unit.SetUnitSoundFX();
        //unit.SetActiveSpellContainers();
        unit.SetUpResourceSliders();

        unit.SetStats(character.GetBaseStats());
        unit.SetSpells(character.GetBasicAttack(), character.GetSpells());

        unit.GetComponent<Health>().onDeath += OnUnitDeath;
    }

    private void SetBattleUnitTransform(BattleUnit battleUnit, Transform newTransform)
    {
        battleUnit.transform.position = newTransform.position;
        battleUnit.transform.rotation = newTransform.rotation;
    }

    private void AddBattleUnitToLists(BattleUnit battleUnit, bool isPlayer)
    {
        if (isPlayer)
        {
            playerUnits.Add(battleUnit);
            allPlayerUnits.Add(battleUnit);
        }
        else
        {
            enemyUnits.Add(battleUnit);
        }

        battleUnits.Add(battleUnit);
    }

    public void HandlePlayerDeaths()
    {
        foreach (BattleUnit unit in allPlayerUnits)
        {
            if (unit.DeathCheck())
            {
                unit.Die();
            }
        }
    }

    public List<BattleUnit> GetBattleUnits()
    {
        return battleUnits;
    }

    public List<BattleUnit> GetAllPlayerUnits()
    {
        return allPlayerUnits;
    }

    public List<BattleUnit> GetPlayerUnits()
    {
        return playerUnits;
    }

    public List<BattleUnit> GetDeadPlayerUnits()
    {
        return deadPlayerUnits;
    }

    public List<BattleUnit> GetEnemyUnits()
    {
        return enemyUnits;
    }

    public List<BattleUnit> GetDeadEnemyUnits()
    {
        return deadEnemyUnits;
    }

    private void OnUnitDeath(BattleUnit deadUnit)
    {
        if (deadUnit.IsPlayer())
        {
            playerUnits.Remove(deadUnit);
            deadPlayerUnits.Add(deadUnit);            
        }
        else
        {
            enemyUnits.Remove(deadUnit);
            deadEnemyUnits.Add(deadUnit);
        }

        if(deadPlayerUnits.Count == startingPlayerTeamSize)
        {
            onTeamWipe(false);
        }
        else if(deadEnemyUnits.Count == startingEnemyTeamSize)
        {
            onTeamWipe(true);
        }

        onUnitListUpdate();
    }

    public BattleUnit GetRandomPlayerUnit()
    {
        int randomInt = UnityEngine.Random.Range(0, playerUnits.Count);

        return playerUnits[randomInt];
    }

    public BattleUnit GetRandomEnemyUnit()
    {
        int randomInt = UnityEngine.Random.Range(0, enemyUnits.Count);

        return enemyUnits[randomInt];
    }
}

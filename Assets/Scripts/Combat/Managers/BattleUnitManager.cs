using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnitManager : MonoBehaviour
{ 
    BattleUnit[] battleUnits = null;

    PlayerTeam playerTeamInfo = null;
    List<Character> playerTeam = new List<Character>();
    List<BattleUnit> playerUnits = new List<BattleUnit>();
    List<BattleUnit> allPlayerUnits = new List<BattleUnit>();
    List<BattleUnit> deadPlayerUnits = new List<BattleUnit>();
    int startingPlayerTeamSize = 0;

    List<Character> enemyTeam = new List<Character>();
    List<BattleUnit> enemyUnits = new List<BattleUnit>();
    List<BattleUnit> deadEnemyUnits = new List<BattleUnit>();
    int startingEnemyTeamSize = 0;

    public event Action<bool> onTeamWipe;
    public event Action onUnitListUpdate;
    
    public void SetUpUnits(PlayerTeam _playerTeamInfo, List<Character> _enemyTeam, List<Transform> playerPositions, List<Transform> enemyPositions)
    {
        playerTeamInfo = _playerTeamInfo;

        playerTeam = playerTeamInfo.GetPlayerTeam();
        enemyTeam = _enemyTeam;

        SetUpPlayerUnits(playerTeam.Count, playerPositions);
        SetUpEnemyUnits(enemyTeam.Count, enemyPositions);

        startingPlayerTeamSize = playerTeam.Count;
        startingEnemyTeamSize = enemyTeam.Count;

        battleUnits = FindObjectsOfType<BattleUnit>();
    }

    private void SetUpPlayerUnits(int playerTeamSize, List<Transform> playerPositions)
    {
        for (int i = 0; i < playerTeamSize; i++)
        {
            Character currentCharacter = playerTeam[i];
            GameObject newUnitInstance = Instantiate(currentCharacter.GetBattleUnit().gameObject, playerPositions[i].position, playerPositions[i].rotation);
            BattleUnit newUnit = newUnitInstance.GetComponent<BattleUnit>();
            TeamInfo teamInfo = playerTeamInfo.GetTeamInfo(currentCharacter);

            newUnit.SetName(currentCharacter.GetName());

            playerUnits.Add(newUnit);
            allPlayerUnits.Add(newUnit);

            newUnit.SetUnitLevel(teamInfo.GetLevel());

            SetupUnitComponents(currentCharacter, newUnit, true);
            
            newUnit.SetResources(teamInfo.GetHealth(), teamInfo.GetMaxHealth(), teamInfo.GetSoulWell(), teamInfo.GetMaxSoulWell());            
        }
    }

    private void SetUpEnemyUnits(int enemyTeamSize, List<Transform> enemyPositions)
    {
        for (int i = 0; i < enemyTeamSize; i++)
        {
            Character currentCharacter = enemyTeam[i];
            GameObject newUnitInstance = Instantiate(currentCharacter.GetBattleUnit().gameObject, enemyPositions[i].position, enemyPositions[i].rotation);
            BattleUnit newUnit = newUnitInstance.GetComponent<BattleUnit>();

            enemyUnits.Add(newUnitInstance.GetComponent<BattleUnit>());

            newUnit.SetName(currentCharacter.GetName());

            newUnit.SetUnitXPAward(currentCharacter.GetXPAward());

            SetupUnitComponents(currentCharacter, newUnit, false);

            newUnit.CalculateResources();
        }
    }

    private void SetupUnitComponents(Character character, BattleUnit unit, bool isPlayer)
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

    public BattleUnit[] GetBattleUnits()
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

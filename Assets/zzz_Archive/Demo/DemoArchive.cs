using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoArchive : MonoBehaviour
{

    ////FromPlayerTeamManager -> PopulateTeamInfos()
    //if (characterKey == CharacterKey.Player) 
    //{
    //    EquipmentManager equipmentManager = FindObjectOfType<EquipmentManager>(true);
    //    bool hasSwordEquipped = equipmentManager.HasSwordEquipped();
    //    unit = unitDatabase.SetSelectedPlayerUnit(hasSwordEquipped);
    //}            


    ////From UnitDatabase
    //[SerializeField] Unit swordPlayer = null;
    //[SerializeField] Unit staffPlayer = null;
    //public Unit SetSelectedPlayerUnit(bool _isSword)
    //{
    //    Unit unit = null;

    //    if (!unitsDict.ContainsKey(CharacterKey.Player))
    //    {
    //        if (_isSword) unit = swordPlayer;
    //        else unit = staffPlayer;

    //        unitsDict.Add(CharacterKey.Player, unit);
    //    }
    //    else
    //    {
    //        unit = unitsDict[CharacterKey.Player];
    //    }

    //    return unit;
    //}

    ////From PlayerController
    //public void EquipWeapon(bool _isSword)
    //{
    //    EquipmentManager[] equipmentManager = FindObjectsOfType<EquipmentManager>(true);

    //    equipmentManager[0].EquipWeapon(_isSword);
    //}

    //public void RemoveTeammate()
    //{
    //    playerTeamManager.RemoveTeammate(PlayerKey.Rogue);
    //}
}

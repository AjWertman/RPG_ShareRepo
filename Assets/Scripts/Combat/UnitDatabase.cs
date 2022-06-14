using RPGProject.Core;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Combat
{
    public class UnitDatabase : MonoBehaviour
    {
        [SerializeField] Unit[] units = null;

        //Demo
        [SerializeField] Unit swordPlayer = null;
        [SerializeField] Unit staffPlayer = null;

        Dictionary<CharacterKey, Unit> unitsDict = new Dictionary<CharacterKey, Unit>();

        public void PopulateDatabase()
        {
            foreach (Unit unit in units)
            {
                CharacterKey characterKey = unit.GetCharacterKey();
                unitsDict.Add(characterKey, unit);
            }
        }

        public Unit GetUnit(CharacterKey _characterKey)
        {
            return unitsDict[_characterKey];
        }

        public Unit SetSelectedPlayerUnit(bool _isSword)
        {
            Unit unit = null;

            if (!unitsDict.ContainsKey(CharacterKey.Player))
            {
                if (_isSword) unit = swordPlayer;
                else unit = staffPlayer;

                unitsDict.Add(CharacterKey.Player, unit);
            }
            else
            {
                unit = unitsDict[CharacterKey.Player];
            }

            return unit;
        }
    }
}
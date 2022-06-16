using RPGProject.Core;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Combat
{
    public class UnitDatabase : MonoBehaviour
    {
        [SerializeField] Unit[] units = null;

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
    }
}
using System;

namespace RPGProject.Combat
{
    [Serializable]
    public struct UnitStartingPosition
    {
        public Unit unit;
        public GridCoordinates startCoordinates;
    }
}

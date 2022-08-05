using System;

namespace RPGProject.Combat.Grid
{
    [Serializable]
    public struct UnitStartingPosition
    {
        public Unit unit;
        public GridCoordinates startCoordinates;
    }
}

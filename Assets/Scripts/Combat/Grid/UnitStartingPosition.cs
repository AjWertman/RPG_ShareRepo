using System;

namespace RPGProject.Combat.Grid
{
    /// <summary>
    /// Struct that contains a unit and their starting coordinates based on the 
    /// center grid coordinates of their team.
    /// </summary>
    [Serializable]
    public struct UnitStartingPosition
    {
        public Unit unit;
        public GridCoordinates startCoordinates;
    }
}

using System;

namespace RPGProject.Combat.Grid
{
    /// <summary>
    /// Struct that contains a unit and their starting coordinates based on the 
    /// (0,0) or center of their team.
    /// </summary>
    [Serializable]
    public struct UnitStartingPosition
    {
        public Unit unit;
        public GridCoordinates startCoordinates;
    }
}

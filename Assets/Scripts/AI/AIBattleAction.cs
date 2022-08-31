using RPGProject.Combat.Grid;

namespace RPGProject.Combat
{
    /// <summary>
    /// The base of any action in combat.
    /// It contains the ability used, who it's used on, and the block to move to.
    /// All three variables are NOT required.
    /// </summary>
    public struct AIBattleAction
    {
        public GridBlock targetBlock;
        public Fighter target;
        public Ability selectedAbility;

        public AIBattleAction(GridBlock _targetBlock, Fighter _target, Ability _selectedAbility)
        {
            targetBlock = _targetBlock;
            target = _target;
            selectedAbility = _selectedAbility;
        }
    }
}


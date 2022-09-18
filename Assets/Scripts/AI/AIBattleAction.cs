using RPGProject.Combat.Grid;

namespace RPGProject.Combat
{
    /// <summary>
    /// The base of any AI action in combat.
    /// All three variables are NOT required to perform an action.
    /// If all three variables are null/empty, the AI will end its turn.
    /// </summary>
    public struct AIBattleAction
    {
        public GridBlock targetBlock; //The block the combatant will move to.
        public Fighter target; //The target they will use the ability on.
        public Ability selectedAbility; //The selected ability of the combatant.

        public AIBattleAction(GridBlock _targetBlock, Fighter _target, Ability _selectedAbility)
        {
            targetBlock = _targetBlock;
            target = _target;
            selectedAbility = _selectedAbility;
        }
    }
}


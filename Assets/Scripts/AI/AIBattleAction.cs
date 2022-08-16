using RPGProject.Combat.Grid;

namespace RPGProject.Combat
{
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


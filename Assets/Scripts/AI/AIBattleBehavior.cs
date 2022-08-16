using System;

namespace RPGProject.Combat.AI
{
    [Serializable]
    public struct AIBattleBehavior
    {
        public AIBattleType aiType;
        public AIActionType[] actionTypes;

        public AIBattleBehavior(AIBattleType _type, AIActionType[] _actionTypes)
        {
            aiType = _type;
            actionTypes = _actionTypes;
        }
    }

    public enum AIBattleType { Tank, mDamage, rDamage, Healer, Custom }

    public enum AIActionType
    {
        DealDamage_Or_Debuff,
        Taunt_Or_PullAgro,
        Heal,
        Support_Or_Buff,
        FindBetterPosition,
        SelfInterest
    }
}
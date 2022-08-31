using System;

namespace RPGProject.Combat.AI
{
    /// <summary>
    /// Allows for preset priorities of different combat actions based on AI type.
    /// </summary>
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

    /// <summary>
    /// Type of AI in combat.
    /// Custom is meant for customized AI behaviors for the player or unique enemies that require different behaviors.
    /// </summary>
    public enum AIBattleType { Tank, mDamage, rDamage, Healer, Custom }

    /// <summary>
    /// Different types of actions a unit could perform in combat.
    /// </summary>
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
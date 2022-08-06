using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CombatAIType { Tank, mDamage, rDamage, Healer, Custom}

[Serializable]
public struct CombatAIBehavior
{
    public CombatAIType aiType;
    public AIBehaviorPercentages aiBehaviors;
}

public enum AIBehaviorType { Attack, Agro, Heal, Support, Runaway}
[Serializable]
public struct AIBehaviorPercentages
{
    //Attack, damage, and kill targets
    public int attack;

    //Make enemies want to attack self
    public int agro;

    //Heal teammates and self
    public int heal;

    //Buff teammates and self
    public int support;

    //Distance self from enemies
    public int runaway;

    public Dictionary<AIBehaviorType, int> GetAIBehaviors()
    {
        Dictionary<AIBehaviorType, int> aiBehaviors = new Dictionary<AIBehaviorType, int>();

        aiBehaviors.Add(AIBehaviorType.Attack, attack);
        aiBehaviors.Add(AIBehaviorType.Agro, agro);
        aiBehaviors.Add(AIBehaviorType.Heal, heal);
        aiBehaviors.Add(AIBehaviorType.Support, support);
        aiBehaviors.Add(AIBehaviorType.Runaway, runaway);

        return aiBehaviors;
    }
}

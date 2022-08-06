using RPGProject.Combat;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIBehavior : MonoBehaviour
{
    public event Action<Fighter, Ability> onAIMoveSelection;

    public abstract void PlanMove(List<Fighter> _allFighters);
}

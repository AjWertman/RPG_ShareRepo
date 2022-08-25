using UnityEngine;

namespace RPGProject.Combat
{
    public interface CombatTarget
    {
        string Name();
        Transform GetAimTransform();
        T GetComponent<T>();
    }
}

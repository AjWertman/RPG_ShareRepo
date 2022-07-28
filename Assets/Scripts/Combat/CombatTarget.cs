using UnityEngine;

namespace RPGProject.Combat
{
    public interface CombatTarget
    {
        Transform GetAimTransform();
        T GetComponent<T>();
    }
}

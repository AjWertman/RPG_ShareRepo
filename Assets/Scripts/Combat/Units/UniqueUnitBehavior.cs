using RPGProject.GameResources;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Combat
{
    /// <summary>
    /// Base class to implement unique behaviors for specific combatants.
    /// </summary>
    public abstract class UniqueUnitBehavior: MonoBehaviour
    {
        protected Fighter fighter = null;
        protected Health health = null;

        public virtual void Initialize()
        {
            fighter = GetComponentInParent<Fighter>();
            health = GetComponentInParent<Health>();
        }

        public abstract List<AbilityBehavior> GetUniqueAbilityBehaviors();
        public abstract List<AbilityBehavior> GetNegatedBehaviors();
    }
}

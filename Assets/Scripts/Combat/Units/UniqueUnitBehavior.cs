using RPGProject.GameResources;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Combat
{
    public abstract class UniqueUnitBehavior: MonoBehaviour
    {
        protected Fighter fighter = null;
        protected Health health = null;

        public virtual void Initialize()
        {
            fighter = GetComponentInParent<Fighter>();
            health = GetComponentInParent<Health>();
        }

        public abstract List<AbilityBehavior> GetAbilityBehaviors();
    }
}

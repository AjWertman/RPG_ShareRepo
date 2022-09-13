using System;
using System.Collections.Generic;

namespace RPGProject.Combat
{
    public interface IUniqueUnit
    {
        void Initialize();
        List<AbilityBehavior> GetAbilityBehaviors();
    }
}

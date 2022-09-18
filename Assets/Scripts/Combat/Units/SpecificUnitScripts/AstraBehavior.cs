using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Combat
{
    public class AstraBehavior : UniqueUnitBehavior
    {
        [SerializeField] List<AbilityBehavior> negatedBehaviors = new List<AbilityBehavior>();

        public override List<AbilityBehavior> GetNegatedBehaviors()
        {
            return negatedBehaviors;
        }

        public override List<AbilityBehavior> GetUniqueAbilityBehaviors()
        {
            return new List<AbilityBehavior>();
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Combat
{
    public class AbilityObjectPool : MonoBehaviour
    {
        [SerializeField] Ability[] allAbilities = null;
        [SerializeField] int amountOfAbilityObjects = 4;

        //Refactor - only use for the abilities of that zone including known player abilities
        Dictionary<Ability, List<AbilityBehavior>> abilityPool = new Dictionary<Ability, List<AbilityBehavior>>();
        //Dictionary<Ability, GameObject> hitFXPool = new Dictionary<Ability, GameObject>();
        
        private void Awake()
        {
            CreateAbilityPool();
            //CreateHitFXPool();
        }

        private void CreateAbilityPool()
        {
            foreach (Ability ability in allAbilities)
            {
                List<AbilityBehavior> abilityBehaviorInstances = new List<AbilityBehavior>();

                for (int i = 0; i < amountOfAbilityObjects; i++)
                {
                    GameObject abilityInstance = Instantiate(ability.GetAbilityPrefab(), transform);
                    AbilityBehavior abilityBehavior = abilityInstance.GetComponent<AbilityBehavior>();
                    abilityBehavior.InitializeAbility(ability);
                    abilityBehavior.onAbilityDeath += ResetAbilityBehavior;

                    abilityBehaviorInstances.Add(abilityBehavior);
                }
                abilityPool.Add(ability, abilityBehaviorInstances);
            }

            ResetAbilityObjectPool();
        }

        private void ResetAbilityBehavior(AbilityBehavior _abilityToReset)
        {
            _abilityToReset.transform.parent = transform;
            _abilityToReset.transform.localPosition = Vector3.zero;
            _abilityToReset.gameObject.SetActive(false);
        }

        public void ResetAbilityObjectPool()
        {
            foreach (List<AbilityBehavior> abilityBehaviors in abilityPool.Values)
            {
                foreach (AbilityBehavior abilityBehavior in abilityBehaviors)
                {
                    ResetAbilityBehavior(abilityBehavior);
                }
            }
        }

        public AbilityBehavior GetAbilityInstance(Ability _ability)
        {
            AbilityBehavior availableAbilityBehavior = null;

            foreach (AbilityBehavior abilityBehavior in abilityPool[_ability])
            {
                if (!abilityBehavior.gameObject.activeSelf)
                {
                    availableAbilityBehavior = abilityBehavior;
                    break;
                }
            }

            return availableAbilityBehavior;
        }
    }
}

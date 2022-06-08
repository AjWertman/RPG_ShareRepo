using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Combat
{
    public class AbilityObjectPool : MonoBehaviour
    {
        [SerializeField] AbilityPrefab[] abilityPrefabs = null;
        [SerializeField] int amountOfAbilityObjects = 4;

        //Refactor - only use for the abilities of that zone including known player abilities

        Dictionary<AbilityObjectKey, List<AbilityBehavior>> abilityPool = new Dictionary<AbilityObjectKey, List<AbilityBehavior>>();
        //[SerializeField] HitFXPrefab[] hitFXPrefabs = null;
        //Dictionary<HitFXObjectKey, List<HitFXBehavior>> hitFXPool = new Dictionary<HitFXObjectKey, List<HitFXBehavior>>();

        private void Awake()
        {
            CreateAbilityPool();
            //CreateHitFXPool();
        }

        private void CreateAbilityPool()
        {
            foreach(AbilityPrefab abilityPrefab in abilityPrefabs)
            {
                List<AbilityBehavior> abilityBehaviorInstances = new List<AbilityBehavior>();

                for (int i = 0; i < 4; i++)
                {
                    GameObject abilityInstance = Instantiate(abilityPrefab.GetAbilityPrefab(), transform);
                    AbilityBehavior abilityBehavior = abilityInstance.GetComponent<AbilityBehavior>();
                    //abilityBehavior.InitializeAbility(abilityPrefabDict[abilityPrefab]);
                    abilityBehavior.onAbilityDeath += ResetAbilityBehavior;

                    abilityBehaviorInstances.Add(abilityBehavior);
                }

                abilityPool.Add(abilityPrefab.GetAbilityObjectKey(), abilityBehaviorInstances);
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

        public AbilityBehavior GetAbilityInstance(AbilityObjectKey _abilityObjectKey)
        {
            AbilityBehavior availableAbilityBehavior = null;

            foreach (AbilityBehavior abilityBehavior in abilityPool[_abilityObjectKey])
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

    [Serializable]
    public class AbilityPrefab
    {
        [SerializeField] AbilityObjectKey abilityObjectKey = AbilityObjectKey.None;
        [SerializeField] GameObject abilityPrefab = null;

        public AbilityObjectKey GetAbilityObjectKey()
        {
            return abilityObjectKey;
        }

        public GameObject GetAbilityPrefab()
        {
            return abilityPrefab;
        }
    }

    [Serializable]
    public class HitFXPrefab
    {
        [SerializeField] HitFXObjectKey hitFXObjectKey = HitFXObjectKey.None;
        [SerializeField] GameObject hitFXPrefab = null;

        public HitFXObjectKey GetHitFXObjectKey()
        {
            return hitFXObjectKey;
        }

        public GameObject GetHitFXPrefab()
        {
            return hitFXPrefab;
        }
    }
}

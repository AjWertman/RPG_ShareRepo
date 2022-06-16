using RPGProject.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Combat
{
    public class AbilityObjectPool : MonoBehaviour
    {
        [SerializeField] AbilityPrefab[] abilityPrefabs = null;
        [SerializeField] int amountOfAbilityObjects = 4;

        [SerializeField] HitFXPrefab[] hitFXPrefabs = null;

        Dictionary<AbilityObjectKey, List<AbilityBehavior>> abilityPool = new Dictionary<AbilityObjectKey, List<AbilityBehavior>>();
        Dictionary<HitFXObjectKey, GameObject> hitFXPool = new Dictionary<HitFXObjectKey, GameObject>();

        private void Start()
        {
            CreateAbilityPool();
            CreateHitFXPool();
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
                    abilityBehavior.onAbilityDeath += ResetAbilityBehavior;
                    abilityBehavior.hitFXSpawnRequest += SpawnHitFX;

                    abilityBehaviorInstances.Add(abilityBehavior);
                }

                abilityPool.Add(abilityPrefab.GetAbilityObjectKey(), abilityBehaviorInstances);
            }

            ResetAbilityObjectPool();
        }

        private void CreateHitFXPool()
        {
            foreach (HitFXPrefab hitFXPrefab in hitFXPrefabs)
            {
                GameObject hitFXInstance = Instantiate(hitFXPrefab.GetHitFXPrefab(), transform);
                HitFXObjectKey hitFXObjectKey = hitFXPrefab.GetHitFXObjectKey();
                hitFXInstance.GetComponent<ReturnAfterEffect>().onEffectCompletion += ReturnToPool;

                hitFXPool.Add(hitFXObjectKey, hitFXInstance);
                hitFXInstance.SetActive(false);
            }
        }

        private void SpawnHitFX(HitFXObjectKey _hitFXObjectKey, Vector3 _position)
        {
            GameObject hitFX = hitFXPool[_hitFXObjectKey];

            hitFX.transform.position = _position;

            hitFX.gameObject.SetActive(true);
        }

        private void ResetAbilityBehavior(AbilityBehavior _abilityToReset)
        {
            _abilityToReset.transform.parent = transform;
            _abilityToReset.transform.localPosition = Vector3.zero;
            _abilityToReset.gameObject.SetActive(false);
        }

        private void ReturnToPool(ReturnAfterEffect _returnAfterEffect)
        {
            _returnAfterEffect.transform.localPosition = Vector3.zero;
            _returnAfterEffect.gameObject.SetActive(false);
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

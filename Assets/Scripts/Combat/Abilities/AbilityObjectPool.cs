using RPGProject.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Combat
{
    /// <summary>
    /// Pool of all abilities
    /// </summary>
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

        public GameObject GetHitFX(HitFXObjectKey _hitFXObjectKey)
        {
            return hitFXPool[_hitFXObjectKey];
        }

        private void CreateAbilityPool()
        {
            foreach(AbilityPrefab abilityPrefab in abilityPrefabs)
            {
                List<AbilityBehavior> abilityBehaviorInstances = new List<AbilityBehavior>();

                for (int i = 0; i < 4; i++)
                {
                    GameObject abilityInstance = Instantiate(abilityPrefab.abilityPrefab, transform);
                    AbilityBehavior abilityBehavior = abilityInstance.GetComponent<AbilityBehavior>();
                    abilityBehavior.onAbilityDeath += ResetAbilityBehavior;
                    abilityBehavior.hitFXSpawnRequest += SpawnHitFX;

                    abilityBehaviorInstances.Add(abilityBehavior);

                    CreateChildBehaviors(abilityBehavior);
                }

                abilityPool.Add(abilityPrefab.abilityObjectKey, abilityBehaviorInstances);
            }

            ResetAbilityObjectPool();
        }

        private void CreateChildBehaviors(AbilityBehavior _abilityBehavior)
        {
            if(_abilityBehavior.childBehavior != null)
            {
                AbilityBehavior childBehavior = Instantiate(_abilityBehavior.childBehavior, _abilityBehavior.transform);
                childBehavior.hitFXSpawnRequest += SpawnHitFX;

                _abilityBehavior.SetChildAbilityBehavior(childBehavior);
                childBehavior.gameObject.SetActive(false);
            }
        }

        private void CreateHitFXPool()
        {
            foreach (HitFXPrefab hitFXPrefab in hitFXPrefabs)
            {
                GameObject hitFXInstance = Instantiate(hitFXPrefab.hitFXPrefab, transform);
                HitFXObjectKey hitFXObjectKey = hitFXPrefab.hitFXObjectKey;
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
    }

    [Serializable]
    public class AbilityPrefab
    {
        public AbilityObjectKey abilityObjectKey = AbilityObjectKey.None;
        public GameObject abilityPrefab = null;
    }

    [Serializable]
    public class HitFXPrefab
    {
        public HitFXObjectKey hitFXObjectKey = HitFXObjectKey.None;
        public GameObject hitFXPrefab = null;
    }
}

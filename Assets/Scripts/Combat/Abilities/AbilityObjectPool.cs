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
        [SerializeField] int amountOfAbilityObjects = 4;

        [SerializeField] HitFXPrefab[] hitFXPrefabs = null;

        Dictionary<AbilityObjectKey, List<AbilityBehavior>> abilityPoolDict = new Dictionary<AbilityObjectKey, List<AbilityBehavior>>();
        Dictionary<HitFXObjectKey, GameObject> hitFXPool = new Dictionary<HitFXObjectKey, GameObject>();

        private void Start()
        {
            CreateHitFXPool();
        }

        public void CreateAbilityObjects(List<Fighter> _allFighters)
        {           
            foreach (AbilityBehavior abilityBehavior in GetAbilityPrefabs(_allFighters))
            {
                List<AbilityBehavior> behaviorInstances = new List<AbilityBehavior>();
                AbilityObjectKey abilityObjectKey = abilityBehavior.abilityObjectKey;

                if (abilityObjectKey == AbilityObjectKey.None) continue;
                if (abilityPoolDict.ContainsKey(abilityObjectKey)) continue;

                int abiltyBehaviorCount = GetAmountToSpawn(_allFighters.Count +1, abilityBehavior.GetType());
                for (int i = 0; i < abiltyBehaviorCount; i++)
                {
                    AbilityBehavior abilityBehaviorInstance = Instantiate(abilityBehavior, transform);

                    abilityBehaviorInstance.onAbilityDeath += ResetAbilityBehavior;
                    abilityBehaviorInstance.hitFXSpawnRequest += SpawnHitFX;

                    CreateChildBehaviors(abilityBehaviorInstance);
                    behaviorInstances.Add(abilityBehaviorInstance);
                    abilityBehavior.gameObject.SetActive(false);
                }

                if(behaviorInstances.Count <= 0) continue;

                abilityPoolDict.Add(abilityObjectKey, behaviorInstances);
            }
        }

        private int GetAmountToSpawn(int _amountOfFighters, Type _abilityType)
        {
            int amountToSpawn = amountOfAbilityObjects;

            if (_abilityType == typeof(Turret) || _abilityType == typeof(JustPlayEffect)) amountToSpawn = 1;
            else if (_abilityType == typeof(EffectOverTime)) amountToSpawn = _amountOfFighters;
            else if (_abilityType == typeof(BattleTeleporter)) amountToSpawn = 2;

            return amountToSpawn;
        }

        private List<AbilityBehavior> GetAbilityPrefabs(List<Fighter> _allFighters)
        {
            List<AbilityBehavior> abilityBehaviorPrefabs = new List<AbilityBehavior>();
            foreach(Fighter fighter in _allFighters)
            {
                foreach (Ability ability in fighter.GetKnownAbilities())
                {
                    if (ability.combo == null || ability.combo.Count <= 0) continue;
                    foreach (ComboLink comboLink in ability.combo)
                    {
                        AbilityBehavior behaviorPrefab = comboLink.abilityBehavior;
                        if (comboLink.abilityBehavior == null || abilityBehaviorPrefabs.Contains(behaviorPrefab)) continue;

                        abilityBehaviorPrefabs.Add(comboLink.abilityBehavior);

                        AbilityBehavior childPrefab = behaviorPrefab.childBehavior;
                        if (childPrefab == null || abilityBehaviorPrefabs.Contains(childPrefab)) continue;

                        abilityBehaviorPrefabs.Add(childPrefab);
                    }
                }

                foreach(UniqueUnitBehavior uniqueUnit in fighter.GetComponentsInChildren<UniqueUnitBehavior>())
                {
                    foreach(AbilityBehavior abilityBehavior in uniqueUnit.GetAbilityBehaviors())
                    {
                        if (abilityBehavior == null || abilityBehaviorPrefabs.Contains(abilityBehavior)) continue;
                        abilityBehaviorPrefabs.Add(abilityBehavior);
                    }
                }
            }

            return abilityBehaviorPrefabs;
        }

        public AbilityBehavior GetAbilityInstance(AbilityObjectKey _abilityObjectKey)
        {
            AbilityBehavior availableAbilityBehavior = null;

            foreach (AbilityBehavior abilityBehavior in abilityPoolDict[_abilityObjectKey])
            {
                if (!abilityBehavior.gameObject.activeSelf)
                {
                    availableAbilityBehavior = abilityBehavior;
                    break;
                }
            }

            return availableAbilityBehavior;
        }

        ///

        public void ResetAbilityObjectPool()
        {
            foreach (List<AbilityBehavior> abilityBehaviors in abilityPoolDict.Values)
            {
                foreach (AbilityBehavior abilityBehavior in abilityBehaviors)
                {
                    ResetAbilityBehavior(abilityBehavior);
                }
            }
        }

        public GameObject GetHitFX(HitFXObjectKey _hitFXObjectKey)
        {
            return hitFXPool[_hitFXObjectKey];
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

        public void SpawnHitFX(HitFXObjectKey _hitFXObjectKey, Vector3 _position)
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
    public class HitFXPrefab
    {
        public HitFXObjectKey hitFXObjectKey = HitFXObjectKey.None;
        public GameObject hitFXPrefab = null;
    }
}

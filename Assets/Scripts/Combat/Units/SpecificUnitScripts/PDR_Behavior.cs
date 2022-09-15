using System.Collections.Generic;
using RPGProject.Core;
using RPGProject.GameResources;
using UnityEngine;

namespace RPGProject.Combat
{
    public class PDR_Behavior : UniqueUnitBehavior
    {
        [SerializeField] float percentageThatCausesRage = .25f;
        [SerializeField] AbilityBehavior abilityBehaviorPrefab = null;

        [SerializeField] SkinnedMeshRenderer meshRenderer = null;
        [SerializeField] Material normalMaterial = null;
        [SerializeField] Material enragedMaterial = null;

        AbilityBehavior abilityBehaviorInstance = null;

        UnitStatus myStatus = new UnitStatus();

        Fighter protagonistFighter = null;
        Health protagonistHealth = null;

        public override void Initialize()
        {
            base.Initialize();

            myStatus = fighter.unitStatus;

            CharacterKey protagonistKey = CharacterKey.Protagonist;
            foreach(Fighter fighter in FindObjectsOfType<Fighter>())
            {
                CharacterKey characterKey = fighter.unitInfo.characterKey;
                
                if(characterKey == protagonistKey)
                {
                    protagonistFighter = fighter;
                    protagonistHealth = protagonistFighter.GetHealth();
                    protagonistHealth.onHealthChange += EvaluateProtagonistStatus;
                    break;
                }
            }

            abilityBehaviorInstance = Instantiate(abilityBehaviorPrefab,fighter.characterMesh.headTransform);
            abilityBehaviorInstance.SetupAbility(fighter, fighter, 0, false, 99);
            abilityBehaviorInstance.gameObject.SetActive(false);
        }

        private void EvaluateProtagonistStatus(bool NaN0, float NaN1)
        {
            float protagonistHealthPercentage = protagonistHealth.healthPercentage;
            bool isAlreadyEnraged = myStatus.IsAlreadyAffected(abilityBehaviorInstance);

            if(protagonistHealthPercentage <= percentageThatCausesRage)
            {
                if (isAlreadyEnraged) return;
                fighter.unitStatus.ApplyActiveAbilityBehavior(abilityBehaviorInstance);
                abilityBehaviorInstance.PerformAbilityBehavior();
                abilityBehaviorInstance.gameObject.SetActive(true);
                meshRenderer.material = enragedMaterial;
            }
            else
            {
                if (!isAlreadyEnraged) return;
                fighter.unitStatus.RemoveActiveAbilityBehavior(abilityBehaviorInstance);
                abilityBehaviorInstance.gameObject.SetActive(false);
                meshRenderer.material = normalMaterial;
            }
        }

        public override List<AbilityBehavior> GetAbilityBehaviors()
        {
            return new List<AbilityBehavior>();
        }
    }
}
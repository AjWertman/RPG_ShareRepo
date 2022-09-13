using System.Collections.Generic;
using RPGProject.Core;
using RPGProject.GameResources;
using UnityEngine;

namespace RPGProject.Combat
{
    public class PDR_Behavior : MonoBehaviour, IUniqueUnit
    {
        [SerializeField] float percentageThatCausesRage = .25f;
        [SerializeField] AbilityBehavior abilityBehaviorPrefab = null;

        [SerializeField] SkinnedMeshRenderer meshRenderer = null;
        [SerializeField] Material normalMaterial = null;
        [SerializeField] Material enragedMaterial = null;

        AbilityBehavior abilityBehaviorInstance = null;

        Fighter myFighter = null;
        UnitStatus myStatus = null;

        Fighter protagonistFighter = null;
        Health protagonistHealth = null;

        float startingStrength = 0f;

        public void Initialize()
        {
            myFighter = GetComponentInParent<Fighter>();
            myStatus = myFighter.unitStatus;

            startingStrength = myFighter.strength;
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

            abilityBehaviorInstance = Instantiate(abilityBehaviorPrefab,myFighter.characterMesh.headTransform);
            abilityBehaviorInstance.SetupAbility(myFighter, myFighter, 0, false, 99);
            abilityBehaviorInstance.gameObject.SetActive(false);
        }

        private void EvaluateProtagonistStatus(bool NaN0, float NaN1)
        {
            float protagonistHealthPercentage = protagonistHealth.healthPercentage;
            bool isAlreadyEnraged = myStatus.IsAlreadyAffected(abilityBehaviorInstance);

            if(protagonistHealthPercentage <= percentageThatCausesRage)
            {
                if (isAlreadyEnraged) return;
                myFighter.unitStatus.ApplyActiveAbilityBehavior(abilityBehaviorInstance);
                myFighter.strength *= 1.5f;
                abilityBehaviorInstance.gameObject.SetActive(true);
                meshRenderer.material = enragedMaterial;
            }
            else
            {
                if (!isAlreadyEnraged) return;
                myFighter.unitStatus.RemoveActiveAbilityBehavior(abilityBehaviorInstance);
                myFighter.strength = startingStrength;
                abilityBehaviorInstance.gameObject.SetActive(false);
                meshRenderer.material = normalMaterial;
            }
        }

        public List<AbilityBehavior> GetAbilityBehaviors()
        {
            return new List<AbilityBehavior>();
        }
    }
}
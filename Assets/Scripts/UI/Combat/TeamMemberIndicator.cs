using System;
using RPGProject.Combat;
using RPGProject.GameResources;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RPGProject.UI
{
    /// <summary>
    /// Used to give the player reference to the current status of their team during combat.
    /// </summary>
    public class TeamMemberIndicator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] Image background = null;
        [SerializeField] Image faceImage = null;
        [SerializeField] Slider healthSlider = null;
        [SerializeField] Slider energySlider = null;

        Fighter fighter = null;
        Health teamMemberHealth = null;
        Energy teamMemberEnergy = null;

        RectTransform rect;

        public event Action<Fighter> onHighlight;
        public event Action onUnhighlight;
        
        private void Awake()
        {
            rect = GetComponent<RectTransform>();
        }

        public void SetupIndicator(Fighter _fighter)
        {
            fighter = _fighter;

            CharacterMesh characterMesh = fighter.characterMesh;
            teamMemberHealth = fighter.GetHealth();
            teamMemberEnergy = fighter.GetEnergy();

            faceImage.sprite = characterMesh.faceImage;
            background.color = characterMesh.uiColor;
            //refactor - Shield? AP/Energy? Rage/Special Power source?

            SetHealthPercentage(false, 42069);
            SetEnergyPercentage();
            
            teamMemberHealth.onHealthChange += SetHealthPercentage;
            teamMemberEnergy.onEnergyChange += SetEnergyPercentage;
        }

        public void HideIndicator(bool _shouldHide)
        {
            background.gameObject.SetActive(!_shouldHide);
        }

        public Fighter GetFighter()
        {
            return fighter;
        }

        public RectTransform GetRect()
        {
            return rect;
        }

        private void SetHealthPercentage(bool _arg1, float _arg2)
        {
            healthSlider.value = teamMemberHealth.healthPercentage;
        }

        private void SetEnergyPercentage()
        {
            energySlider.value = teamMemberEnergy.GetEnergyPercentage();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (fighter == null) return;
            onHighlight(fighter);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onUnhighlight();
        }
    }
}
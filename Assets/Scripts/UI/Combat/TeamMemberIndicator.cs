using System;
using RPGProject.Combat;
using RPGProject.GameResources;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RPGProject.UI
{
    public class TeamMemberIndicator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] Image background = null;
        [SerializeField] Image faceImage = null;
        [SerializeField] Slider shieldSlider = null;
        [SerializeField] Slider healthSlider = null;

        Fighter fighter = null;
        Health teamMemberHealth = null;

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
            teamMemberHealth = fighter.GetHealthComponent();

            faceImage.sprite = characterMesh.faceImage;
            background.color = characterMesh.uiColor;
            //refactor - Shield? AP/Energy? Rage/Special Power source?
            shieldSlider.value = 1;
            SetHealthPercentage(false, 42069);

            teamMemberHealth.onHealthChange += SetHealthPercentage;
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
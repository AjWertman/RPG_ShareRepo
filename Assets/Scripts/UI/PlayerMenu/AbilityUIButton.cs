using RPGProject.Combat;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RPGProject.UI
{
    public class AbilityUIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        Ability myAbility = null;

        TextMeshProUGUI text = null;
        Image image = null;

        public event Action<Ability> onMouseEnter;
        public event Action onMouseExit;

        private void Awake()
        {
            text = GetComponentInChildren<TextMeshProUGUI>();
            image = GetComponent<Image>();
        }

        public void SetupAbilityButton(Ability _ability)
        {
            if (_ability == null) return;
            myAbility = _ability;
            text.text = myAbility.abilityName;
            text.color = myAbility.textColor;
            image.color = myAbility.buttonColor;
        }

        public void ResetSpellButton()
        {
            myAbility = null;
            text.text = "Ability";
            text.color = Color.black;
            image.color = Color.white;
        }

        public Ability GetAbility()
        {
            return myAbility;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (myAbility == null) return;
            onMouseEnter(myAbility);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onMouseExit();
        }
    }
}

using RPGProject.Combat;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RPGProject.UI
{
    /// <summary>
    /// A button used to be an interactive instance of an ability.
    /// </summary>
    public class AbilityButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        Ability assignedAbility = null;

        Button button = null;
        Image buttonImage = null;
        TextMeshProUGUI buttonText = null;

        public event Action<Ability> onSelect;
        public event Action<Ability> onPointerEnter;
        public event Action onPointerExit;

        public void InitalizeAbilityButton()
        {
            button = GetComponent<Button>();
            buttonText = GetComponentInChildren<TextMeshProUGUI>();
            button.onClick.AddListener(() => onSelect(assignedAbility));
        }

        public void SetAssignedAbility(Ability _abilityToSet)
        {
            button.interactable = true;
            assignedAbility = _abilityToSet;
            button.image.color = assignedAbility.buttonColor;
            buttonText.text = assignedAbility.abilityName;
            buttonText.color = assignedAbility.textColor;
        }

        public void ResetAbilityButton()
        {
            button.interactable = false;
            assignedAbility = null;
            button.image.color = Color.white;
            buttonText.text = "Ability";
            buttonText.color = Color.black;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (assignedAbility == null) return;
            onPointerEnter(assignedAbility);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (assignedAbility == null) return;
            onPointerExit();
        }

        public Ability GetAssignedAbility()
        {
            return assignedAbility;
        }

        public Button GetButton()
        {
            return button;
        }
    }
}

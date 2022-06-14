using RPGProject.Combat;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RPGProject.UI
{
    public class TargetButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        Button button = null;
        TextMeshProUGUI buttonText = null;

        Fighter assignedTarget = null;

        public event Action<Fighter> onSelect;
        public event Action<Fighter> onPointerEnter;
        public event Action onPointerExit;

        public void InitalizeTargetButton()
        {
            button = GetComponent<Button>();
            buttonText = GetComponentInChildren<TextMeshProUGUI>();

            button.onClick.AddListener(OnSelect);
        }

        public void SetupTargetButton(Fighter _target)
        {
            assignedTarget = _target;
            buttonText.text = assignedTarget.GetUnitInfo().GetUnitName();
        }

        public void OnSelect()
        {
            onSelect(assignedTarget);
            onPointerExit();
        }

        public void ResetTargetButton()
        {
            assignedTarget = null;
            buttonText.text = "Target";
        }

        public Fighter GetTarget()
        {
            return assignedTarget;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            onPointerEnter(assignedTarget);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onPointerExit();
        }
    }
}

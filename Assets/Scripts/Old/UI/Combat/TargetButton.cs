using RPGProject.Combat;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TargetButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Button button = null;
    TextMeshProUGUI buttonText = null;

    BattleUnit assignedTarget = null;

    public event Action<BattleUnit> onSelect;
    public event Action<BattleUnit> onPointerEnter;
    public event Action onPointerExit;

    private void Awake()
    {
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();

        button.onClick.AddListener(() => onSelect(assignedTarget));
    }

    public void SetupTargetButton(BattleUnit _target)
    {
        assignedTarget = _target;
        buttonText.text = assignedTarget.GetBattleUnitInfo().GetUnitName();
    }

    public void ResetTargetButton()
    {
        assignedTarget = null;
        buttonText.text = "Target";
    }

    public BattleUnit GetTarget()
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

using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class AbilityButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Ability assignedAbility = null;
    string cantCastReason = "";

    public event Action<Ability,string> onPointerEnter;
    public event Action onPointerExit;

    public void SetAssignedAbility(Ability abilityToSet)
    {
        assignedAbility = abilityToSet;
    }

    public Ability GetAssignedAbility()
    {
        return assignedAbility;
    }

    public void SetCantCastReason(string reason)
    {
        cantCastReason = reason;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (assignedAbility == null) return;
        if (assignedAbility.abilityType == AbilityType.Physical) return;
        onPointerEnter(assignedAbility, cantCastReason);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (assignedAbility == null) return;
        if (assignedAbility.abilityType == AbilityType.Physical) return;
        onPointerExit();
    }
}

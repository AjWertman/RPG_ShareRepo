using RPGProject.Combat;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpellUIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Ability myAbility = null;

    public event Action<Ability> onMouseEnter;
    public event Action onMouseExit;

    public void SetupSpellButton(Ability spell)
    {
        myAbility = spell;
        GetComponentInChildren<TextMeshProUGUI>().text = myAbility.GetAbilityName();
        GetComponentInChildren<TextMeshProUGUI>().color = myAbility.GetTextColor();
        GetComponent<Image>().color = myAbility.GetButtonColor();
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

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
        GetComponentInChildren<TextMeshProUGUI>().text = myAbility.abilityName;
        GetComponentInChildren<TextMeshProUGUI>().color = myAbility.textColor;
        GetComponent<Image>().color = myAbility.buttonColor;
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

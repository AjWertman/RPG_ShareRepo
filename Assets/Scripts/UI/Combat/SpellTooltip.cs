using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellTooltip : MonoBehaviour
{
    [SerializeField] Image background = null;
    [SerializeField] TextMeshProUGUI abilityNameText = null;
    [SerializeField] TextMeshProUGUI emblemText = null;
    [SerializeField] GameObject cantCastObject = null;
    [SerializeField] TextMeshProUGUI cantCastReasonText = null;
    [SerializeField] TextMeshProUGUI descriptionText = null;

    public void SetupTooltip(Ability ability)
    {
        cantCastObject.SetActive(false);

        SetupColors(ability);

        abilityNameText.text = ability.abilityName;
        emblemText.text = ability.emblem.ToString();
        descriptionText.text = ability.spellDescription;      
    }

    private void SetupColors(Ability ability)
    {
        background.color = ability.buttonColor;
        foreach(TextMeshProUGUI text in GetComponentsInChildren<TextMeshProUGUI>())
        {
            text.color = ability.textColor;
        }
    }

    public void SetupCantCast(string reason)
    {
        cantCastObject.SetActive(true);
        cantCastReasonText.text = reason;
    }
}

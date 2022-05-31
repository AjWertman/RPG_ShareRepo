using RPGProject.Combat;
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

        abilityNameText.text = ability.GetAbilityName();
        descriptionText.text = ability.GetDescription();      
    }

    private void SetupColors(Ability ability)
    {
        background.color = ability.GetButtonColor();
        foreach(TextMeshProUGUI text in GetComponentsInChildren<TextMeshProUGUI>())
        {
            text.color = ability.GetTextColor();
        }
    }

    public void SetupCantCast(string reason)
    {
        cantCastObject.SetActive(true);
        cantCastReasonText.text = reason;
    }
}

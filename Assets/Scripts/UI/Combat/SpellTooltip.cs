using RPGProject.Combat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPGProject.UI
{
    /// <summary>
    /// Indicates the information of an ability, 
    /// including if the caster can use an ability.
    /// </summary>
    public class SpellTooltip : MonoBehaviour
    {
        [SerializeField] Image background = null;
        [SerializeField] TextMeshProUGUI abilityNameText = null;
        [SerializeField] TextMeshProUGUI emblemText = null;
        [SerializeField] GameObject cantCastObject = null;
        [SerializeField] TextMeshProUGUI cantCastReasonText = null;
        [SerializeField] TextMeshProUGUI descriptionText = null;

        public void SetupTooltip(Ability _ability)
        {
            cantCastObject.SetActive(false);

            SetupColors(_ability);

            abilityNameText.text = _ability.abilityName;
            descriptionText.text = _ability.description;
        }

        private void SetupColors(Ability _ability)
        {
            background.color = _ability.buttonColor;
            foreach (TextMeshProUGUI text in GetComponentsInChildren<TextMeshProUGUI>())
            {
                text.color = _ability.textColor;
            }
        }

        public void SetupCantCast(string _reason)
        {
            cantCastObject.SetActive(true);
            cantCastReasonText.text = _reason;
        }
    }
}

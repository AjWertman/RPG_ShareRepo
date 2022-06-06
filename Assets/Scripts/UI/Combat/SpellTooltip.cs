using RPGProject.Combat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPGProject.UI
{
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

            abilityNameText.text = _ability.GetAbilityName();
            descriptionText.text = _ability.GetDescription();
        }

        private void SetupColors(Ability _ability)
        {
            background.color = _ability.GetButtonColor();
            foreach (TextMeshProUGUI text in GetComponentsInChildren<TextMeshProUGUI>())
            {
                text.color = _ability.GetTextColor();
            }
        }

        public void SetupCantCast(string reason)
        {
            cantCastObject.SetActive(true);
            cantCastReasonText.text = reason;
        }
    }
}

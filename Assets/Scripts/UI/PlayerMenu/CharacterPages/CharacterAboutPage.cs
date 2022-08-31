using RPGProject.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPGProject.UI
{
    /// <summary>
    /// The page of the character menu that contains the general info/summary of a character.
    /// </summary>
    public class CharacterAboutPage : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI nameText0 = null;
        [SerializeField] TextMeshProUGUI nameText1 = null;

        [SerializeField] Image classImage = null;

        [SerializeField] TextMeshProUGUI descriptionText = null;

        [SerializeField] Image characterImage = null;

        [SerializeField] TextMeshProUGUI levelText = null;
        [SerializeField] TextMeshProUGUI ageText = null;
        [SerializeField] TextMeshProUGUI classText = null;
        [SerializeField] TextMeshProUGUI subClassText = null;

        public void SetupCharacterPage(PlayableCharacter _character, int _level)
        {
            ChangeNameTexts(_character.characterName);
            characterImage.sprite = _character.fullBodyImage;
            descriptionText.text = _character.summaryText;
            ChangeExtraInfoTexts(_character, _level);
        }

        private void ChangeNameTexts(string _name)
        {
            nameText0.text = _name;
            nameText1.text = _name;
        }

        private void ChangeExtraInfoTexts(PlayableCharacter _character, int _level)
        {
            string levelString = ("Level: " + _level.ToString());
            levelText.text = levelString;

            string ageString = ("Age: " + _character.age.ToString());
            ageText.text = ageString;

            //classText.text = _character.GetClass().ToString();
            //subClassText.text = _character.GetSubClass().ToString();
        }
    }
}

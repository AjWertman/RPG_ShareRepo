using RPGProject.Combat;
using RPGProject.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPGProject.UI
{
    public class CharacterAboutPage : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI nameText0 = null;
        [SerializeField] TextMeshProUGUI nameText1 = null;

        [SerializeField] Image emblemImage = null;

        [SerializeField] TextMeshProUGUI descriptionText = null;

        [SerializeField] Image characterImage = null;

        [SerializeField] TextMeshProUGUI levelText = null;
        [SerializeField] TextMeshProUGUI ageText = null;
        [SerializeField] TextMeshProUGUI classText = null;
        [SerializeField] TextMeshProUGUI subClassText = null;

        public void SetupCharacterPage(PlayableCharacter _character)
        {
            //Refactor Playable charactewr;
            //ChangeNameTexts(_character.GetUnitName());
            //characterImage.sprite = character.GetFullBodyImage();
            //descriptionText.text = character.GetSummary();
            //ChangeExtraInfoTexts(_character, _teamInfo);
        }

        private void ChangeNameTexts(string _name)
        {
            nameText0.text = _name;
            nameText1.text = _name;
        }

        private void ChangeExtraInfoTexts(PlayableCharacter _character)
        {
            //string levelString = ("Level: " + _teamInfo.GetLevel().ToString());
            //levelText.text = levelString;

            //string ageString = ("Age: " + character.GetAge().ToString());
            //ageText.text = ageString;

            //affiliationText.text = character.GetAffiliation();
        }
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterAboutPage : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText0 = null;
    [SerializeField] TextMeshProUGUI nameText1 = null;

    [SerializeField] Image emblemImage = null;

    [SerializeField] TextMeshProUGUI descriptionText = null;

    [SerializeField] Image characterImage = null;

    [SerializeField] TextMeshProUGUI levelText = null;
    [SerializeField] TextMeshProUGUI ageText = null;
    [SerializeField] TextMeshProUGUI emblemNameText = null;
    [SerializeField] TextMeshProUGUI affiliationText = null;

    public void SetupCharacterPage(Unit character, TeamInfo teamInfo)
    {
        ChangeNameTexts(character.GetName());
        characterImage.sprite = character.GetFullBodyImage();
        //descriptionText.text = character.GetSummary();
        ChangeExtraInfoTexts(character, teamInfo);
    }

    private void ChangeNameTexts(string name)
    {
        nameText0.text = name;
        nameText1.text = name;
    }

    private void ChangeExtraInfoTexts(Unit character, TeamInfo teamInfo)
    {
        string levelString = ("Level: " + teamInfo.GetLevel().ToString());
        levelText.text = levelString;

        //string ageString = ("Age: " + character.GetAge().ToString());
        //ageText.text = ageString;

        //affiliationText.text = character.GetAffiliation();
    }
}

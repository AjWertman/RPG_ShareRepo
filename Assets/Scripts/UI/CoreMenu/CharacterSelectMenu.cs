using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectMenu : MonoBehaviour
{
    [SerializeField] Transform buttonContainer = null;
    [SerializeField] GameObject characterSelectButton = null;

    [SerializeField] Button backButton = null;

    public event Action<Character> onCharacterSelect;
    public event Action onBackToMainCoreMenu;

    public void SetupCharacterSelectMenu(List<Character> characters)
    {
        backButton.onClick.AddListener(() => onBackToMainCoreMenu());

        if(buttonContainer.childCount > 0)
        {
            foreach(Transform button in buttonContainer)
            {
                Destroy(button.gameObject);
            }
        }
        foreach (Character character in characters)
        {
            GameObject buttonInstance = Instantiate(characterSelectButton, buttonContainer);
            Button button = buttonInstance.GetComponent<Button>();
            buttonInstance.GetComponentInChildren<TextMeshProUGUI>().text = character.GetName();

            button.onClick.AddListener(() => onCharacterSelect(character));
            button.onClick.AddListener(() => ForceDeactivateHighlight(buttonInstance.GetComponent<MenuItemHighlighter>()));
        }
    }

    private void ForceDeactivateHighlight(MenuItemHighlighter highlighter)
    {
        highlighter.ForceUnhighlight();
    }
}
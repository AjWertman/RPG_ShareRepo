using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPGProject.Dialogue
{
    /// <summary>
    /// Button that the player can select to choose a dialogue option.
    /// </summary>
    public class DialogueButton : MonoBehaviour
    {
        Button button = null;
        TextMeshProUGUI text = null;

        DialogueNode dialogueNode = null;

        public event Action<DialogueNode> onDialogueChoiceSelect;

        public void InitalizeDialogueButton()
        {
            button = GetComponent<Button>();
            text = GetComponentInChildren<TextMeshProUGUI>();

            button.onClick.AddListener(() => onDialogueChoiceSelect(dialogueNode));
        }

        public void SetupDialogueButton(DialogueNode _dialogueNode)
        {
            dialogueNode = _dialogueNode;
            button.interactable = true;
            text.text = dialogueNode.nodeText;
        }

        public void ResetDialogueButton()
        {
            text.text = "";
            button.interactable = false;
            dialogueNode = null;
        }
    }
}
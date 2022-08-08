using RPGProject.Control;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPGProject.Dialogue
{
    public class DialogueUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI conversantName = null;

        [SerializeField] GameObject aiResponseRoot = null;
        [SerializeField] TextMeshProUGUI dialogueText = null;
        [SerializeField] Button nextButton = null;
        TextMeshProUGUI nextButtonText = null;

        [SerializeField] Transform choiceRoot = null;
        [SerializeField] GameObject choiceButtonPrefab = null;

        [SerializeField] Button quitButton = null;

        PlayerConversant playerConversant = null;

        List<DialogueButton> dialogueButtonsPool = new List<DialogueButton>();

        private void Awake()
        {
            CreateDialogueButtonsPool();
        }

        private void Start()
        {
            playerConversant = GameObject.FindWithTag("Player").GetComponent<PlayerConversant>();

            nextButtonText = nextButton.GetComponentInChildren<TextMeshProUGUI>();

            playerConversant.onConversationUpdated += UpdateUIText;
            quitButton.onClick.AddListener(() => playerConversant.Quit());

            UpdateUIText();
        }

        private void UpdateUIText()
        {
            gameObject.SetActive(playerConversant.IsChatting());

            if (!playerConversant.IsChatting())
            {
                return;
            }

            quitButton.interactable = !playerConversant.GetCurrentDialogue().isEssentialDialogue;

            SetConversantName();

            aiResponseRoot.SetActive(!playerConversant.IsChoosing());
            choiceRoot.gameObject.SetActive(playerConversant.IsChoosing());

            if (playerConversant.IsChoosing())
            {
                BuildChoiceList();
            }
            else
            {
                dialogueText.text = playerConversant.GetText();
                nextButton.onClick.RemoveAllListeners();

                if (playerConversant.HasNext())
                {
                    nextButtonText.text = "Next";
                    nextButton.onClick.AddListener(() => playerConversant.OnNextButton());
                }
                else
                {
                    nextButtonText.text = "Ok";
                    nextButton.onClick.AddListener(() => playerConversant.Quit());
                }
            }
        }

        private void SetConversantName()
        {
            string overrideName = playerConversant.GetCurrentDialogueNode().overrideConversantName;

            if (overrideName == "")
            {
                conversantName.text = playerConversant.GetCurrentConversantName();
            }
            else
            {
                conversantName.text = overrideName;
            }
        }

        private void OnDialogueChoiceSelect(DialogueNode _dialogueNode)
        {
            playerConversant.SelectChoice(_dialogueNode);
        }

        private void BuildChoiceList()
        {
            ClearChoiceList();

            int index = 0;

            foreach (DialogueNode node in playerConversant.GetChoices())
            {
                DialogueButton newDialogueButton = dialogueButtonsPool[index];
                newDialogueButton.SetupDialogueButton(node);

                newDialogueButton.gameObject.SetActive(true);

                index++;
            }
        }

        private void CreateDialogueButtonsPool()
        {
            for (int i = 0; i < 4; i++)
            {
                GameObject buttonInstance = Instantiate(choiceButtonPrefab, choiceRoot);
                DialogueButton dialogueButton = buttonInstance.GetComponent<DialogueButton>();

                dialogueButton.InitalizeDialogueButton();
                dialogueButton.onDialogueChoiceSelect += OnDialogueChoiceSelect;

                dialogueButtonsPool.Add(dialogueButton);
            }

            ClearChoiceList();
        }

        private void ClearChoiceList()
        {
            foreach (DialogueButton dialogueButton in dialogueButtonsPool)
            {
                dialogueButton.ResetDialogueButton();
                dialogueButton.gameObject.SetActive(false);
            }
        }
    }
}
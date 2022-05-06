using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI conversantName = null;    
    
    [SerializeField] GameObject aiResponseRoot = null;
    [SerializeField] TextMeshProUGUI dialogueText = null;
    [SerializeField] Button nextButton = null;
    
    [SerializeField] Transform choiceRoot = null;
    [SerializeField] GameObject choiceButtonPrefab = null;

    [SerializeField] Button quitButton = null;

    PlayerConversant playerConversant = null;

    private void Start()
    {
        playerConversant = GameObject.FindWithTag("Player").GetComponent<PlayerConversant>();

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

        quitButton.interactable = !playerConversant.GetCurrentDialogue().IsEssentialDialogue();

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
                nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Next";
                nextButton.onClick.AddListener(() => playerConversant.OnNextButton());
            }
            else
            {
                nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Ok";
                nextButton.onClick.AddListener(() => playerConversant.Quit());
            }       
        }
    }

    private void SetConversantName()
    {
        string overrideName = playerConversant.GetCurrentDialogueNode().GetOverrideName();

        if (overrideName == "")
        {
            conversantName.text = playerConversant.GetCurrentConversantName();
        }
        else
        {
            conversantName.text = overrideName;
        }
    }

    private void BuildChoiceList()
    {
        foreach (Transform child in choiceRoot.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (DialogueNode node in playerConversant.GetChoices())
        {
            GameObject buttonInstance = Instantiate(choiceButtonPrefab, choiceRoot);

            buttonInstance.GetComponentInChildren<TextMeshProUGUI>().text = node.GetNodeText();

            buttonInstance.GetComponentInChildren<Button>().onClick.AddListener(() =>
            {
                playerConversant.SelectChoice(node);
            });
        }
    }
}

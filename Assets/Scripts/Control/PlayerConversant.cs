using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerConversant : MonoBehaviour
{
    [SerializeField] string playerName = "Ren";

    Dialogue currentDialogue = null;
    DialogueNode currentNode = null;

    AIConversant currentConversant = null;

    bool isChoosing = false;

    public event Action onConversationUpdated;

    public void StartDialogue(AIConversant conversant, Dialogue newDialogue)
    {
        currentConversant = conversant;
        currentDialogue = newDialogue;

        currentNode = currentDialogue.GetRootNode();

        TriggerEnterAction();
        onConversationUpdated();
    }

    public bool IsChatting()
    {
        if(currentDialogue != null)
        {
            return true;
        }

        return false;
    }


    public string GetText()
    {
        if(currentNode == null)
        {
            return "";
        }

        return currentNode.GetNodeText();
    }

    public IEnumerable<DialogueNode> GetChoices()
    {
        return FilterOnCondition(currentDialogue.GetPlayerChildren(currentNode));
    }

    public void SelectChoice(DialogueNode chosenNode)
    {
        currentNode = chosenNode;
        TriggerEnterAction();
        isChoosing = false;
        OnNextButton();
    }

    public bool IsChoosing()
    {
        return isChoosing;
    }

    public void OnNextButton()
    {
        int numberOfPlayerResponses = FilterOnCondition(currentDialogue.GetPlayerChildren(currentNode)).Count();

        if(numberOfPlayerResponses > 0)
        {
            isChoosing = true;
            TriggerExitAction();
            onConversationUpdated();
            return;
        }

        DialogueNode[] childNodes = FilterOnCondition(currentDialogue.GetAIChildren(currentNode)).ToArray();
        int randomIndex = UnityEngine.Random.Range(0, childNodes.Count() - 1);

        TriggerExitAction();
        currentNode = childNodes[randomIndex];
        TriggerEnterAction();

        onConversationUpdated();
    }

    public bool HasNext()
    {
        return FilterOnCondition(currentDialogue.GetAllChildrenNodes(currentNode)).Count() > 0;
    }

    private IEnumerable<DialogueNode> FilterOnCondition(IEnumerable<DialogueNode> inputNode)
    {
        foreach(var node in inputNode)
        {
            if (node.CheckCondition(GetPredicateEvaluators()))
            {
                yield return node;
            }
        }
    }

    public void Quit()
    {
        currentDialogue = null;
        TriggerExitAction();
        currentConversant = null;
        currentNode = null;
        isChoosing = false;
        onConversationUpdated();
    }

    public string GetCurrentConversantName()
    {
        if (isChoosing)
        {
            return playerName;
        }
        else
        {
            return currentConversant.GetName();
        }
    }

    public Dialogue GetCurrentDialogue()
    {
        return currentDialogue;
    }

    public DialogueNode GetCurrentDialogueNode()
    {
        return currentNode;
    }

    private void TriggerEnterAction()
    {
        if(currentNode != null && currentNode.GetOnEnterAction() != "")
        {
            TriggerAction(currentNode.GetOnEnterAction());
        }
    }

    private void TriggerExitAction()
    {
        if (currentNode != null && currentNode.GetOnExitAction() != "")
        {
            TriggerAction(currentNode.GetOnExitAction());
        }
    }

    private void TriggerAction(string actionToTrigger)
    {
        if (actionToTrigger == "") return;

        foreach(DialogueTrigger trigger in currentConversant.GetComponents<DialogueTrigger>())
        {
            trigger.Trigger(actionToTrigger);
        }    
    }

    private IEnumerable<IPredicateEvaluator> GetPredicateEvaluators()
    {
        return GetComponents<IPredicateEvaluator>();
    }
}

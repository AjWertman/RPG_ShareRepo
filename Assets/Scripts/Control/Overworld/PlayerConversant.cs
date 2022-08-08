using RPGProject.Dialogue;
using RPGProject.Questing;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPGProject.Control
{
    public class PlayerConversant : MonoBehaviour
    {
        [SerializeField] string playerName = "Conversant";

        DialogueScripObj currentDialogue = null;
        DialogueNode currentNode = null;

        AIConversant currentConversant = null;

        bool isChoosing = false;

        public event Action onConversationUpdated;

        public void StartDialogue(AIConversant _conversant, DialogueScripObj _newDialogue)
        {
            currentConversant = _conversant;
            currentDialogue = _newDialogue;

            currentNode = currentDialogue.GetRootNode();

            TriggerEnterAction();
            onConversationUpdated();
        }

        public void SelectChoice(DialogueNode _chosenNode)
        {
            currentNode = _chosenNode;
            TriggerEnterAction();
            isChoosing = false;
            OnNextButton();
        }

        public void OnNextButton()
        {
            int numberOfPlayerResponses = FilterOnCondition(currentDialogue.GetPlayerChildren(currentNode)).Count();

            if (numberOfPlayerResponses > 0)
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

        private void TriggerEnterAction()
        {
            if (currentNode != null && currentNode.onEnterAction != "")
            {
                TriggerAction(currentNode.onEnterAction);
            }
        }

        private void TriggerExitAction()
        {
            if (currentNode != null && currentNode.onExitAction!= "")
            {
                TriggerAction(currentNode.onExitAction);
            }
        }

        private void TriggerAction(string _actionToTrigger)
        {
            if (_actionToTrigger == "") return;

            foreach (DialogueTrigger trigger in currentConversant.GetComponents<DialogueTrigger>())
            {
                trigger.Trigger(_actionToTrigger);
            }
        }

        public void Quit()
        {
            if (currentDialogue == null) return;
            currentDialogue = null;
            TriggerExitAction();
            currentConversant = null;
            currentNode = null;
            isChoosing = false;
            onConversationUpdated();
        }

        public bool HasNext()
        {
            return FilterOnCondition(currentDialogue.GetAllChildrenNodes(currentNode)).Count() > 0;
        }

        public bool IsChoosing()
        {
            return isChoosing;
        }

        public bool IsChatting()
        {
            if (currentDialogue != null)
            {
                return true;
            }

            return false;
        }

        public string GetText()
        {
            if (currentNode == null)
            {
                return "";
            }

            return currentNode.nodeText;
        }

        public IEnumerable<DialogueNode> GetChoices()
        {
            return FilterOnCondition(currentDialogue.GetPlayerChildren(currentNode));
        }

        private IEnumerable<DialogueNode> FilterOnCondition(IEnumerable<DialogueNode> _inputNode)
        {
            foreach (var node in _inputNode)
            {
                IEnumerable<IPredicateEvaluator> predicateEvaluators = GetPredicateEvaluators();
                if (node.CheckCondition(predicateEvaluators))
                {
                    yield return node;
                }
            }
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

        public DialogueScripObj GetCurrentDialogue()
        {
            return currentDialogue;
        }

        public DialogueNode GetCurrentDialogueNode()
        {
            return currentNode;
        }

        private IEnumerable<IPredicateEvaluator> GetPredicateEvaluators()
        {
            return GetComponents<IPredicateEvaluator>();
        }
    }
}

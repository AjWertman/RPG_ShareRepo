using System;
using UnityEngine;
using UnityEngine.Events;

namespace RPGProject._Dialogue
{
    public class DialogueTrigger : MonoBehaviour
    {
        [SerializeField] DialogueAction[] dialogueActions = null;

        public void Trigger(string _action)
        {
            DialogueAction dialogueAction = GetDialogueAction(_action);
            dialogueAction.GetOnTrigger().Invoke();
        }

        private DialogueAction GetDialogueAction(string _action)
        {
            DialogueAction dialogueActionToGet = null;

            foreach (DialogueAction dialogueAction in dialogueActions)
            {
                if (dialogueAction.GetAction() == _action)
                {
                    dialogueActionToGet = dialogueAction;
                    break;
                }
            }

            return dialogueActionToGet;
        }
    }

    [Serializable]
    public class DialogueAction
    {
        [SerializeField] string action = null;
        [SerializeField] UnityEvent onTrigger;

        public string GetAction()
        {
            return action;
        }

        public UnityEvent GetOnTrigger()
        {
            return onTrigger;
        }
    }
}
using System;
using UnityEngine;
using UnityEngine.Events;

namespace RPGProject.Dialogue
{
    public class DialogueTrigger : MonoBehaviour
    {
        [SerializeField] DialogueAction[] dialogueActions = null;

        public void Trigger(string _action)
        {
            DialogueAction dialogueAction = GetDialogueAction(_action);
            dialogueAction.onTrigger.Invoke();
        }

        private DialogueAction GetDialogueAction(string _action)
        {
            DialogueAction dialogueActionToGet = null;

            foreach (DialogueAction dialogueAction in dialogueActions)
            {
                if (dialogueAction.action == _action)
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
        public string action = null;
        public UnityEvent onTrigger;
    }
}
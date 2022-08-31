using System;
using UnityEngine;
using UnityEngine.Events;

namespace RPGProject.Dialogue
{
    /// <summary>
    /// Placed on something that triggers dialogue (usually npcs).
    /// </summary>
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

    /// <summary>
    /// Actions that the dialogue can trigger.
    /// Examples - giving quests, completing quests, and starting combat. 
    /// </summary>
    [Serializable]
    public class DialogueAction
    {
        public string action = null;
        public UnityEvent onTrigger;
    }
}
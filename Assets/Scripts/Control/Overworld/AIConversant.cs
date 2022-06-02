using RPGProject.Dialogue;
using UnityEngine;

namespace RPGProject.Control
{
    public class AIConversant : MonoBehaviour, IRaycastable
    {
        [SerializeField] DialogueScripObj dialogue = null;

        [SerializeField] float conversationMinDistance = 3f;
        [SerializeField] string conversantName = "";

        public string GetName()
        {
            return conversantName;
        }

        public DialogueScripObj GetDialogue()
        {
            return dialogue;
        }

        public void SetDialogue(DialogueScripObj _newDialogue)
        {
            dialogue = _newDialogue;
        }

        public void StartDialogue(PlayerConversant _playerConversant)
        {
            _playerConversant.StartDialogue(this, dialogue);
        }

        public bool HandleRaycast(PlayerController _playerController)
        {
            if (dialogue == null)
            {
                print("dialogue null");
                return false;
            }

            float distanceToTarget = Vector3.Distance(_playerController.transform.position, transform.position);
            if (distanceToTarget <= conversationMinDistance)
            {
                return true;
            }
            print("not in distance");
            return false;
        }

        public string WhatToActivate()
        {
            return "Talk to " + GetName();
        }

        public void WhatToDoOnClick(PlayerController _playerController)
        {
            if (dialogue == null) return;
            StartDialogue(_playerController.GetComponent<PlayerConversant>());
        }
    }
}

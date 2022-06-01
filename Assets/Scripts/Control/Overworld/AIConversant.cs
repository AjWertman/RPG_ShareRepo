using RPGProject._Dialogue;
using UnityEngine;

namespace RPGProject.Control
{
    public class AIConversant : MonoBehaviour, IRaycastable
    {
        [SerializeField] Dialogue dialogue = null;

        [SerializeField] float conversationMinDistance = 3f;
        [SerializeField] string conversantName = "";

        public string GetName()
        {
            return conversantName;
        }

        public Dialogue GetDialogue()
        {
            return dialogue;
        }

        public void SetDialogue(Dialogue _newDialogue)
        {
            dialogue = _newDialogue;
        }

        public void StartDialogue(PlayerConversant _playerConversant)
        {
            _playerConversant.StartDialogue(this, dialogue);
        }

        public bool HandleRaycast(PlayerController _playerController)
        {
            if (dialogue == null) return false;
            float distanceToTarget = Vector3.Distance(_playerController.transform.position, transform.position);
            if (distanceToTarget <= conversationMinDistance)
            {
                return true;
            }

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

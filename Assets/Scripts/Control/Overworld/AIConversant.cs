using RPGProject.Dialogue;
using UnityEngine;

namespace RPGProject.Control
{
    /// <summary>
    /// The conversant the "PlayerConversant" will interact and dialogue with.
    /// </summary>
    public class AIConversant : MonoBehaviour, IRaycastable
    {
        public string conversantName = "";
        public DialogueScripObj dialogue = null;

        [SerializeField] float conversationMinDistance = 3f;

        public void StartDialogue(PlayerConversant _playerConversant)
        {
            _playerConversant.StartDialogue(this, dialogue);
        }

        public bool HandleRaycast(PlayerController _playerController)
        {
            if (dialogue == null) return false;

            float distanceToTarget = Vector3.Distance(_playerController.transform.position, transform.position);
            if (distanceToTarget <= conversationMinDistance) return true;

            return false;
        }

        public string WhatToActivate()
        {
            return "Talk to " + conversantName;
        }

        public void WhatToDoOnClick(PlayerController _playerController)
        {
            if (dialogue == null) return;
            StartDialogue(_playerController.GetComponent<PlayerConversant>());
        }
    }
}

using UnityEngine;

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

    public void SetDialogue(Dialogue newDialogue)
    {
        dialogue = newDialogue;
    }

    public void StartDialogue(PlayerController playerController)
    {
        playerController.GetComponent<PlayerConversant>().StartDialogue(this, dialogue);
    }

    public bool HandleRaycast(PlayerController playerController)
    {
        if (dialogue == null) return false;
        if (Vector3.Distance(playerController.transform.position, transform.position) <= conversationMinDistance)
        {                                
            return true;
        }

        return false;
    }

    public string WhatToActivate()
    {
        return "Talk to " + GetName();
    }

    public void WhatToDoOnClick(PlayerController playerController)
    {
        if (dialogue == null) return;
        StartDialogue(playerController);
    }
}

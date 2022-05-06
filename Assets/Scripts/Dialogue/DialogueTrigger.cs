﻿using UnityEngine;
using UnityEngine.Events;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] string action = null;
    [SerializeField] UnityEvent onTrigger;

    public void Trigger(string actionToTrigger)
    {
        if(actionToTrigger == action)
        {
            onTrigger.Invoke();
        }
    }
}

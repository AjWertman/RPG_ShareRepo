using RPGProject.Control;
using RPGProject.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoDialoguer : MonoBehaviour
{
    PlayerController player = null;
    AIConversant conversant = null;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
        PlayerConversant playerConversant = player.GetComponent<PlayerConversant>();

        conversant = GetComponent<AIConversant>();
        StartCoroutine(StartDialogueSoon(playerConversant));

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private IEnumerator StartDialogueSoon(PlayerConversant _playerConversant)
    {
        yield return new WaitForSeconds(1f);
        conversant.StartDialogue(_playerConversant);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

using System;
using RPGProject.Control;
using RPGProject.Core;
using UnityEngine;

public class DemoBattleHandler : MonoBehaviour
{
    OldBattleHandler battleHandler = null;

    SceneManagerScript sceneManagerScript = null;

    private void Awake()
    {
        battleHandler = GetComponent<OldBattleHandler>();
        battleHandler.onBattleEnd += FinishGame;
    }

    private void Start()
    {
        sceneManagerScript = FindObjectOfType<SceneManagerScript>();
    }

    private void FinishGame()
    {
        sceneManagerScript.LoadScene(3);
    }
}

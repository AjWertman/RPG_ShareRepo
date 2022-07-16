using System;
using RPGProject.Control;
using RPGProject.Core;
using UnityEngine;

public class DemoBattleHandler : MonoBehaviour
{
    BattleHandler battleHandler = null;

    SceneManagerScript sceneManagerScript = null;

    private void Awake()
    {
        battleHandler = GetComponent<BattleHandler>();
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

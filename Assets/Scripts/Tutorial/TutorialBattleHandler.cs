using System;
using System.Collections;
using UnityEngine;

public enum TutorialPhaseKey { Null, Phase1, Phase2, Phase3, Phase4, Phase5}

[Serializable]
public class TutorialPhase
{
    [SerializeField] TutorialPhaseKey phaseKey = TutorialPhaseKey.Null;
    [SerializeField] AIConversant aiConversant = null;

    public TutorialPhaseKey GetPhaseKey()
    {
        return phaseKey;
    }

    public AIConversant GetAIConversant()
    {
        return aiConversant;
    }
}

public class TutorialBattleHandler : MonoBehaviour
{
    [SerializeField] TutorialPhaseKey currentPhase = TutorialPhaseKey.Null;
    [SerializeField] TutorialPhase[] tutorialPhases = null;

    BattleHandler battleHandler = null;
    BattleUIManager battleUIManager = null;
    TutorialUIHandler tutorialUIHandler = null;

    private void Awake()
    {
        battleHandler = GetComponent<BattleHandler>();
        battleHandler.onBattleSetup += SetupTutorial;
        battleHandler.onAdvanceTutorial += ProgressPhase;
    }

    private void SetupTutorial()
    {
        SetupUIManager();

        currentPhase = TutorialPhaseKey.Phase1;

        SetupPhase();
    }

    private void SetupUIManager()
    {
        foreach (BattleUIManager uiManager in GetComponentsInChildren<BattleUIManager>())
        {
            if (uiManager.GetComponent<TutorialUIHandler>())
            {
                battleUIManager = uiManager;
                tutorialUIHandler = uiManager.GetComponent<TutorialUIHandler>();
            }
            else
            {
                uiManager.gameObject.SetActive(false);
            }
        }

        tutorialUIHandler.ActivatePlayerSelectObject(false);
    }

    private void SetupPhase()
    {
        TutorialPhase phase = GetCurrentPhase(currentPhase);

        AIConversant conversant = phase.GetAIConversant();
        conversant.StartDialogue(FindObjectOfType<PlayerController>());
    }

    public void ProgressPhase()
    {
        tutorialUIHandler.ActivatePlayerSelectObject(false);
        int nextPhaseIndex = (int)(currentPhase + 1);

        if (nextPhaseIndex > 5) return;

        currentPhase = (TutorialPhaseKey)nextPhaseIndex;

        SetupPhase();
    }

    public void EnemyAttackPhase()
    {
        StartCoroutine(battleHandler.AIUseAbility());
    }

    public void RelinquishControl()
    {
        tutorialUIHandler.RelinquishControl();
        battleHandler.RelinquishTutorialControl();
    }

    public void SetBattleButtonToInteractable()
    {
        if(currentPhase == TutorialPhaseKey.Phase1)
        {
            tutorialUIHandler.ActivateAttackButton();
        }
        else if(currentPhase == TutorialPhaseKey.Phase2)
        {
            tutorialUIHandler.ActivateSpellsButton();
        }
        else if(currentPhase == TutorialPhaseKey.Phase4)
        {
            tutorialUIHandler.ActivateItemsButton();
        }
        else if(currentPhase == TutorialPhaseKey.Phase5)
        {
            RelinquishControl();
        }
    }

    private TutorialPhase GetCurrentPhase(TutorialPhaseKey phaseKey)
    {
        TutorialPhase phase = null;

        foreach(TutorialPhase tutorialPhase in tutorialPhases)
        {
            if (tutorialPhase.GetPhaseKey() == phaseKey)
            {
                phase = tutorialPhase;
            }
        }

        return phase;
    }
}

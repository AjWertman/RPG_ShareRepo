using System;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUIHandler : MonoBehaviour
{
    [SerializeField] GameObject playerMoveCanvas = null;

    [SerializeField] Button attackButton = null;
    [SerializeField] Button spellsButton = null;
    [SerializeField] Button itemsButton = null;
    [SerializeField] Button escapeButton = null;

    public event Action onPlayerAction;

    public void ActivatePlayerSelectObject(bool shouldActivate)
    {
        if (shouldActivate)
        {
            SetAllButtonsUninteractable();
            GetComponent<BattleUIManager>().ActivatePlayerMoveCanvas();
        }
        else
        {
            playerMoveCanvas.SetActive(false);
        }
    }

    public void ActivateAttackButton()
    {
        SetAllButtonsUninteractable();
        attackButton.interactable = true;
    }

    public void ActivateSpellsButton()
    {
        SetAllButtonsUninteractable();
        spellsButton.interactable = true;
    }

    public void ActivateItemsButton()
    {
        SetAllButtonsUninteractable();
        itemsButton.interactable = true;
    }

    public void ActivateEscapeButton()
    {
        SetAllButtonsUninteractable();
        escapeButton.interactable = true;
    }

    public void SetAllButtonsUninteractable()
    {
        attackButton.interactable = false;
        spellsButton.interactable = false;
        itemsButton.interactable = false;
        escapeButton.interactable = false;
    }

    public void RelinquishControl()
    {
        attackButton.interactable = true;
        spellsButton.interactable = true;
        itemsButton.interactable = true;
        escapeButton.interactable = true;

        playerMoveCanvas.SetActive(true);
    }
}

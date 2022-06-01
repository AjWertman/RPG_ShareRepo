using RPGProject.Control;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICanvas : MonoBehaviour
{
    //Testing
    [SerializeField] GameObject tutorial = null;
    //Testing

    [SerializeField] CoreMenuHandler coreMenuObject = null;
    [SerializeField] GameObject checkpointMenuObject = null;
    [SerializeField] GameObject activateUI = null;

    CheckpointMenuHandler checkpointMenuHandler = null;

    private void Awake()
    {
        checkpointMenuHandler = checkpointMenuObject.GetComponent<CheckpointMenuHandler>();
    }

    private void Start()
    {
        DeactivateAllMenus();
    }

    public void ActivateCoreMenu(bool shouldActivate)
    {
        if (shouldActivate)
        {
            coreMenuObject.ActivateCoreMainMenu(shouldActivate);
        }
        else
        {
            coreMenuObject.DeactivateAllMenus();
        }       
    }

    public void ActivateCheckpointMenu(Checkpoint checkpoint)
    {
        checkpointMenuObject.SetActive(true);
        checkpointMenuHandler.SetupMenu(checkpoint);
    }
    
    public void ActivateActivateUI(string whatToActivate)
    {
        activateUI.GetComponentInChildren<TextMeshProUGUI>().text = whatToActivate;
        activateUI.SetActive(true);
    }

    public void DeactivateActivateUI()
    {
        activateUI.GetComponentInChildren<TextMeshProUGUI>().text = "";
        activateUI.SetActive(false);
    }

    public void DeactivateAllMenus()
    {
        coreMenuObject.DeactivateAllMenus();
        checkpointMenuObject.SetActive(false);
        activateUI.SetActive(false);
    }

    public void ForceDeactivateCoreMenu()
    {
        coreMenuObject.ActivateCoreMainMenu(false);
    }

    public void ForceDeactivateCheckpointMenu()
    {
        checkpointMenuObject.SetActive(false);
    }

    public bool IsCoreMenuActive()
    {
        return coreMenuObject.IsAnyMenuActive();
    }

    public bool IsCheckPointMenuActive()
    {
        return checkpointMenuObject.activeSelf;
    }

    public bool IsAnyMenuActive()
    {
        if (coreMenuObject.IsAnyMenuActive() || checkpointMenuObject.activeSelf)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool AreAnyCoreMenusActive()
    {
        return coreMenuObject.IsAnyMenuActive();
    }

    public bool IsTutorialActive()
    {
        return tutorial.gameObject.activeSelf;
    }
}

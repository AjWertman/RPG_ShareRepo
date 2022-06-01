using RPGProject.Control;
using RPGProject.Core;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CheckpointMenuHandler : MonoBehaviour
{
    [Header("Checkpoint Main Menu")]
    [SerializeField] GameObject mainMenuObject = null;

    [SerializeField] Button healTeamButton = null;
    [SerializeField] Button fastTravelButton = null;
    [SerializeField] Button saveButton = null;
    [SerializeField] Button exitButton = null;

    [Header("Fast Travel Menu")]
    [SerializeField] GameObject fastTravelMenuObject = null;

    [SerializeField] GameObject fastTravelButtonPrefab = null;
    [SerializeField] RectTransform contentRectTransform = null;
    [SerializeField] Button backButton = null;


    FastTravelPoint[] fastTravelPoints = null;
    Checkpoint currentCheckpoint = null;

    public event Action<Checkpoint, FastTravelPoint> onFastTravelSelected;
    public event Action onExitButton;

    private void Awake()
    {
        fastTravelPoints = FindObjectsOfType<FastTravelPoint>();
    }

    public void SetupMenu(Checkpoint checkpoint)
    {
        currentCheckpoint = checkpoint;

        ActivateMainMenu();

        SetupMainMenu();
    }

    private void SetupMainMenu()
    {
        healTeamButton.onClick.AddListener(() => FindObjectOfType<PlayerTeam>().RestoreAllResources());
        fastTravelButton.onClick.AddListener(ActivateFastTravelMenu);
        //saveButton.onClick.AddListener(() => FindObjectOfType<SavingWrapper>().Save());
        exitButton.onClick.AddListener(() => onExitButton());
    }

    public void SetupFastTravelMenu()
    {
        ClearFastTravelButtons();

        backButton.onClick.AddListener(ActivateMainMenu);

        foreach (FastTravelPoint fastTravelPoint in fastTravelPoints)
        {
            if (fastTravelPoint != currentCheckpoint.GetFastTravelPoint())
            {
                GameObject buttonInstance = Instantiate(fastTravelButtonPrefab, contentRectTransform);
                Button button = buttonInstance.GetComponent<Button>();
                buttonInstance.GetComponentInChildren<TextMeshProUGUI>().text = fastTravelPoint.GetName();

                button.onClick.AddListener(() => currentCheckpoint.FastTravel(fastTravelPoint));
                button.onClick.AddListener(DeactivateAllMenus);
            }
        }
    }

    private void ActivateMainMenu()
    {
        fastTravelMenuObject.SetActive(false);
        mainMenuObject.SetActive(true);
    }

    private void ActivateFastTravelMenu()
    {
        mainMenuObject.SetActive(false);
        fastTravelMenuObject.SetActive(true);

        SetupFastTravelMenu();
    }

    public void DeactivateAllMenus()
    {
        currentCheckpoint = null;
        mainMenuObject.SetActive(false);
        fastTravelMenuObject.SetActive(false);
    }

    private void ClearFastTravelButtons()
    {
        foreach (RectTransform transform in contentRectTransform)
        {
            Destroy(transform.gameObject);
        }
    }
}

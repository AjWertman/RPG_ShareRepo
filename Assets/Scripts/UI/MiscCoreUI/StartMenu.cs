using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum StartMenuButtonType { New, Continue, Quit}

public class StartMenu : MonoBehaviour
{
    [SerializeField] Texture2D cursor = null;

    [SerializeField] Button continueButton = null;
    [SerializeField] Button newGameButton = null;
    [SerializeField] Button optionsButton = null;
    [SerializeField] Button quitButton = null;

    CanvasGroup[] startMenuCanvasGroups = null;

    private void Start()
    { 
        Cursor.SetCursor(cursor, Vector2.zero, CursorMode.ForceSoftware);
        SetupStartMenu();

        startMenuCanvasGroups = FindObjectsOfType<CanvasGroup>();
    }

    private void SetupStartMenu()
    {
        //if (FindObjectOfType<SavingWrapper>().DoesDataPathExist())
        //{
        //    continueButton.onClick.AddListener(OnContinueButton);
        //}
        //else
        //{
        //    continueButton.gameObject.SetActive(false);
        //}

        newGameButton.onClick.AddListener(() => OnButtonSelection(StartMenuButtonType.New));
        //optionsButton.onClick.AddListener(OnOptionsButton);
        //quitButton.onClick.AddListener(OnQuitButton);

        GetComponent<MusicOverride>().OverrideMusic();
    }

    private void OnButtonSelection(StartMenuButtonType startMenuButtonType)
    {
        if(startMenuButtonType == StartMenuButtonType.New)
        {
            OnNewGameButton();
        }
        else if(startMenuButtonType == StartMenuButtonType.Continue)
        {
            OnContinueButton();
        }
    }

    private void OnContinueButton()
    {
        FindObjectOfType<SavingWrapper>().Load();
    }

    private void OnNewGameButton()
    {
        //SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();
        //if (wrapper.DoesDataPathExist())
        //{
        //    wrapper.DeleteSaveFile();
        //}

        FindObjectOfType<SceneManagerScript>().LoadScene(1);
        //GetComponent<MusicOverride>().ClearOverride();
    }

    private void OnOptionsButton()
    {
        //open options page
        //sounds?
        //other options????????????
    }

    private void OnQuitButton()
    {
        Application.Quit();
    }
}

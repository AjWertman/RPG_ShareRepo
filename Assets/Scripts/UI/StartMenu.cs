using RPGProject.Core;
using RPGProject.Saving;
using RPGProject.Sound;
using UnityEngine;
using UnityEngine.UI;

namespace RPGProject.UI
{
    public enum StartMenuButtonType { New, Continue, Options, Quit }

    public class StartMenu : MonoBehaviour
    {
        [SerializeField] Texture2D cursor = null;

        [SerializeField] Button continueButton = null;
        [SerializeField] Button newGameButton = null;
        [SerializeField] Button optionsButton = null;
        [SerializeField] Button quitButton = null;

        SavingWrapper savingWrapper = null;

        CanvasGroup[] startMenuCanvasGroups = null;

        private void Start()
        {
            Cursor.SetCursor(cursor, Vector2.zero, CursorMode.ForceSoftware);
            SetupStartMenu();

            savingWrapper = FindObjectOfType<SavingWrapper>();

            startMenuCanvasGroups = FindObjectsOfType<CanvasGroup>();
        }

        private void SetupStartMenu()
        {
            //Refactor
            //if (HasPreviousSaveFile())
            //{
            //    continueButton.onClick.AddListener(OnContinueButton);
            //}
            //else
            //{
            //    continueButton.gameObject.SetActive(false);
            //}

            newGameButton.onClick.AddListener(() => OnButtonSelection(StartMenuButtonType.New));
            optionsButton.onClick.AddListener(() => OnButtonSelection(StartMenuButtonType.Options));
            quitButton.onClick.AddListener(() => OnButtonSelection(StartMenuButtonType.Quit));

            GetComponent<MusicOverride>().OverrideMusic();
        }

        private void OnButtonSelection(StartMenuButtonType _startMenuButtonType)
        {
            switch (_startMenuButtonType)
            {
                case StartMenuButtonType.New:

                    OnNewGameButton();
                    break;

                case StartMenuButtonType.Continue:

                    OnContinueButton();
                    break;

                case StartMenuButtonType.Options:

                    OnOptionsButton();
                    break;

                case StartMenuButtonType.Quit:

                    OnQuitButton();
                    break;
            }
        }

        private void OnContinueButton()
        {
            savingWrapper.Load();
        }

        private void OnNewGameButton()
        {
            if (HasPreviousSaveFile())
            {
                savingWrapper.DeleteSaveFile();
            }

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

        public bool HasPreviousSaveFile()
        {
            return savingWrapper.DoesDataPathExist();
        }
    }
}

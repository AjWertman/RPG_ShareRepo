using RPGProject.Core;
using RPGProject.Saving;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DeathScreen : MonoBehaviour
{
    [SerializeField] GameObject options = null;
    [SerializeField] Button loadLastSaveButton = null;
    [SerializeField] Button mainMenuButton = null;
    [SerializeField] Button quitGameButton = null;

    CanvasGroup canvasGroup = null;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        SetupButtons();
        options.SetActive(false);
    }

    private void SetupButtons()
    {
        loadLastSaveButton.onClick.AddListener(() => LoadLastSave());
        mainMenuButton.onClick.AddListener(() => ToMainMenu());
        quitGameButton.onClick.AddListener(() => QuitGame());
    }

    public IEnumerator FadeInDeathScreen()
    {
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime / 2;
            yield return null;
        }
    }

    public void ActivateMenuOptions()
    {
        options.SetActive(true);
    }

    private void LoadLastSave()
    {
        FindObjectOfType<SavingWrapper>().Load();
    }

    private void ToMainMenu()
    {
        FindObjectOfType<SceneManagerScript>().LoadScene(0);
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}

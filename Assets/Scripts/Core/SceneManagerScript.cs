using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    [SerializeField] float fadeWaitTime = .5f;

    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(LoadSceneCoroutine(sceneIndex));
    }

    public void LoadMainMenu()
    {
        StartCoroutine(LoadSceneCoroutine(0));
    }

    public IEnumerator LoadSceneCoroutine(int sceneIndex)
    {
        //FindObjectOfType<SavingWrapper>().Save();

        yield return FindObjectOfType<Fader>().FadeOut(Color.white, .5f);

        yield return SceneManager.LoadSceneAsync(sceneIndex);

        //FindObjectOfType<SavingWrapper>().Load();

        yield return FindObjectOfType<Fader>().FadeIn(.25f);
    }
}

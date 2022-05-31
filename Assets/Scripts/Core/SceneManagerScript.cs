using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPGProject.Core
{
    public class SceneManagerScript : MonoBehaviour
    {
        [SerializeField] float fadeWaitTime = .5f;

        Fader fader = null;

        private void Awake()
        {
            fader = FindObjectOfType<Fader>();
        }

        public void LoadScene(int _sceneIndex)
        {
            StartCoroutine(LoadSceneCoroutine(_sceneIndex));
        }

        public void LoadMainMenu()
        {
            StartCoroutine(LoadSceneCoroutine(0));
        }

        public IEnumerator LoadSceneCoroutine(int _sceneIndex)
        {
            //FindObjectOfType<SavingWrapper>().Save();

            yield return fader.FadeOut(Color.white, .5f);

            yield return SceneManager.LoadSceneAsync(_sceneIndex);

            //FindObjectOfType<SavingWrapper>().Load();

            yield return fader.FadeIn(.25f);
        }
    }
}

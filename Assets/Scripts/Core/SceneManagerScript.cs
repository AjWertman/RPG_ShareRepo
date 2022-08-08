using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPGProject.Core
{
    public class SceneManagerScript : MonoBehaviour
    {
        [SerializeField] float fadeWaitTime = .5f;

        Fader fader = null;

        bool canPortal = true;

        private void Awake()
        {
            fader = FindObjectOfType<Fader>();
        }

        private void Update()
        {
            //Refactor -- Testing;
            if (Input.GetKeyDown(KeyCode.R))
            {
                LoadMainMenu();
            }
        }

        public void LoadScene(int _sceneIndex)
        {
            if (!canPortal) return;
            StartCoroutine(LoadSceneCoroutine(_sceneIndex));
        }

        public void LoadMainMenu()
        {
            if (!canPortal) return;
            StartCoroutine(LoadSceneCoroutine(0));
        }

        public IEnumerator LoadSceneCoroutine(int _sceneIndex)
        {
            canPortal = false;
            //FindObjectOfType<SavingWrapper>().Save();

            yield return fader.FadeOut(Color.white, .5f);

            yield return SceneManager.LoadSceneAsync(_sceneIndex);

            //FindObjectOfType<SavingWrapper>().Load();

            yield return fader.FadeIn(.25f);

            canPortal = true;
        }
    }
}

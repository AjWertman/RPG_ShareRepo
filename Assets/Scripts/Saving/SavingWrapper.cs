using RPGProject.Core;
using System.Collections;
using UnityEngine;

namespace RPGProject.Saving
{
    /// <summary>
    /// Gives control to the player to call save and load on the SavingSystem.
    /// </summary>
    public class SavingWrapper : MonoBehaviour
    {
        SavingSystem savingSystem = null;
        Fader fader = null;

        const string defaultSaveFile = "save";

        private void Awake()
        {
            savingSystem = GetComponent<SavingSystem>();
            fader = FindObjectOfType<Fader>();
        }

        //private IEnumerator Start()
        //{
        //    yield return savingSystem.LoadLastScene(defaultSaveFile);
        //}

        public void Save()
        {
            savingSystem.Save(defaultSaveFile);
        }

        public void Load()
        {
            StartCoroutine(LoadBehavior());
        }

        private IEnumerator LoadBehavior()
        {
            bool doesDataPathExist = DoesDataPathExist();

            if (doesDataPathExist)
            {
                yield return fader.FadeOut(Color.white, 1f);
                savingSystem.Load(defaultSaveFile);
                yield return fader.FadeIn(.5f);
            }
            else
            {
                savingSystem.Load(defaultSaveFile);
            }
        }

        public void DeleteSaveFile()
        {
            savingSystem.DeleteSaveFile(defaultSaveFile);
        }

        public bool DoesDataPathExist()
        {
            return savingSystem.DoesDataPathExist(defaultSaveFile);
        }
    }
}
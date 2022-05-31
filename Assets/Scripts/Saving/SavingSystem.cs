using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPGProject.Saving
{
    public class SavingSystem : MonoBehaviour
    {
        public IEnumerator LoadLastScene(string _saveFile)
        {
            Dictionary<string, object> state = LoadFile(_saveFile);

            if (state.ContainsKey("lastSceneBuildIndex"))
            {
                int buildIndex = (int)state["lastSceneBuildIndex"];

                if (buildIndex != SceneManager.GetActiveScene().buildIndex)
                {
                    yield return SceneManager.LoadSceneAsync(buildIndex);
                }
            }

            RestoreState(state);
        }

        public void Save(string _saveFile)
        {
            Dictionary<string, object> state = LoadFile(_saveFile);
            CaptureState(state);
            SaveFile(_saveFile, state);
        }

        public void Load(string _saveFile)
        {
            RestoreState(LoadFile(_saveFile));
        }

        public void DeleteSaveFile(string _saveFile)
        {
            string path = GetPersistantDataPath(_saveFile);
            File.Delete(path);
        }

        private void SaveFile(string _saveFile, object _state)
        {
            string path = GetPersistantDataPath(_saveFile);

            using (FileStream stream = File.Open(path, FileMode.Create))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();

                binaryFormatter.Serialize(stream, _state);
            }
        }

        private Dictionary<string, object> LoadFile(string _saveFile)
        {
            string path = GetPersistantDataPath(_saveFile);

            if (!File.Exists(path))
            {
                return new Dictionary<string, object>();
            }

            using (FileStream stream = File.Open(path, FileMode.Open))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();

                return (Dictionary<string, object>)binaryFormatter.Deserialize(stream);
            }
        }

        private void CaptureState(Dictionary<string, object> _state)
        {
            foreach (SaveableEntity saveableEntity in FindObjectsOfType<SaveableEntity>())
            {
                _state[saveableEntity.GetUniqueIdentifier()] = saveableEntity.CaptureState();
            }

            _state["lastSceneBuildIndex"] = SceneManager.GetActiveScene().buildIndex;
        }

        private void RestoreState(Dictionary<string, object> _state)
        {
            foreach (SaveableEntity saveableEntity in FindObjectsOfType<SaveableEntity>())
            {
                string id = saveableEntity.GetUniqueIdentifier();

                if (_state.ContainsKey(id))
                {
                    saveableEntity.RestoreState(_state[id]);
                }
            }
        }

        public string GetPersistantDataPath(string _saveFile)
        {
            return Path.Combine(Application.persistentDataPath, _saveFile + ".sav");
        }

        public bool DoesDataPathExist(string _saveFile)
        {
            string path = GetPersistantDataPath(_saveFile);

            return File.Exists(path);
        }
    }
}
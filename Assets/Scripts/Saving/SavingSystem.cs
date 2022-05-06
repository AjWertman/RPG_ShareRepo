using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SavingSystem : MonoBehaviour
{
    public IEnumerator LoadLastScene(string saveFile)
    {
        Dictionary<string, object> state = LoadFile(saveFile);

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

    public void Save(string saveFile)
    {
        Dictionary<string, object> state = LoadFile(saveFile);
        CaptureState(state);
        SaveFile(saveFile, state);
    }

    public void Load(string saveFile)
    {   
        RestoreState(LoadFile(saveFile));
    }

    public void DeleteSaveFile(string saveFile)
    {
        string path = GetPersistantDataPath(saveFile);
        File.Delete(path);
    }

    private void SaveFile(string saveFile, object state)
    {
        string path = GetPersistantDataPath(saveFile);

        using (FileStream stream = File.Open(path, FileMode.Create))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            binaryFormatter.Serialize(stream, state);
        }
    }

    private Dictionary<string, object> LoadFile(string saveFile)
    {
        string path = GetPersistantDataPath(saveFile);

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

    private void CaptureState(Dictionary<string, object> state)
    {
        foreach (SaveableEntity saveableEntity in FindObjectsOfType<SaveableEntity>())
        {
            state[saveableEntity.GetUniqueIdentifier()] = saveableEntity.CaptureState();
        }

        state["lastSceneBuildIndex"] = SceneManager.GetActiveScene().buildIndex; 
    }

    private void RestoreState(Dictionary<string, object> state)
    {
        foreach (SaveableEntity saveableEntity in FindObjectsOfType<SaveableEntity>())
        {
            string id = saveableEntity.GetUniqueIdentifier();

            if (state.ContainsKey(id))
            {
                saveableEntity.RestoreState(state[id]);
            }
        }
    }

    public string GetPersistantDataPath(string saveFile)
    {
        return Path.Combine(Application.persistentDataPath, saveFile + ".sav");
    }

    public bool DoesDataPathExist(string saveFile)
    {
        string path = GetPersistantDataPath(saveFile);

        return File.Exists(path);
    }
}

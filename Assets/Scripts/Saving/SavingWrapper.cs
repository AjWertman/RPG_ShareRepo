using System.Collections;
using UnityEngine;

public class SavingWrapper : MonoBehaviour
{
    const string defaultSaveFile = "save";

    //private IEnumerator Start()
    //{
    //    yield return GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile);
    //}

    public void Save()
    {
        GetComponent<SavingSystem>().Save(defaultSaveFile);
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
            yield return FindObjectOfType<Fader>().FadeOut(Color.white, 1f);
            GetComponent<SavingSystem>().Load(defaultSaveFile);
            yield return FindObjectOfType<Fader>().FadeIn(.5f);
        }
        else
        {
            GetComponent<SavingSystem>().Load(defaultSaveFile);
        }
    }

    public void DeleteSaveFile()
    {
        GetComponent<SavingSystem>().DeleteSaveFile(defaultSaveFile);
    }

    public bool DoesDataPathExist()
    {
        return GetComponent<SavingSystem>().DoesDataPathExist(defaultSaveFile);
    }
}

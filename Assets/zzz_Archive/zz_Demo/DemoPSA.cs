using RPGProject.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DemoPSA : MonoBehaviour
{
    [SerializeField] Button closeButton = null;
    SceneManagerScript sceneManagerScript;

    private void Awake()
    {
        closeButton = GetComponentInChildren<Button>();

        closeButton.onClick.AddListener(ClosePSA);
    }

    private void Start()
    {
        sceneManagerScript = FindObjectOfType<SceneManagerScript>();
    }

    public void ClosePSA()
    {
        sceneManagerScript.LoadScene(1);
        gameObject.SetActive(false);    
    }
}

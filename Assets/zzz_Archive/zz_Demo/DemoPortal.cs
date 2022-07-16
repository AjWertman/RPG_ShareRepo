using RPGProject.Control;
using RPGProject.Core;
using System.Collections;
using UnityEngine;

public class DemoPortal : MonoBehaviour, IRaycastable
{
    [SerializeField] GameObject purplePortal = null;
    [SerializeField] GameObject redPortal = null;
    ParticleSystem.MainModule portalParticles;
    SceneManagerScript sceneManagerScript = null;

    bool canPort = false;

    private void Start()
    {
        sceneManagerScript = FindObjectOfType<SceneManagerScript>();
    }

    public bool HandleRaycast(PlayerController _playerController)
    {
        if (!canPort) return false;
        if (Vector3.Distance(transform.position, _playerController.transform.position) <= 8) return true;
        return false;
    }

    public string WhatToActivate()
    {
        return "Take Portal";
    }

    public void WhatToDoOnClick(PlayerController _playerController)
    {
        sceneManagerScript.LoadScene(2);
    }

    public void SetCanPort(bool _shouldActivate)
    {
        canPort = _shouldActivate;
    }

    public void ChangeToRed()
    {
        StartCoroutine(ChangeToRedCoroutine());
    }

    public IEnumerator ChangeToRedCoroutine()
    {
        purplePortal.SetActive(false);
        yield return new WaitForSeconds(1f);
        redPortal.SetActive(true);
    }
}

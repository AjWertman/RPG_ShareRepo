using Cinemachine;
using System;
using System.Collections;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{ 
    CinemachineFreeLook followCamera = null;
    PlayerController player = null;

    bool canRotate = true;

    private void Awake()
    {
        followCamera = GetComponent<CinemachineFreeLook>();
        player = FindObjectOfType<PlayerController>();
    }

    private void Start()
    {
        followCamera.LookAt = player.GetCamLookTransform();
        followCamera.Follow = player.transform;

        followCamera.m_XAxis.Value = 0f;
        followCamera.m_YAxis.Value = .5f;
    }

    private void Update()
    {
        if (canRotate)
        {
            followCamera.m_XAxis.m_InputAxisName = "Mouse X";
            followCamera.m_YAxis.m_InputAxisName = "Mouse Y";
            followCamera.m_RecenterToTargetHeading.m_enabled = false;
            followCamera.m_YAxisRecentering.m_enabled = false;
        }
        else
        {
            followCamera.m_XAxis.m_InputAxisName = "";
            followCamera.m_YAxis.m_InputAxisName = "";
            followCamera.m_RecenterToTargetHeading.m_enabled = true;
            followCamera.m_YAxisRecentering.m_enabled = true;
        }
    }

    public void SetCanRotate(bool _canRotate)
    {
        canRotate = _canRotate;
    }

    //private void OldUpdateMethod()
    //{
    //  if (Input.GetMouseButton(1))
    //  {
    //      followCamera.m_XAxis.m_InputAxisName = "Mouse X";
    //      followCamera.m_YAxis.m_InputAxisName = "Mouse Y";
    //      followCamera.m_RecenterToTargetHeading.m_enabled = false;
    //      Cursor.lockState = CursorLockMode.Locked;
    //      Cursor.visible = false; 
    //  }
    //else
    //  {
    //    followCamera.m_XAxis.m_InputAxisName = "";
    //    followCamera.m_YAxis.m_InputAxisName = "";
    //    followCamera.m_XAxis.m_InputAxisValue = 0;
    //    followCamera.m_YAxis.m_InputAxisValue = 0;
    //    followCamera.m_RecenterToTargetHeading.m_enabled = true;
    //    Cursor.lockState = CursorLockMode.None;
    //    Cursor.visible = true;
    //  }
    //}
}

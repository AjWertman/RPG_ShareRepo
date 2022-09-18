using Cinemachine;
using UnityEngine;

namespace RPGProject.Control
{
    /// <summary>
    /// The camera behavior for the overworld.
    /// </summary>
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

        public void SetCanRotate(bool _canRotate)
        {
            canRotate = _canRotate;
            ActivateRotatation(canRotate);
        }

        /// <summary>
        /// If activated, the player will be able to pan around the map using the mouse.
        /// </summary>
        private void ActivateRotatation(bool _shouldActivate)
        {
            if (_shouldActivate)
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
    }
}

using UnityEngine;

namespace RPGProject.Core
{
    /// <summary>
    /// Behavior for making UI face the camera.
    /// </summary>
    public class LookAtCam : MonoBehaviour
    {
        public void LookAtCamTransform(Transform _camTransform)
        {
            transform.LookAt(_camTransform);
        }
    }
}

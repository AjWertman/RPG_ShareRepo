using UnityEngine;

namespace RPGProject.Core
{
    public class LookAtCam : MonoBehaviour
    {
        public void LookAtCamTransform(Transform camTransform)
        {
            transform.LookAt(camTransform);
        }
    }
}

using UnityEngine;

namespace AjsUtilityPackage
{
    public class LayerMaskTrigger : MonoBehaviour
    {
        [SerializeField] LayerMask layerMask;

        Camera mainCam = null;

        private void Awake()
        {
            mainCam = Camera.main;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (mainCam.cullingMask == layerMask) return;
            mainCam.cullingMask = layerMask;
        }
    }
}
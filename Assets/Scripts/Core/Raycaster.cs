using UnityEngine;

namespace RPGProject.Core
{
    public class Raycaster : MonoBehaviour
    {
        public bool isRaycasting = true;

        Camera mainCamera = null;

        private void Awake()
        {
            mainCamera = Camera.main;
        }

        public RaycastHit GetRaycastHit()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            Physics.Raycast(ray, out hit);

            return hit;
        }
    }
}
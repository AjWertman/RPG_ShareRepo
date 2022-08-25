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

        public RaycastHit GetRaycastHit(Vector3 _originPosition, Vector3 _targetPosition)
        {
            Vector3 direction = (_targetPosition - _originPosition).normalized;
            Ray ray = new Ray(_originPosition, direction);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);
            return hit;
        }

        public Vector3 GetMousePosition()
        {
            RaycastHit hit = GetRaycastHit();

            if (hit.collider != null) return hit.point;
            else return Vector3.zero;
        }
    }
}
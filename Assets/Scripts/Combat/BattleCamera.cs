using Cinemachine;
using UnityEngine;

namespace RPGProject.Combat
{
    /// <summary>
    /// Controls the camera used in combat.
    /// </summary>
    public class BattleCamera : MonoBehaviour
    {
        [SerializeField] float spinSensitivity = 125f;
        [SerializeField] float zoomSensitivity = 125f;
        [SerializeField] float moveSpeed = 12f;

        [SerializeField] float fieldOfViewMin, fieldOfViewMax;
        [SerializeField] float fieldOfViewDefault = 40f;

        CinemachineFreeLook freeLook = null;

        public Transform followTarget = null;
        Transform currentTarget = null;

        float turnSmoothVelocity;

        int minX, minZ, maxX, maxZ;

        bool canMove = true;

        //Future ideas
        //1. have a recenter button
        //2. possibly a way to have it lock on a character so the camera movement will follow the character while still being able to rotate
        //3. a way to increase speed of the camera controls in the menu

        private void Awake()
        {
            freeLook = GetComponent<CinemachineFreeLook>();
        }

        private void Update()
        {
            if (currentTarget == followTarget) return;

            Vector3 currentPosition = followTarget.position;
            Vector3 currentTargetPosition = currentTarget.position;
            Vector3 trueAimPosition = new Vector3(currentTargetPosition.x, 1, currentTargetPosition.z);

            if (currentPosition != trueAimPosition)
            {
                followTarget.position = trueAimPosition;
            }
        }

        public void InitalizeBattleCamera(int _minX, int _minZ, int _maxX, int _maxZ)
        {
            minX = _minX;
            minZ = _minZ;
            maxX = _maxX;
            maxZ = _maxZ;

            RecenterCamera();
            freeLook.Follow = followTarget;
            freeLook.LookAt = followTarget;

            SetFollowTarget(followTarget);
        }

        /// <summary>
        /// Changes the target that the camera follows and rotates around.
        /// </summary>
        public void SetFollowTarget(Transform _newTarget)
        {
            if (currentTarget == _newTarget) return;
            currentTarget = _newTarget;
            followTarget.transform.position = currentTarget.transform.position;
        }

        /// <summary>
        /// Moves the object that the camera follows, in turn moving the camera
        /// based on the direction the follow object is facing.
        /// </summary>
        public void MoveFollowTransform(Vector3 _inputDirection)
        {
            if (currentTarget != followTarget) SetFollowTarget(followTarget);
            float aimAngle = Mathf.Atan2(_inputDirection.x, _inputDirection.z) * Mathf.Rad2Deg + transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(followTarget.eulerAngles.y, aimAngle, ref turnSmoothVelocity, .1f);

            Vector3 moveDirection = Quaternion.Euler(0f, aimAngle, 0f) * Vector3.forward;

            followTarget.Translate(moveDirection * moveSpeed * Time.deltaTime);

            Vector3 localPosition = followTarget.localPosition;

            localPosition.x = Mathf.Clamp(localPosition.x, minX, maxX);
            localPosition.z = Mathf.Clamp(localPosition.z, minZ, maxZ);

            followTarget.localPosition = localPosition;
        }

        public void RotateCamera(bool _rotateClockwise)
        {
            float rotationAmount = spinSensitivity * Time.deltaTime;

            if (!_rotateClockwise) rotationAmount *= -1f;

            Vector3 eulers = followTarget.localEulerAngles;
            Vector3 newEulers = new Vector3(eulers.x, eulers.y + rotationAmount, eulers.z);
            followTarget.localEulerAngles = newEulers;
        }

        public void Zoom(bool _zoomingIn)
        {
            float zoomAmount = zoomSensitivity;
            if (_zoomingIn) zoomAmount *= -1f;

            float newFieldOfView = freeLook.m_Lens.FieldOfView + (zoomAmount * Time.deltaTime);
            newFieldOfView = Mathf.Clamp(newFieldOfView, fieldOfViewMin, fieldOfViewMax);
            freeLook.m_Lens.FieldOfView = newFieldOfView;
        }

        public void RecenterCamera()
        {
            currentTarget = followTarget;
            followTarget.localPosition = Vector3.zero;

            freeLook.m_Lens.FieldOfView = fieldOfViewDefault;

            freeLook.m_YAxis.Value = .5f;
            freeLook.m_XAxis.Value = 0f;

            followTarget.transform.position = Vector3.zero;
            followTarget.transform.eulerAngles = Vector3.zero;
        }
    }
}

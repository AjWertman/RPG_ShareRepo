using Cinemachine;
using UnityEngine;

public class BattleCamera : MonoBehaviour
{
    [SerializeField] float spinSensitivity = .75f;
    [SerializeField] float moveSpeed = 10f;

    CinemachineFreeLook freeLook = null;

    [SerializeField] Transform centerOfBattle = null;

    public Transform followTarget = null;

    float turnSmoothVelocity;

    int minX, minZ, maxX, maxZ;

    //Future ideas
    //1. have a recenter button
    //2. possibly a way to have it lock on a character so the camera movement will follow the character while still being able to rotate
    //3. a way to increase speed of the camera controls in the menu

    private void Awake()
    {
        freeLook = GetComponent<CinemachineFreeLook>();
    }

    public void InitalizeBattleCamera(int _minX, int _minZ, int _maxX, int _maxZ)
    {
        minX = _minX;
        minZ = _minZ;
        maxX = _maxX;
        maxZ = _maxZ;

        RecenterCamera();
        SetFollowTarget(centerOfBattle);
    }

    public void RotateFreeLook(bool _rotateClockwise)
    {
        float rotationAmount = spinSensitivity * Time.deltaTime;

        if (!_rotateClockwise) rotationAmount *= -1f;

        //freeLook.m_XAxis.Value += rotationAmount;
        Vector3 eulers = followTarget.localEulerAngles;
        Vector3 newEulers = new Vector3(eulers.x, eulers.y + rotationAmount, eulers.z);
        followTarget.localEulerAngles = newEulers;
    }

    public void RecenterCamera()
    {
        freeLook.m_YAxis.Value = .5f;
        freeLook.m_XAxis.Value = 0f;
    }

    public void SetFollowTarget(Transform _followTarget)
    {
        if (followTarget == _followTarget) return;

        followTarget = _followTarget;

        freeLook.LookAt = followTarget;
        freeLook.Follow = followTarget;
    }

    public void MoveFollowTransform(Vector3 _inputDirection)
    {
        float aimAngle = Mathf.Atan2(_inputDirection.x, _inputDirection.z) * Mathf.Rad2Deg + transform.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(followTarget.eulerAngles.y, aimAngle, ref turnSmoothVelocity, .1f);

        Vector3 moveDirection = Quaternion.Euler(0f, aimAngle, 0f) * Vector3.forward;

        followTarget.Translate(moveDirection * moveSpeed * Time.deltaTime);

        Vector3 localPosition = followTarget.localPosition;

        localPosition.x = Mathf.Clamp(localPosition.x, minX, maxX);
        localPosition.z = Mathf.Clamp(localPosition.z, minZ, maxZ);

        followTarget.localPosition = localPosition;
    }
}

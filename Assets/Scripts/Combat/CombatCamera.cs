using Cinemachine;
using UnityEngine;

public class CombatCamera : MonoBehaviour
{
    [SerializeField] float spinSensitivity = .75f;
    CinemachineFreeLook freeLook = null;

    private void Awake()
    {
        freeLook = GetComponent<CinemachineFreeLook>();
        RecenterCamera();
    }

    public void RotateFreeLook(bool _rotateClockwise)
    {
        float rotationAmount = spinSensitivity * Time.deltaTime;

        if (!_rotateClockwise) rotationAmount *= -1f;

        freeLook.m_XAxis.Value += rotationAmount;
    }

    public void RecenterCamera()
    {
        freeLook.m_YAxis.Value = .5f;
        freeLook.m_XAxis.Value = 0f;
    }
}

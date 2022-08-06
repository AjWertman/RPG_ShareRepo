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

    private void RecenterCamera()
    {
        freeLook.m_YAxis.Value = .5f;
        freeLook.m_XAxis.Value = 0f;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            RotateFreeLook(true);
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            RotateFreeLook(false);
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            RecenterCamera();
        }
    }

    public void RotateFreeLook(bool _rotateClockwise)
    {
        float rotationAmount = spinSensitivity * Time.deltaTime;

        if (!_rotateClockwise) rotationAmount *= -1f;

        freeLook.m_XAxis.Value += rotationAmount;     
    }
}

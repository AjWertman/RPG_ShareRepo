using UnityEngine;

public class LookAtCam : MonoBehaviour
{
    public void LookAtCamTransform(Transform camTransform)
    {
        transform.LookAt(camTransform);
    }
}

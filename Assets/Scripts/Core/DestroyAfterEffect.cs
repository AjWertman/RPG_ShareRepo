using UnityEngine;

public class DestroyAfterEffect : MonoBehaviour
{
    [SerializeField] GameObject objectToDestroy = null;

    ParticleSystem particleSystem = null;

    private void OnEnable()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (particleSystem.isStopped)
        {
            Destroy(objectToDestroy);
        }
    }
}

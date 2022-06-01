using UnityEngine;

public class DestroyAfterEffect : MonoBehaviour
{
    [SerializeField] GameObject objectToDestroy = null;

    ParticleSystem myParticleSystem = null;

    private void OnEnable()
    {
        myParticleSystem = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (myParticleSystem.isStopped)
        {
            //Deactivate and then return to pool
            Destroy(objectToDestroy);
        }
    }
}

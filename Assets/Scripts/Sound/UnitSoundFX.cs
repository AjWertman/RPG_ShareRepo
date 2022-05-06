using UnityEngine;

public class UnitSoundFX : MonoBehaviour
{
    [SerializeField] GameObject footStepSound = null;
    [SerializeField] GameObject jumpSound = null;
    [SerializeField] GameObject physicalAttackSound = null;
    [SerializeField] GameObject magicalAttackSound = null;
    [SerializeField] GameObject getHurtSound = null;
    [SerializeField] GameObject dieSound = null;

    public void CreateSoundFX(GameObject soundFXPrefab)
    {
        GameObject soundFXInstance = Instantiate(soundFXPrefab, transform);
        AudioSource audioSource = soundFXInstance.GetComponent<AudioSource>();
        float clipLifetime = audioSource.clip.length;

        Destroy(soundFXInstance, clipLifetime);
    }
    
    public GameObject GetFootStepSound()
    {
        return footStepSound;
    }
    public GameObject GetJumpSound()
    {
        return jumpSound;
    }
    public GameObject GetPhysAttackSound()
    {
        return physicalAttackSound;
    }
    public GameObject GetMagicalAttackSound()
    {
        return magicalAttackSound;
    }
    public GameObject GetHurtSound()
    {
        return getHurtSound;
    }
    public GameObject GetDieSound()
    {
        return dieSound;
    }
}

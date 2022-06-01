using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Sound
{
    public class SoundFXManager : MonoBehaviour
    {
        [SerializeField] GameObject soundFXSourcePrefab = null;
        [SerializeField] int amountOfObjectsToPool = 20;

        Dictionary<AudioSource, bool> soundFXSourceInstances = new Dictionary<AudioSource, bool>();

        private void Awake()
        {
            CreateSoundFXObjects();
        }

        private void CreateSoundFXObjects()
        {
            for (int i = 0; i < amountOfObjectsToPool; i++)
            {
                GameObject newSourceInstance = Instantiate(soundFXSourcePrefab, transform);
                AudioSource audioSource = newSourceInstance.GetComponent<AudioSource>();

                soundFXSourceInstances.Add(audioSource, false);
            }
        }

        public void CreateSoundFX(AudioClip _clip, Transform _clipLocation, float _volume)
        {
            StartCoroutine(CreateSoundFXCoroutine(_clip, _clipLocation, _volume));
        }

        private IEnumerator CreateSoundFXCoroutine(AudioClip _clip, Transform _clipLocation, float _volume)
        {
            if (_clipLocation == null)
            {
                _clipLocation = Camera.main.transform;
            }

            AudioSource availableAudioSource = GetAvailableAudioSource();
            soundFXSourceInstances[availableAudioSource] = true;

            float clipLength = _clip.length;

            availableAudioSource.clip = _clip;
            availableAudioSource.volume = _volume;

            availableAudioSource.transform.parent = _clipLocation;
            availableAudioSource.transform.localPosition = Vector3.zero;

            availableAudioSource.Play();

            yield return new WaitForSeconds(clipLength);

            soundFXSourceInstances[availableAudioSource] = false;
            availableAudioSource.clip = null;
            availableAudioSource.transform.parent = transform;
            availableAudioSource.transform.localPosition = Vector3.zero;
        }

        public AudioSource AssignNewAudioSource()
        {
            AudioSource availableAudioSource = GetAvailableAudioSource();
            soundFXSourceInstances[availableAudioSource] = true;
            return availableAudioSource;
        }

        private AudioSource GetAvailableAudioSource()
        {
            AudioSource availableAudioSource = null;

            foreach (AudioSource audioSource in soundFXSourceInstances.Keys)
            {
                if (soundFXSourceInstances[audioSource]) continue;

                availableAudioSource = audioSource;
                break;
            }

            return availableAudioSource;
        }
    }
}

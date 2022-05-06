using System;
using System.Collections;
using UnityEngine;

public class Song : MonoBehaviour
{
    [SerializeField] string nameKey = "";
    [SerializeField] AudioClip song = null;

    [Range(0,1)][SerializeField] float volume = 1f;

    public AudioClip GetSong()
    {
        return song;
    }

    public float GetVolume()
    {
        return volume;
    }

    //float songLength = 0;

    //public event Action onSongEnd;

    //bool isOverride = false;

    //public void SetupSong(bool _isOverride)
    //{
    //    isOverride = _isOverride;
    //    songLength = audioSource.clip.length;

    //    if (isOverride)
    //    {
    //        audioSource.loop = true;
    //    }
    //}

    //public IEnumerator BeginSongLifetime()
    //{
    //    yield return new WaitForSeconds(songLength);
    //    onSongEnd();
    //}

    //private void OnDisable()
    //{
    //    StopCoroutine(BeginSongLifetime());
    //}
}

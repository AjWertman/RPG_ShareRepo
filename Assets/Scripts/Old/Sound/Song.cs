using System;
using UnityEngine;

[Serializable]
public class Song
{
    [SerializeField] string nameKey = "";
    [SerializeField] AudioClip song = null;

    [Range(0,1)][SerializeField] float volume = 1f;


    public string GetSongName()
    {
        return nameKey;
    }

    public AudioClip GetSong()
    {
        return song;
    }

    public float GetVolume()
    {
        return volume;
    }
}

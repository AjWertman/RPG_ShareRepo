using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] Song[] mainPlaylist = null;
    AudioSource audioSource = null;

    int songIndex = 0;

    //List<GameObject> songInstances = new List<GameObject>();

    //GameObject activeSong = null;
    //GameObject overrideSong = null;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        if (mainPlaylist.Length <= 0) return;
        StartMusicPlayer();
    }

    private void StartMusicPlayer()
    {      
        songIndex = (UnityEngine.Random.Range(0, mainPlaylist.Length));

        StartCoroutine(PlayNextSong(songIndex));
    }

    private IEnumerator PlayNextSong(int songIndex)
    {
        Song nextSong = mainPlaylist[songIndex];
        AudioClip songClip = nextSong.GetSong();
        float songLength = songClip.length;

        audioSource.volume = nextSong.GetVolume();
        audioSource.clip = songClip;

        audioSource.Play();

        yield return new WaitForSeconds(songLength);

        StartCoroutine(PlayNextSong(UpdatedSongIndex()));
    }

    private int UpdatedSongIndex()
    {
        int playlistSize = mainPlaylist.Length - 1;

        if(songIndex == playlistSize)
        {
            songIndex = 0;
        }
        else
        {
            songIndex += 1;
        }

        return songIndex;
    }

    public void OverrideSong(AudioClip overrideSong)
    {
        audioSource.Stop();
        StopAllCoroutines();

        audioSource.clip = overrideSong;
        audioSource.loop = true;

        audioSource.Play();
    }

    public void ClearOverrideSong()
    {
        audioSource.Stop();

        audioSource.loop = false;

        StartCoroutine(PlayNextSong(songIndex));
    }
}

using AjsUtilityPackage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] List<Song> mainPlaylist = new List<Song>();
    AudioSource audioSource = null;

    int currentSongIndex = 0;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        if (mainPlaylist.Count <= 0) return;
        StartMusicPlayer();
    }

    private void StartMusicPlayer()
    {
        int randomStartIndex = RandomGenerator.GetRandomNumber(0, mainPlaylist.Count - 1);

        StartCoroutine(PlayNextSong(randomStartIndex));
    }

    private IEnumerator PlayNextSong(int songIndex)
    {
        currentSongIndex = songIndex;
        Song nextSong = mainPlaylist[songIndex];

        AudioClip songClip = nextSong.GetSong();
        float songLength = songClip.length;

        audioSource.volume = nextSong.GetVolume();
        audioSource.clip = songClip;

        audioSource.Play();

        yield return new WaitForSeconds(songLength);

        int nextSongIndex = UpdatedSongIndex(songIndex);

        StartCoroutine(PlayNextSong(nextSongIndex));
    }

    private int UpdatedSongIndex(int songIndex)
    {
        int playlistSize = mainPlaylist.Count - 1;

        if (songIndex == playlistSize) return 0;
        else return songIndex + 1;
    }

    public void OverrideSong(AudioClip overrideSong)
    {
        //Add fade out behavior
        audioSource.Stop();

        audioSource.clip = overrideSong;
        audioSource.loop = true;

        //Add Fade in behavior
        audioSource.Play();
    }

    public void ClearOverrideSong()
    {
        audioSource.Stop();

        audioSource.loop = false;

        StartCoroutine(PlayNextSong(currentSongIndex));
    }
}

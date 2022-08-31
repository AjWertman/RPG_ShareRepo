using RPGProject.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Sound
{
    /// <summary>
    /// Handles the playlist of songs that is currently playing.
    /// </summary>
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

        private IEnumerator PlayNextSong(int _songIndex)
        {
            currentSongIndex = _songIndex;
            Song nextSong = mainPlaylist[_songIndex];

            AudioClip songClip = nextSong.GetSong();
            float songLength = songClip.length;

            audioSource.volume = nextSong.GetVolume();
            audioSource.clip = songClip;

            audioSource.Play();

            yield return new WaitForSeconds(songLength);

            int nextSongIndex = UpdatedSongIndex(_songIndex);

            StartCoroutine(PlayNextSong(nextSongIndex));
        }

        private int UpdatedSongIndex(int _songIndex)
        {
            int playlistSize = mainPlaylist.Count - 1;

            if (_songIndex == playlistSize) return 0;
            else return _songIndex + 1;
        }

        public void OverrideSong(AudioClip _overrideSong)
        {
            //Refactor
            //Add fade out behavior
            audioSource.Stop();

            audioSource.clip = _overrideSong;
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
}

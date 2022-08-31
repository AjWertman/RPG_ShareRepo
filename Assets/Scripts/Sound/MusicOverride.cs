using UnityEngine;

namespace RPGProject.Sound
{
    /// <summary>
    /// Placed on objects that will override the current song/playlist that is playing.
    /// </summary>
    public class MusicOverride : MonoBehaviour
    {
        //Refactor
        //[SerializeField] Song[] overridePlayist = null????
        [SerializeField] Song overrideSong = null;

        MusicManager musicManager = null;

        private void Awake()
        {
            musicManager = FindObjectOfType<MusicManager>();
        }

        public void OverrideMusic()
        {
            musicManager.OverrideSong(overrideSong.GetSong());
        }

        public void ClearOverride()
        {
            musicManager.ClearOverrideSong();
        }
    }
}

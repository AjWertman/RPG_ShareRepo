using UnityEngine;

namespace RPGProject.Sound
{
    public abstract class GameSound : ScriptableObject
    {
        [SerializeField] protected string nameKey = "";
        [SerializeField] protected AudioClip song = null;

        [Range(0, 1)] [SerializeField] protected float volume = 1f;

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
}

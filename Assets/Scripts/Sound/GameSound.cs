using UnityEngine;

namespace RPGProject.Sound
{
    /// <summary>
    /// A scriptable object that is used to store data for anything that makes a sound in a game (music + sound effects).
    /// </summary>
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

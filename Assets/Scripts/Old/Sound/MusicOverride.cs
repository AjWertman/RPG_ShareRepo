using UnityEngine;

public class MusicOverride : MonoBehaviour
{
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

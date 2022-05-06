using UnityEngine;

public class MusicOverride : MonoBehaviour
{
    [SerializeField] Song overrideSong = null;

    public void OverrideMusic()
    {
        FindObjectOfType<MusicManager>().OverrideSong(overrideSong.GetSong());
    }

    public void ClearOverride()
    {
        FindObjectOfType<MusicManager>().ClearOverrideSong();
    }
}

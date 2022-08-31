using UnityEngine;

namespace RPGProject.Core
{
    /// <summary>
    /// Used for testing and marking things in the editor for devs.
    /// </summary>
    public class Notepad : MonoBehaviour
    {
        [TextArea(10,10)]
        [SerializeField] string[] notes = null;
    }
}
using UnityEngine;

namespace RPGProject.Core
{
    public class Notepad : MonoBehaviour
    {
        [TextArea(10,10)]
        [SerializeField] string[] notes = null;
    }
}
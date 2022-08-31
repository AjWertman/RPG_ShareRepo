using UnityEngine;

namespace RPGProject.Core
{
    /// <summary>
    /// Makes an object persistent on awake.
    /// </summary>
    public class DontDestroyOnLoad : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}

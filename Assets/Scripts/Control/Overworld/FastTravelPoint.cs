using UnityEngine;

namespace RPGProject.Control
{
    /// <summary>
    /// A location in the current scene that the player can travel to and from.
    /// </summary>
    public class FastTravelPoint : MonoBehaviour
    {
        public Transform teleportLocation = null;
        public string pointName = "Default Point";
    }
}
